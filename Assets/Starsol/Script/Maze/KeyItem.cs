using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("¿­¼è¸¦ È¹µæÇß½À´Ï´Ù!");

            // ¿©±â¿¡ ¿­¼è È¹µæ ÈÄ Ã³¸® ·ÎÁ÷ Ãß°¡
            Destroy(gameObject); // ¿­¼è ¿ÀºêÁ§Æ® Á¦°Å
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
