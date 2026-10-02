using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPaddle : MonoBehaviour
{
    [Header("Keyboard Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Arena Limits")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;

    [Header("Mouse / Touch")]
    [SerializeField] private float dragSmoothness = 20f;

    private Vector3 startPosition;

    private Camera mainCamera;

    private bool isDragging = false;

    // Difference between the paddle and the finger/mouse
    private float dragOffsetX;

    private float targetX;


    private void Awake()
    {
        startPosition = transform.position;

        mainCamera = Camera.main;
    }


    private void Update()
    {
        HandleKeyboard();

        // Mouse and touch are alternative controls.
        // Keyboard still works independently.
        if (!HandleMouse())
        {
            HandleTouch();
        }
    }


    // =========================================================
    // KEYBOARD
    // =========================================================

    private void HandleKeyboard()
    {
        if (Keyboard.current == null)
            return;

        float horizontalInput = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput = 1f;
        }

        if (horizontalInput == 0f)
            return;

        Vector3 position = transform.position;

        position.x +=
            horizontalInput *
            moveSpeed *
            Time.deltaTime;

        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX
            );

        transform.position = position;
    }


    // =========================================================
    // MOUSE
    // =========================================================

    private bool HandleMouse()
    {
        if (Mouse.current == null)
            return false;

        bool mousePressed =
            Mouse.current.leftButton.wasPressedThisFrame;

        bool mouseHeld =
            Mouse.current.leftButton.isPressed;

        bool mouseReleased =
            Mouse.current.leftButton.wasReleasedThisFrame;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        // -----------------------------------------
        // START DRAG
        // -----------------------------------------

        if (mousePressed)
        {
            StartDrag(mousePosition);
        }


        // -----------------------------------------
        // DRAG
        // -----------------------------------------

        if (mouseHeld && isDragging)
        {
            UpdateDrag(mousePosition);
        }


        // -----------------------------------------
        // RELEASE
        // -----------------------------------------

        if (mouseReleased)
        {
            isDragging = false;
        }


        return mouseHeld || mousePressed;
    }


    // =========================================================
    // TOUCH
    // =========================================================

    private void HandleTouch()
    {
        if (Touchscreen.current == null)
            return;

        var touch =
            Touchscreen.current.primaryTouch;

        Vector2 touchPosition =
            touch.position.ReadValue();


        bool touchStarted =
            touch.press.wasPressedThisFrame;

        bool touchHeld =
            touch.press.isPressed;

        bool touchEnded =
            touch.press.wasReleasedThisFrame;


        // -----------------------------------------
        // START DRAG
        // -----------------------------------------

        if (touchStarted)
        {
            StartDrag(touchPosition);
        }


        // -----------------------------------------
        // DRAG
        // -----------------------------------------

        if (touchHeld && isDragging)
        {
            UpdateDrag(touchPosition);
        }


        // -----------------------------------------
        // RELEASE
        // -----------------------------------------

        if (touchEnded)
        {
            isDragging = false;
        }
    }


    // =========================================================
    // START DRAG
    // =========================================================

    private void StartDrag(Vector2 screenPosition)
    {
        if (mainCamera == null)
            return;

        Vector3 worldPosition =
            ScreenToWorld(screenPosition);

        // Remember where the player touched relative
        // to the center of the paddle.
        dragOffsetX =
            transform.position.x -
            worldPosition.x;

        targetX =
            transform.position.x;

        isDragging = true;
    }


    // =========================================================
    // UPDATE DRAG
    // =========================================================

    private void UpdateDrag(Vector2 screenPosition)
    {
        if (mainCamera == null)
            return;

        Vector3 worldPosition =
            ScreenToWorld(screenPosition);

        // Move paddle by the same amount as the finger/mouse,
        // while preserving the original touch offset.
        targetX =
            worldPosition.x +
            dragOffsetX;

        targetX =
            Mathf.Clamp(
                targetX,
                minX,
                maxX
            );

        MoveTowardsTarget();
    }


    // =========================================================
    // SMOOTH MOVEMENT
    // =========================================================

    private void MoveTowardsTarget()
    {
        Vector3 position =
            transform.position;

        position.x =
            Mathf.Lerp(
                position.x,
                targetX,
                dragSmoothness *
                Time.deltaTime
            );

        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX
            );

        transform.position =
            position;
    }


    // =========================================================
    // SCREEN TO WORLD
    // =========================================================

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        if (mainCamera == null)
            return transform.position;

        float distance =
            Mathf.Abs(
                mainCamera.transform.position.z
            );

        Vector3 screenPoint =
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                distance
            );

        return mainCamera.ScreenToWorldPoint(
            screenPoint
        );
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetPaddle()
    {
        transform.position =
            startPosition;

        targetX =
            startPosition.x;

        dragOffsetX = 0f;

        isDragging = false;
    }
}