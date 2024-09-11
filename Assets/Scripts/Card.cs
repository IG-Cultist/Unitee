/*
 * CardScript
 * Creator:���Y�W�� Update:2024/09/02
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

public class Card : MonoBehaviour
{
    // �J�[�h�z�u�̐e
    [SerializeField] GameObject parentCard;

    // GameEnd Button
    [SerializeField] GameObject button;

    // TurnEnd Button
    [SerializeField] GameObject turnEndButton;

    // Retry Button
    [SerializeField] GameObject retryButton;

    // Pause Button
    [SerializeField] GameObject pauseButton;

    // Pause Parent
    [SerializeField] GameObject pauseParent;

    // Pause Panel
    [SerializeField] GameObject pausePanel;

    // Option Panel
    [SerializeField] GameObject optionPanel;

    // Deck Panel
    [SerializeField] GameObject deckPanel;

    // Deck Card's Parent
    [SerializeField] GameObject deckCardParent;

     // Hand's Cards
    [SerializeField] List<GameObject> card;
    
    // Deck's Cards
    [SerializeField] List<GameObject> deckCard;

    // Active Card's List
    [SerializeField] List<GameObject> activeList;

    // Discard's CountText
    [SerializeField] Text discardText;

    // Deck Panel
    [SerializeField] Text infoText;

    // GameSpeed Slider
    [SerializeField] Slider gameSpeedSlider;

    // Block Icon
    GameObject protectIcon;

    // Discard Target's GameObject
    GameObject discardTarget;

    // Enemy's Texture
    GameObject enemyTexture;

    // Enemy's HP
    GameObject[] enemyHP;

    // Passives
    List<string> passive;

    // ��D
    List<GameObject> handCard;

    //Passive Dictionary
    Dictionary<string, ItemResponse> passiveDictionary; 

    // HealthScript
    Health healthScript;

    // PassiveScript
    Passive passiveScript;

    // EnemyScript
    Enemy enemyScript;

    // Selected Cards
    List<GameObject> selectedCard;

    // Defence Value
    public int block;

    // Card Turn's Count
    int count;

    // Passive Turn's Count
    int passiveCnt;

    //Battle's Speed
    int battleSpeed;

    // HP Count
    int enemyLife;

    //Damage Value
    int dmg;

    // Discard's Count
    [SerializeField] int discardCnt;

    // Enemy's Dead
    bool isDead;

    // Pause Check
    bool isPause;

    // Panel's Active Check
    bool panelActive;


    [SerializeField] string[] deckCardName;

    // Start is called before the first frame update
    void Start()
    {
        handCard = new List<GameObject>();
        SetCard();

        // Set Battle Speed
        battleSpeed = (int)Math.Ceiling(gameSpeedSlider.value);

        // Set GameState
        isDead = false;
        isPause = false;

        // Set Buttons
        retryButton.SetActive(false);
        turnEndButton.SetActive(false);
        button.SetActive(false);

        // Set Panels
        panelActive = false;

        // Get for Enemy Object from tag
        enemyTexture = GameObject.FindGameObjectWithTag("Enemy");
        // Get for Enemy Life from tag
        enemyHP = GameObject.FindGameObjectsWithTag("EnemyHP");
        // Add Health Value from got one
        healthScript = FindObjectOfType<Health>();
        enemyLife = healthScript.EnemHealth;

        enemyScript = FindObjectOfType<Enemy>();

        // Set Lists
        activeList = new List<GameObject>();
        selectedCard = new List<GameObject>();
        deckCard = new List<GameObject>();
        passive = new List<string>();
        // Set Counts
        count = 0;
        passiveCnt = 0;
        block = 0;

        // Add Passive Value from got one
        passiveScript = FindObjectOfType<Passive>();


        // Set Deck's Cards From DeckPanel
        for (int i = 0; i < deckCardName.Length; i++)
        {
            // Get Card's GameObjects from Resources Folder
            GameObject prefab = (GameObject)Resources.Load("Cards/Card/" + deckCardName[i]);

            // Create Instance from Now Turn's Cards
            GameObject obj = Instantiate(prefab, new Vector2(-0.38f + (0.25f * i ), 0.14f), Quaternion.identity);
            obj.transform.localScale = new Vector2(0.035f,0.08f);
            obj.name = prefab.name;

            // Add Tag 
            if (prefab.tag == "Attack") obj.tag = "DeckAttack";
            else if (prefab.tag == "Defence") obj.tag = "DeckDefence";
            obj.transform.SetParent(deckCardParent.transform, false);
            deckCard.Add(obj);
            deckCard[i].GetComponent<BoxCollider2D>().enabled = false;
        }

        deckPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        battleSpeed = (int)Math.Ceiling(gameSpeedSlider.value);
        discardText.text = "" + discardCnt;

        if (enemyScript.isDead == true)
        {
            button.SetActive(true);
            retryButton.SetActive(true);
        }

        if(enemyLife <= 0)
        {
            isDead = true;

            for (int i = 0; i < selectedCard.Count; i++)
            { // Reset Color
                selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            retryButton.SetActive(true);
            turnEndButton.SetActive(false);
            button.SetActive(true);

            enemyTexture.SetActive(false);
        }
        // If Selected Card's Value Will 4
        if (activeList.Count == 4)
        {
            // Can Use TurnEnd Button
            turnEndButton.SetActive(true);
        }
        else
        {
            turnEndButton.SetActive(false);
        }

        // Select Card
        if (Input.GetMouseButtonUp(0))
        {
            CardClick();
        }
    }

    /// <summary>
    /// Card Click
    /// </summary>
    void CardClick()
    {
        if (isDead == true || isPause == true || enemyScript.isDead == true) return;

        // Shot Ray from Touch Point
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // ������ ���C�͂ǂ̕����ɐi�ނ�(zero=�w��_)
        RaycastHit2D hit2d = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Hit Process
        if (hit2d)
        {
            //�q�b�g�����I�u�W�F�N�g�擾
            GameObject hitObj = hit2d.collider.gameObject;

            if (panelActive == true)
            {
                foreach (var item in handCard)
                {
                    item.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                }
                discardTarget = null;

                if (!selectedCard.Contains(hitObj)) //Select yet
                {
                    // Change the Card's Color
                    hitObj.GetComponent<Renderer>().material.color = new Color32(127, 127, 127, 255);
                    discardTarget = hitObj;
                }
            }
            else if (panelActive != true && hitObj.tag != "passive")
            {
                if (!selectedCard.Contains(hitObj)) //Select yet
                {
                    // Change the Card's Color
                    hitObj.GetComponent<Renderer>().material.color = new Color32(127, 127, 127, 255);
                    // Add to List Selected Card
                    selectedCard.Add(hitObj);


                    // Get Card's GameObjects from Resources Folder
                    GameObject prefab = (GameObject)Resources.Load("Cards/" + hitObj.name);

                    // Create Instance from Now Turn's Cards
                    GameObject obj = Instantiate(prefab, new Vector2(-8.2f + (2.0f * count), -4.1f), Quaternion.identity);
                    // Reset Object's Color
                    obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

                    // Rename Item
                    obj.name = hitObj.name;
                    // Add Tag for Clone Items
                    obj.tag = hitObj.tag;
                    // Add ActiveList
                    activeList.Add(obj);

                    // Add count
                    count++;

                    // Update Info 
                    switch (hitObj.name)
                    {
                        case "Sword":
                            infoText.text = "Sword:1�_���[�W��^����";
                            infoText.color = Color.red;
                            break;

                        case "A.X.E":
                            infoText.text = "A.X.E:1�_���[�W��^����\n�u���b�N�𖳎����j��";
                            infoText.color = Color.red;
                            break;

                        case "M.A.C.E":
                            infoText.text = "M.A.C.E:1+�u���b�N�̒l���_���[�W��^����";
                            infoText.color = Color.red;
                            break;

                        case "Shield":
                            infoText.text = "Shield:1�u���b�N���󂯂�";
                            infoText.color = Color.blue;
                            break;
                    }
                }
                else //Already Selected
                {
                    // Reset Selected Card's Color
                    hitObj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                    // Remove Selected Card from Lists
                    selectedCard.Remove(hitObj);

                    foreach (var item in activeList)
                    {
                        if (item.name == hitObj.name)
                        {
                            // Delete Card
                            Destroy(item);
                            // Remove from List
                            activeList.Remove(item);
                            // Refresh
                            cardRefresh(activeList);
                            break;
                        }
                    }
                }
            }
            else if (panelActive != true && hitObj.tag == "passive")
            {
                switch (hitObj.name)
                {
                    case "Spike":
                        infoText.text = passiveDictionary["Spike"].Explain;
                        infoText.color = Color.red;
                        break;

                    case "ArmorChip":
                        infoText.text = passiveDictionary["ArmorChip"].Explain;
                        infoText.color = Color.blue;
                        break;

                    case "Slime":
                        infoText.text = passiveDictionary["Slime"].Explain;
                        infoText.color = Color.red;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Turn End Process
    /// </summary>
    public async void TurnEnd()
    {
        if(isDead == true || isPause == true) return;
        // Reset Info
        infoText.text = "";

        foreach (var item in activeList)
        {
            if (isDead == true || enemyScript.isDead == true) return;
            bool isBlock = false;
            dmg = 1;

            switch (item.tag) //Judge the Card's Tag
            {
                case "Attack":
                    //Active to Passives
                    passiveEffect(item);

                    switch (item.name)
                    {
                        case "Sword":
                            infoText.text = "You:" + dmg + "�_���[�W��^����";
                            break;

                        case "A.X.E":
                            enemyScript.block = 0;
                            infoText.text = "You:�V�[���h�j��I\n" + dmg + "�_���[�W��^����";
                            break;

                        case "M.A.C.E":
                            dmg += block;
                            if (dmg < 3)
                            {
                                infoText.text = "You:" + dmg + "�_���[�W��^����";
                            }else infoText.text = "You:�Ӑg�̈ꌂ!\n" + dmg + "�_���[�W��^����!";

                            break;

                        case "T.N.T":
                            dmg =999;
                            infoText.text = "You:�h�J�[��!\n" + dmg + "�_���[�W��^����!";
                            break;
                    }

                    // If Enemy has Block
                    if (enemyScript.block != 0 && item.name != "A.X.E")
                    {
                        enemyScript.block--;
                        if (enemyScript.block <= 0) if (enemyScript.protectIcon != null) Destroy(enemyScript.protectIcon);
                        blockEffect();
                        isBlock = true;

                        infoText.text = "Enemy:�U�����u���b�N";
                    }
                    else
                    {
                        //Loop to Damage Values
                        for (int i = 0; i < dmg; i++)
                        {
                            //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                            if (enemyLife <= 0)
                            {
                                break;
                            }
                            //�\�������炷
                            Destroy(enemyHP[(enemyLife - 1)]);

                            //���������炷
                            enemyLife--;
                            Debug.Log("Enemy's HP:" + enemyLife);
                        }
                        // if Enemy Dead
                        if (enemyLife <= 0) isDead = true;
                    }
                    attackEffect(isBlock);
                    item.SetActive(false);
                    break;

                case "Defence":
                    passiveEffect(item);
                    switch (item.name)
                    {
                        case "Shield":
                            block++;
                            infoText.text = "You:" + block + "�u���b�N���󂯂�";
                            break;

                        case "SpikeShield":
                            block++;
                            infoText.text = "You:" + block +"�u���b�N���󂯂�\n�V�[���h�o�b�V�������܂����I" + dmg + "�_���[�W��^����";

                            //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                            if (enemyLife <= 0)
                            {
                                break;
                            }
                            //�\�������炷
                            Destroy(enemyHP[(enemyLife - 1)]);

                            //���������炷
                            enemyLife--;
                            Debug.Log("Enemy's HP:" + enemyLife);

                            // if Enemy Dead
                            if (enemyLife <= 0) isDead = true;

                            attackEffect(isBlock);
                            break;
                    }

                    // �u���b�N�l��0�ȉ����A�C�R������������Ă��Ȃ��ꍇ
                    if (block > 0 && protectIcon == null)
                    {
                        //Get Card's GameObjects from Resources Folder
                        GameObject prefab = (GameObject)Resources.Load("ProtectIcon");

                        // Create Instance from Now Turn's Cards
                        protectIcon = Instantiate(prefab, new Vector2(-4.35f, -1.2f), Quaternion.identity);
                    }
                    break;

                case "Support":
                    Debug.Log("�⏕");
                    passiveEffect(item);
                    break;

                default:
                    break;
            }

            item.SetActive(false);
            await Task.Delay(battleSpeed);

            if (isDead == true)
            {
                infoText.text = "�G��|�����I";
                return;
            }
            // Enemy's Action
            enemyScript.Attack();
            if (block <= 0) if (protectIcon != null) Destroy(protectIcon);
            await Task.Delay(battleSpeed);
        }

        // Reset Color
        for (int i = 0; i < selectedCard.Count; i++) selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        // Delete All Item from ActiveList
        for (int i = 0; i < activeList.Count; i++) Destroy(activeList[i]);

        // Delete All List's Items
        selectedCard.Clear();
        activeList.Clear();
        // Reset Count
        count = 0;
        button.SetActive(true);
        retryButton.SetActive(true);
    }

    /// <summary>
    /// Card Refresh Process
    /// </summary>
    void cardRefresh(List<GameObject> refreshTarget)
    {
        // �ꎞ�I�Ɍ��݃A�N�e�B�u�ȃJ�[�h�������郊�X�g
        List<GameObject> keepList = new List<GameObject>();
        foreach (var item in refreshTarget) //Drstroy All Active Card & Add Assumed List
        {
            Destroy(item);
            keepList.Add(item);
        }
        // Delete All List's Item
        refreshTarget.Clear();
        // Reset Count
        count = 0;

        foreach (var item in keepList) //���ׂȂ���
        {
            //���݂̃J�[�h�v���n�u�����ɁA�C���X�^���X�𐶐��A
            GameObject obj = Instantiate(item, new Vector2(-8.2f + (2.0f * count), -4.1f), Quaternion.identity);
            //�I�u�W�F�N�g�̐F�����
            obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            //�N���[�������I�u�W�F�N�g�̖��O�����
            obj.name = item.name;
            //�J�[�h�̃^�O���N���[���I�u�W�F�N�g�ɂ��ǉ�
            obj.tag = item.tag;
            refreshTarget.Add(obj);
            //���������Z
            count++;
        }
    }

    /// <summary>
    /// Passive's Effect Process
    /// </summary>
    /// <param name="item"></param>
    void passiveEffect(GameObject item)
    {
        //Select to Passive's name
        switch (item.tag)
        {
            case "Attack":
                if (passive[passiveCnt] == "Spike")
                {
                    dmg++;
                }
                else if (passive[passiveCnt] == "Slime")
                {
                    enemyScript.dmg--;
                }
                break;
            case "Defence":

                if (passive[passiveCnt] == "Spike" && item.name =="Shield")
                {
                    dmg++;
                }
                if (passive[passiveCnt] == "ArmorChip")
                {
                    Debug.Log("�A�[�}�[�`�b�v���� DEF+1");

                    if (block <= 0)
                    {
                        //Get Card's GameObjects from Resources Folder
                        GameObject prefab = (GameObject)Resources.Load("ProtectIcon");

                        // Create Instance from Now Turn's Cards
                        protectIcon = Instantiate(prefab, new Vector2(-4.35f, -1.2f), Quaternion.identity);
                    }

                    block++;
                }
                break;
        }

        if (passiveCnt < (passive.Count - 1)) passiveCnt++;
    }

    public void endGame()
    {
        SceneManager.LoadScene("SelectScene");
    }
    public void retryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Attack's Effect Progress
    /// </summary>
    async void attackEffect(bool isBlock)
    {
        //Get Card's GameObjects from Resources Folder
        GameObject prefab = (GameObject)Resources.Load("AttackEffect");

        // Create Instance from Now Turn's Cards
        GameObject obj = Instantiate(prefab, new Vector2(0.0f, 1.4f), Quaternion.identity);
        await Task.Delay(100);
        Destroy(obj);

        if (isBlock != true)
        {
            for (int i = 0; i < 5; i++)
            {
                enemyTexture.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);
                await Task.Delay(10);
                enemyTexture.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                await Task.Delay(10);
            }
        }
    }

    /// <summary>
    /// Block's Effect Progress
    /// </summary>
    async void blockEffect()
    {
        //Get Card's GameObjects from Resources Folder
        GameObject prefab = (GameObject)Resources.Load("BlockEffect");

        // Create Instance from Now Turn's Cards
        GameObject obj = Instantiate(prefab, new Vector2(0.0f, 1.0f), Quaternion.identity);
        await Task.Delay(100);
        Destroy(obj);
    }


    /// <summary>
    /// ShowDeck Progress
    /// </summary>
    public void showDeck()
    {
        if (isDead == true || isPause == true) return;
        // Reset Info
        infoText.text = "";

        // Panel Active yet
        if (panelActive == false)
        {
            deckPanel.SetActive(true);
            panelActive = true;

            // Reset Selected Card
            for (int i = 0; i < selectedCard.Count; i++)
            { //Reset Color
                selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            for (int i = 0; i < activeList.Count; i++)
            { //Delete All Item from ActiveList
                Destroy(activeList[i]);
            }

            // Delete All List's Items
            selectedCard.Clear();
            activeList.Clear();
            count = 0;
        }
        else
        { //Panel is Already Active 
            deckPanel.SetActive(false);
            panelActive = false;

            // Reset Selected Card's Color
            foreach (var item in handCard)
            {
                item.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }
            discardTarget = null;
        }
    }

    /// <summary>
    /// Discard Progress
    /// </summary>
    public void discard()
    {
        if (discardCnt > 0 && discardTarget != null)
        {
            int cnt = 0;
            foreach (var item in handCard)
            {
                if (item.name == discardTarget.name) //�I���J�[�h�ƌ��݂̃J�[�h�̖��O����v�����ꍇ
                {
                    // �e�K�v�ȃI�u�W�F�N�g��Transform���擾
                    Vector2 pos = item.transform.position;
                    Vector2 scale = item.transform.localScale;
                    // �R�D�̍��[�̃J�[�h�𐶐�
                    GameObject obj = Instantiate(deckCard[0], new Vector2(pos.x, pos.y), Quaternion.identity);
                    // �R���C�_�[��ݒ�
                    obj.AddComponent<BoxCollider2D>();
                    obj.GetComponent<BoxCollider2D>().isTrigger = true;
                    // �X�P�[���𒲐�
                    obj.transform.localScale = scale;
                    // �e�ɓ����
                    obj.transform.SetParent(parentCard.transform, false);
                    // ���O���C��
                    obj.name = deckCard[0].name;

                    // �^�O�ɉ������V���ȃ^�O��t�^
                    switch (deckCard[0].tag)
                    {
                        case "DeckAttack": //�U���^�O�̏ꍇ

                            obj.tag = "Attack";
                            break;

                        case "DeckDefence": //�h��^�O�̏ꍇ

                            obj.tag = "Defence";
                            break;

                        default:
                            break;
                    }
                    obj.GetComponent<BoxCollider2D>().enabled = true;

                    // ������̃J�[�h�̌��֒ǉ����A������̃J�[�h������
                    handCard.Insert(cnt,obj);
                    handCard.Remove(item);
                    item.SetActive(false);

                    // Reset Selected Card's Color
                    foreach (var items in handCard)
                    {
                        items.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                    }

                    // Delete Card
                    Destroy(deckCard[0]);
                    // Remove from List
                    deckCard.Remove(deckCard[0]);

                    // �ꎞ�I�Ɍ��݃A�N�e�B�u�ȃJ�[�h�������郊�X�g
                    List<GameObject> keepList = new List<GameObject>();
                    foreach (var card in deckCard) //Drstroy All Active Card & Add Assumed List
                    {
                        Destroy(card);
                        keepList.Add(card);
                    }
                    // Delete All List's Item
                    deckCard.Clear();

                    int refreshCnt = 0;
                    foreach (var card in keepList) //���ׂȂ���
                    {
                        // ���݂̃J�[�h�v���n�u�����ɁA�C���X�^���X�𐶐��A
                        GameObject objKeep = Instantiate(card, new Vector2(-0.38f + (0.25f * refreshCnt), 0.14f), Quaternion.identity);

                        // �X�P�[�����C��
                        objKeep.transform.localScale = new Vector2(0.035f, 0.08f);
                        // �N���[�������I�u�W�F�N�g�̖��O�����
                        objKeep.name = card.name;
                        // �J�[�h�̃^�O���N���[���I�u�W�F�N�g�ɂ��ǉ�
                        objKeep.tag = card.tag;
                        // �e�ɓ˂�����
                        objKeep.transform.SetParent(deckCardParent.transform, false);

                        deckCard.Add(objKeep);
                        // ���������Z
                        refreshCnt++;
                    }

                    deckPanel.SetActive(false);
                    panelActive = false;
                    discardTarget = null;
                    break;
                }
                cnt++;
            }
            // �����\�񐔂����炷
            discardCnt--;
        }
        else return;
    }

    /// <summary>
    /// Pause Button Progress
    /// </summary>
    public void pause()
    {
        pauseParent.SetActive(true);
        Time.timeScale = 0.0f;
        isPause = true;
        pausePanel.SetActive(true);
    }

    /// <summary>
    /// Close Pause Panel Progress
    /// </summary>
    public void closePause()
    {
        pauseParent.SetActive(false);
        Time.timeScale = 1.0f;
        isPause = false;
        pausePanel.SetActive(false);
    }

    /// <summary>
    /// Option Panel Progress
    /// </summary>
    public void openOption()
    {
        pausePanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    /// <summary>
    /// Close Option Panel Progress
    /// </summary>
    public void closeOption()
    {
        pausePanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void SetPassives(List<string> passiveList, Dictionary<string, ItemResponse> dictionary)
    {
        passive = passiveList;
        passiveDictionary = dictionary;
    }

    void SetCard()
    {
        int cnt = 0;

        foreach (var cards in card)
        {
            //  Get Prefabs from List
            GameObject obj = (GameObject)Resources.Load("Cards/Card/" + cards.name);
            // Create Action Objects
            GameObject item = Instantiate(obj, new Vector2(1.17f + (2.2f * cnt), -3.4f), Quaternion.identity);
            // Rename
            item.name = cards.name;

            item.transform.SetParent(parentCard.transform, false);
            handCard.Add(item);
            cnt++;
        }
    }
}