using UnityEngine;
using Unity.Cinemachine;

public class InteractionZone : MonoBehaviour
{
    [Header("Dialogue Content")]
    public Conversation conversation;

    [Header("Cutscene Camera")]
    [Tooltip("이 대화에서 활성화할 시네머신 가상 카메라")]
    public CinemachineCamera dialogueCamera;

    private bool canInteract = false;
    private PlayerMovePlatform playerToInteract;

    public void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (canInteract && playerToInteract != null && Input.GetKeyDown(KeyCode.Z))
        {
            DialogueManager.instance.StartDialogue(conversation, dialogueCamera, playerToInteract);
            canInteract = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            playerToInteract = other.GetComponent<PlayerMovePlatform>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            playerToInteract = null;
        }
    }
}