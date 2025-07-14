using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject scareTriggerPrefab;
    public GameObject WallPrefab;
    public GameObject TilePrefab;

    public int width = 21;
    public int height = 21;
    public float tileRatio = 1f;

    private bool[,] grid;

    void Start()
    {
        GenerateMaze();         // 미로 논리 생성
        GenerateTiles();        // 타일(프리팹) 생성
        PlaceScareTriggers();   // 공포 트리거 배치
        CenterCamera();         // 카메라 중앙 정렬
    }

    void CenterCamera()
    {
        float centerX = (width - 1) * tileRatio / 2f;
        float centerY = (height - 1) * tileRatio / 2f;

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = height / 2f + 1f; // 1 정도는 여유
        Camera.main.transform.position = new Vector3(centerX, centerY, -10f);
    }

    void PlaceScareTriggers()
    {
        List<Vector2> pathTiles = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y]) 
                {
                    pathTiles.Add(new Vector2(x, y));
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (pathTiles.Count == 0) break;

            int index = Random.Range(0, pathTiles.Count);
            Vector2 pos = pathTiles[index];
            pathTiles.RemoveAt(index);

            GameObject trigger = Instantiate(scareTriggerPrefab);
            trigger.transform.position = new Vector2(pos.x * tileRatio, pos.y * tileRatio);

            ScareTrigger scare = trigger.GetComponent<ScareTrigger>();
            scare.scareType = (ScareTrigger.ScareType)Random.Range(0, 3);
        }
    }


    void GenerateMaze()
    {
        grid = new bool[width, height];

        // 모든 셀을 벽으로 초기화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
                grid[x, y] = false;
        }

        // 미로 생성 시작
        Carve(1, 1);
    }

    void Carve(int x, int y)
    {
        grid[x, y] = true;

        // 무작위 방향 섞기
        List<Vector2Int> dirs = new List<Vector2Int> {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };
        Shuffle(dirs);

        foreach (var dir in dirs)
        {
            int nx = x + dir.x * 2;
            int ny = y + dir.y * 2;

            if (nx > 0 && ny > 0 && nx < width && ny < height && !grid[nx, ny])
            {
                grid[x + dir.x, y + dir.y] = true;
                Carve(nx, ny);
            }
        }
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    void GenerateTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject obj = Instantiate(grid[x, y] ? TilePrefab : WallPrefab);
                obj.transform.position = new Vector2(x * tileRatio, y * tileRatio);
            }
        }
    }
}
