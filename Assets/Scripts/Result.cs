/*
 * ResultScript
 * Creator:���Y�W�� Update:2024/07/24
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
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
            SceneManager.LoadScene("Title");
        }
    }
}
