// KeyItem.cs
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private SceneFader sceneFader;
    private bool playerInRange = false;
    private GameObject player;
    void Start()
    {
        sceneFader = FindFirstObjectByType<SceneFader>();
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            sceneFader.FadeOutAndLoadScene("Stage4");
            Destroy(gameObject);
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
