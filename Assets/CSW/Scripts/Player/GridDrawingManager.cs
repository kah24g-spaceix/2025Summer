using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Collections;

public class GridDrawingManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float requiredMatchRatio = 0.7f;
    private bool[,] answer = new bool[90, 20];

    public float cellSize = 1f;
    public int gridWidth = 10, gridHeight = 10;

    [Header("Drawing Settings")]
    public LineRenderer linePrefab;
    public SpriteRenderer drawingAreaMask;

    public Transform player;
    public Transform TargetPosition;

    private bool[,] visited;
    private LineRenderer currentLine;
    private List<Vector3> points;
    private Vector2Int lastGridPos = new Vector2Int(-1, -1);
    private Vector3 originWorld;

    void Start()
    {
        Init();
    }

    void Init()
    {
        visited = new bool[gridWidth, gridHeight];
        points = new List<Vector3>();
        if (currentLine)
            currentLine.positionCount = 0;
        currentLine = null;
        if (drawingAreaMask != null)
            originWorld = drawingAreaMask.bounds.min;
        else
            originWorld = Vector3.zero;
        drawingAreaMask.gameObject.SetActive(false);
        InitializeAnswer(); // ✅ 직접 좌표로 정답 초기화
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryStartLine();
        else if (Input.GetMouseButton(0) && currentLine != null) ContinueLine();
        else if (Input.GetMouseButtonUp(0) && currentLine != null) EndLine();
    }

    void TryStartLine()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        Vector2Int gridPos = WorldToGrid(worldPos);
        Vector3 snapped = GridToWorld(gridPos);

        if (!PointInsideSprite(snapped)) return;

        currentLine = Instantiate(linePrefab, transform);
        points.Clear();
        points.Add(snapped);
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, snapped);

        MarkVisited(gridPos);
        lastGridPos = gridPos;
    }

    void ContinueLine()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector2Int gridPos = WorldToGrid(worldPos);

        if (gridPos == lastGridPos) return;
        if (!IsNeighbor(gridPos, lastGridPos)) return;

        Vector3 snapped = GridToWorld(gridPos);
        if (!PointInsideSprite(snapped)) return;

        points.Add(snapped);
        currentLine.positionCount = points.Count;
        currentLine.SetPosition(points.Count - 1, snapped);

        MarkVisited(gridPos);
        lastGridPos = gridPos;
    }

    void EndLine()
    {
        lastGridPos = new Vector2Int(-1, -1);
        CheckDrawSuccess();
    }

    Vector2Int WorldToGrid(Vector3 world)
    {
        Vector3 local = world - originWorld;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.y / cellSize);
        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        return originWorld + new Vector3((grid.x + 0.5f) * cellSize, (grid.y + 0.5f) * cellSize, 0f);
    }

    bool IsNeighbor(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    bool PointInsideSprite(Vector3 point)
    {
        if (drawingAreaMask == null) return true;
        Vector3 localPos = drawingAreaMask.transform.InverseTransformPoint(point);
        Rect rect = drawingAreaMask.sprite.rect;
        Vector2 pivot = drawingAreaMask.sprite.pivot;

        float px = localPos.x * drawingAreaMask.sprite.pixelsPerUnit + pivot.x;
        float py = localPos.y * drawingAreaMask.sprite.pixelsPerUnit + pivot.y;
        return px >= 0 && py >= 0 && px < rect.width && py < rect.height;
    }

    void OnDrawGizmos()
    {
        if (visited == null || answer == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 center = originWorld + new Vector3((x + 0.5f) * cellSize, (y + 0.5f) * cellSize, 0f);
                Gizmos.color = visited[x, y] ? Color.red : new Color(0.8f, 0.8f, 0.8f, 0.2f);
                Gizmos.DrawCube(center, Vector3.one * cellSize * 0.9f);
                Gizmos.color = answer[x, y] ? Color.blue : new Color(0.8f, 0.8f, 0.8f, 0.1f);
                Gizmos.DrawCube(center, Vector3.one * cellSize * 0.9f);
            }
        }
    }

    void MarkVisited(Vector2Int gridPos)
    {
        if (gridPos.x >= 0 && gridPos.x < gridWidth && gridPos.y >= 0 && gridPos.y < gridHeight)
        {
            visited[gridPos.x, gridPos.y] = true;
        }
    }
    public void CheckDrawSuccess()
    {
        StartCoroutine(CheckDrawSuccessCoroutine());
    }

    private IEnumerator CheckDrawSuccessCoroutine()
    {
        if (answer == null || visited == null) yield break;

        int total = 0;
        int matched = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (visited[x, y])
                {
                    total++;
                    if (answer[x, y])
                    {
                        matched++;
                    }
                }
            }
        }

        float ratio = (total == 0) ? 0f : (float)matched / total;

        if (ratio >= requiredMatchRatio)
        {
            Debug.Log("✅ 성공! 충분히 정확하게 그림을 그렸습니다.");
        }
        else
        {
            Debug.Log("❌ 실패. 거의 일치하지 않습니다.");

            // 👉 페이드 오브젝트 찾기 (씬에서 직접 찾음)
            ScreenFader fader = FindObjectOfType<ScreenFader>();
            if (fader != null)
            {
                yield return StartCoroutine(fader.FadeOut());

                // 순간이동
                player.transform.position = TargetPosition.transform.position;
                Init();

                yield return StartCoroutine(fader.FadeIn());
            }
            else
            {
                // ScreenFader가 없으면 그냥 이동
                player.transform.position = TargetPosition.transform.position;
            }
        }

        Debug.Log(ratio + " " + total + " " + matched + " 완료.");
    }





    // ✅ 추가: 정답지 초기화 함수
    void InitializeAnswer()
    {
        answer = new bool[gridWidth, gridHeight];

        // 새로운 "S"자 형태 정답 좌표
        answer[56, 6] = true;
        answer[57, 7] = true;
        answer[57, 8] = true;
        answer[57, 9] = true;
        answer[56, 10] = true;
        answer[55, 10] = true;
        answer[55, 11] = true;
        answer[56, 11] = true;
        answer[57, 11] = true;
        answer[58, 11] = true;
        answer[58, 12] = true;
        answer[58, 13] = true;
        answer[58, 14] = true;
        answer[59, 11] = true;
        answer[59, 12] = true;
        answer[59, 13] = true;
        answer[59, 14] = true;
        answer[57, 14] = true;
        answer[56, 14] = true;
        answer[55, 14] = true;
        answer[55, 15] = true;


        


    }


}