/*
 * PassiveScript
 * Creator:���Y�W�� Update:2024/07/26
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Passive : MonoBehaviour
{
    //�p�b�V�u���X�g
    [SerializeField] List<string> passiveList;

    //�Z�b�g���ꂽ�p�b�V�u�����郊�X�g
    List<GameObject> activePassives;

    /// <summary>
    /// �A�N�e�B�u�ȃp�b�V�u�̃v���p�e�B
    /// </summary>
    public List<GameObject> ActivePassives
    {
        get { return activePassives; }
    }
    // Start is called before the first frame update
    void Start()
    {
        //�擾���Ă����p�b�V�u��˂����ޗ\��
        passiveList = new List<string>() {"Spike","Spike","Spike","ArmorChip" };

        activePassives = new List<GameObject>();

        //�擾�����p�b�V�u��ݒu
        SetPassives();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �p�b�V�u�ݒu����
    /// </summary>
    void SetPassives()
    {
        int cnt = 0;
        Vector2 pos = this.transform.position;

        foreach (var passive in passiveList)
        {
            // ���X�g���ɂ��閼�O�Ɠ����v���n�u���擾
            GameObject obj = (GameObject)Resources.Load(passive);
            // �ݒ肳�ꂽ�p�b�V�u�𐶐��A
            GameObject item = Instantiate(obj, new Vector2(pos.x + (2.0f * cnt),pos.y), Quaternion.identity);
            // ���O�����
            item.name = passive;
            // �A�N�e�B�u���X�g�ɒǉ�
            activePassives.Add(item);
            cnt++;
        }
    }
}
