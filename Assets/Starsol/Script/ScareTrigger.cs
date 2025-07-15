using System.Collections;
using UnityEngine;

public class ScareTrigger : MonoBehaviour
{
    public enum ScareType
    {
        JumpscareImage,
        CameraShakeWithBGM,
        FakeTrigger
    }

    public ScareType scareType;

    public GameObject jumpscareImagePrefab;
    public AudioSource scaryBGM;

    public CameraShaker cameraShaker;
    private bool activated = false;

    private void Start()
    {
        cameraShaker = FindFirstObjectByType<CameraShaker>();
        if (cameraShaker == null)
        {
            Debug.LogWarning("CameraShaker");
        }
    }

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
                break;
        }
    }

    IEnumerator ShowJumpscare()
    {
        if (jumpscareImagePrefab != null)
        {
            GameObject instance = Instantiate(jumpscareImagePrefab);
            yield return new WaitForSeconds(2f);
            Destroy(instance);
        }
        else
        {
            Debug.LogWarning("Jumpscare Image Prefab.");
        }
    }

    IEnumerator ShakeAndPlaySound()
    {
        if (cameraShaker != null)
            cameraShaker.ShakeInstance(1.5f, 0.2f);

        if (scaryBGM != null)
            scaryBGM.Play();

        yield return new WaitForSeconds(3f);

        if (scaryBGM != null)
            scaryBGM.Stop();
    }
}
