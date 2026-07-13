using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepInterval = 0.5f;
    public static bool allowTurnWhilePaused = false;
    private PlayerInput playerInput;

    [Header("Footstep surface detection")]
    public Tilemap groundTilemap;
    public Tilemap carpetTilemap;
    public TileBase[] grassTiles;
    public TileBase[] floorTiles;
    public TileBase[] carpetTiles;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        if (PauseController.isGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastMoveInput.x);
            animator.SetFloat("LastInputY", lastMoveInput.y);
            StopFootsteps();
            return;
        }
        rb.linearVelocity = moveInput * moveSpeed;
        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);

        if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
        {
            StartFootsteps();
        }
        else if(rb.linearVelocity.magnitude == 0)
        {
            StopFootsteps();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (PauseController.isGamePaused && !allowTurnWhilePaused)
            return;

        moveInput = context.ReadValue<Vector2>();
        if (moveInput == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastMoveInput.x);
            animator.SetFloat("LastInputY", lastMoveInput.y);
        }
        else
        {

            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
            lastMoveInput = moveInput;
        }
    }

    public void ResyncMoveInput()
    {
        if (playerInput == null)
            return;

        InputAction moveAction = playerInput.actions["Move"];
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        if (moveInput == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastMoveInput.x);
            animator.SetFloat("LastInputY", lastMoveInput.y);
        }
        else
        {

            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
            lastMoveInput = moveInput;
        }
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepInterval);
        
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        string soundName = GetFootstepSound();
        SoundEffectManager.Play(soundName, true);
    }

    private string GetFootstepSound()
    {
        if(carpetTilemap != null)
        {
            Vector3Int carpetCell = carpetTilemap.WorldToCell(transform.position);
            TileBase carpetTile = carpetTilemap.GetTile(carpetCell);

            if (ContainsTile(carpetTiles, carpetTile))
                return "FootstepCarpet";
        }

        if (groundTilemap == null)
            return "Footstep";

        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        TileBase tile = groundTilemap.GetTile(cellPos);

        if (ContainsTile(grassTiles, tile))
            return "Footstep";

        return "FootstepFloor";
    }

    private bool ContainsTile(TileBase[] tileSet, TileBase tile)
    {
        if (tileSet == null)
            return false;
        
        foreach(TileBase t in tileSet)
        {
            if (t == tile)
                return true;
        }

        return false;
    }
}