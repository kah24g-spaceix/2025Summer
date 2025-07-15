using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject cam;

    private void Awake()
    {
        cam = GameObject.Find("Main Camera");
    }
    void LateUpdate()
    {
        cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
    }
}
