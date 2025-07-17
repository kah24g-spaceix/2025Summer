// KeyItem.cs
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("���踦 ȹ���߽��ϴ�!");
            Destroy(gameObject); // ���� ������Ʈ ����

            // �� ��ȯ�� 
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}
