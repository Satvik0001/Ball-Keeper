using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int health = 1;

    // Bright arcade colors based on hits left (1 hit left up to 5 hits left)
    private static readonly Color[] HealthColors =
    {
        new Color(0.2f, 1.0f, 0.3f),  // 1 hit left  -> Bright Neon Green (Almost broken!)
        new Color(0.0f, 0.8f, 1.0f),  // 2 hits left -> Bright Cyan Blue
        new Color(1.0f, 0.9f, 0.1f),  // 3 hits left -> Bright Yellow
        new Color(1.0f, 0.5f, 0.0f),  // 4 hits left -> Bright Orange
        new Color(1.0f, 0.2f, 0.3f)   // 5 hits left -> Bright Neon Red (Toughest)
    };

    private void Awake()
    {
        // Grab SpriteRenderer if not dragged in the inspector
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called when the spawner creates this brick
    public void SetHits(int hits)
    {
        health = Mathf.Clamp(hits, 1, 5);
        UpdateColor();
    }

    [System.Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react if hit by the ball
        if (!collision.gameObject.CompareTag("Ball")) return;

        health--; // Lose 1 health!

        if (health <= 0)
        {
            BreakBrick();
        }
        else
        {
            UpdateColor(); // Change to the next color!
        }
    }

    // Picks the bright color matching how much health is left
    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = HealthColors[health - 1];
        }
    }

    [System.Obsolete]
    private void BreakBrick()
    {
        // Tell spawner that 1 brick was destroyed
        BrickSpawner spawner = FindFirstObjectByType<BrickSpawner>();
        if (spawner != null)
        {
            spawner.BrickDestroyed();
        }

        // Pop! Remove the brick
        Destroy(gameObject);
    }
}