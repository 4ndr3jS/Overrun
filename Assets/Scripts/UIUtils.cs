using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{ 
    public static void FitToParent(RectTransform rt, float padding = 15f) 
    {
        RectTransform parentRt = rt.parent as RectTransform;
        if (parentRt != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRt);

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(padding, padding);
        rt.offsetMax = new Vector2(-padding, -padding);
        rt.localScale = Vector3.one;
    }

    public static void FitAndPreserveAspectRatio(RectTransform rt, float padding = 15f)
    {
        RectTransform parentRt = rt.parent as RectTransform;
        if (parentRt == null)
        {
            FitToParent(rt, padding);
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRt);

        float aspect = 1f;
        Image img = rt.GetComponent<Image>();

        if(img != null && img.sprite != null)
        {
            Sprite s = img.sprite;
            aspect = s.rect.width / s.rect.height;
        }
        else
        {
            SpriteRenderer sr = rt.GetComponent<SpriteRenderer>();
            
            if(sr != null && sr.sprite != null)
            {
                Sprite s = sr.sprite;
                aspect = s.rect.width / s.rect.height;
            }
        }

        float availW = parentRt.rect.width - padding * 2f;
        float availH = parentRt.rect.height - padding * 2f;

        float targetW = availW;
        float targetH = availW / aspect;

        if (targetH > availH)
        {
            targetH = availH;
            targetW = targetH * aspect;
        }

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(targetW, targetH);
        rt.localScale = Vector3.one;
    }
}
