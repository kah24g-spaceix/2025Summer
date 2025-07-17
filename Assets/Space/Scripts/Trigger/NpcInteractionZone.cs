using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class NpcInteractionZone : MonoBehaviour, IInteractable
{
    [Header("Sprite")]
    [SerializeField] private Sprite hintSprite; // 힌트 상태일 때의 스프라이트
    private SpriteRenderer spriteRenderer;

    [Header("Dialogue Content")]
    public List<Conversation> initialConversations; // 초기 대화 목록
    public Conversation loopConversation;           // 반복 대화
    public Conversation hintConversation;           // 힌트 대화 (모든 조각을 모았을 때)

    [Header("Cutscene Camera")]
    [Tooltip("이 대화에서 활성화할 시네머신 가상 카메라")]
    public CinemachineCamera dialogueCamera;

    [Header("Scene Transition")]
    public string nextSceneName;
    public float sceneChangeDelay = 45f;

    private int conversationIndex = 0;
    private bool hasCompletedFirstInteraction = false; // "방법 1"의 핵심 플래그

    private int fragmentCount = 0;
    private const int MAX_FRAGMENTS = 3;

    private bool startSceneChangeTimer = false;
    private float timer = 0f;

    private PlayerController currentPlayerController; // 추가: 현재 플레이어 컨트롤러 참조

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        // Start()는 모든 Awake()가 실행된 후에 호출되므로, DialogueManager.instance가 null이 아님을 보장할 수 있습니다.
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueFinished += HandleDialogueFinish;
        }
        else
        {
            Debug.LogError("DialogueManager 인스턴스를 찾을 수 없습니다!");
        }
    }

    private void OnDisable()
    {
        // DialogueManager 인스턴스가 아직 존재할 때만 이벤트 구독을 해제합니다.
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueFinished -= HandleDialogueFinish;
        }
    }

    private void Update()
    {
        if (fragmentCount >= MAX_FRAGMENTS) spriteRenderer.sprite = hintSprite;
        if (!hasCompletedFirstInteraction) startSceneChangeTimer = true;
        HandleSceneChangeTimer();
    }

    public void Interact(PlayerController playerController)
    {
        this.currentPlayerController = playerController; // 추가: 플레이어 컨트롤러 참조 저장
        Debug.Log($"NpcInteractionZone: Interact called. conversationIndex: {conversationIndex}, hasCompletedFirstInteraction: {hasCompletedFirstInteraction}, fragmentCount: {fragmentCount}");

        // 대화가 이미 진행 중이면 새로운 대화를 시작하지 않습니다.
        if (DialogueManager.instance != null && DialogueManager.instance.isDialogueActive)
        {
            Debug.Log("Dialogue is already active. Ignoring new interaction request.");
            return;
        }

        // 1. 힌트 조건 확인 (가장 높은 우선순위)
        if (fragmentCount >= MAX_FRAGMENTS)
        {
            DialogueManager.instance.StartDialogue(hintConversation, dialogueCamera, playerController);
            // 힌트 대화 후 씬 전환 타이머 시작
        }
        // 2. 첫 상호작용이 끝나지 않았을 경우
        else if (!hasCompletedFirstInteraction)
        {
            // 대화를 시작하기만 하고, 인덱스는 여기서 증가시키지 않습니다.
            DialogueManager.instance.StartDialogue(initialConversations[conversationIndex], dialogueCamera, playerController);
        }
        // 3. 첫 상호작용이 끝난 후 (반복 대화)
        else
        {
            DialogueManager.instance.StartDialogue(loopConversation, dialogueCamera, playerController);
        }
    }

    // DialogueManager로부터 대화가 끝났다는 신호를 받으면 이 함수가 실행됩니다.
    private void HandleDialogueFinish()
    {
        Debug.Log($"NpcInteractionZone: HandleDialogueFinish called. Before - conversationIndex: {conversationIndex}, hasCompletedFirstInteraction: {hasCompletedFirstInteraction}");

        // 첫 번째 상호작용 중이었을 때만 인덱스를 증가시킵니다.
        if (!hasCompletedFirstInteraction)
        {
            conversationIndex++;

            // 인덱스를 증가시킨 후, 모든 초기 대화가 끝났는지 확인합니다.
            if (conversationIndex >= initialConversations.Count)
            {
                hasCompletedFirstInteraction = true;
            }
        }
        Debug.Log($"NpcInteractionZone: HandleDialogueFinish called. After - conversationIndex: {conversationIndex}, hasCompletedFirstInteraction: {hasCompletedFirstInteraction}");
    }

    private void HandleSceneChangeTimer()
    {
        if (startSceneChangeTimer)
        {
            timer += Time.deltaTime;

            // 플레이어가 움직이면 타이머 초기화
            if (currentPlayerController != null && currentPlayerController.movement != null && currentPlayerController.movement.enabled && currentPlayerController.movement.IsMoving)
            {
                timer = 0f;
                Debug.Log("Scene change timer reset due to player movement.");
            }

            if (timer >= sceneChangeDelay)
            {
                // SceneManager.LoadScene(nextSceneName);
                startSceneChangeTimer = false; // 타이머 중복 실행 방지
                Debug.Log("Scene change triggered.");
            }
        }
    }

    // 외부(예: FragmentZone)에서 조각을 획득했음을 알리기 위한 public 함수
    public void AddFragment()
    {
        if (fragmentCount < MAX_FRAGMENTS)
        {
            fragmentCount++;
        }
    }
}