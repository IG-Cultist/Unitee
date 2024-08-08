/*
 * EnemyScript
 * Creator:���Y�W�� Update:2024/07/25
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //�GHP
    GameObject[] playerHP;
    //HP�c��
    int playerLife;
    //�h��l
    int block;
    //�U���l
    int dmg;

    int count;
    //�w���X�X�N���v�g
    Health healthScript;
    [SerializeField] GameObject health;
    [SerializeField] Text action;

    List<string> actionList = new List<string>() { "attack", "attack", "defence", "attack" };

    enum EnemyType
    {
        Viper = 4,
        Slime = 4,
        Face = 4
    }

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        block = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �s������
    /// </summary>
    public string Attack()
    {
        dmg = 1;
        if (count == 0)
        {
            //�v���C���[HP���^�O�Ŏ擾
            playerHP = GameObject.FindGameObjectsWithTag("PlayerHP");
            healthScript = FindObjectOfType<Health>();
            playerLife = healthScript.PlayerHealth;
        }

        if (actionList[count] == "attack")
        {
            for (int i = 0; i < dmg; i++)
            {
                Debug.Log(playerLife);
                //HP�c�ʂ�0�̏ꍇ�A�������s��Ȃ�
                if (playerLife <= 0)
                {
                    Debug.Log("You Deadadad�I");
                    break;
                }
                action.text = "�G�̍U���I";
                //�\��HP�����炷
                Destroy(playerHP[(playerLife - 1)]);

                //���������炷
                playerLife--;
                Debug.Log("Player's HP:" + playerLife);
            }
        }
        else if (actionList[count] == "defence")
        {
            action.text = "�G�͖h�䂵���I";
            block++;
        }

        if(count < (actionList.Count - 1)) count++;   
        
        return actionList[count];
    }
}
