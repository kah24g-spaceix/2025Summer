using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public MazeGenerator mazeGenerator; // �̷� ������
    public GameObject playerPrefab;     // �÷��̾� ������

    void Start()
    {
        // �̷ΰ� ������ �� �÷��̾� ��ġ
        StartGame();
    }

    void StartGame()
    {
        Vector2 startPos = new Vector2(1 * mazeGenerator.tileRatio, 1 * mazeGenerator.tileRatio);
        Instantiate(playerPrefab, startPos, Quaternion.identity);
    }
}
