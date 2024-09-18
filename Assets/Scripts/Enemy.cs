/*
 * EnemyScript
 * Creator:���Y�W�� Update:2024/09/02
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    // Parent
    [SerializeField] GameObject parent;

    //�GHP
    public GameObject[] playerHP;
    //HP�c��
    public int playerLife;
    //�h��l
    public int block;

    //���˂��邩
    public bool isReflect;

    //�U���l
    public int dmg;

    int count;
    // Health Script
    Health healthScript;

    // Block Icon
    public GameObject protectIcon;

    List<GameObject> actionObject;

    public bool isDead;

    //Card Script
    Card cardScript;

    // Deck Panel
    [SerializeField] Text infoText;

    [SerializeField] GameObject health;

    [SerializeField] List<string> actionList;

    AudioSource audioSource;

    // Attack's SoundEffect
    AudioClip attackSE;

    // Heavy Attack's SoundEffect
    AudioClip heavyAttackSE;

    // Boom Attack's SoundEffect
    AudioClip boomAttackSE;

    //Parry's SoundEffect
    AudioClip parrySE;

    //Defence's SoundEffect
    AudioClip defenceSE;

    // Boom Attack's SoundEffect
    AudioClip poisonSE;

    //Parry's SoundEffect
    AudioClip copySE;

    //Parry's SoundEffect
    AudioClip waitSE;

    Transform iconTxt;

    string SEType;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {        //�v���C���[HP���^�O�Ŏ擾
        playerHP = GameObject.FindGameObjectsWithTag("PlayerHP");
        healthScript = FindObjectOfType<Health>();
        playerLife = healthScript.PlayerHealth;
        iconTxt = protectIcon.transform.Find("Text");
        // SetSE
        this.gameObject.AddComponent<AudioSource>(); 
        audioSource = GetComponent<AudioSource>();

        attackSE = (AudioClip)Resources.Load("SE/EnemyAttack");
        heavyAttackSE = (AudioClip)Resources.Load("SE/HeavyAttack");
        boomAttackSE = (AudioClip)Resources.Load("SE/Boom");
        parrySE = (AudioClip)Resources.Load("SE/Parry");
        defenceSE = (AudioClip)Resources.Load("SE/Defence");

        poisonSE = (AudioClip)Resources.Load("SE/Poison");
        copySE = (AudioClip)Resources.Load("SE/Copy");
        waitSE = (AudioClip)Resources.Load("SE/Wait");

        if (block != 0)
        {
            protectIcon.SetActive(true);
        }
        else
        {
            protectIcon.SetActive(false);
        }

        actionObject = new List<GameObject>();
        cardScript = FindObjectOfType<Card>();

        isDead = false;
        dmg = 0;
        count = 0;

        // �s���A�C�R���𐶐�
        SetActions();
    }

    // Update is called once per frame
    void Update()
    {
        if (protectIcon != null) iconTxt.gameObject.GetComponent<Text>().text = block.ToString();

        if (isReflect == false) this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        if (block <= 0 ) if (protectIcon != null) protectIcon.SetActive(false);
    }

    /// <summary>
    /// �s������
    /// </summary>
    public string Attack()
    {
        if(count > 0) actionObject[count-1].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        isReflect = false;
        actionObject[count].GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 255);
        SEType = "";
        switch (actionList[count])
        {
            case "Wait":
                dmg = 0;
                infoText.text = "Enemy:�l�q�����������Ă���";
                SEType = "wait";
                break;

            case "Copy":
                SEType = "copy";
                if (cardScript.activeList[count].tag == "Attack")
                {
                    if (cardScript.activeList[count].name == "A.X.E")
                    {
                        dmg = 1;
                        cardScript.block = 0;
                    }
                    else if (cardScript.activeList[count].name == "M.A.C.E")
                    {
                        dmg += block;
                    }
                    else
                    {
                        dmg = cardScript.dmg;
                    }
                }
                else
                {
                    if (block <= 0)
                    {
                        protectIcon.SetActive(true);
                    }
                    block++;
                    iconTxt.gameObject.GetComponent<Text>().text = block.ToString();
                }

                infoText.text = "Enemy:�G�̓v���C���[�̍s�����R�s�[�����I";
                break;

            case "Sword":
                dmg += 1;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                SEType = "light";
                break;

            case "Poison":
                dmg = 0;
                cardScript.dmg -= 1;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����\nPlayer�͓łɂ��DMG-1";
                SEType = "poison";
                break;

            case "Destruction":
                dmg = 999;
                audioSource.PlayOneShot(boomAttackSE);
                cardScript.isDead = true;
                cardScript.enemyLife = 0;
                infoText.text = "Enemy:���������I";

                break;

            case "M.A.C.E":
                dmg += block;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                SEType = "heavy";
                break;

            case "DeathS.Y.T.H":
                dmg += 3;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                SEType = "heavy";
                break;

            case "S.Y.T.H":
                dmg += 2;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                SEType = "heavy";
                break;

            case "T.N.T":
                dmg = 999;
                infoText.text = "Enemy:�h�J�[��!" + dmg + "�_���[�W��^����";
                SEType = "boom";
                break;

            case "A.X.E":
                dmg += 1;
                cardScript.block = 0;
                infoText.text = "Enemy:�V�[���h�j��I" + dmg + "�_���[�W��^����";
                SEType = "heavy";
                break;

            case "Shield":
                dmg = 0;
                if (block <= 0)
                {
                    protectIcon.SetActive(true);
                }
                audioSource.PlayOneShot(defenceSE);
                block++;
                iconTxt.gameObject.GetComponent<Text>().text = block.ToString();
                infoText.text = "Enemy:" + 1 + "�u���b�N���󂯂�";
                break;

            case "Reflection":
                dmg = 0;
                isReflect = true;
                if (isReflect == true)
                {
                    this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 255);
                }
                infoText.text = "Enemy:���˃o���A��W�J";
                audioSource.PlayOneShot(defenceSE);
                break;
        }

        if (cardScript.block != 0 && dmg > 0)
        {
            audioSource.PlayOneShot(parrySE);
            blockEffect();
            cardScript.block--;
            infoText.text += "\nYou:�U�����u���b�N";
            infoText.color = Color.white;
        }
        else
        {
            // �����SE
            switch (SEType)
            {
                case "wait":
                    audioSource.PlayOneShot(waitSE);
                    break;

                case "copy":
                    audioSource.PlayOneShot(copySE);
                    break;

                case "light":
                    audioSource.PlayOneShot(attackSE);
                    break;

                case "heavy":
                    audioSource.PlayOneShot(heavyAttackSE);
                    break;

                case "boom":
                    audioSource.PlayOneShot(boomAttackSE);
                    break;

                case "poison":
                    audioSource.PlayOneShot(poisonSE);
                    break;

                    default:
                    break;
            }

            for (int i = 0; i < dmg; i++)
            {
                //�\��HP�����炷
                Destroy(playerHP[(playerLife - 1)]);

                //���������炷
                playerLife--;
                damageEffect();

                //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                if (playerLife <= 0)
                {
                    isDead = true;
                    break;
                }
            }
        }

        infoText.color = Color.white;

        if (count < (actionList.Count - 1)) count++;
        dmg = 0;
        return actionList[count];
    }

    /// <summary>
    /// Damage's Effect Progress
    /// </summary>
    async void damageEffect()
    {
        //Get Card's GameObjects from Resources Folder
        GameObject prefab = (GameObject)Resources.Load("AttackFlash");

        // Create Instance from Now Turn's Cards
        GameObject obj = Instantiate(prefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        await Task.Delay(100);
        Destroy(obj);
    }

    /// <summary>
    /// Block's Effect Progress
    /// </summary>
    async void blockEffect()
    {
        //Get Card's GameObjects from Resources Folder
        GameObject prefab = (GameObject)Resources.Load("BlockFlash");

        // Create Instance from Now Turn's Cards
        GameObject obj = Instantiate(prefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        await Task.Delay(100);
        Destroy(obj);
    }

    /// <summary>
    /// Set Actions
    /// </summary>
    void SetActions()
    {
        int cnt = 0;

        foreach (var action in actionList)
        {
            //  Get Prefabs from List
            GameObject obj = (GameObject)Resources.Load("Cards/" + action);
            // Create Action Objects
            GameObject item = Instantiate(obj, new Vector2(3.4f+ (1.5f * cnt), 3.7f), Quaternion.identity);
            // Rename
            item.name = action;
            item.tag = "EnemyAction";

            item.AddComponent<BoxCollider2D>();
            item.GetComponent<BoxCollider2D>().isTrigger = true;

            item.transform.SetParent(parent.transform, false);
            actionObject.Add(item);
            cnt++;
        }
    }

    /// <summary>
    /// EnemyExplain
    /// </summary>
    public void EnemyExplain(GameObject enemy)
    {
        infoText.color = Color.white;
        switch (enemy.GetComponent<SpriteRenderer>().sprite.name)
        {        
            case "OffensiveSlime":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���������ĉ��l���E���Ă��鋥�\�ȃX���C��";
                break;
            case "SwatSlime":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���܂��܏E������i�I�ȑ����Őg���ł߂��X���C��";
                break;
            case "Lich":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���̂����ł͂Ȃ��_�C�i�}�C�g�Ŏ��̐鍐���s���ςȃ��b�`";
                break;
            case "Knight":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���ʂȌ��Z���g���㋉���m\n���ʂɍ������";
                break;
            case "Spider":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":�_�o�łŒE�͔�����N�������Ƃ���׈��Ȓw�";
                break;
            case "Sn@il":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���̕ϓN���Ȃ����Q�ȃJ�^�c����";
                break;
            case "CopyBot":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":������̍s�����قڊ��؂��ɃR�s�[���Ă�����f�ȃ{�b�g";
                break;
            case "MirrorBot":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":�o���A�W�J�ɓ��������o���A�t���[����Ȃ��{�b�g";
                break;
            case "Mine":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":�C���̍r���n��\n���Ƀf���P�[�g";
                break;
            case "Ghost":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���b�`�ȏ�̒n�ʂ�ۗL���Ă����F�̈����S�[�X�g";
                break;
            case "KingSlime":
                infoText.text = enemy.GetComponent<SpriteRenderer>().sprite.name + ":���ʂɒn�ʂ̂��邽���̘V���ڂ�X���C��";
                break;
        }
        if (SceneManager.GetActiveScene().name == "Tutorial") infoText.text += "\n�E�̃n�[�g���U���Ō��点�΃v���C���[�̏����ƂȂ�";
    }
}
