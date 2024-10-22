/*
 * BattleModePlayerScript
 * Creator:���Y�W�� Update:2024/10/21
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class BattleModePlayer : MonoBehaviour
{
    // �J�[�h�z�u�̐e
    [SerializeField] GameObject parentCard;

    // GameEnd Button
    [SerializeField] GameObject button;

    // TurnEnd Button
    [SerializeField] GameObject turnEndButton;

    // Pause Button
    [SerializeField] GameObject pauseButton;

    // Pause Parent
    [SerializeField] GameObject pauseParent;

    // Pause Panel
    [SerializeField] GameObject pausePanel;

    // Option Panel
    [SerializeField] GameObject optionPanel;

    //Protect Icon
    [SerializeField] GameObject protectIcon;

    // Hand's Cards
    [SerializeField] List<GameObject> card;

    // Deck Panel
    [SerializeField] Text infoText;

    // GameSpeed Slider
    [SerializeField] Slider gameSpeedSlider;

    // Attack's SoundEffect
    AudioClip attackSE;

    // Heavy Attack's SoundEffect
    AudioClip heavyAttackSE;

    // Boom Attack's SoundEffect
    AudioClip boomAttackSE;

    //Parry's SoundEffect
    AudioClip parrySE;

    // Defence's SoundEffect
    AudioClip defenceSE;

    // Clear's SoundEffect
    AudioClip clearSE;

    // Clear's SoundEffect
    AudioClip clickSE;

    // Discard Target's GameObject
    GameObject discardTarget;

    // Enemy's Texture
    GameObject enemyTexture;

    // Enemy's HP
    public GameObject[] enemyHP;

    // Passives
    List<string> passive;

    // Hand
    List<GameObject> handCard;

    // Passive Dictionary
    Dictionary<string, ItemResponse> passiveDictionary;

    // HealthScript
    Health healthScript;

    // PassiveScript
    Passive passiveScript;

    // EnemyScript
    BattleModeEnemy enemyScript;

    // Selected Cards
    List<GameObject> selectedCard;

    Transform iconTxt;

    // Active Card's List
    public List<GameObject> activeList;
    // Defence Value
    public int block;

    // Card Turn's Count
    int count;

    // Passive Turn's Count
    int passiveCnt;

    // Battle's Speed
    static int battleSpeed = 1000;

    // HP Count
    public int enemyLife;

    // Damage Value
    public int dmg;

    // SE Type
    string SEType = "";

    // Enemy's Dead
    public bool isDead;

    // �o������
    public bool isBleeding;

    // Pause Check
    bool isPause;

    // ���틭���t���O
    bool isForge;

    // �s�X�g���g�p�\�t���O
    bool isReload;

    bool isFight;
    // Panel's Active Check
    bool panelActive;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        iconTxt = protectIcon.transform.Find("Text");
        // SetSE
        this.gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();

        attackSE = (AudioClip)Resources.Load("SE/NormalAttack");
        heavyAttackSE = (AudioClip)Resources.Load("SE/HeavyAttack");
        boomAttackSE = (AudioClip)Resources.Load("SE/Boom");
        parrySE = (AudioClip)Resources.Load("SE/Parry");
        defenceSE = (AudioClip)Resources.Load("SE/Defence");
        clearSE = (AudioClip)Resources.Load("SE/Clear");
        clickSE = (AudioClip)Resources.Load("SE/Click");

        // ��D�J�[�h��ݒ�
        handCard = new List<GameObject>();
        SetCard();

        // Set Battle Speed
        // battleSpeed = (int)Math.Ceiling(gameSpeedSlider.value);
        gameSpeedSlider.value = battleSpeed;

        // Set GameState
        isDead = false;
        isPause = false;
        isBleeding = false;
        isFight = false;
        isForge = false;
        isReload = false;

        // Set Buttons
        turnEndButton.SetActive(false);
        button.SetActive(false);
        protectIcon.SetActive(false);

        // Set Panels
        panelActive = false;

        // Get for Enemy Object from tag
        enemyTexture = GameObject.FindGameObjectWithTag("Enemy");
        // Get for Enemy Life from tag
        enemyHP = GameObject.FindGameObjectsWithTag("EnemyHP");
        // Add Health Value from got one
        healthScript = FindObjectOfType<Health>();
        enemyLife = healthScript.BattleModeHealth;

        enemyScript = FindObjectOfType<BattleModeEnemy>();

        // Set Lists
        activeList = new List<GameObject>();
        selectedCard = new List<GameObject>();

        // Set Counts
        count = 0;
        passiveCnt = 0;
        block = 0;
        dmg = 0;

        // Add Passive Value from got one
        passiveScript = FindObjectOfType<Passive>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);

        battleSpeed = (int)Math.Ceiling(gameSpeedSlider.value);
        if (protectIcon != null) iconTxt.gameObject.GetComponent<Text>().text = block.ToString();

        if (enemyScript.isDead == true)
        {
            button.SetActive(true);
        }

        if (enemyLife <= 0)
        {
            isDead = true;

            for (int i = 0; i < selectedCard.Count; i++)
            { // Reset Color
                selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            button.SetActive(true);

            enemyTexture.SetActive(false);
        }
        // If Selected Card's Value Will 4
        if (activeList.Count == 4 && turnEndButton != null)
        {
            // Can Use TurnEnd Button
            turnEndButton.SetActive(true);
        }
        else if (activeList.Count != 4 && turnEndButton != null)
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
        if (isDead == true || isPause == true || enemyScript.isDead == true || isFight == true) return;

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
            else if (panelActive != true && hitObj.tag == "Enemy")
            {
                enemyScript.EnemyExplain(hitObj);
            }
            else if (panelActive != true && hitObj.tag == "EnemyAction")
            {
                infoText.color = Color.white;
                switch (hitObj.name)
                {
                    case "Sword":
                        infoText.text = "Sword:1�_���[�W��^����";
                        break;

                    case "DeathS.Y.T.H":
                        infoText.text = "DeathS.Y.T.H:3�_���[�W��^�����F�H�̈����劙";
                        break;

                    case "S.Y.T.H":
                        infoText.text = "S.Y.T.H:2�_���[�W��^����";
                        break;

                    case "A.X.E":
                        infoText.text = "A.X.E:1�_���[�W��^����\n�u���b�N�𖳎����j��";
                        break;

                    case "M.A.C.E":
                        infoText.text = "M.A.C.E:1�_���[�W�ɉ����u���b�N�̒l���_���[�W��^����";
                        break;

                    case "T.N.T":
                        infoText.text = "T.N.T:��œI�ȃ_���[�W��^����...";
                        break;

                    case "Shield":
                        infoText.text = "Shield:1�u���b�N���󂯂�";
                        break;

                    case "Reflection":
                        infoText.text = "Reflection:�U���𔽎˂���o���A��W�J";
                        break;

                    case "ForgeHammer":
                        infoText.text = hitObj.name + ":1�_���[�W��^����\n���̍s���̍U����+1";
                        break;

                    case "Injector":
                        infoText.text = hitObj.name + ":1�_���[�W��^����\n�G���o��������(�s����1�_���[�W)";
                        break;

                    case "PoisonKnife":
                        infoText.text = hitObj.name + ":1�_���[�W��^����\n�G�̍U����-1";
                        break;

                    case "6mmBullet":
                        infoText.text = hitObj.name + ":3�_���[�W��^����\n...�e������΂̘b";
                        break;

                    case "SwatShield":
                        infoText.text = hitObj.name + ":2�u���b�N���󂯂�";
                        break;
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
                    infoText.color = Color.white;
                    // Update Info 
                    switch (hitObj.name)
                    {
                        case "Sword":
                            infoText.text = hitObj.name + ":1�_���[�W��^����";
                            break;

                        case "S.Y.T.H":
                            infoText.text = hitObj.name + ":2�_���[�W��^����";
                            break;

                        case "A.X.E":
                            infoText.text = hitObj.name + ":1�_���[�W��^����\n�u���b�N�𖳎����j��";
                            break;

                        case "M.A.C.E":
                            infoText.text = hitObj.name + ":1�_���[�W�ɉ����u���b�N�̒l���_���[�W��^����";
                            break;

                        case "T.N.T":
                            infoText.text = hitObj.name + ":��œI�ȃ_���[�W��^����...";
                            break;

                        case "Shield":
                            infoText.text = hitObj.name + ":1�u���b�N���󂯂�";
                            break;

                        case "ForgeHammer":
                            infoText.text = hitObj.name + ":1�_���[�W��^����\n���̍s���̍U����+1";
                            break;

                        case "Injector":
                            infoText.text = hitObj.name + ":1�_���[�W��^����\n�G���o��������(�s����1�_���[�W)";
                            break;

                        case "PoisonKnife":
                            infoText.text = hitObj.name + ":1�_���[�W��^����\n�G�̍U����-1";
                            break;

                        case "6mmBullet":
                            infoText.text = hitObj.name + ":3�_���[�W��^����\n...�e������΂̘b";
                            break;

                        case "SwatShield":
                            infoText.text = hitObj.name + ":2�u���b�N���󂯂�";
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
                // �^�b�`�����I�u�W�F�N�g���f�B���N�V���i������擾
                infoText.text = passiveDictionary[hitObj.name].Name + ":" + passiveDictionary[hitObj.name].Explain;
            }
        }
    }

    /// <summary>
    /// Turn End Process
    /// </summary>
    public async void TurnEnd()
    {
        isFight = true;
        Destroy(turnEndButton);
        if (isDead == true || isPause == true || enemyScript.isDead == true) return;
        // Reset Info
        infoText.text = "";

        foreach (var item in activeList)
        {
            if (isDead == true || enemyScript.isDead == true || enemyScript.isDead == true) return;
            bool isBlock = false;
            switch (item.tag) //Judge the Card's Tag
            {
                case "Attack": //�J�[�h�^�C�v���U���̏ꍇ
                    //Active to Passives
                    passiveEffect(item);

                    // �J�[�h���ʏ���
                    switch (item.name)
                    {
                        case "Sword":
                            dmg += 1;
                            infoText.text = "You:" + dmg + "�_���[�W��^����";
                            SEType = "light";
                            break;

                        case "A.X.E":
                            dmg += 1;
                            enemyScript.block = 0;
                            infoText.text = "You:�V�[���h�j��I\n" + dmg + "�_���[�W��^����";
                            SEType = "heavy";
                            break;

                        case "S.Y.T.H":
                            dmg += 2;
                            infoText.text = "You:" + dmg + "�_���[�W��^����";
                            SEType = "heavy";
                            break;

                        case "M.A.C.E":
                            dmg += block;
                            if (dmg < 3)
                            {
                                infoText.text = "You:" + dmg + "�_���[�W��^����";
                            }
                            else infoText.text = "You:�Ӑg�̈ꌂ!\n" + dmg + "�_���[�W��^����!";
                            SEType = "heavy";

                            break;

                        case "T.N.T":
                            dmg += 999;
                            infoText.text = "You:�h�J�[��!\n" + dmg + "�_���[�W��^����!";
                            SEType = "boom";
                            break;

                        case "ForgeHammer":
                            dmg += 1;
                            infoText.text = "You:" + dmg + "�_���[�W��^����\n���̍s���ōU����+1";
                            SEType = "heavy";
                            isForge = true;
                            break;

                        case "Injector":
                            enemyScript.isBleeding = true;
                            dmg += 1;
                            infoText.text = "You:" + dmg + "�_���[�W��^����\n����ɏo����t�^!";
                            SEType = "light";
                            break;

                        case "PoisonKnife":
                            enemyScript.dmg -= 1;
                            dmg += 1;
                            infoText.text = "You:" + dmg + "�_���[�W��^����\n����͓łɂ��U����-1";
                            SEType = "light";
                            break;

                        case "6mmBullet":
                            if (isReload == true)
                            {
                                dmg += 3;
                                infoText.text = "You:�f�����e�ۂ����߁A�g���K�[���������I\n" + dmg + "�_���[�W��^����";
                                isReload = false;
                            }
                            else
                            {
                                dmg = 0;
                                infoText.text = "You:�d���Ȃ��e�ۂ𓊂����I\n�Ӗ����Ȃ��I" + dmg + "�_���[�W��^����";
                            }

                            SEType = "light";
                            break;
                    }

                    // �G���u���b�N�������Ă��邩���J�[�h��A.X.E�ł͂Ȃ��ꍇ
                    if (enemyScript.block != 0 && item.name != "A.X.E")
                    {
                        // �u���b�N�l�����炷
                        enemyScript.block--;
                        if (enemyScript.block <= 0) if (enemyScript.protectIcon != false) enemyScript.protectIcon.SetActive(false);
                        blockEffect();
                        isBlock = true;

                        // �Ή�����������UI���o��
                        audioSource.PlayOneShot(parrySE);
                        infoText.text = "Rival:�U�����u���b�N";
                        dmg = 0;
                    }
                    else
                    {
                        // ���U���l���J��Ԃ�
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
                        }
                        if (enemyScript.playerLife <= 0) enemyScript.isDead = true;

                        // �G�����񂾏ꍇ
                        if (enemyLife <= 0) isDead = true;
                    }
                    // �����SE
                    switch (SEType)
                    {
                        case "light":
                            audioSource.PlayOneShot(attackSE);
                            break;

                        case "heavy":
                            audioSource.PlayOneShot(heavyAttackSE);
                            break;

                        case "boom":
                            audioSource.PlayOneShot(boomAttackSE);
                            break;
                    }

                    attackEffect(isBlock);
                    item.SetActive(false);
                    break;

                case "Defence":�@//�J�[�h�^�C�v���h��̏ꍇ
                    audioSource.PlayOneShot(defenceSE);
                    passiveEffect(item);
                    switch (item.name)
                    {
                        case "Shield":
                            block++;
                            infoText.text = "You:" + block + "�u���b�N���󂯂�";
                            break;

                        case "SwatShield":
                            block += 2;
                            infoText.text = "You:" + block + "�u���b�N���󂯂�";
                            break;

                        case "SpikeShield":
                            block++;
                            infoText.text = "You:" + block + "�u���b�N���󂯂�\n�V�[���h�o�b�V�������܂����I" + dmg + "�_���[�W��^����";

                            //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                            if (enemyLife <= 0)
                            {
                                break;
                            }
                            //�\�������炷
                            Destroy(enemyHP[(enemyLife - 1)]);

                            //���������炷
                            enemyLife--;

                            // if Enemy Dead
                            if (enemyLife <= 0) isDead = true;

                            attackEffect(isBlock);
                            break;
                    }

                    // �u���b�N�l��0�ȏォ�A�C�R������������Ă��Ȃ��ꍇ
                    if (block > 0)
                    {
                        protectIcon.SetActive(true);
                        iconTxt.gameObject.GetComponent<Text>().text = block.ToString();
                    }
                    break;

                case "Support":
                    passiveEffect(item);
                    break;

                default:
                    break;
            }

            item.SetActive(false);
            await Task.Delay(battleSpeed);

            // �G�����S���Ă����ꍇ
            if (isDead == true)
            {
                // �e�L�X�g��\�����A�N���A�T�E���h���Đ�
                infoText.text = "�����I";
                audioSource.PlayOneShot(clearSE);
                return;
            }

            // ���񂾏ꍇ�e�L�X�g��\�����A���^�[������
            if (enemyScript.isDead == true)
            {
                infoText.text += "\n�s�k...";
                return;
            }

            // Enemy's Action
            enemyScript.Attack();

            // �G�����S���v���C���[�����S���Ă��Ȃ��ꍇ
            if (isDead == true && enemyScript.isDead != true)
            {
                // �G���S���̃e�L�X�g�\������
                if (enemyScript.isBleeding == true)
                {
                    infoText.text = "\nRival:�o����!\n�����I";
                }
                audioSource.PlayOneShot(clearSE);
                return;
            }

            //�@�v���C���[���S���̃e�L�X�g�\��
            if (enemyScript.isDead == true)
            {
                infoText.text += "\n�s�k...";
                return;
            }

            // �t�H�[�W�n���}�[���g�p�����ꍇDMG+1
            if (isForge == true)
            {
                dmg = 1;
                isForge = false;
            }
            else dmg = 0;

            if (block <= 0) if (protectIcon != null) protectIcon.SetActive(false);
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
        infoText.text = "���j���s...\n���C�o���̏���";
        button.SetActive(true);
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
                switch (passive[passiveCnt])
                {
                    case "Spike":
                        dmg++;
                        break;
                    case "Slime":
                        enemyScript.dmg--;
                        break;
                    case "VampireWrench":
                        enemyScript.playerLife++;
                        break;
                    case "HandyDrill":
                        enemyScript.block = 0;
                        break;

                    case "HandGun":
                        if (item.name == "6mmBullet")
                        {
                            isReload = true;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "Defence":
                switch (passive[passiveCnt])
                {
                    case "Spike":
                        if (item.name == "Shield")
                        {
                            dmg++;
                        }
                        break;
                    case "ArmorChip":

                        if (block <= 0)
                        {
                            protectIcon.SetActive(true);
                            iconTxt.gameObject.GetComponent<Text>().text = block.ToString();
                        }

                        block++;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        if (passiveCnt < (passive.Count - 1)) passiveCnt++;
    }

    public void endGame()
    {
        SceneManager.LoadScene("Battle");
    }

    /// <summary>
    /// Attack's Effect Progress
    /// </summary>
    async void attackEffect(bool isBlock)
    {
        // Get Card's GameObjects from Resources Folder
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
    /// Pause Button Progress
    /// </summary>
    public void pause()
    {
        if (isFight == true) return;

        infoText.text = "";
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

    /// <summary>
    /// ��D�J�[�h��\�����鏈��
    /// </summary>
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