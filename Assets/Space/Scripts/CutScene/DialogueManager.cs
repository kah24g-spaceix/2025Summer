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
    private bool ignoreInput = false; // 입력 무시 플래그
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
        // 대화 중이고, 입력 무시 상태가 아닐 때 Z키를 누르면 다음 대사 표시
        if (isDialogueActive && !ignoreInput && Input.GetKeyDown(KeyCode.Z))
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

        // 코루틴을 통해 첫 대사 표시 및 입력 무시 상태 제어
        StartCoroutine(StartDialogueRoutine());
    }

    private System.Collections.IEnumerator StartDialogueRoutine()
    {
        // 대화 시작 시, 첫 Z키 입력을 무시하도록 설정
        ignoreInput = true;

        // 첫 대사 표시
        DisplayNextSentence();

        // 한 프레임 대기
        yield return null;

        // 다음 프레임부터 Z키 입력이 가능하도록 플래그 해제
        ignoreInput = false;
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