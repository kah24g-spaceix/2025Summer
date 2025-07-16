using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject scareTriggerPrefab;
    public GameObject WallPrefab;
    public GameObject TilePrefab;
    public GameObject keyPrefab;        // 열쇠 프리팹
    public GameObject monsterPrefab;    // 몬스터 프리팹

    public int width = 21;
    public int height = 21;
    public float tileRatio = 1f;

    private bool[,] grid;

    private CameraShaker cameraShaker;
    private AudioSource scaryBGM;

    void Start()
    {
        cameraShaker = FindFirstObjectByType<CameraShaker>();
        scaryBGM = FindFirstObjectByType<AudioSource>();

        GenerateMaze();
        GenerateTiles();
        PlaceScareTriggers();
        PlaceKeyItem();
        PlaceMonster();
        CenterCamera();
    }

    void CenterCamera()
    {
        float centerX = (width - 1) * tileRatio / 2f;
        float centerY = (height - 1) * tileRatio / 2f;

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = height / 2f + 1f;
        Camera.main.transform.position = new Vector3(centerX, centerY, -10f);
    }

    void GenerateMaze()
    {
        grid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
                grid[x, y] = false;
        }

        Carve(1, 1, Vector2Int.zero, 0);

        RemoveRandomWalls(10);  // 벽 10개를 무작위로 길로 변경
    }

    void RemoveRandomWalls(int count)
    {
        int removed = 0;
        int attempts = 0;

        while (removed < count && attempts < count * 10)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);

            // 벽이면서 양쪽에 길이 연결되도록(복잡성 유지) 약간 조건 걸 수 있음
            if (!grid[x, y])
            {
                grid[x, y] = true;
                removed++;
            }
            attempts++;
        }
    }

    void Carve(int x, int y, Vector2Int lastDir, int straightLength)
    {
        grid[x, y] = true;

        List<Vector2Int> dirs = new List<Vector2Int> {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        Shuffle(dirs);

        if (lastDir != Vector2Int.zero)
        {
            dirs.Remove(lastDir);
            dirs.Insert(0, lastDir);
        }

        foreach (var dir in dirs)
        {
            int nx = x + dir.x * 2;
            int ny = y + dir.y * 2;

            if (nx > 0 && ny > 0 && nx < width && ny < height && !grid[nx, ny])
            {
                grid[x + dir.x, y + dir.y] = true;

                int nextStraightLength = (dir == lastDir) ? straightLength + 1 : 1;

                if (nextStraightLength >= 4 && Random.value < 0.6f)
                {
                    continue;
                }

                Carve(nx, ny, dir, nextStraightLength);
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

    void PlaceScareTriggers()
    {
        List<Vector2> pathTiles = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                    pathTiles.Add(new Vector2(x, y));
            }
        }

        int totalTriggers = 9;
        int jumpscareCount = 4;

        for (int i = 0; i < totalTriggers; i++)
        {
            if (pathTiles.Count == 0) break;

            int index = Random.Range(0, pathTiles.Count);
            Vector2 pos = pathTiles[index];
            pathTiles.RemoveAt(index);

            GameObject trigger = Instantiate(scareTriggerPrefab);
            trigger.transform.position = pos * tileRatio;

            ScareTrigger scare = trigger.GetComponent<ScareTrigger>();

            if (i < jumpscareCount)
                scare.scareType = ScareTrigger.ScareType.JumpscareImage;
            else
                scare.scareType = (Random.value < 0.5f)
                    ? ScareTrigger.ScareType.CameraShakeWithBGM
                    : ScareTrigger.ScareType.FakeTrigger;

            scare.cameraShaker = cameraShaker;
            scare.scaryBGM = scaryBGM;
        }
    }

    void PlaceKeyItem()
    {
        List<Vector2> pathTiles = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                    pathTiles.Add(new Vector2(x, y));
            }
        }

        if (pathTiles.Count == 0 || keyPrefab == null)
        {
            Debug.LogWarning("KeyItem을 배치할 수 없습니다.");
            return;
        }

        int index = Random.Range(0, pathTiles.Count);
        Vector2 keyPos = pathTiles[index];

        GameObject key = Instantiate(keyPrefab);
        key.transform.position = keyPos * tileRatio;
    }

    void PlaceMonster()
    {
        List<Vector2> pathTiles = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                    pathTiles.Add(new Vector2(x, y));
            }
        }

        if (pathTiles.Count == 0 || monsterPrefab == null)
        {
            Debug.LogWarning("괴물을 배치할 수 없습니다.");
            return;
        }

        int index = Random.Range(0, pathTiles.Count);
        Vector2 monsterPos = pathTiles[index];

        GameObject monster = Instantiate(monsterPrefab);
        monster.transform.position = monsterPos * tileRatio;
    }

    public bool[,] GetGrid()
    {
        return grid;
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x / tileRatio), Mathf.RoundToInt(worldPos.y / tileRatio));
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(gridPos.x * tileRatio, gridPos.y * tileRatio);
    }
}
