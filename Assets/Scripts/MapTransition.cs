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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player"))
            return;
        if(DoorA == null || DoorA.DoorB == null)
        {
            Debug.LogWarning("Something didn't link");
            return;
        }
        TeleportPlayer(other.transform);
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
    }
}
