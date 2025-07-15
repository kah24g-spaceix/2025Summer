using UnityEngine;

public enum BubbleType
{
    Speech,
    Thought
}
[System.Serializable]
public class DialogueLine
{
    [Tooltip("대화의 주제. 이 Transform의 위치에 말풍선이 생성됩니다.")]
    public Transform speakerTransform;

    [Tooltip("말풍선 종류")]
    public BubbleType bubbleType;

    [Tooltip("대화 내용")]
    public string sentence;
}

[System.Serializable]
public class Conversation
{
    public DialogueLine[] lines;
}