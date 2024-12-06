/*
 * BattleModeScript
 * Creator:���Y�W�� Update:2024/11/27
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class BattleMode : MonoBehaviour
{
    // ���݂̃f�b�L�\���p�e
    [SerializeField] GameObject deckParent;

    // ���[�f�B���O�p�l��
    [SerializeField] GameObject loadingPanel;

    // ���[�f�B���O�A�C�R��
    [SerializeField] GameObject loadingIcon;

    // ���C�o���p�l���̃v���n�u
    [SerializeField] GameObject profilePrefab;

    // ���C�o���f�b�L�̃p�l��
    [SerializeField] GameObject[] rivalDeckPanel;

    // �x���e�L�X�g�̃I�u�W�F�N�g
    [SerializeField] GameObject warning;

    // �x���e�L�X�g
    [SerializeField] Text warningText;

    // �|�C���g�e�L�X�g
    [SerializeField] Text pointText;

    // �f�b�L�\�z�ς݊m�F�ϐ�
    bool isSetDeck;

    // ���݂̃f�b�LID���X�g
    List<int> activeDeckID = new List<int>();

    //���C�o���̃f�[�^�f�B�N�V���i��
    Dictionary<int,List<int>> rivalDataDictionary = new Dictionary<int,List<int>>();

    // Start is called before the first frame update
    void Start()
    {
        // �񓯊����������܂őҋ@������
        Loading();

        isSetDeck = true;
        warning.SetActive(false);

        StartCoroutine(NetworkManager.Instance.ShowDeck(cards =>
        {
            // ���[�U�̃f�b�L������4���łȂ��ꍇ
            if (cards.Length != 4)
            {
                isSetDeck = false;
                warningText.text = "���F�f�b�L�̃J�[�h�������s�\���Ȃ���\r\n�@�@�퓬���J�n�ł��܂���";
                warning.SetActive(true);
            }

            foreach (var card in cards)
            {
                string strID = card.CardID.ToString();
                int.TryParse(strID, out int cardID);
                activeDeckID.Add(cardID);
            }
            SetDeck();

            StartCoroutine(NetworkManager.Instance.ShowDefenceDeck(cards =>
            {
                // ���[�U�̖h�q�f�b�L������4���łȂ��ꍇ
                if (cards.Length != 4)
                {
                    isSetDeck = false;
                    warningText.text = "���F�f�B�t�F���X�f�b�L�̃J�[�h�������s�\���Ȃ���\r\n�@�@�퓬���J�n�ł��܂���";
                    warning.SetActive(true);
                }
            }));
        }));

        StartCoroutine(NetworkManager.Instance.GetProfile(rivalData =>
        {
            // ����p���X�g
            List<int> cardList = new List<int>();
            // ���[�UID�ۑ��p�ϐ�
            int userID = 0;
            int cnt = 0;

            // �������C�o���f�[�^�������[�v
            foreach (var item in rivalData)
            {
                // ID��int��
                int.TryParse(item.UserID.ToString(), out int id);

                // �ۑ�����ID�ƈقȂ邩���݂̃��X�g�J�E���g��4�̏ꍇ
                if (userID != id && cardList.Count == 4)
                {
                    rivalDataDictionary.Add(userID, cardList);
                    // ����p���X�g�����Z�b�g
                    cardList = new List<int>();
                    cnt++;
                }
                userID = id;
                // �J�[�hID��int��
                int.TryParse(item.CardID.ToString(), out int cardID);
                // ����p���X�g�Ɏ擾�J�[�h������
                cardList.Add(cardID);
            }
            rivalDataDictionary.Add(userID, cardList);
            SetRivalDeck();
        }));


        StartCoroutine(NetworkManager.Instance.GetMyProfile(userData =>
        {
            // �������C�o���f�[�^�������[�v
            foreach (var item in userData)
            {
                pointText.text = "Your Point:" + item.Point.ToString();
            }
        }));
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// �퓬�V�[���֑J��
    /// </summary>
    public void goFight(List<int> cardList, int rivalID)
    {
        if (isSetDeck == false) return;

        SetRivalData(cardList, rivalID);
        SceneManager.LoadScene("Fight");
    }

    /// <summary>
    /// �f�b�L�\������
    /// </summary>
    void SetDeck()
    {
        // ��������Ă���J�[�h�����ׂč폜
        foreach (Transform n in deckParent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        // �e�J�[�h�̃X�^�b�N�������[�v
        for (int i = 0; i < activeDeckID.Count; i++)
        {
            if (activeDeckID[i] == 0) continue;
            // �����̃J�[�h�����\�[�X�t�@�C������擾
            GameObject obj = (GameObject)Resources.Load("Cards(ID)/" + activeDeckID[i]);
            // �擾�����J�[�h�𐶐�
            GameObject cards = Instantiate(obj, new Vector2(-680f + (450f * i), 0f), Quaternion.identity);
            cards.name = activeDeckID[i].ToString();
            cards.transform.localScale = new Vector2(2.7f, 3.9f);
            // ���C���f�b�L�p�l���ɐ���
            cards.transform.SetParent(deckParent.transform, false);
        }
    }

    /// <summary>
    /// ���C�o���f�b�L�\������
    /// </summary>
    void SetRivalDeck()
    {
        int cnt = 0;
        foreach (var cards in rivalDataDictionary)
        {
            // �e�J�[�h�̖��������[�v
            for (int i = 0; i < cards.Value.Count; i++)
            {
                // �����̃J�[�h�����\�[�X�t�@�C������擾
                GameObject obj = (GameObject)Resources.Load("Cards(ID)/" + cards.Value[i]);
                // �擾�����J�[�h�𐶐�
                GameObject cardObj = Instantiate(obj, new Vector2(-330f + (220f * i), 0f), Quaternion.identity);
                cardObj.name = cards.Value[i].ToString();
                cardObj.transform.localScale = new Vector2(1.1f, 2f);
                // ���C�o���f�b�L�p�l���ɐ���
                cardObj.transform.SetParent(rivalDeckPanel[cnt].transform, false);
                rivalDeckPanel[cnt].name = cards.Key.ToString();

            }
            GameObject child = rivalDeckPanel[cnt].transform.GetChild(0).gameObject;
            child.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => goFight(cards.Value,cards.Key));
            cnt++;
        }
    }

    /// <summary>
    /// ���O�m�F����
    /// </summary>
    public void CheckLog()
    {

    }

    /// <summary>
    /// �Z���N�g��ʂɖ߂鏈��
    /// </summary>
    public void exitBattleScene()
    {
        SceneManager.LoadScene("SelectScene");
    }

    /// <summary>
    /// ���[�f�B���O
    /// </summary>
    async void Loading()
    {
        loadingPanel.SetActive(true);

        float angle = 8;
        bool rot = true;

        for (int i = 0; i < 80; i++)
        {
            if (rot)
            {
                loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.back);
            }
            else
            {
                loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
            }
            await Task.Delay(10);
        }
        loadingPanel.SetActive(false);
    }

    void SetRivalData(List<int> cardList, int rivalID)
    {
        RivalData.cardIDList = cardList;
        RivalData.rivalID = rivalID;
    }
}
