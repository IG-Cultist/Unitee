/*
 * HealthScript
 * Creator:���Y�W�� Update:2024/07/25
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //�n�[�g�̃Q�[���I�u�W�F�N�g
    [SerializeField] GameObject heart;
    //�G�̗̑͒l
    int enemHealth;
    //�v���C���[�̗̑͒l
    int playerHealth;

    /// <summary>
    /// �G�l�~�[HP�̃v���p�e�B
    /// </summary>
    public int EnemHealth
    {
        get { return enemHealth; }
    }

    /// <summary>
    /// �v���C���[HP�̃v���p�e�B
    /// </summary>
    public int PlayerHealth
    {
        get { return playerHealth; }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemHealth = 3;
        playerHealth = 5;
        EnemyLife();
        PlayerLife();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �GHP�ݒ�
    /// </summary>
    public void EnemyLife()
    {
        Vector2 pos = this.transform.position;
        for (int i = 0; i < enemHealth; i++) //�ݒ�health���J��Ԃ�
        {
            //�v���n�u�̃^�O���G�l�~�[�̂��̂ɕύX
            heart.tag = "EnemyHP";
            // Heart�v���n�u�����ɁA�C���X�^���X�𐶐��A
            Instantiate(heart, new Vector2(pos.x + (0.8f * i), pos.y), Quaternion.identity);
        }
    }

    /// <summary>
    /// �v���C���[HP�ݒ�
    /// </summary>
    public void PlayerLife()
    {
        for (int i = 0; i < playerHealth; i++) //�ݒ�health���J��Ԃ�
        {
            //�v���n�u�̃^�O���v���C���[�̂��̂ɕύX
            heart.tag = "PlayerHP"; 
            // Heart�v���n�u�����ɁA�C���X�^���X�𐶐��A
            Instantiate(heart, new Vector2(-8.5f + (0.8f * i), 0.7f), Quaternion.identity);
        }
    }
}
