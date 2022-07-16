using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("ScreenPaddings")]
    [SerializeField] float leftPadding = 0.5f;
    [SerializeField] float rightPadding = 0.5f;
    [SerializeField] float upPadding = 3f;
    [SerializeField] float downPadding = 2f;

    [Header("Thrusters")]
    [SerializeField] TrailRenderer backThruster;
    [SerializeField] TrailRenderer frontThruster1;
    [SerializeField] TrailRenderer frontThruster2;

    [Header("Pickup")]
    [SerializeField] ItemPickup pickup = null;

    Shooter shooter;
    Shield shield;
    PlayerInput playerInput;

    Vector2 rawInput;

    Vector2 minimumBound;
    Vector2 maximumBound;
    Vector2 additionalMovement;

    private void Awake()
    {
        shooter = GetComponent<Shooter>();
        shield = GetComponentInChildren<Shield>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        FindObjectOfType<PauseMenuUI>().OnGamePaused += InputSwitchOnPause;
    }

    private void OnDisable()
    {
        var pauseMenu = FindObjectOfType<PauseMenuUI>();
        if (!pauseMenu) return;
        pauseMenu.OnGamePaused -= InputSwitchOnPause;
    }

    private void Start()
    {
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", new InputDevice[] { Keyboard.current, Mouse.current });

        InitBounds();
    }

    private void LateUpdate()
    {
        Move();
    }

    public void AffectMovement(Vector2 additionalPosition)
    {
        additionalMovement = additionalPosition;
    }

    public bool HasPickup()
    {
        return pickup != null;
    }

    public void SetPickup(ItemPickup item)
    {
        pickup = item;
    }

    private void Move()
    {
        Vector2 delta = rawInput * Time.deltaTime * moveSpeed;
        Vector2 newPos = new Vector2();
        newPos.x = Mathf.Clamp(transform.position.x + delta.x + additionalMovement.x, minimumBound.x + leftPadding, maximumBound.x - rightPadding);
        newPos.y = Mathf.Clamp(transform.position.y + delta.y + additionalMovement.y, minimumBound.y + downPadding, maximumBound.y - upPadding);
        ChangeThrusters(newPos);
        transform.position = newPos;

        if (additionalMovement != Vector2.zero)
        {
            additionalMovement = Vector2.zero;
        }
    }

    void InitBounds()
    {
        Camera mainCamera = Camera.main;
        minimumBound = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maximumBound = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void ChangeThrusters(Vector2 newPos)
    {
        if (newPos.y > transform.position.y)
        {
            backThruster.emitting = true;
            frontThruster1.emitting = false;
            frontThruster2.emitting = false;
        }
        else if (newPos.y < transform.position.y)
        {
            backThruster.emitting = false;
            frontThruster1.emitting = true;
            frontThruster2.emitting = true;
        }    
        else
        {
            backThruster.emitting = false;
            frontThruster1.emitting = false;
            frontThruster2.emitting = false;
        }
    }

    // Input System Events

    public void InputSwitchOnPause(bool isGamePaused)
    {
        
        if (isGamePaused)
        {
            shooter.EnableShooting(false);
            playerInput.actions.Disable();
        }
        else
        {
            playerInput.actions.Enable();
            shooter.EnableShooting(true);
        }
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (shooter != null)
        {
            shooter.SetIsFiring(value.isPressed);
        }
    }

    void OnShield()
    {
        StartCoroutine(shield.ActivateShield());
    }

    void OnPickup()
    {
        if (!pickup) return;
        pickup.UsePickup();
        pickup = null;
    }
}
