using UnityEngine;

public static class CanvasExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition)
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        
        return new Vector3(
            viewportPosition.x * canvasSize.x - (canvasSize.x * 0.5f),
            viewportPosition.y * canvasSize.y - (canvasSize.y * 0.5f)
        );
    }
}