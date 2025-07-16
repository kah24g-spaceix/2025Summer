using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject scareTriggerPrefab;
    public GameObject WallPrefab;
    public GameObject TilePrefab;
    public GameObject keyPrefab; // 열쇠 프리팹

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
        PlaceKeyItem(); // 열쇠 배치
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

        Carve(1, 1);
    }

    void Carve(int x, int y)
    {
        grid[x, y] = true;

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
                scare.scareType = (Random.value < 0.5f) ? ScareTrigger.ScareType.CameraShakeWithBGM : ScareTrigger.ScareType.FakeTrigger;

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
}
