using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ ���Ϳ� ��ҽ��ϴ�! ������ ������մϴ�.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
