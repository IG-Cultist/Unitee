/*
 * BattleModeScript
 * Creator:���Y�W�� Update:2024/11/07
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class BattleMode : MonoBehaviour
{
    // ���݂̃f�b�L�\���p�e
    [SerializeField] GameObject deckParent;

    // �x���e�L�X�g
    [SerializeField] GameObject warning;

    [SerializeField] Text warningText;

    // ���[�f�B���O�p�l��
    [SerializeField] GameObject loadingPanel;

    // ���[�f�B���O�A�C�R��
    [SerializeField] GameObject loadingIcon;

    // �f�b�L�\�z�ς݊m�F�ϐ�
    bool isSetDeck;

    // ���݂̃f�b�LID���X�g
    List<int> activeDeckID = new List<int>();

    int[] rivalID = {0,0,0};

    // Start is called before the first frame update
    void Start()
    {
        // �񓯊����������܂őҋ@������
        Loading();

        isSetDeck = false;
        warning.SetActive(false);

        StartCoroutine(NetworkManager.Instance.ShowDeck(cards =>
        {
            if (cards.Length != 4)
            {
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
                if (cards.Length != 4)
                {
                    warningText.text = "���F�f�B�t�F���X�f�b�L�̃J�[�h�������s�\���Ȃ���\r\n�@�@�퓬���J�n�ł��܂���";
                    warning.SetActive(true);
                }
                else if (cards.Length == 4) isSetDeck = true;
            }));
        }));

        StartCoroutine(NetworkManager.Instance.GetProfile(users =>
        {

        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �퓬�V�[���֑J��
    /// </summary>
    public void goFight()
    {
        if (isSetDeck == false) return;
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
}
