using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI sentenceText;
    public Image tailImage;

    [Header("Tail Sprites")]
    public Sprite speechTailSprite;
    public Sprite thoughtTailSprite;

    [Header("Tracking")]
    private Transform targetTransform;
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Setup(DialogueLine line)
    {
        targetTransform = line.speakerTransform;
        sentenceText.text = line.sentence;

        if (line.tailType == TailType.Speech)
        {
            tailImage.sprite = speechTailSprite;
        }
        else
        {
            tailImage.sprite = thoughtTailSprite;
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


