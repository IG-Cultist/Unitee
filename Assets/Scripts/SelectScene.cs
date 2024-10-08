using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectScene : MonoBehaviour
{
    // �X�e�[�W���
    [SerializeField] GameObject info;

    // �X�e�[�W�{�^��
    [SerializeField] GameObject btnPrefab;

    // �����{�^��
    [SerializeField] GameObject infoButton;

    [SerializeField] Text infoTxt;

    // �e
    [SerializeField] GameObject stageParent;

    // �X�e�[�W����p�l��
    [SerializeField] GameObject infoPanel;

    // �f�b�L�\�z�p�l��
    [SerializeField] GameObject deckBuildPanel;

    // �J�[�h�Q�ƃp�l��
    [SerializeField] GameObject showCardPanel;

    //Clear's SoundEffect
    AudioClip clickSE;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {      
        // SetSE
        this.gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();

        clickSE = (AudioClip)Resources.Load("SE/Click");

        NetworkManager networkManager = NetworkManager.Instance;
        List<int> stageIDs = networkManager.GetID();
        
        // �S�Ẵp�l�������
        infoPanel.SetActive(false);
        info.SetActive(false);
        deckBuildPanel.SetActive(false);
        showCardPanel.SetActive(false);
        StartCoroutine(NetworkManager.Instance.GetStage(stages =>
        {
            int cnt = 0;
            foreach (var stage in stages)
            {  
                // Create Stage Button from Server
                GameObject btn = new GameObject();
                if (cnt < 5)
                {
                    btn = Instantiate(btnPrefab, new Vector2(-550 + (150 * cnt), 300), Quaternion.identity);
                }
                else
                {
                    btn = Instantiate(btnPrefab, new Vector2(-550 + (150 * (cnt-5)), 150), Quaternion.identity);
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

                if(stageIDs!= null && stageIDs.Contains(stage.StageID))
                {
                    btn.GetComponent<Image>().color = Color.yellow;
                }
                cnt++;
            }
        }));
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);
    }

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

    public void exitInfo()
    {
        info.SetActive(false);
    }

    public void startScene()
    {
        Transform infoText = info.transform.Find("Text");
        SceneManager.LoadScene(infoText.gameObject.GetComponent<Text>().text);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

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
                infoTxt.text = "���o�[�W�����ł͍Ō�̓G�ƂȂ�\n�_�C�i�}�C�g�ł��Ԃ��Ă�낤";
                break;
        }
    }

    public void exitExplain()
    {
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// Open Deck Build Panel Progress
    /// </summary>
    public void openBuildPanel()
    {
        deckBuildPanel.SetActive(true);
        infoPanel.SetActive(false);
        showCardPanel.SetActive(false);
    }

    /// <summary>
    /// Close Deck Build Panel Progress
    /// </summary>
    public void closeBuildPanel()
    {
        deckBuildPanel.SetActive(false);
    }

    /// <summary>
    /// Open Show Card Panel Progress
    /// </summary>
    public void openShowCardPanel()
    {
        showCardPanel.SetActive(true);
        deckBuildPanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// Close Deck Build Panel Progress
    /// </summary>
    public void closeCardPanel()
    {
        showCardPanel.SetActive(false);
    }
}
