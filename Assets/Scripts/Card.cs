/*
 * CardScript
 * Creator:���Y�W�� Update:2024/07/25
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Card : MonoBehaviour
{
    //Active Cards
    [SerializeField] List<GameObject> card;

    //TurnEnd Button
    [SerializeField] GameObject button;

    //Passive's GameObject
    GameObject spike;
    GameObject armorChip;

    //Passives
    List<GameObject> passive;

    //HealthScript
    Health healthScript;

    //EnemyScript
    Enemy enemyScript;

    //Selected Cards
    List<GameObject> selectedCard;

    //Active Card's List
    [SerializeField] List<GameObject> activeList;

    //Card Turn's Count
    int count;

    //Passive Turn's Count
    int passiveCnt;

    //Enemy's HP
    GameObject[] enemyHP;

    //HP Count
    int enemyLife;

    //Damage Value
    int dmg;

    //Defence Value
    int block = 1;

    //Enemy's Defence Value
    int enemBlock;

    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
        //Get for Enemy Life from tag
        enemyHP = GameObject.FindGameObjectsWithTag("EnemyHP");
        //�擾����HP�̌�����
        healthScript = FindObjectOfType<Health>();
        enemyLife = healthScript.EnemHealth;

        enemyScript = FindObjectOfType<Enemy>();

        //Set Lists
        activeList = new List<GameObject>();
        selectedCard = new List<GameObject>();
        //Set Counts
        count = 0;
        passiveCnt = 0;
        enemBlock = 0;

        //�e�p�b�V�u�̐ݒ�(��)
        spike = new GameObject();
        spike.name = "spike";
        armorChip = new GameObject();
        armorChip.name = "armorChip";
        passive = new List<GameObject>();
        for (int i = 1; i < 3; i++) passive.Add(spike);
        passive.Add(armorChip);
    }

    // Update is called once per frame
    void Update()
    {
        //�J�[�h�I��
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
        //�^�b�`�����ʒu�Ƀ��C���΂�
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //������ ���C�͂ǂ̕����ɐi�ނ�(zero=�w��_)
        RaycastHit2D hit2d = Physics2D.Raycast(worldPosition, Vector2.zero);

        //�����蔻��
        if (hit2d)
        {
            //�q�b�g�����I�u�W�F�N�g�擾
            GameObject hitCard = hit2d.collider.gameObject;

            if (!selectedCard.Contains(hitCard)) //�����I�����Ă��Ȃ������ꍇ
            {
                //Change the Card's Color
                hitCard.GetComponent<Renderer>().material.color = new Color32(127, 127, 127, 255);
                //Add List for Selected Card
                selectedCard.Add(hitCard);

                foreach (var item in card)
                {
                    if (item.name == hitCard.name) //�I���J�[�h�ƌ��݂̃J�[�h�̖��O����v�����ꍇ
                    {
                        // Create Instance from Now Turn's Cards
                        GameObject obj = Instantiate(item, new Vector2(-8.0f + (2.0f * count), -3.5f), Quaternion.identity);
                        //�I�u�W�F�N�g�̐F�����
                        obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                        //Rename Item
                        obj.name = item.name;
                        //Add Tag for Clone Items
                        obj.tag = item.tag;
                        //Add ActiveList
                        activeList.Add(obj);
                        //���������Z
                        count++;
                    }
                }
            }
            else //���łɃJ�[�h��I�����Ă����ꍇ
            {
                //�I���J�[�h�̐F��߂�
                hitCard.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                //Remove Selected Card from Lists
                selectedCard.Remove(hitCard);

                foreach (var item in activeList)
                {
                    if (item.name == hitCard.name)
                    {
                        //Delete Card
                        Destroy(item);
                        //Remove from List
                        activeList.Remove(item);
                        //Refresh
                        cardRefresh();
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Turn End Process
    /// </summary>
    public async void TurnEnd()
    {
        if (activeList.Count != 4)
        {
            Debug.Log("Should Use 4 Cards");
        }
        else
        {
            foreach (var item in activeList)
            {
                dmg = 1;

                switch (item.tag) //Judge the Card's Tag
                {
                    case "Attack":

                        //Active to Passives
                        passiveEffect(item);

                        //Loop to Damage Values
                        for (int i = 0; i < dmg; i++)
                        {
                            if (enemBlock != 0)
                            {
                                Debug.Log("�h���ꂽ�I");
                                enemBlock--;
                            }
                            else
                            {
                                //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                                if (enemyLife <= 0)
                                {
                                    Debug.Log("�G���񂾁I");
                                    break;
                                }
                                //�\�������炷
                                Destroy(enemyHP[(enemyLife - 1)]);

                                //���������炷
                                enemyLife--;
                                Debug.Log("Enemy's HP:" + enemyLife);
                            }
                        }
                        Destroy(item);

                        await Task.Delay(1000);
                        enemyAction();
                        break;

                    case "Defence":
                        Debug.Log("�h��");
                        passiveEffect(item);
                        block++;
                        Destroy(item);

                        await Task.Delay(1000);
                        enemyAction();
                        break;

                    case "Support":
                        Debug.Log("�⏕");
                        passiveEffect(item);
                        Destroy(item);

                        await Task.Delay(1000);
                        enemyAction();
                        break;

                    default:
                        break;
                }
            }

            for (int i = 0; i < selectedCard.Count; i++)
            { //Reset Color
                selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            //Delete All List's Items
            selectedCard.Clear();
            activeList.Clear();
            //Reset Count
            count = 0;
            button.SetActive(true);
        }
    }

    /// <summary>
    /// Card Refresh Process
    /// </summary>
    private void cardRefresh()
    {
        //�ꎞ�I�Ɍ��݃A�N�e�B�u�ȃJ�[�h�������郊�X�g
        List<GameObject> keepList = new List<GameObject>();
        foreach (var item in activeList) //�A�N�e�B�u�ȃJ�[�h��j�󂵁A�����X�g�ɒǉ�
        {
            Destroy(item);
            keepList.Add(item);
        }
        //���X�g���ɂ���J�[�h�S�Ă�����
        activeList.Clear();
        //���������Z�b�g
        count = 0;

        foreach (var item in keepList) //���ׂȂ���
        {
            //���݂̃J�[�h�v���n�u�����ɁA�C���X�^���X�𐶐��A
            GameObject obj = Instantiate(item, new Vector2(-8.0f + (2.0f * count), -3.5f), Quaternion.identity);
            //�I�u�W�F�N�g�̐F�����
            obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            //�N���[�������I�u�W�F�N�g�̖��O�����
            obj.name = item.name;
            //�J�[�h�̃^�O���N���[���I�u�W�F�N�g�ɂ��ǉ�
            obj.tag = item.tag;
            activeList.Add(obj);
            //���������Z
            count++;
        }
    }

    /// <summary>
    /// Passive's Effect Process
    /// </summary>
    /// <param name="item"></param>
    private void passiveEffect(GameObject item)
    {
        //Select to Passive's name
        switch (passive[passiveCnt].name)
        {
            case "spike":
                if (item.tag == "Attack")
                {
                    Debug.Log("�Ƃ����� DMG+1");
                    dmg++;
                }
                break;
            case "armorChip":
                if (item.tag == "Defence")
                {
                    Debug.Log("�A�[�}�[�`�b�v���� DEF+1");
                    block++;
                }
                break;
        }

        if (passiveCnt < (passive.Count - 1)) passiveCnt++;
    }

    /// <summary>
    /// Enemy's Action Process
    /// </summary>
    async void enemyAction()
    {
        if (enemyLife <= 0)
        {
            return;
        }
        //enemyScript.Attack();

        if (enemyScript.Attack() == "defence")
        {
            enemBlock++;
        }
        await Task.Delay(1000);
    }
    public void endGame()
    {
        SceneManager.LoadScene("SelectScene");
    }

    void cardType()
    {

    }
}