using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [Header("Linking")]
    public MapTransition DoorA;
    public Transform DoorB;

    [Header("Confiner ")]
    public Collider2D roomBounds;

    [Header("Refrences")]
    public CinemachineCamera cmCamera;
    public CinemachineConfiner2D confiner;

    private Collider2D myCollider;
    private bool ignoreTrigger = false;
    private Transform ignoredPlayer;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>(); ;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player"))
            return;

        if (ignoreTrigger)
            return;

        if(DoorA == null || DoorA.DoorB == null)
        {
            Debug.LogWarning("Something didn't link");
            return;
        }

        TeleportPlayer(other.transform);
    }
    
    private void Update()
    {
        if (ignoreTrigger && ignoredPlayer != null && myCollider != null)
        {
            bool stillInside = myCollider.OverlapPoint(ignoredPlayer.position);
            if (!stillInside)
            {
                ignoreTrigger = false;
                ignoredPlayer = null;
            }
        }
    }

    public void TeleportPlayer(Transform player)
    {

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Vector3 oldPosition = player.position;
        Vector3 newPosition = DoorA.DoorB.position;
        player.position = newPosition;  

        if(confiner != null && DoorA.roomBounds != null)
        {
            confiner.BoundingShape2D = DoorA.roomBounds;
            confiner.InvalidateBoundingShapeCache();
        }
        
        if(cmCamera != null)
        {
            Vector3 delta = player.position - oldPosition;
            cmCamera.OnTargetObjectWarped(player, delta);
        }

        DoorA.ArmIgnore(player);
    }

    private void ArmIgnore(Transform player)
    {
        ignoreTrigger = true;
        ignoredPlayer = player;
    }
}
