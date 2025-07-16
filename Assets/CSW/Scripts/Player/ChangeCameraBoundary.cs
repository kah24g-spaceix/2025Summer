using UnityEngine;

public class ChangeCameraBoundary : MonoBehaviour
{
    public SpriteRenderer newBoundary;     // 바꿀 경계 이미지
    public CameraFollow2 cameraFollow;     // MainCamera에 붙은 스크립트

    public void ChangeBoundary()
    {
        cameraFollow.SetBoundary(newBoundary);
    }
}
