using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Canvases")]
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject winCanvas;

    [Header("Game Objects")]
    [SerializeField] private GameObject playerPaddle;
    [SerializeField] private GameObject ball;

    [Header("Brick System")]
    [SerializeField] private BrickSpawner brickSpawner;

    private int currentLevel = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowHome();
    }

    public void ShowHome()
    {
        Time.timeScale = 1f;

        homeCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loseCanvas.SetActive(false);

        if (winCanvas != null)
        {
            winCanvas.SetActive(false);
        }

        DisableGameObjects();

        currentLevel = 1;
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;

        currentLevel = 1;

        homeCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        loseCanvas.SetActive(false);

        if (winCanvas != null)
        {
            winCanvas.SetActive(false);
        }

        EnableGameObjects();

        StartLevel();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        currentLevel++;

        Debug.Log(
            "Starting Level " +
            currentLevel
        );

        homeCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        loseCanvas.SetActive(false);

        if (winCanvas != null)
        {
            winCanvas.SetActive(false);
        }

        EnableGameObjects();

        StartLevel();
    }

    private void StartLevel()
    {
        ResetGame();

        if (brickSpawner != null)
        {
            brickSpawner.GenerateLevel(
                currentLevel
            );
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;

        homeCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        loseCanvas.SetActive(true);

        if (winCanvas != null)
        {
            winCanvas.SetActive(false);
        }
    }

    public void LevelComplete()
    {
        Time.timeScale = 0f;

        homeCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        loseCanvas.SetActive(false);

        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
        }

        Debug.Log(
            "Level " +
            currentLevel +
            " Complete!"
        );
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        homeCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        loseCanvas.SetActive(false);

        if (winCanvas != null)
        {
            winCanvas.SetActive(false);
        }

        EnableGameObjects();

        StartLevel();
    }

    private void ResetGame()
    {
        if (playerPaddle != null)
        {
            PlayerPaddle paddle =
                playerPaddle.GetComponent<PlayerPaddle>();

            if (paddle != null)
            {
                paddle.ResetPaddle();
            }
        }

        if (ball != null)
        {
            BallController ballController =
                ball.GetComponent<BallController>();

            if (ballController != null)
            {
                ballController.ResetBall();
            }
        }
    }

    private void EnableGameObjects()
    {
        if (playerPaddle != null)
        {
            playerPaddle.SetActive(true);
        }

        if (ball != null)
        {
            ball.SetActive(true);
        }
    }

    private void DisableGameObjects()
    {
        if (playerPaddle != null)
        {
            playerPaddle.SetActive(false);
        }

        if (ball != null)
        {
            ball.SetActive(false);
        }
    }
}