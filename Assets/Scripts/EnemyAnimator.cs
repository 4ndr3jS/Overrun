using UnityEngine;

// this script is for automating the animation procces I woud've needed to do 40 animation manually if i did't type this script

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimator : MonoBehaviour
{
    // dir = direction
    private enum Dir { Down = 0, Up = 1, Left = 2, Right = 3 } 

    [Header("All 16 sprites")]
    [SerializeField] private Sprite[] frames = new Sprite[16];

    [SerializeField, Min(1)] private int framePerDir = 4;
    [SerializeField, Min(1)] private float framesPerSec = 8f;
    [SerializeField] private float movementThreshold = 0.01f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Dir currentDir = Dir.Down;
    private int currentFrame;
    private float frameTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        updateDisplayedSprite();
    }

    private void Update()
    {
        Vector2 movement = rb.linearVelocity;

        bool isWalking = movement.sqrMagnitude > movementThreshold * movementThreshold;

        if (!isWalking)
        {
            currentFrame = 0;
            frameTimer = 0f;
            updateDisplayedSprite();
            return;
        }

        UpdateDir(movement);

        frameTimer += Time.deltaTime;

        float frameLength = 1f / framesPerSec;

        if(frameTimer >= frameLength)
        {
            frameTimer -= frameLength;

            currentFrame = (currentFrame + 1) % framePerDir;

            updateDisplayedSprite();
        }
    }

    private void UpdateDir(Vector2 movement)
    {
        Dir newDir;

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            newDir = movement.x > 0f ? Dir.Right : Dir.Left;
        } 
        else
            newDir = movement.y > 0f ? Dir.Up : Dir.Down;

        if (newDir == currentDir)
            return;

        currentDir = newDir;

        currentFrame = 0;
        frameTimer = 0f;

        updateDisplayedSprite();
    }

    private void updateDisplayedSprite()
    {
        if (sr == null || frames == null || frames.Length == 0)
            return;

        int spriteIndex = currentFrame * 4 + (int)currentDir;
        if (spriteIndex < 0 || spriteIndex >= frames.Length)
            return;

        sr.sprite = frames[spriteIndex];
    }
}
