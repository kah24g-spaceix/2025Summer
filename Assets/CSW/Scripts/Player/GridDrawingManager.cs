using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class GridDrawingManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float requiredMatchRatio = 0.7f;
    private bool[,] answer = new bool[145, 35]; // 정답 패턴을 외부에서 정의된 배열로 초기화
 

   
    public float cellSize = 1f;
    public int gridWidth = 10, gridHeight = 10;

    [Header("Drawing Settings")]
    public LineRenderer linePrefab;
    public SpriteRenderer drawingAreaMask;

    private bool[,] visited;
    private LineRenderer currentLine;
    private List<Vector3> points;
    private Vector2Int lastGridPos = new Vector2Int(-1, -1);
    private Vector3 originWorld;

    private enum DrawResult { None, Success, Fail }
    private DrawResult lastResult = DrawResult.None;

    void Start()
    {
        visited = new bool[gridWidth, gridHeight];
        points = new List<Vector3>();

        if (drawingAreaMask != null)
            originWorld = drawingAreaMask.bounds.min;
        else
            originWorld = Vector3.zero;
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

        CheckDrawSuccess();
    }

    void EndLine()
    {
        currentLine = null;
        lastGridPos = new Vector2Int(-1, -1);
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
        if (visited == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 center = originWorld + new Vector3((x + 0.5f) * cellSize, (y + 0.5f) * cellSize, 0f);
                Gizmos.color = visited[x, y] ? Color.red : new Color(0.8f, 0.8f, 0.8f, 0.3f);
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
        if (answer == null || visited == null) return;

        int total = 0;
        int matched = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (answer[x, y])
                {
                    total++;
                    if (visited[x, y]) matched++;
                }
            }
        }

        float ratio = (total == 0) ? 0f : (float)matched / total;

        if (ratio >= 0.7f && lastResult != DrawResult.Success)
        {
            Debug.Log("✅ 성공! 충분히 정확하게 그림을 그렸습니다.");
            lastResult = DrawResult.Success;
        }
        else if (ratio <= 0.3f && lastResult != DrawResult.Fail)
        {
            Debug.Log("❌ 실패. 거의 일치하지 않습니다.");
            lastResult = DrawResult.Fail;
        }
    }
}
