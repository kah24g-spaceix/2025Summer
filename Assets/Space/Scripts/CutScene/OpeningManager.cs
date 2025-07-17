using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween 네임스페이스 추가
using TMPro;

[System.Serializable]
public class MessageImagePair
{
    public int messageIndex;
    public Texture2D imageToShow; // 이 메시지에 표시할 이미지
    [Tooltip("0 for image1, 1 for image2")]
    public int targetImageSlot; // 0: image1, 1: image2
}

public class OpeningManager : MonoBehaviour
{
    public TextMeshProUGUI messageText; // 메시지 출력용 Text UI
    public RectTransform dialoguePanel; // 대화창 패널의 RectTransform
    public RawImage image1; // 첫 번째 이미지
    public RawImage image2; // 두 번째 이미지

    public SceneFader sceneFader; // SceneFader 참조 추가

    [Header("Dialogue Settings")]
    public List<string> messages; // 표시할 메시지 리스트
    
    public float textSpeed = 0.05f; // 텍스트 한 글자씩 출력 속도
    public float fadeInDuration = 1.5f; // 이미지 페이드 인 시간
    public float dialoguePanelMoveDuration = 1.0f; // 대화창 이동 시간

    [Header("Image Settings")]
    public List<MessageImagePair> messageImageSettings; // 메시지 인덱스에 따른 이미지 설정

    private int currentMessageIndex = 0;
    private bool isTyping = false; // 텍스트 타이핑 중인지 여부
    private bool canAdvance = false; // 다음 단계로 진행 가능한지 여부
    private Tween currentTextTween; // 현재 텍스트 타이핑 DOTween
    private Tween currentImageTween; // 현재 이미지 페이드 인 DOTween

    void Start()
    {
        // 초기 UI 설정
        if (messageText != null) messageText.text = "";
        if (image1 != null) image1.color = new Color(1, 1, 1, 0); // 투명하게 시작
        if (image2 != null) image2.color = new Color(1, 1, 1, 0); // 투명하게 시작

        // 대화창 초기 위치 설정 (에디터에서 설정된 위치를 기준으로 애니메이션)
        if (dialoguePanel != null)
        {
            Vector2 initialAnchoredPosition = dialoguePanel.anchoredPosition; // 에디터에서 설정된 초기 위치 저장
            
            // 대화창을 화면 아래로 완전히 숨김
            dialoguePanel.anchoredPosition = new Vector2(initialAnchoredPosition.x, -dialoguePanel.rect.height);
            
            // 대화창을 에디터에서 설정된 위치로 올라오는 애니메이션
            dialoguePanel.DOAnchorPosY(initialAnchoredPosition.y, dialoguePanelMoveDuration).SetEase(Ease.OutBack)
                .OnComplete(StartOpeningSequence); // 애니메이션 완료 후 오프닝 시퀀스 시작
        }
        else
        {
            StartOpeningSequence(); // 대화창이 없으면 바로 시퀀스 시작
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isTyping) // 텍스트 타이핑 중이면 스킵
            {
                SkipCurrentText();
            }
            else if (canAdvance) // 다음 단계로 진행 가능하면
            {
                AdvanceOpeningSequence();
            }
        }
    }

    void StartOpeningSequence()
    {
        // 첫 번째 메시지 출력 시작
        DisplayNextMessage();
    }

    void DisplayNextMessage()
    {
        if (currentMessageIndex < messages.Count)
        {
            messageText.text = ""; // 이전 텍스트 초기화
            isTyping = true;
            canAdvance = false; // 텍스트 타이핑 중에는 다음 진행 불가

            // 텍스트 한 글자씩 출력 (maxVisibleCharacters 애니메이션)
            messageText.text = messages[currentMessageIndex]; // 전체 텍스트를 먼저 설정
            messageText.maxVisibleCharacters = 0; // 처음에는 아무것도 보이지 않게 설정

            currentTextTween = DOTween.To(
                () => messageText.maxVisibleCharacters, // 현재 값 가져오기
                x => messageText.maxVisibleCharacters = x, // 값 설정하기
                messages[currentMessageIndex].Length, // 목표 값 (텍스트 길이)
                messages[currentMessageIndex].Length * textSpeed // 애니메이션 시간
            )
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                isTyping = false;
                canAdvance = true; // 텍스트 타이핑 완료 후 다음 진행 가능
            });

            // --- 이미지 처리 시작 ---
            // bool image1Updated = false;
            // bool image2Updated = false;

            foreach (var setting in messageImageSettings)
            {
                if (setting.messageIndex == currentMessageIndex)
                {
                    if (setting.imageToShow != null)
                    {
                        RawImage targetImage = null;
                        if (setting.targetImageSlot == 0 && image1 != null)
                        {
                            targetImage = image1;
                            
                        }
                        else if (setting.targetImageSlot == 1 && image2 != null)
                        {
                            targetImage = image2;
                            
                        }

                        if (targetImage != null)
                        {
                            targetImage.texture = setting.imageToShow;
                            // 새 이미지를 페이드 인합니다.
                            currentImageTween = targetImage.DOFade(1, fadeInDuration);
                        }
                    }
                }
            }

            // 업데이트되지 않은 이미지는 페이드 아웃합니다.
            // if (!image1Updated && image1 != null)
            // {
            //     image1.DOFade(0, fadeInDuration);
            // }
            // if (!image2Updated && image2 != null)
            // {
            //     image2.DOFade(0, fadeInDuration);
            // }
            // --- 이미지 처리 끝 ---
        }
        else
        {
            // 모든 메시지 및 시퀀스 완료
            Debug.Log("오프닝 시퀀스 완료!");
            // 예: 다음 씬 로드 또는 게임 시작
            if (sceneFader != null)
            {
                sceneFader.FadeOutAndLoadScene("Stage1"); // SceneFader를 통해 씬 이동
            }
            else
            {
                SceneManager.LoadScene("Stage1"); // SceneFader가 없으면 직접 씬 이동
            }
        }
    }

    void SkipCurrentText()
    {
        if (currentTextTween != null && currentTextTween.IsActive() && currentTextTween.IsPlaying())
        {
            currentTextTween.Complete(); // 텍스트 타이핑 즉시 완료
            isTyping = false;
            canAdvance = true; // 스킵 후 다음 진행 가능
        }
    }

    void AdvanceOpeningSequence()
    {
        if (!isTyping) // 텍스트 타이핑 중이 아닐 때만 진행
        {
            currentMessageIndex++;
            DisplayNextMessage();
        }
    }

    // DOTween 시퀀스 제어를 위한 추가 함수 (필요시)
    // public void PauseSequence() { DOTween.Pause(); }
    // public void ResumeSequence() { DOTween.Play(); }
}