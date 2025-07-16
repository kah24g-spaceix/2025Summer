using UnityEngine;
using System.Collections;

public class CameraFollow2 : MonoBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 5f;
    public SpriteRenderer boundaryImage; // 카메라가 벗어나지 않을 이미지

    private Transform target;
    private float halfHeight;
    private float halfWidth;
    private Bounds cameraBounds;

    void Start()
    {
        StartCoroutine(FindPlayer());

        if (boundaryImage != null)
        {
            // 카메라 사이즈 계산
            Camera cam = Camera.main;
            halfHeight = cam.orthographicSize;
            halfWidth = halfHeight * cam.aspect;

            // 이미지의 경계
            cameraBounds = boundaryImage.bounds;
        }
    }

    void LateUpdate()
    {
        if (target == null || boundaryImage == null) return;

        // 따라갈 위치 계산
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // 카메라 위치 제한 (이미지 경계 - 카메라 절반크기 고려)
        float minX = cameraBounds.min.x + halfWidth;
        float maxX = cameraBounds.max.x - halfWidth;
        float minY = cameraBounds.min.y + halfHeight;
        float maxY = cameraBounds.max.y - halfHeight;

        float clampedX = Mathf.Clamp(smoothPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothPos.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, smoothPos.z);
    }

    IEnumerator FindPlayer()
    {
        while (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                target = playerObj.transform;
                yield break;
            }
            yield return null; 
        }
    }
}
