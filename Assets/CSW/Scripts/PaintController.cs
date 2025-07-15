using UnityEngine;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{
    public RawImage canvasImage;
    private Texture2D drawTexture;
    private int textureWidth = 512;
    private int textureHeight = 512;

    private Color brushColor = Color.red;
    private int brushSize = 5;

    void Start()
    {
        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        canvasImage.texture = drawTexture;
        ClearCanvas();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasImage.rectTransform,
                Input.mousePosition,
                null,
                out localPos
            );

            float px = localPos.x + canvasImage.rectTransform.rect.width / 2;
            float py = localPos.y + canvasImage.rectTransform.rect.height / 2;

            int x = Mathf.FloorToInt(px * textureWidth / canvasImage.rectTransform.rect.width);
            int y = Mathf.FloorToInt(py * textureHeight / canvasImage.rectTransform.rect.height);

            DrawCircle(x, y);
        }
    }

    void DrawCircle(int cx, int cy)
    {
        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        drawTexture.SetPixel(px, py, brushColor);
                    }
                }
            }
        }
        drawTexture.Apply();
    }

    public void ClearCanvas()
    {
        Color[] colors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }

        drawTexture.SetPixels(colors);
        drawTexture.Apply();
    }
}
