using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class VitalsVisibController : MonoBehaviour
{
    [Header("Map Bounds")]
    public List<Collider2D> mapBounds = new List<Collider2D>();

    [Header("Vitals UI")]
    public GameObject vitalsUI;

    [Header("Player")]
    public Transform player;

    private bool isVisable = true;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (vitalsUI != null)
        {
            isVisable = isPlayerInBound();
            vitalsUI.SetActive(isVisable);
        }
    }

    private void Update()
    {
        if (vitalsUI == null || player == null || mapBounds.Count == 0)
            return;

        bool inside = isPlayerInBound();

        if(inside != isVisable)
        {
            isVisable = inside;
            vitalsUI.SetActive(isVisable);
        }
    }

    private bool isPlayerInBound()
    {
        for(int i = 0; i < mapBounds.Count; i++)
        {
            Collider2D bound = mapBounds[i];
            if (bound != null && bound.OverlapPoint(player.position))
                return true;
        }

        return false;
    }
}
