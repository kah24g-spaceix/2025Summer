using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ScareTrigger : MonoBehaviour
{
    public enum ScareType
    {
        JumpscareImage,    // ��¦ �̹���
        CameraShakeWithBGM,// ȭ�� ��鸲 + ����� BGM
        FakeTrigger        // �ƹ� �ϵ� ����
    }

    public ScareType scareType;

    public GameObject jumpscareImagePrefab;  // ��Ÿ�ӿ� ������ �̹��� ������ (UI�� �ƴ� �Ϲ� ������Ʈ�� ����)
    public AudioSource scaryBGM;             // ����� �����
    public CameraShaker cameraShaker;        // ȭ�� ���� ��ũ��Ʈ

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
                // �ƹ� �ϵ� �� �� (��¥ Ʈ����)
                break;
        }
    }

    IEnumerator ShowJumpscare()
    {
        if (jumpscareImagePrefab != null)
        {
            GameObject instance = Instantiate(jumpscareImagePrefab);

            // ���ϴ� ��ġ�� ȸ�� ������ �ʿ��ϴٸ� ���⿡ �߰�
            // ��: instance.transform.position = somePosition;

            yield return new WaitForSeconds(2f);

            Destroy(instance);
        }
        else
        {
            Debug.LogWarning("Jumpscare Image Prefab�� ����ֽ��ϴ�.");
        }
    }

    IEnumerator ShakeAndPlaySound()
    {
        if (cameraShaker != null)
            CameraShaker.Shake(1.5f, 0.2f); // ���� �ð�, ����

        if (scaryBGM != null)
            scaryBGM.Play();

        yield return new WaitForSeconds(3f);

        if (scaryBGM != null)
            scaryBGM.Stop();
    }

}
