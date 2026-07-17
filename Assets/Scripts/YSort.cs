using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private SpriteRenderer sr;

    public int sortingPrecision = 100;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        sr.sortingOrder = (int)(-transform.position.y * sortingPrecision);
    }
}
