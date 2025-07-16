using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSize = 3f;

    void LateUpdate()
    {
        Camera cam = Camera.main;

        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = zoomSize;
        }
    }
}
