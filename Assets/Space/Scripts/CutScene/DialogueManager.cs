using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Components")]
    public GameObject dialogueBubblePrefab;

    private Queue<DialogueLine> sentences;
    private bool isDialogueActive = false;
    private DialogueBubble currentBubble;

    private PlayerMovePlatform playerMoveScript;
    private CinemachineCamera currentDialogueCamera;
    private int originalCameraPriority;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        sentences = new Queue<DialogueLine>();
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Z))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Conversation conv, CinemachineCamera cam, PlayerMovePlatform player)
    {
        isDialogueActive = true;

        playerMoveScript = player;
        playerMoveScript.SetMovement(false);

        currentDialogueCamera = cam;
        if (currentDialogueCamera != null)
        {
            originalCameraPriority = currentDialogueCamera.Priority;
            currentDialogueCamera.Priority = 20;
        }

        sentences.Clear();
        foreach (DialogueLine line in conv.lines)
            sentences.Enqueue(line);

        // 바로 첫 대사를 표시하지 않고 코루틴을 통해 한 프레임 뒤에 호출
        StartCoroutine(StartDialogueAfterFrame());
    }

    private System.Collections.IEnumerator StartDialogueAfterFrame()
    {
        // 한 프레임 대기하여 Z키 입력 충돌을 방지
        yield return null; 
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (currentBubble != null) Destroy(currentBubble.gameObject);

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = sentences.Dequeue();
        GameObject bubbleObj = Instantiate(dialogueBubblePrefab);
        currentBubble = bubbleObj.GetComponent<DialogueBubble>();
        currentBubble.Setup(currentLine);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        if (currentBubble != null) Destroy(currentBubble.gameObject);

        if (currentDialogueCamera != null)
        {
            currentDialogueCamera.Priority = originalCameraPriority;
        }

        if (playerMoveScript != null)
        {
            playerMoveScript.SetMovement(true);
        }
    }
}