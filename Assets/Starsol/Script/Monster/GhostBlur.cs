// Assets/Scripts/GhostBlurFollow.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostBlur: MonoBehaviour
{
    [Header("Blur Settings")]
    [Tooltip("���� �ֺ��� �� ���� �������� ��������")]
    public int copyCount = 5;
    [Tooltip("�� �������� �۶߸� �ִ� ������")]
    public float radius = 0.2f;
    [Tooltip("�������� �ִ� ���� (0~1)")]
    [Range(0f, 1f)]
    public float copyAlpha = 0.3f;
    [Tooltip("������� ���߿��� ��� ������ ��ġ�� ��������")]
    public bool jitterEveryFrame = false;

    [Header("Fade Settings")]
    [Tooltip("���̵��� ���� �ð� (��)")]
    public float fadeDuration = 1f;

    private SpriteRenderer original;
    private List<SpriteRenderer> copies = new List<SpriteRenderer>();

    void Awake()
    {
        original = GetComponent<SpriteRenderer>();
        original.enabled = false;

        for (int i = 0; i < copyCount; i++)
        {
            GameObject go = new GameObject("GhostCopy");
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = original.sprite;
            sr.material = original.sharedMaterial;
            sr.sortingLayerID = original.sortingLayerID;
            sr.sortingOrder = original.sortingOrder;

            Color init = original.color;
            init.a = 0f;
            sr.color = init;

            copies.Add(sr);
        }
    }

    void Start()
    {
        StartCoroutine(FadeInCoroutine1());
    }

    private IEnumerator FadeInCoroutine1()
    {
        foreach (var sr in copies)
        {
            sr.transform.localPosition = Random.insideUnitCircle * radius;
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, copyAlpha, t / fadeDuration);
            foreach (var sr in copies)
            {
                var c = sr.color;
                c.a = a;
                sr.color = c;
            }
            yield return null;
        }

        foreach (var sr in copies)
        {
            var c = sr.color;
            c.a = copyAlpha;
            sr.color = c;
        }
    }

    void Update()
    {
        if (jitterEveryFrame)
        {
            JitterCopies1();
        }
    }

    private void JitterCopies1()
    {
        foreach (var sr in copies)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            sr.transform.localPosition = offset;
        }
    }
}
