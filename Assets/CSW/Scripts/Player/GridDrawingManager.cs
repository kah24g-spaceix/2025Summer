using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 한 붓 그리기를 위한 2D 격자 기반 드로잉 매니저
/// - 격자는 눈에 보이지 않음
/// - 마지막 그린 점에서만 이어서 그리기
/// - 특정 영역(예: Sprite 영역) 내에서만 그리기
/// - 그린 영역을 2차원 배열로 저장
/// </summary>
public class GridDrawingManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;                  // 격자 셀 크기
    public int gridWidth = 10, gridHeight = 10;  // 배열 크기

    [Header("Drawing Settings")]
    public LineRenderer linePrefab;              // 그리기할 LineRenderer 프리팹
    public SpriteRenderer drawingAreaMask;       // 드로잉 가능 영역을 정의하는 SpriteRenderer

    private bool[,] visited;                     // 그린 점을 저장할 2D 배열
    private LineRenderer currentLine;            // 현재 그리고 있는 LineRenderer
    private List<Vector3> points;               // 현재 선의 점 목록
    private Vector2Int lastGridPos = new Vector2Int(-1, -1);
    private Vector3 originWorld;                // 그리드 원점 (이미지 좌하단)

    void Start()
    {
        visited = new bool[gridWidth, gridHeight];
        points = new List<Vector3>();

        // 이미지 좌측 하단을 격자 원점으로 설정
        if (drawingAreaMask != null)
            originWorld = drawingAreaMask.bounds.min;
        else
            originWorld = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartLine();
        else if (Input.GetMouseButton(0) && currentLine != null)
            ContinueLine();
        else if (Input.GetMouseButtonUp(0) && currentLine != null)
            EndLine();

        if (Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < gridWidth; i++)
            {
                string output = "";
                for (int j = 0; j < gridHeight; j++)
                {
                    output += visited[i, j] + " ";
                }
                Debug.Log(output);
            }
        }
    }

    void TryStartLine()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        // 격자에 스냅
        Vector2Int gridPos = WorldToGrid(worldPos);
        Vector3 snapped = GridToWorld(gridPos);

        // 영역 안에 있는지 확인 (Sprite 영역 내)
        if (!PointInsideSprite(snapped)) return;

        // 새 선 시작
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
        currentLine = null;
        lastGridPos = new Vector2Int(-1, -1);
    }

    // 월드 좌표 -> 격자 인덱스 (이미지 좌하단 기준)
    Vector2Int WorldToGrid(Vector3 world)
    {
        Vector3 local = world - originWorld;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.y / cellSize);
        return new Vector2Int(x, y);
    }

    // 격자 인덱스 -> 월드 좌표 (이미지 좌하단 기준)
    Vector3 GridToWorld(Vector2Int grid)
    {
        return originWorld + new Vector3((grid.x + 0.5f) * cellSize,
                                         (grid.y + 0.5f) * cellSize,
                                         0f);
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
        if (visited == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 center = originWorld + new Vector3((x + 0.5f) * cellSize,
                                                          (y + 0.5f) * cellSize,
                                                          0f);
                Gizmos.color = visited[x, y] ? Color.red : new Color(0.8f, 0.8f, 0.8f, 0.3f);
                Gizmos.DrawCube(center, Vector3.one * cellSize * 0.9f);
            }
        }
    }

    void MarkVisited(Vector2Int gridPos)
    {
        if (gridPos.x >= 0 && gridPos.x < gridWidth &&
            gridPos.y >= 0 && gridPos.y < gridHeight)
        {
            visited[gridPos.x, gridPos.y] = true;
        }
    }
}
