using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridPainter : MonoBehaviour
{
    public RawImage canvasImage;
    public int textureSize = 512;
    public int gridCount = 10;
    private Texture2D texture;

    private List<Vector2Int> gridPoints = new List<Vector2Int>();
    private int spacing;
    private Vector2Int? currentPoint = null; // 시작점 또는 마지막 점

    void Start()
    {
        spacing = textureSize / gridCount;
        CreateTexture();
        GenerateGridPoints();
        DrawGridPoints(); // 점을 시각화 (작은 점)
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int mousePixel = GetMousePixelPosition();
            Vector2Int nearest = FindNearestGridPoint(mousePixel);

            if (currentPoint == null)
            {
                currentPoint = nearest;
            }
            else if (IsAdjacent(currentPoint.Value, nearest))
            {
                DrawLine(currentPoint.Value, nearest);
                currentPoint = nearest;
            }
        }
    }

    void CreateTexture()
    {
        texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Point;
        Color[] clear = new Color[textureSize * textureSize];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.white;
        texture.SetPixels(clear);
        texture.Apply();
        canvasImage.texture = texture;
    }

    void GenerateGridPoints()
    {
        gridPoints.Clear();
        for (int x = 0; x <= gridCount; x++)
        {
            for (int y = 0; y <= gridCount; y++)
            {
                gridPoints.Add(new Vector2Int(x * spacing, y * spacing));
            }
        }
    }

    Vector2Int FindNearestGridPoint(Vector2 pos)
    {
        Vector2Int nearest = gridPoints[0];
        float minDist = Vector2.Distance(pos, nearest);
        foreach (var p in gridPoints)
        {
            float d = Vector2.Distance(pos, p);
            if (d < minDist)
            {
                minDist = d;
                nearest = p;
            }
        }
        return nearest;
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx == spacing && dy == 0) || (dx == 0 && dy == spacing);
    }

    Vector2Int GetMousePixelPosition()
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasImage.rectTransform,
            Input.mousePosition,
            null,
            out local
        );

        float px = local.x + canvasImage.rectTransform.rect.width / 2;
        float py = local.y + canvasImage.rectTransform.rect.height / 2;

        int x = Mathf.FloorToInt(px * textureSize / canvasImage.rectTransform.rect.width);
        int y = Mathf.FloorToInt(py * textureSize / canvasImage.rectTransform.rect.height);

        return new Vector2Int(x, y);
    }

    void DrawLine(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x), dy = Mathf.Abs(to.y - from.y);
        int sx = from.x < to.x ? 1 : -1;
        int sy = from.y < to.y ? 1 : -1;
        int err = dx - dy;

        int x = from.x;
        int y = from.y;

        while (true)
        {
            texture.SetPixel(x, y, Color.black);
            if (x == to.x && y == to.y) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x += sx; }
            if (e2 < dx) { err += dx; y += sy; }
        }
        texture.Apply();
    }

    void DrawGridPoints()
    {
        foreach (var point in gridPoints)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int px = point.x + dx;
                    int py = point.y + dy;
                    if (px >= 0 && px < textureSize && py >= 0 && py < textureSize)
                        texture.SetPixel(px, py, Color.gray); // 회색 점
                }
            }
        }
        texture.Apply();
    }
}