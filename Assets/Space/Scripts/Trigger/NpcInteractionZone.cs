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
    public float sceneChangeDelay = 3f;

    // --- 내부 상태 변수 ---
    private bool canInteract = false;
    private PlayerMovePlatform playerToInteract;

    private int conversationIndex = 0;
    private bool hasCompletedFirstInteraction = false; // "방법 1"의 핵심 플래그

    private int fragmentCount = 0;
    private const int MAX_FRAGMENTS = 3;

    private bool startSceneChangeTimer = false;
    private float timer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        HandleSceneChangeTimer();
    }

    public void Interact()
    {
        if (canInteract && playerToInteract != null && Input.GetKeyDown(KeyCode.Z))
        {
            // 1. 힌트 조건 확인 (가장 높은 우선순위)
            if (fragmentCount >= MAX_FRAGMENTS)
            {
                spriteRenderer.sprite = hintSprite;
                DialogueManager.instance.StartDialogue(hintConversation, dialogueCamera, playerToInteract);
                startSceneChangeTimer = true; // 힌트 대화 후 씬 전환 타이머 시작
            }
            // 2. 첫 상호작용이 끝나지 않았을 경우
            else if (!hasCompletedFirstInteraction)
            {
                DialogueManager.instance.StartDialogue(initialConversations[conversationIndex], dialogueCamera, playerToInteract);
                conversationIndex++;

                // 모든 초기 대화가 끝났는지 확인
                if (conversationIndex >= initialConversations.Count)
                {
                    hasCompletedFirstInteraction = true;
                }
            }
            // 3. 첫 상호작용이 끝난 후 (반복 대화)
            else
            {
                DialogueManager.instance.StartDialogue(loopConversation, dialogueCamera, playerToInteract);
            }

            // 한 번 상호작용 했으므로 다음 프레임까지 대기
            canInteract = false;
        }
    }

    private void HandleSceneChangeTimer()
    {
        if (startSceneChangeTimer)
        {
            timer += Time.deltaTime;
            if (timer >= sceneChangeDelay)
            {
                // SceneManager.LoadScene(nextSceneName);
                startSceneChangeTimer = false; // 타이머 중복 실행 방지
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