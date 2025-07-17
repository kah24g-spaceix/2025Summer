using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneFadeInManager : MonoBehaviour
{
    public Image fadeImage; // 화면을 덮을 UI Image
    public float fadeInDuration = 1.0f; // 페이드 인 시간

    void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogError("SceneFadeInManager: Fade Image is not assigned!");
            return;
        }

        // 페이드 이미지를 활성화하고 완전히 불투명하게 설정
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        // 페이드 인 애니메이션 시작
        fadeImage.DOFade(0, fadeInDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false); // 페이드 인 완료 후 비활성화
        });
    }
}