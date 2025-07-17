using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem; // Added for new Input System
using System.Collections; // Added for IEnumerator

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // 대화가 끝났을 때 호출될 이벤트입니다.
    public event Action OnDialogueFinished;

    [Header("Components")]
    public GameObject dialogueBubblePrefab;

    private Queue<DialogueLine> sentences;
    public bool isDialogueActive = false;
    private float lastZPressTime = 0f;
    private const float DEBOUNCE_TIME = 0.2f; // 필요에 따라 조절
    private DialogueBubble currentBubble;

    private PlayerController playerController;
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
        Debug.Log($"isDialogueActive:{isDialogueActive}");
        Debug.Log($"zKey:{Keyboard.current.zKey.wasPressedThisFrame}");
        Debug.Log($"{isDialogueActive && Keyboard.current.zKey.wasPressedThisFrame && Time.time - lastZPressTime > DEBOUNCE_TIME}");
        if (isDialogueActive && Keyboard.current.zKey.wasPressedThisFrame && Time.time - lastZPressTime > DEBOUNCE_TIME)
        {
            lastZPressTime = Time.time;
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Conversation conv, CinemachineCamera cam, PlayerController player)
    {
        isDialogueActive = true;

        playerController = player;
        playerController.SetMovement(false);

        currentDialogueCamera = cam;
        if (currentDialogueCamera != null)
        {
            originalCameraPriority = currentDialogueCamera.Priority;
            currentDialogueCamera.Priority = 20;
        }

        sentences.Clear();
        foreach (DialogueLine line in conv.lines)
            sentences.Enqueue(line);

        
    }

    public void DisplayNextSentence()
    {
        Debug.Log($"DisplayNextSentence called. Sentences in queue: {sentences.Count}");
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

        // 대사 표시 후 입력 무시 플래그를 잠시 설정
    }

    public void EndDialogue()
    {
        Debug.Log("EndDialogue called. Dialogue finished.");
        isDialogueActive = false;
        
        if (currentBubble != null) Destroy(currentBubble.gameObject);

        if (currentDialogueCamera != null)
        {
            currentDialogueCamera.Priority = originalCameraPriority;
        }

        if (playerController != null)
        {
            playerController.SetMovement(true);
        }

        // 대화가 끝났음을 구독자들에게 알립니다.
        OnDialogueFinished?.Invoke();
        
    }
}