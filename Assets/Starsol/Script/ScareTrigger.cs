using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScareTrigger : MonoBehaviour
{
    public enum ScareType
    {
        JumpscareImage,    // 깜짝 이미지
        CameraShakeWithBGM,// 화면 흔들림 + 긴박한 BGM
        FakeTrigger        // 아무 일도 없음
    }

    public ScareType scareType;

    public GameObject jumpscareImage;         // 캔버스 안의 이미지 (UI)
    public AudioSource scaryBGM;         // 긴박한 배경음
    public CameraShaker cameraShaker;    // 화면 흔들기 스크립트

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        switch (scareType)
        {
            case ScareType.JumpscareImage:
                StartCoroutine(ShowJumpscare());
                break;

            case ScareType.CameraShakeWithBGM:
                StartCoroutine(ShakeAndPlaySound());
                break;

            case ScareType.FakeTrigger:
                // 아무 일도 안 함 (가짜 트리거)
                break;
        }
    }

    IEnumerator ShowJumpscare()
    {
        if (jumpscareImage != null)
        {
            jumpscareImage.SetActive(true);
            yield return new WaitForSeconds(2f);
            jumpscareImage.SetActive(false);
        }
    }

    IEnumerator ShakeAndPlaySound()
    {
        if (cameraShaker != null)
            CameraShaker.Shake(1.5f, 0.2f); // 지속 시간, 강도

        if (scaryBGM != null)
            scaryBGM.Play();

        yield return new WaitForSeconds(3f);

        if (scaryBGM != null)
            scaryBGM.Stop();
    }
}
