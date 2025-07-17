// Assets/Scripts/GhostBlurFollow.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostBlur: MonoBehaviour
{
    [Header("Blur Settings")]
    [Tooltip("원본 주변에 몇 개의 복제본을 생성할지")]
    public int copyCount = 5;
    [Tooltip("각 복제본을 퍼뜨릴 최대 반지름")]
    public float radius = 0.2f;
    [Tooltip("복제본의 최대 투명도 (0~1)")]
    [Range(0f, 1f)]
    public float copyAlpha = 0.3f;
    [Tooltip("따라오는 도중에도 계속 복제본 위치를 갱신할지")]
    public bool jitterEveryFrame = false;

    [Header("Fade Settings")]
    [Tooltip("페이드인 지속 시간 (초)")]
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
