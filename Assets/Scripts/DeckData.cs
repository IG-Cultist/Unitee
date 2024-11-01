/*
 * DeckDataScript
 * Creator:���Y�W�� Update:2024/10/30
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckData : MonoBehaviour
{
    // �g�p�̃J�[�h�I�u�W�F�N�g�񎟌����X�g
    public List<List<int>> usableObjList = new List<List<int>>();

    // �g�p�\�J�[�h�̃f�B�N�V���i���[
    public Dictionary<string, UsableCardResponse> cardDictionary = new Dictionary<string, UsableCardResponse>();

    // ���݂̃f�b�L���J�[�hID���X�g
    List<int> activeCardID = new List<int>();

    // ���݂̃f�b�L���J�[�hID���X�g
    List<int> activeDefenceCardID = new List<int>();

    // �X�e�[�W�I����ʃX�N���v�g
    SelectScene selectScene;

    // Start is called before the first frame update
    void Start()
    {
        // �X�e�[�W�I����ʃX�N���v�g���擾
        selectScene = FindObjectOfType<SelectScene>();

        StartCoroutine(NetworkManager.Instance.ShowDeck(cards =>
        {
            if (cards == null) return;
            // ���[�U�̃f�b�L�����擾
            foreach (var card in cards)
            {
                if (card == null) continue;
                // �J�[�hID���擾
                string strID = card.CardID.ToString();
                int.TryParse(strID, out int cardID);

                // �f�b�L�ɃJ�[�hID��ǉ�
                activeCardID.Add(cardID);
            }

            StartCoroutine(NetworkManager.Instance.ShowDefenceDeck(cards =>
            {
                if (cards == null) return;
                // ���[�U�̖h�q�f�b�L�����擾
                foreach (var card in cards)
                {
                    if (card == null) continue;
                    // �J�[�hID���擾
                    string strID = card.CardID.ToString();
                    int.TryParse(strID, out int cardID);

                    // �h�q�f�b�L�ɃJ�[�hID��ǉ�
                    activeDefenceCardID.Add(cardID);
                }

                StartCoroutine(NetworkManager.Instance.GetUsableCard(cards =>
                {
                    foreach (var card in cards)
                    {
                        // �e�����擾
                        string strID = card.CardID.ToString();
                        int.TryParse(strID, out int cardID);

                        string strStack = card.Stack.ToString();
                        int.TryParse(strStack, out int cardStack);

                        string cardName = card.Name.ToString();

                        // �g�p�\�J�[�h���X�^�b�N�������X�g�ɓ˂�����
                        List<int> Items = new List<int>();
                        for (int i = 0; i < cardStack; i++)
                        {
                            Items.Add(0);
                        }
                        usableObjList.Add(Items);

                        // �J�[�h�����f�B�N�V���i���[�ɂ܂Ƃ߂�
                        cardDictionary.Add(card.Name, card);
                    }

                    foreach (var item in activeCardID)
                    {
                        int cnt = 0;
                        foreach (var id in usableObjList[item - 1])
                        {
                            if (id != 1 && id != 2)
                            {
                                usableObjList[item - 1][cnt] = 1;
                                break;
                            }
                            cnt++;
                        }
                    }

                    foreach (var item in activeDefenceCardID)
                    {
                        int cnt = 0;
                        foreach (var id in usableObjList[item - 1])
                        {
                            if (id != 1 && id != 2)
                            {
                                usableObjList[item - 1][cnt] = 2;
                                break;
                            }
                            cnt++;
                        }
                    }
                }));
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ���݂̃f�b�L��DB�ɕۑ����鏈��
    /// </summary>
    /// <param name="cardID"></param>
    public void SetDeck()
    {
        List<int> list = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            // �e�X�^�b�N�������[�v
            for (int j = 0; j < usableObjList[i].Count; j++)
            {
                if (usableObjList[i][j] == 1)
                {
                    list.Add(i+1);
                }
            }
        }

        // ���݊i�[����Ă���ID���폜
        activeCardID.Clear();

        int cnt = 0;
        // DB���M�p�z��
        int[] sendData = { 0, 0, 0, 0 };

        // �擾���Ă���ID����
        foreach(var id in list)
        {
            if (list == null) break;
            // ���݂̃f�b�L���X�g��ID������
            activeCardID.Add((int)id);

            // ���M�p�z���ID������
            sendData[cnt] = (int)id;
            cnt++;
        }

        selectScene.UpdateDeck(activeCardID);
        // �擾����ID��DB�ɑ��M
        StartCoroutine(NetworkManager.Instance.StoreCard(sendData));
    }

    /// <summary>
    /// ���݂̖h�q�f�b�L��DB�ɕۑ����鏈��
    /// </summary>
    /// <param name="cardID"></param>
    public void SetDefenceDeck()
    {
        List<int> list = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            // �e�X�^�b�N�������[�v
            for (int j = 0; j < usableObjList[i].Count; j++)
            {
                if (usableObjList[i][j] == 2)
                {
                    list.Add(i + 1);
                }
            }
        }

        // ���݊i�[����Ă���ID���폜
        activeDefenceCardID.Clear();

        int cnt = 0;
        // DB���M�p�z��
        int[] sendData = { 0, 0, 0, 0 };

        // �擾���Ă���ID����
        foreach (var id in list)
        {
            if (list == null) break;
            // ���݂̃f�b�L���X�g��ID������
            activeDefenceCardID.Add((int)id);

            // ���M�p�z���ID������
            sendData[cnt] = (int)id;
            cnt++;
        }

        selectScene.UpdateDeck(activeDefenceCardID);
        // �擾����ID��DB�ɑ��M
        StartCoroutine(NetworkManager.Instance.StoreDefenceCard(sendData));
    }

    /// <summary>
    /// ���݂̃f�b�L���J�[�hID�擾����
    /// </summary>
    public List<int> GetDeck()
    {
        return activeCardID;
    }

    /// <summary>
    /// ���݂̖h�q�f�b�L���J�[�hID�擾����
    /// </summary>
    public List<int> GetDefenceDeck()
    {
        return activeDefenceCardID;
    }

    /// <summary>
    /// �I����ԃ��X�g�擾����
    /// </summary>
    public List<List<int>> GetUsable()
    {
        return usableObjList;
    }

    /// <summary>
    /// �I����Ԕ��ʏ���
    /// </summary>
    public bool CheckUsable(string requestStr)
    {
        for (int i = 1; i <= 9; i++)
        {
            // 4�񃋁[�v
            for (int j = 1; j <= 4; j++)
            {
                // �����񔻒�
                if (requestStr == i.ToString() + "," + j.ToString())
                {
                    //�I������Ă���Ȃ�True��Ԃ�
                    if (usableObjList[i - 1][j - 1] == 1 || usableObjList[i - 1][j - 1] == 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// �I����ԍX�V����
    /// </summary>
    /// <param name="requestStr"></param>
    /// <param name="val"></param>
    public void UpdateUsable(string requestStr,int val)
    {
        for (int i = 1; i <= 9; i++)
        {
            // 4�񃋁[�v
            for (int j = 1; j <= 4; j++)
            {
                // �����񔻒�
                if (requestStr == i.ToString() + "," + j.ToString() )
                {
                    // �I����Ԃ����N�G�X�g���ꂽ�l�ɍX�V
                    usableObjList[i-1][j-1] = val;
                    return;
                }
            }
        }
    }


    /// <summary>
    /// ���O��ID�ɕϊ����鏈��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int ConvertName(string requestStr)
    {
        for (int i = 1; i <= 9; i++)
        {
            // 4�񃋁[�v
            for (int j = 1; j <= 4; j++)
            {
                // �����񔻒�
                if (requestStr == i.ToString() + "," + j.ToString())
                {
                    return i;
                }
            }
        }
        return 0;
    }
}
