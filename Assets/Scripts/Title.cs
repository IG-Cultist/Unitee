/*
 * TitleScript
 * Creator:���Y�W�� Update:2024/08/26
*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.AddressableAssets;

public class Title : MonoBehaviour
{
    [SerializeField] Text txt;
    [SerializeField] GameObject title;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�N���b�N
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(checkCatalog());
            bool isSuccess = NetworkManager.Instance.LoadUserData();

            if (!isSuccess)
            { //���[�U�f�[�^���ۑ�����Ă��Ȃ������ꍇ
                StartCoroutine(NetworkManager.Instance.StoreUser(
                    Guid.NewGuid().ToString(),  //Set Random Name
                    result => //After Set's Process
                    {
                        SceneManager.LoadScene("SelectScene");
                    }));
            }
            else
            {
                SceneManager.LoadScene("SelectScene");
            }
        }
    }

    IEnumerator checkCatalog()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        var updates = checkHandle.Result;
        Addressables.Release(checkHandle);
        if(updates.Count >= 1)
        {
            //�X�V������ꍇ�̓��[�h��ʂ�
            SceneManager.LoadScene("LoadScene");
        }
    }
}
