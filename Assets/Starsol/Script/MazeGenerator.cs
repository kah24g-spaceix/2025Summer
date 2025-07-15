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

    //  추가: CameraShaker와 AudioSource 참조
    private CameraShaker cameraShaker;
    private AudioSource scaryBGM;

    void Start()
    {
        //  CameraShaker와 BGM 오브젝트 찾기
        cameraShaker = FindFirstObjectByType<CameraShaker>();
        scaryBGM = FindFirstObjectByType<AudioSource>();

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
        Camera.main.orthographicSize = height / 2f + 1f;
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

        int totalTriggers = 9;

        //  목표: Jump 3개, 나머지 4개
        int jumpscareCount = 4;
        int otherCount = totalTriggers - jumpscareCount;

        for (int i = 0; i < totalTriggers; i++)
        {
            if (pathTiles.Count == 0) break;

            int index = Random.Range(0, pathTiles.Count);
            Vector2 pos = pathTiles[index];
            pathTiles.RemoveAt(index);

            GameObject trigger = Instantiate(scareTriggerPrefab);
            trigger.transform.position = new Vector2(pos.x * tileRatio, pos.y * tileRatio);

            ScareTrigger scare = trigger.GetComponent<ScareTrigger>();

            //  트리거 종류 나누기
            if (i < jumpscareCount)
            {
                scare.scareType = ScareTrigger.ScareType.JumpscareImage;
            }
            else
            {
                scare.scareType = (Random.value < 0.5f) ? ScareTrigger.ScareType.CameraShakeWithBGM : ScareTrigger.ScareType.FakeTrigger;
            }

            // 자동 연결
            scare.cameraShaker = cameraShaker;
            scare.scaryBGM = scaryBGM;
        }
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
}
