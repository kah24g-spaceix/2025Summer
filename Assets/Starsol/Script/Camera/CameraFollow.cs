using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 5f;

    private Transform target;

    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
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
