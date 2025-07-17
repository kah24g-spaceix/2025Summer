// Assets/Scripts/GhostBlurFollow.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostBlurFollow : MonoBehaviour
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

    [Header("Follow Settings")]
    [Tooltip("플레이어 Transform을 할당하세요.")]
    public Transform target;
    [Tooltip("플레이어를 따라오는 속도")]
    public float followSpeed = 1f;

    private SpriteRenderer original;
    private List<SpriteRenderer> copies = new List<SpriteRenderer>();
    private bool hasFaded = false;

    void Awake()
    {
        original = GetComponent<SpriteRenderer>();

        // 완전히 안 보이게
        original.enabled = false;
        original.color = new Color(0, 0, 0, 0); // 추가 안전 조치
        Transform capsule = transform.Find("Capsule");
        if (capsule != null)
            Destroy(capsule.gameObject);
        // 복제본 생성
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

            // 완전 투명한 검정색
            sr.color = new Color(0f, 0f, 0f, 0f);

            copies.Add(sr);
        }
    }



    void Start()
    {
        // 씬에 등장하자마자 페이드인 시작
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        // 1) 복제본 위치 분산 & 알파 0 세팅
        foreach (var sr in copies)
        {
            sr.transform.localPosition = Random.insideUnitCircle * radius;
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        // 2) 페이드인 (0 → copyAlpha)
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

        // 3) 최종 알파 보정
        foreach (var sr in copies)
        {
            var c = sr.color;
            c.a = copyAlpha;
            sr.color = c;
        }

        hasFaded = true;
    }

    void Update()
    {
        // A) 페이드 완료 후에만 플레이어를 따라오기
        if (hasFaded && target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                followSpeed * Time.deltaTime
            );
        }

        // B) jitterEveryFrame 옵션이 켜져 있으면 매 프레임마다 위치 갱신
        if (jitterEveryFrame)
        {
            JitterCopies();
        }
    }

    /// <summary>
    /// 복제본들을 원본 주변 radius 반경 내에서 무작위로 퍼뜨립니다.
    /// </summary>
    private void JitterCopies()
    {
        foreach (var sr in copies)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            sr.transform.localPosition = offset;
        }
    }
}
