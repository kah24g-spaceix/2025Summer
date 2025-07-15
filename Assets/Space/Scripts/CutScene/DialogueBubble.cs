using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI sentenceText;
    private Image bubbleImage;

    [Header("Bubble Sprites")]
    public Sprite speechBubbleSprite;
    public Sprite thoughtBubbleSprite;

    [Header("Tracking")]
    private Transform targetTransform;
    public Vector3 offset = new Vector3(0, 2.0f, 0);

    private Camera mainCamera;

    private void Awake()
    {
        bubbleImage = GetComponent<Image>();
        mainCamera = Camera.main;
    }

    public void Setup(DialogueLine line)
    {
        targetTransform = line.speakerTransform;
        sentenceText.text = line.sentence;

        if (line.bubbleType == BubbleType.Thought)
        {
            bubbleImage.sprite = thoughtBubbleSprite;
        }
        else
        {
            bubbleImage.sprite = speechBubbleSprite;
        }
        UpdatePosition();
    }

    private void LateUpdate()
    {
        if (targetTransform != null)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        transform.position = targetTransform.position + offset;
        transform.rotation = mainCamera.transform.rotation;
    }
}


