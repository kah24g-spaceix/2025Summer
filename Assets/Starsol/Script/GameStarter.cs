using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public MazeGenerator mazeGenerator; // 미로 생성기
    public GameObject playerPrefab;     // 플레이어 프리팹

    void Start()
    {
        // 미로가 생성된 뒤 플레이어 배치
        StartGame();
    }

    void StartGame()
    {
        Vector2 startPos = new Vector2(1 * mazeGenerator.tileRatio, 1 * mazeGenerator.tileRatio);
        Instantiate(playerPrefab, startPos, Quaternion.identity);
    }
}
