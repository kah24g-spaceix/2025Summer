using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static void Shake(float duration, float magnitude)
    {
        instance.StartCoroutine(instance.ShakeRoutine(duration, magnitude));
    }

    private static CameraShaker instance;
    void Awake() { instance = this; }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
}
