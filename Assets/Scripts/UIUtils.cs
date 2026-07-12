using UnityEngine;

public static class UIUtils
{ 
    public static void FitToParent(RectTransform rt, float padding = 15f) 
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(padding, padding);
        rt.offsetMax = new Vector2(-padding, -padding);
        rt.localScale = Vector3.one;
    }
}
