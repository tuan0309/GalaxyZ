using UnityEngine;

public class CrosshairCursor : MonoBehaviour
{
    [SerializeField] private Texture2D crosshairTexture;

    private void Start()
    {
        SetCrosshair();
    }

    private void SetCrosshair()
    {
        if (crosshairTexture == null)
        {
            Debug.LogWarning("Chưa gán Crosshair Texture!");
            return;
        }

        Vector2 hotspot = new Vector2(
            crosshairTexture.width * 0.5f,
            crosshairTexture.height * 0.5f
        );

        Cursor.SetCursor(
            crosshairTexture,
            hotspot,
            CursorMode.ForceSoftware
        );

        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Cursor.SetCursor(
            null,
            Vector2.zero,
            CursorMode.Auto
        );
    }
}