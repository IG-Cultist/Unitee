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

public class Enemy : MonoBehaviour
{
    // Parent
    [SerializeField] GameObject parent;

    //�GHP
    GameObject[] playerHP;
    //HP�c��
    int playerLife;
    //�h��l
    public int block;
    //�U���l
    public int dmg;

    int count;
    // Health Script
    Health healthScript;

    // Block Icon
    public GameObject protectIcon;

    public bool isDead;

    //Card Script
    Card cardScript;

    // Deck Panel
    [SerializeField] Text infoText;

    [SerializeField] GameObject health;

    [SerializeField] List<string> actionList;

    enum EnemyType
    {
        Viper = 4,
        Slime = 4,
        Face = 4
    }

    // Start is called before the first frame update
    void Start()
    {
        cardScript = FindObjectOfType<Card>();

        isDead = false;
        dmg = 1;
        count = 0;
        block = 0;

        // �s���A�C�R���𐶐�
        SetActions();
    }

    // Update is called once per frame
    void Update()
    {
        if(block <= 0) if (protectIcon != null) Destroy(protectIcon);
    }

    /// <summary>
    /// �s������
    /// </summary>
    public string Attack()
    {
        if (count == 0)
        {
            //�v���C���[HP���^�O�Ŏ擾
            playerHP = GameObject.FindGameObjectsWithTag("PlayerHP");
            healthScript = FindObjectOfType<Health>();
            playerLife = healthScript.PlayerHealth;
        }

        switch (actionList[count])
        {
            case "Sword":
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                break;

            case "M.A.C.E":
                dmg += block;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����";
                break;

            case "T.N.T":
                dmg = 999;
                infoText.text = "Enemy:�h�J�[��!" + dmg + "�_���[�W��^����";
                break;

            case "Shield":
                if (block <= 0)
                {
                    //Get Card's GameObjects from Resources Folder
                    GameObject prefab = (GameObject)Resources.Load("ProtectIcon");

                    // Create Instance from Now Turn's Cards
                    protectIcon = Instantiate(prefab, new Vector2(8.4f, 2.0f), Quaternion.identity);
                    protectIcon.name = "ProtectIcon";
                }
                block++;
                infoText.text = "Enemy:" + block + "�u���b�N���󂯂�";
                break;
        }

        for (int i = 0; i < dmg; i++)
        {
            if (cardScript.block != 0)
            {
                blockEffect();
                cardScript.block--;
                infoText.text = "Enemy:" + dmg + "�_���[�W��^����\nYou:�U�����u���b�N";
                infoText.color = Color.white;
            }
            else
            {
                //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                if (playerLife <= 0)
                {
                    isDead = true;
                    break;
                }
                //�\��HP�����炷
                Destroy(playerHP[(playerLife - 1)]);

                //���������炷
                playerLife--;
                damageEffect();
            }
        }

        infoText.color = Color.white;

        dmg = 1;
        if (count < (actionList.Count - 1)) count++;

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

            item.AddComponent<BoxCollider2D>();
            item.GetComponent<BoxCollider2D>().isTrigger = true;

            item.transform.SetParent(parent.transform, false);
            cnt++;
        }
    }
}
