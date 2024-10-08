using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    /// <summary>
    /// �V���O���g��
    /// </summary>
    const string API_BASE_URL = "https://api-deadlydraw.japaneast.cloudapp.azure.com/api/";
    private int userID = 0;
    private string userName = "";
    private List<int> stageList = new List<int>();
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get {
            // create yet
            if(instance == null)
            {
                GameObject gameObj = new GameObject("NetworkManger");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    /// <summary>
    /// ���[�U�o�^����
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator StoreUser(string name,Action<bool> result)
    {
        //Create Object Send for Server
        StoreUserRequest requestData = new StoreUserRequest();
        requestData.Name = name;
        //�T�[�o�ɑ��M����I�u�W�F�N�g��JAON�ɕϊ�response
        string json = JsonConvert.SerializeObject(requestData);
        //Send
        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/store", json, "application/json");

        yield return request.SendWebRequest();
        bool isSuccess = false;
        if(request.result == UnityWebRequest.Result.Success
             && request.responseCode == 200)
        {
            //�ʐM�����������ꍇ�A�Ԃ��Ă���Json���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            StoreUserresponse response =
                JsonConvert.DeserializeObject<StoreUserresponse>(resultJson);
            //�t�@�C���Ƀ��[�UID��ۑ�
            this.userName = name;
            this.userID = response.UserID;
            SaveUserData();
            isSuccess = true;
        }
        result?.Invoke(isSuccess);//�����ŌĂяo������result�������Ă�
    }

    /// <summary>
    /// ���[�U���̕ۑ�
    /// </summary>
    private void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.Name = this.userName;
        saveData.UserID = this.userID;
        saveData.StageList = this.stageList;
        string json = JsonConvert.SerializeObject(saveData);
        var writer =
            new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// ���[�U���̓ǂݍ���
    /// </summary>
    /// <returns></returns>
    public bool LoadUserData()
    {
        string path = Application.persistentDataPath;
        if(!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }
        var reader =
            new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.userID = saveData.UserID;
        this.userName = saveData.Name;
        this.stageList = saveData.StageList;
        return true;
    }

    /// <summary>
    /// �X�e�[�W�ꗗ�擾����
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetStage(Action<StageResponse[]> result)
    {
        // �X�e�[�W�ꗗ�擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "stages");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
             && request.responseCode == 200)
        {
            //�ʐM�����������ꍇ�A�Ԃ��Ă���Json���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            StageResponse[] response =
                JsonConvert.DeserializeObject<StageResponse[]>(resultJson);
            result?.Invoke(response);//�����ŌĂяo������result�������Ă�
        }
        else
        {
            result?.Invoke(null);
        }     
    }

    /// <summary>
    /// �g�p�\�J�[�h�ꗗ�擾
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetUsableCard(Action<UsableCardResponse[]> result)
    {
        // �X�e�[�W�ꗗ�擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "battleMode/usableCards");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
             && request.responseCode == 200)
        {
            //�ʐM�����������ꍇ�A�Ԃ��Ă���Json���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            UsableCardResponse[] response =
                JsonConvert.DeserializeObject<UsableCardResponse[]>(resultJson);
            result?.Invoke(response);//�����ŌĂяo������result�������Ă�
        }
        else
        {
            result?.Invoke(null);
        }
    }

    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetItem(Action<ItemResponse[]> result)
    {
        // �X�e�[�W�ꗗ�擾API�����s
        UnityWebRequest request = UnityWebRequest.Get(
            API_BASE_URL + "items");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
             && request.responseCode == 200)
        {
            //�ʐM�����������ꍇ�A�Ԃ��Ă���Json���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ItemResponse[] response =
                JsonConvert.DeserializeObject<ItemResponse[]>(resultJson);
            result?.Invoke(response);//�����ŌĂяo������result�������Ă�
        }
        else
        {
            result?.Invoke(null);
        }
    }

    public void ClearStage(int stageID)
    {
        if (stageList == null ) stageList = new List<int>();

        if (stageList.Contains(stageID))
        {
            return;
        }
        this.stageList.Add(stageID);
        SaveUserData();
    }

    public List<int> GetID()
    {
        return this.stageList;
    }
}
