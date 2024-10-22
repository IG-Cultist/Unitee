/*
 * SelectSceneScript
 * Creator:���Y�W�� Update:2024/10/10
*/
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SelectScene : MonoBehaviour
{
    // �X�e�[�W���
    [SerializeField] GameObject info;

    // �X�e�[�W�{�^��
    [SerializeField] GameObject btnPrefab;

    // �����{�^��
    [SerializeField] GameObject infoButton;

    // Card Explain Text
    [SerializeField] Text infoTxt;

    // �f�b�L��ʃe�L�X�g
    [SerializeField] Text deckTxt;

    // �X�e�[�W�̐e
    [SerializeField] GameObject stageParent;

    // �J�[�h�̐e
    [SerializeField] GameObject cardParent;

    // �X�e�[�W����p�l��
    [SerializeField] GameObject infoPanel;

    // �f�b�L�\�z�p�l��
    [SerializeField] GameObject deckBuildPanel;

    // �J�[�h�Q�ƃp�l��
    [SerializeField] GameObject showCardPanel;

    // �r���h�p�l��
    [SerializeField] GameObject buildPanel;

    // �J�[�h�p�l��
    [SerializeField] GameObject cardViewPanel;

    // �J�[�h�e
    [SerializeField] GameObject cardViewParent;

    // ���݂̃f�b�L�̐e
    [SerializeField] GameObject activeDeckParent;

    // �h�q�f�b�L�̐e
    [SerializeField] GameObject defenceDeckPanel;

    // Deck Panel
    [SerializeField] Text infoText;

    // �v���C���[��
    [SerializeField] Text playerName;

    // �f�b�L�J�[�h���X�g
    public List<GameObject> deckCards = new List<GameObject>();

    // Clear's SoundEffect
    AudioClip clickSE;

    // �g�p�\�J�[�h�̃f�B�N�V���i���[
    Dictionary<string, UsableCardResponse> cardDictionary = new Dictionary<string, UsableCardResponse>();

    GameObject mainDeck;

    int deckCount = 0;
    bool isSet = false;
    bool isClick = false;


    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        // SetSE
        this.gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        mainDeck = GameObject.Find("MainDeckFrame");

        clickSE = (AudioClip)Resources.Load("SE/Click");

        NetworkManager networkManager = NetworkManager.Instance;
        List<int> stageIDs = networkManager.GetID();

        // �S�Ẵp�l�������
        infoPanel.SetActive(false);
        info.SetActive(false);
        deckBuildPanel.SetActive(false);
        showCardPanel.SetActive(false);
        defenceDeckPanel.SetActive(false);
        StartCoroutine(NetworkManager.Instance.GetStage(stages =>
        {
            int cnt = 0;
            foreach (var stage in stages)
            {
                // Create Stage Button from Server
                GameObject btn = new GameObject();
                Destroy(btn);
                if (cnt < 5)
                {
                    btn = Instantiate(btnPrefab, new Vector2(-550 + (150 * cnt), 300), Quaternion.identity);
                }
                else
                {
                    btn = Instantiate(btnPrefab, new Vector2(-550 + (150 * (cnt - 5)), 150), Quaternion.identity);
                }

                // Rename for StageID
                btn.name =/*"Stage" + */ stage.StageID.ToString();

                //Change Button's Text for StageID
                Transform btnText = btn.transform.Find("Text");
                btnText.gameObject.GetComponent<Text>().text = stage.StageID.ToString();

                // Add Button in Canvas
                string btnName = btn.name;
                btn.transform.SetParent(this.stageParent.transform, false);
                btn.GetComponent<Button>().onClick.AddListener(() => selectStage(btnName));

                if (stageIDs != null && stageIDs.Contains(stage.StageID))
                {
                    btn.GetComponent<Image>().color = Color.yellow;
                }
                cnt++;
            }
        }));

        StartCoroutine(NetworkManager.Instance.GetUsableCard(cards =>
        {
            int cnt = 0;
            // Create Stage Button from Server
            GameObject cardObj = new GameObject();
            foreach (var card in cards)
            {
                // ���O���擾
                string cardName = card.Name.ToString();
                string cardStack = card.Stack.ToString();
                // �擾�������O�Ɉ�v����v���n�u���擾
                GameObject obj = (GameObject)Resources.Load("UI/" + cardName);
                // �擾�����v���n�u�𐶐�
                if (cnt < 5)
                {
                    cardObj = Instantiate(obj, new Vector2(-400 + (200 * cnt), 100), Quaternion.identity);
                }
                else
                {
                    cardObj = Instantiate(obj, new Vector2(-300 + (200 * (cnt - 5)), -130), Quaternion.identity);
                }

                // ���������v���n�u�����l�[��
                cardObj.name = cardName;
                // �e��ʂɉ������^�O��t�^
                switch (card.Type.ToString())
                {
                    case "Attack":
                        cardObj.tag = "Attack";
                        break;
                    case "Defence":
                        cardObj.tag = "Defence";
                        break;
                    default:
                        break;
                }
                // ���������v���n�u��e�ɓ����
                cardObj.transform.SetParent(this.cardParent.transform, false);
                cardObj.GetComponent<Button>().onClick.AddListener(() => CardClick(cardName, cardStack));

                // �J�[�h�����f�B�N�V���i���[�ɂ܂Ƃ߂�
                cardDictionary.Add(card.Name, card);

                cnt++;
            }
        }));
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);

        if(Input.GetMouseButtonUp(1)) CheckSomething();
    }

    /// <summary>
    /// Open Info Panel
    /// </summary>
    /// <param name="btnName"></param>
    public void selectStage(string btnName)
    {
        // Change Info's StageID
        Transform infoText = info.transform.Find("Text");
        infoText.gameObject.GetComponent<Text>().text = btnName;

        // Info�p�l���݂̂��J���A����ȊO�����
        info.SetActive(true);
        deckBuildPanel.SetActive(false);
        infoPanel.SetActive(false);
        showCardPanel.SetActive(false);
    }

    /// <summary>
    /// Close Info Panel
    /// </summary>
    public void exitInfo()
    {
        info.SetActive(false);
    }

    /// <summary>
    /// Load Some Stage Scene
    /// </summary>
    public void startScene()
    {
        Transform infoText = info.transform.Find("Text");
        SceneManager.LoadScene(infoText.gameObject.GetComponent<Text>().text);
    }

    /// <summary>
    /// Load Tutprial Scene
    /// </summary>
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    /// <summary>
    /// Open Stage Info Panel
    /// </summary>
    public void stageInfo()
    {
        deckBuildPanel.SetActive(false);
        infoPanel.SetActive(true);
        Transform stageInfo = info.transform.Find("Text");

        switch (stageInfo.gameObject.GetComponent<Text>().text)
        {
            case "1":
                infoTxt.text = "�������菇�Ō��j��ڎw����";
                break;

            case "2":
                infoTxt.text = "�����ł̓V�[���h��j�󂷂邱�Ƃ��d�v�ƂȂ�\n";
                break;

            case "3":
                infoTxt.text = "�����������Ă���Ɣ��j����Ă��܂�\n�ǂ����ɃV�[���h�͂Ȃ����낤��";
                break;

            case "4":
                infoTxt.text = "���ʂȕ�������W���Ă���\n�U���̌��ʂ��o���Ă�����";
                break;

            case "5":
                infoTxt.text = "�łōU���͂��������Ă��܂�\n��������g���悤";
                break;

            case "6":
                infoTxt.text = "������̍s�����R�s�[�����\n���Η͂̃J�[�h���g���Ƃ��͒��ӂ��悤";
                break;

            case "7":
                infoTxt.text = "���˃o���A�ōU���𒵂˕Ԃ��Ă���\n�O�X�e�[�W���l�J�[�h�I�т͐T�d��";
                break;

            case "8":
                infoTxt.text = "�h��͍ő��...�h��H";
                break;

            case "9":
                infoTxt.text = "�w�؂����肻����...\n�ŏ��̖ҍU��˔j�ł���Ώ����؂͌����Ă��邾�낤";
                break;

            case "10":
                infoTxt.text = "���o�[�W�����ł͍Ō�̓G�ƂȂ�\n�_�C�i�}�C�g�ł��Ԃ��Ă�낤\n...����������������Ă�l�͂���̂��H";
                break;
        }
    }

    /// <summary>
    /// Close Explain Panel
    /// </summary>
    public void exitExplain()
    {
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// �f�b�L�\�z�p�l�����J������
    /// </summary>
    public void openDeckBuildPanel()
    {
        deckBuildPanel.SetActive(true);
        buildPanel.SetActive(true);

        cardViewPanel.SetActive(false);
        infoPanel.SetActive(false);
        showCardPanel.SetActive(false);

        DeckRefresh();
    }

    /// <summary>
    /// �f�b�L�\�z�p�l������鏈��
    /// </summary>
    public void closeDeckBuildPanel()
    {
        deckBuildPanel.SetActive(false);
    }

    /// <summary>
    /// �J�[�h�r���[�p�l������
    /// </summary>
    public void openAttackCardPanel()
    {
        cardViewPanel.SetActive(true);

        buildPanel.SetActive(false);
        infoPanel.SetActive(false);
        showCardPanel.SetActive(false);

        if (isSet == true) return;
        int cnt = 0;
        // �f�B�N�V���i���[���̃A�C�e�������[�v
        foreach (var item in cardDictionary.Keys)
        {
            // �L�[�𕶎���ɕϊ�
            string cardName = item.ToString();
            // �X�^�b�N���𐔎��ɕϊ�
            int.TryParse(cardDictionary[cardName].Stack, out int stack);

            // �e�J�[�h�̃X�^�b�N�������[�v
            for (int i = 0; i < 4; i++)
            {
                // ���[�v�����X�^�b�N�������̏ꍇ
                if (stack > i)
                {
                    // �����̃J�[�h�����\�[�X�t�@�C������擾
                    GameObject obj = (GameObject)Resources.Load("UI/" + cardName);
                    // �擾�����J�[�h�𐶐�
                    GameObject cards = Instantiate(obj, new Vector2(-400f + (200f * i), 125f - (250f * cnt)), Quaternion.identity);
                    // Rename
                    cards.name = cardName;
                    cards.GetComponent<Button>().onClick.AddListener(() => AddDeck(cards));

                    // �����J�[�h���X�N���[���r���[�ɒǉ�
                    cards.transform.SetParent(cardViewParent.transform, false);
                }
                else // ���[�v�����X�^�b�N���𒴂����ꍇ�A�_�~�[�𐶐����Đ��ڂ���
                {
                    // �����ȃ_�~�[�����\�[�X�t�@�C������擾
                    GameObject obj = (GameObject)Resources.Load("UI/Dummy");
                    // �擾�����_�~�[�𐶐�
                    GameObject cards = Instantiate(obj, new Vector2(-400f + (200f * i), 125f - (250f * cnt)), Quaternion.identity);
                    // Rename
                    cards.name = "dummy";

                    // �_�~�[���X�N���[���r���[�ɒǉ�
                    cards.transform.SetParent(cardViewParent.transform, false);
                }
            }
            cnt++;
        }
        isSet = true;
    }

    /// <summary>
    /// �r���h��ʂɖ߂鏈��
    /// </summary>
    public void backBuildPanel()
    {
        buildPanel.SetActive(true);

        cardViewPanel.SetActive(false);

        DeckRefresh();
    }

    /// <summary>
    /// �g�p�\�J�[�h�ꗗ�p�l���Q�Ə���
    /// </summary>
    public void openShowCardPanel()
    {
        showCardPanel.SetActive(true);
        deckBuildPanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// �g�p�\�J�[�h�ꗗ�p�l������鏈��
    /// </summary>
    public void closeCardPanel()
    {
        showCardPanel.SetActive(false);
    }

    /// <summary>
    /// Load Battle Scene
    /// </summary>
    public void goFight()
    {
        SceneManager.LoadScene("Battle");
    }

    /// <summary>
    /// Check Usable Card
    /// </summary>
    /// <param name="name"></param>
    /// <param name="stack"></param>
    public void CardClick(string name, string stack)
    {
        switch (name)
        {
            case "Sword":
                infoText.text = name + ":1�_���[�W��^���� �����~" + stack;
                break;

            case "S.Y.T.H":
                infoText.text = name + ":2�_���[�W��^���� �����~" + stack;
                break;

            case "A.X.E":
                infoText.text = name + ":1�_���[�W��^���� �u���b�N�𖳎����j�� �����~" + stack;
                break;

            case "M.A.C.E":
                infoText.text = name + ":1�_���[�W�ɉ����u���b�N�̒l���_���[�W��^���� �����~" + stack;
                break;

            case "Shield":
                infoText.text = name + ":1�u���b�N���󂯂� �����~" + stack;
                break;

            case "ForgeHammer":
                infoText.text = name + ":1�_���[�W��^���� ���̍s���̍U����+1 �����~" + stack;
                break;

            case "Injector":
                infoText.text = name + ":1�_���[�W��^���� �G���o��������(�s����1�_���[�W) �����~" + stack;
                break;

            case "PoisonKnife":
                infoText.text = name + ":1�_���[�W��^���� �G�̍U����-1 �����~" + stack;
                break;

            case "6mmBullet":
                infoText.text = name + ":3�_���[�W��^���� ...�e������΂̘b �����~" + stack;
                break;

            case "SwatShield":
                infoText.text = name + ":2�u���b�N���󂯂� �����~" + stack;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �f�b�L�ǉ�����
    /// </summary>
    void AddDeck(GameObject obj)
    {
        // �I��(�f�b�L�ɒǉ�)���ꂽ�ꍇ�����݂̃f�b�L��4���ȉ��̏ꍇ
        if (!deckCards.Contains(obj) && deckCards.Count < 4)
        {
            obj.GetComponent<Image>().color = Color.gray;

            // �����̃J�[�h�����\�[�X�t�@�C������擾
            GameObject resource = (GameObject)Resources.Load("UI/" + obj.name);
            // �擾�����J�[�h�𐶐�
            GameObject cards = Instantiate(resource, new Vector2(-450f + (300f * deckCount), 0f), Quaternion.identity);
            cards.transform.localScale = new Vector2(1.8f, 2.5f);
            // Rename
            cards.name = obj.name;
            // ���������J�[�h��e�ɒǉ�
            cards.transform.SetParent(activeDeckParent.transform, false);
            // �f�b�L���X�g�ɒǉ�
            deckCards.Add(obj);
            deckCount++;
        }
        else // �ēx�I��(�폜)���ꂽ�ꍇ
        {
            // ���ݐ�������Ă���I�u�W�F�N�g���폜
            foreach (Transform n in activeDeckParent.transform)
            {
                // �^�b�`�����I�u�W�F�N�g�ƃ��X�g���̃I�u�W�F�N�g�̖��O����v�����ꍇ
                if (obj.name == n.name)
                {
                    // �Ώۂ��폜
                    GameObject.Destroy(n.gameObject);
                    deckCount--;
                    break;
                }
            }

            // �F�����ɖ߂�
            obj.GetComponent<Image>().color = Color.white;
            // �f�b�L���X�g����폜
            deckCards.Remove(obj);
        }
    }

    /// <summary>
    /// �f�b�L�ǂݍ��ݏ���
    /// </summary>
    void DeckRefresh()
    {
        if (deckCards.Count <= 0) return;
        int cnt = 0;
        List<GameObject> keepList = new List<GameObject>();

        foreach (GameObject obj in deckCards)
        {
            keepList.Add(obj);
        }

        foreach (Transform n in activeDeckParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        deckCards.Clear();

        foreach (var item in keepList)
        {
            // �����̃J�[�h�����\�[�X�t�@�C������擾
            GameObject obj = (GameObject)Resources.Load("UI/" + item.name);
            // �擾�����J�[�h�𐶐�
            GameObject cards = Instantiate(obj, new Vector2(-450f + (300f * cnt), 0f), Quaternion.identity);
            cards.transform.localScale = new Vector2(1.8f, 2.5f);
            // Rename
            cards.name = item.name;
            // �ēx���X�g�ɒǉ�
            deckCards.Add(item);
            // ���������J�[�h��e�ɒǉ�
            cards.transform.SetParent(activeDeckParent.transform, false);
            cnt++;
        }
    }

    /// <summary>
    /// �����_���l�[���R���o�[�g����
    /// </summary>
    public void randomName()
    {   
        System.Random rand = new System.Random();
        // �t�@�X�g�l�[����`
        string[] firstName = new string[]{
            "Nice","Abnormal","Delicious","Difficulty","Mr",
            "Mrs","Master","Huge","Tiny","Clever",
            "Wetty","Pretty","Golden","Brave","Godly",
            "Kidly","Burning","Creepy","Fishy","Metallic",
            "Oriental","Muscly","Mudly","More","Strong",
            "Shiny","Sparkle","Legal","Hardest","Dancing"
        };
        // �Z�J���h�l�[����`
        string[] secondtName = new string[]{
            "Cake","Rock","Slime","Clover","Animal",
            "Fish","Earth","Throat","City","Dwarf",
            "Ghost","Tank","Knight","Candy","Worm",
            "Tree","Dice","Baby","Machine","Dog",
            "Thief","Bird","Cat","Water","CowBoy",
            "Skelton","Boots","Game","Card","Data"
        };
        // 1�`30�܂ł̗�������
        int num = rand.Next(1, 30);
        int num2 = rand.Next(1, 30);

        // �e�����ɉ��������O����
        playerName.text ="Name:" + firstName[num] + secondtName[num2];
    }

    void CheckSomething()
    {
        foreach (var deck in deckCards)
        {
            Debug.Log(deck);
        }
    }

    /// <summary>
    /// �f�b�L�؂�ւ�����
    /// </summary>
    public void ChangeDeck()
    {
        if (isClick == false)
        {
            mainDeck.SetActive(false);
            defenceDeckPanel.SetActive(true);
            deckTxt.text = "Defence Deck";
            isClick = true;
        }
        else
        {
            mainDeck.SetActive(true);
            defenceDeckPanel.SetActive(false);
            deckTxt.text = "Main Deck";
            isClick = false;
        }
    }
}
