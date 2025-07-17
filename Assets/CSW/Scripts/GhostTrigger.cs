
// Assets/Scripts/GhostTrigger.cs (수정)
using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    public GameObject ghostPrefab;
    public bool spawnOnce = true;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (spawnOnce && hasSpawned) return;

        // 트리거 영역 위치에 귀신 생성
        GameObject ghost = Instantiate(
            ghostPrefab,
            transform.position,
            Quaternion.identity
        );

        // 플레이어 Transform 할당
        var gbf = ghost.GetComponent<GhostBlurFollow>();
        if (gbf != null)
            gbf.target = other.transform;

        hasSpawned = true;
    }
}
