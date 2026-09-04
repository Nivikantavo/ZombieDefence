using UnityEngine;
using UnityEngine.UI;

public static class AdBlockOverlay
{
    private static GameObject _root;

    public static void Show()
    {
        if (_root != null)
        {
            _root.SetActive(true);
            return;
        }

        _root = new GameObject("AdBlockOverlay");
        Object.DontDestroyOnLoad(_root);

        Canvas canvas = _root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        _root.AddComponent<CanvasScaler>();
        _root.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("Blocker");
        imageObject.transform.SetParent(_root.transform, false);
        Image image = imageObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.35f);
        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public static void Hide()
    {
        if (_root != null)
            _root.SetActive(false);
    }
}
