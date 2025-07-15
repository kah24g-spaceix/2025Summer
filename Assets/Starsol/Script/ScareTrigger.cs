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

    public GameObject jumpscareImagePrefab;  // 런타임에 생성할 이미지 프리팹 (UI가 아닌 일반 오브젝트도 가능)
    public AudioSource scaryBGM;             // 긴박한 배경음
    public CameraShaker cameraShaker;        // 화면 흔들기 스크립트

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
        if (jumpscareImagePrefab != null)
        {
            GameObject instance = Instantiate(jumpscareImagePrefab);

            // 원하는 위치나 회전 조정이 필요하다면 여기에 추가
            // 예: instance.transform.position = somePosition;

            yield return new WaitForSeconds(2f);

            Destroy(instance);
        }
        else
        {
            Debug.LogWarning("Jumpscare Image Prefab이 비어있습니다.");
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
