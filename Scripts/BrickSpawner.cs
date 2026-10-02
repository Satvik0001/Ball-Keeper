using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [Header("Brick Prefab & Parent")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform brickParent;

    [Header("Grid Size")]
    [SerializeField] private int maxRows = 6;
    [SerializeField] private int maxColumns = 4;

    [Header("Spawn Chance")]
    [Range(0.1f, 1f)]
    [Tooltip("0.7 means 70% chance a brick spawns at each spot")]
    [SerializeField] private float spawnChance = 0.7f;

    [Header("Spacing")]
    [SerializeField] private float horizontalSpacing = 1.25f;
    [SerializeField] private float verticalSpacing = 0.6f;
    [SerializeField] private float startY = 3.8f;

    private int remainingBricks;

    public void GenerateLevel(int level)
    {
        ClearBricks();

        // One simple loop through every row and column!
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxColumns; col++)
            {
                // Roll a random number between 0.0 and 1.0
                if (Random.value < spawnChance)
                {
                    SpawnBrick(row, col, level);
                }
            }
        }

        // Just in case it randomly spawned 0 bricks, spawn at least one brick!
        if (remainingBricks == 0)
        {
            SpawnBrick(0, 0, level);
        }

        Debug.Log($"Level {level} generated with {remainingBricks} bricks!");
    }

    // =========================================================
    // SPAWN 1 BRICK
    // =========================================================
    private void SpawnBrick(int row, int col, int level)
    {
        // Center the columns horizontally around 0
        float startX = -((maxColumns - 1) * horizontalSpacing) / 2f;
        float x = startX + (col * horizontalSpacing);
        float y = startY - (row * verticalSpacing);

        GameObject brick = Instantiate(brickPrefab, new Vector3(x, y, 0f), Quaternion.identity, brickParent);

        if (brick.TryGetComponent(out Brick brickScript))
        {
            brickScript.SetHits(GetBrickHits(row, level)); // Brick sets its own bright color!
            remainingBricks++;
        }
    }

    // Deeper rows or higher levels get tougher bricks (1 to 5 hits)
    private int GetBrickHits(int row, int level)
    {
        if (level <= 2) return 1;
        if (level <= 4) return (row >= 2) ? 2 : 1;

        int hits = 1 + (row + level - 1) / 3;
        return Mathf.Clamp(hits, 1, 5);
    }

    public void BrickDestroyed()
    {
        remainingBricks--;

        if (remainingBricks <= 0)
        {
            remainingBricks = 0;
            GameManager.Instance?.LevelComplete();
        }
    }

    private void ClearBricks()
    {
        if (brickParent == null) return;

        foreach (Transform child in brickParent)
        {
            Destroy(child.gameObject);
        }

        remainingBricks = 0;
    }
}