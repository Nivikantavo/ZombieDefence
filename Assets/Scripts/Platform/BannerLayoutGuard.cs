using UnityEngine;

public static class BannerLayoutGuard
{
    private const float DesktopRightPadding = 0.12f;
    private const float MobileTopPadding = 0.12f;

    public static void Apply(bool bannersVisible)
    {
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
                continue;

            RectTransform root = canvas.transform as RectTransform;
            if (root == null)
                continue;

            if (bannersVisible == false)
            {
                root.offsetMin = Vector2.zero;
                root.offsetMax = Vector2.zero;
                continue;
            }

            if (PlaygamaAds.IsMobileDevice())
            {
                float top = canvas.pixelRect.height * MobileTopPadding;
                root.offsetMax = new Vector2(0f, -top);
                root.offsetMin = Vector2.zero;
            }
            else
            {
                float right = canvas.pixelRect.width * DesktopRightPadding;
                root.offsetMax = new Vector2(-right, 0f);
                root.offsetMin = Vector2.zero;
            }
        }
    }
}
