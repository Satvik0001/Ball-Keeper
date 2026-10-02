using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float startingSpeed = 5f;
    [SerializeField] private float speedIncrease = 0.3f;
    [SerializeField] private float increaseEverySeconds = 5f;
    [SerializeField] private float maximumSpeed = 12f;

    [Header("Paddle Bounce Angle")]
    [Tooltip("Maximum bounce angle in degrees (60 = bounces up to 60° left or right)")]
    [SerializeField] private float maxPaddleAngle = 60f;

    [Header("Angle Fixes")]
    [SerializeField] private float minimumVerticalAngle = 20f;
    [SerializeField] private float wallAngleChange = 5f;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector2 direction;
    private float currentSpeed;
    private float speedTimer;
    private bool gameStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;
    }

    private void Start()
    {
        ResetBall();
    }

    private void FixedUpdate()
    {
        if (!gameStarted) return;

        // Move the ball smoothly
        Vector2 movement = direction * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void Update()
    {
        if (!gameStarted) return;

        // Gradually increase speed over time
        speedTimer += Time.deltaTime;
        if (speedTimer >= increaseEverySeconds)
        {
            currentSpeed = Mathf.Min(currentSpeed + speedIncrease, maximumSpeed);
            speedTimer = 0f;
        }
    }

    // =====================================================
    // RESET / LAUNCH
    // =====================================================

    public void ResetBall()
    {
        gameStarted = false;
        transform.position = startPosition;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        currentSpeed = startingSpeed;
        speedTimer = 0f;

        // Launch upwards with a slight angle
        direction = new Vector2(0.5f, 1f).normalized;
        gameStarted = true;
    }

    // =====================================================
    // COLLISIONS
    // =====================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PaddleBounce(collision);
        }
        else
        {
            WallOrBrickBounce(collision);
        }
    }

    // =====================================================
    // 60-DEGREE PADDLE BOUNCE
    // =====================================================

    private void PaddleBounce(Collision2D collision)
    {
        float paddleX = collision.transform.position.x;
        float paddleWidth = collision.collider.bounds.size.x;

        // hitOffset is:
        // -1.0 for far left edge
        //  0.0 for exact center
        // +1.0 for far right edge
        float hitOffset = (transform.position.x - paddleX) / (paddleWidth / 2f);
        hitOffset = Mathf.Clamp(hitOffset, -1f, 1f);

        // Turn hit offset into an angle between -60° and +60°
        float bounceAngle = hitOffset * maxPaddleAngle;
        float radians = bounceAngle * Mathf.Deg2Rad;

        // Sin controls horizontal direction, Cos keeps it moving upward!
        direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)).normalized;

        PreventHorizontalLoop();
    }

    // =====================================================
    // WALL / BRICK BOUNCE
    // =====================================================

    private void WallOrBrickBounce(Collision2D collision)
    {
        if (collision.contactCount == 0) return;

        Vector2 normal = collision.contacts[0].normal;

        // Hit side of wall/brick -> reverse horizontal direction
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            direction.x = -direction.x;
        }
        // Hit top/bottom of ceiling/brick -> reverse vertical direction
        else
        {
            direction.y = -direction.y;
        }

        // Add small random variation so ball never gets stuck in an infinite loop
        float randomAngle = Random.Range(-wallAngleChange, wallAngleChange);
        direction = Quaternion.Euler(0, 0, randomAngle) * direction;

        PreventHorizontalLoop();
    }

    // Prevents the ball from bouncing purely left-to-right forever
    private void PreventHorizontalLoop()
    {
        float minVertical = Mathf.Sin(minimumVerticalAngle * Mathf.Deg2Rad);

        if (Mathf.Abs(direction.y) < minVertical)
        {
            direction.y = Mathf.Sign(direction.y) * minVertical;
            direction.Normalize();
        }
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            gameStarted = false;
            GameManager.Instance?.GameOver();
        }
    }
}