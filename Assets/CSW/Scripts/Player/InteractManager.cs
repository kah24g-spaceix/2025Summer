using UnityEngine;

public class InteractManager : MonoBehaviour
{
    PlayerMovement player;
    private void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            player.interactingObject = collision.GetComponent<Interactable>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && collision.transform == player.interactingObject)
        {
            player.interactingObject = null;
        }
    }
}