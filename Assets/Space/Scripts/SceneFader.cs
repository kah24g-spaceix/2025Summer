using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    private Image fadeImage; // 화면을 덮을 UI Image (CanvasGroup을 사용하는 것이 더 일반적이지만, Image도 가능)
    public float fadeDuration = 1.0f; // 페이드 인/아웃 시간

    private void Awake()
    {
        // 씬이 로드될 때 파괴되지 않도록 설정 (옵션)
        // DontDestroyOnLoad(gameObject);

        // 초기 상태 설정: 완전히 투명하게 시작
        fadeImage = GetComponent<Image>();
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
            fadeImage.gameObject.SetActive(false); // 비활성화하여 초기에는 보이지 않게 함
        }
    }

    /// <summary>
    /// 화면을 페이드 아웃시키고 지정된 씬으로 이동합니다.
    /// </summary>
    /// <param name="sceneName">이동할 씬의 이름</param>
    public void FadeOutAndLoadScene(string sceneName)
    {
        if (fadeImage == null)
        {
            Debug.LogError("SceneFader: Fade Image is not assigned!");
            return;
        }

        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
        fadeImage.DOFade(1, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    /// <summary>
    /// 화면을 페이드 인시킵니다 (씬 로드 후 사용).
    /// </summary>
    public void FadeIn()
    {
        if (fadeImage == null)
        {
            Debug.LogError("SceneFader: Fade Image is not assigned!");
            return;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1); // 완전히 불투명하게 시작
        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
        fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false); // 페이드 인 완료 후 비활성화
        });
    }
}