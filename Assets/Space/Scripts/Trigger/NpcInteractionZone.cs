using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class NpcInteractionZone : MonoBehaviour
{
    [Header("Dialogue Content")]
    public List<Conversation> conversation;
    public Conversation loopConversation;

    [Header("Cutscene Camera")]
    [Tooltip("이 대화에서 활성화할 시네머신 가상 카메라")]
    public CinemachineCamera dialogueCamera;

    [Header("Scene Transition")]
    public string nextSceneName; // 넘어갈 씬의 이름을 입력받을 변수
    public float sceneChangeDelay = 45f; // 씬 전환까지의 대기 시간

    private bool canInteract = false;
    private PlayerMovePlatform playerToInteract;

    private bool isFirstConversation = true;
    private int conversationIndex = 0;

    private float timer = 0f; // 타이머 변수
    private bool startTimer = false; // 타이머 시작을 제어할 플래그

    public void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    

    private void Update()
    {
        if (canInteract && playerToInteract != null && Input.GetKeyDown(KeyCode.Z))
        {
            if (conversationIndex == conversation.Count)
            {
                isFirstConversation = false;
                
                startTimer = true;
            }

            if (isFirstConversation)
            {
                DialogueManager.instance.StartDialogue(conversation[conversationIndex], dialogueCamera, playerToInteract);
                conversationIndex++;
            }
            else
            {
                DialogueManager.instance.StartDialogue(loopConversation, dialogueCamera, playerToInteract);
            }
            canInteract = false;
        }

        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer >= sceneChangeDelay)
            {
                // 지정된 시간이 지나면 다음 씬으로 전환합니다.
                // SceneManager.LoadScene(nextSceneName);
                startTimer = false; // 타이머 중복 실행 방지
            }
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