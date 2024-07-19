using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    //���ݎ����Ă���J�[�h
    [SerializeField] List<GameObject> card;
    //�w���X�X�N���v�g
    private Health healthScript;
    //�I���J�[�h
    List<GameObject> selectedCard;
    //���݂̃f�b�L�̃J�[�h
    GameObject[] activeCards;
    //�A�N�e�B�u�ȃJ�[�h
    [SerializeField] List<GameObject> activeList;
    //�����Ǘ�
    int count;
    //�GHP
    GameObject[] enemyHP;
    //HP�c��
    int enemyLife;

    // Start is called before the first frame update
    void Start()
    {
        //�GHP���^�O�Ŏ擾
        enemyHP = GameObject.FindGameObjectsWithTag("HP");
        //�擾����HP�̌�����
        healthScript = FindObjectOfType<Health>();
        enemyLife = healthScript.EnemHealth;
        activeList = new List<GameObject>();
        selectedCard = new List<GameObject>();
        //������������
        count = 0;
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
    /// �J�[�h�N���b�N����
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
                //�I���J�[�h�̐F��ύX
                hitCard.GetComponent<Renderer>().material.color = new Color32(127, 127, 127, 255);
                //���X�g�ɑI���J�[�h��ǉ�
                selectedCard.Add(hitCard);

                foreach (var item in card)
                {
                    if (item.name == hitCard.name) //�I���J�[�h�ƌ��݂̃J�[�h�̖��O����v�����ꍇ
                    {
                        // ���݂̃J�[�h�v���n�u�����ɁA�C���X�^���X�𐶐��A
                        GameObject obj = Instantiate(item, new Vector2(-8.0f + (2.0f * count), -3.5f), Quaternion.identity);
                        //�I�u�W�F�N�g�̐F�����
                        obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                        //�N���[�������I�u�W�F�N�g�̖��O�����
                        obj.name = item.name;
                        //�J�[�h�̃^�O���N���[���I�u�W�F�N�g�ɂ��ǉ�
                        obj.tag = item.tag;
                        //activeList���X�g�ɒǉ�
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
                //���X�g����I���J�[�h������
                selectedCard.Remove(hitCard);

                foreach (var item in activeList)
                {
                    if (item.name == hitCard.name)
                    {
                        //���̃J�[�h������
                        Destroy(item);
                        //�������J�[�h�����X�g���������
                        activeList.Remove(item);
                        count--;
                        cardRefresh();
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �^�[���I������
    /// </summary>
    public void TurnEnd()
    {
        foreach (var item in activeList)
        {
            switch (item.tag) //�J�[�h�̃^�O�𔻒�
            {
                case "Attack": //�J�[�h���A�^�b�N�̏ꍇ
                    Debug.Log("�U��");
                    if (enemyLife <= 0) //HP�c�ʂ�0�̏ꍇ
                    {
                        Debug.Log("�G���񂾁I");
                    }
                    else //�܂������񂫂ȏꍇ
                    {
                        //�\��HP��1���炷
                        Destroy(enemyHP[(enemyLife - 1)]);
                        //������1���炷
                        enemyLife--;
                        Debug.Log("HP:" + enemyLife);
                        if (enemyLife <= 0) //HP�c�ʂ�0�̏ꍇ
                        {
                            Debug.Log("�G���񂾁I");
                        }
                    }
                    Destroy(item);
                    break;

                case "Defence": //�J�[�h���f�B�t�F���X�̏ꍇ
                    Debug.Log("�h��");
                    Destroy(item);
                    break;

                case "Support": //�J�[�h���T�|�[�g�̏ꍇ
                    Debug.Log("�⏕");
                    Destroy(item);
                    break;

                default:
                    break;
            }
        }

        for (int i = 0; i < selectedCard.Count; i++)
        { //�I�������ۂ̐F�ύX�����ɖ߂�
            selectedCard[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        }

        //�e���X�g���ɂ���J�[�h�S�Ă�����
        selectedCard.Clear();
        activeList.Clear();
        //���������Z�b�g
        count = 0;
    }

    private void cardRefresh()
    {
        //�e���X�g���ɂ���J�[�h�S�Ă�����
        selectedCard.Clear();
        activeList.Clear();
        //���������Z�b�g
        count = 0;

        foreach (var item in activeList)
        {
            // ���݂̃J�[�h�v���n�u�����ɁA�C���X�^���X�𐶐��A
            GameObject obj = Instantiate(item, new Vector2(-8.0f + (2.0f * count), -3.5f), Quaternion.identity);
            //�I�u�W�F�N�g�̐F�����
            obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            //�N���[�������I�u�W�F�N�g�̖��O�����
            obj.name = item.name;
            //�J�[�h�̃^�O���N���[���I�u�W�F�N�g�ɂ��ǉ�
            obj.tag = item.tag;
            //���������Z
            count++;
        }
    }
}