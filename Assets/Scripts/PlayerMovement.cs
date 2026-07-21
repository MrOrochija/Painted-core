using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { Free, Combat, Frozen }

public class PlayerMovement : Sounds
{
    public PlayerState currentState = PlayerState.Free;
    public event Action OnDrawAnimationFinished;

    private Animator anim;
    private Rigidbody2D rb;
    
    private string lastDirection = "down"; 
    private float stepTimer = 0f;

    private Vector2 movementInput;
    private float currentSpeed;
    private bool isRunning;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (currentState != PlayerState.Free)
        {
            anim.SetBool("isMoving", false);
            HandleStepSound(false, false, false);
            movementInput = Vector2.zero;
            return; 
        }

        movementInput = Vector2.zero;
        isRunning = false;

        if (Keyboard.current != null)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;

            movementInput = new Vector2(x, y);

            if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
            {
                isRunning = true;
            }
        }

        if (movementInput.magnitude > 1f)
        {
            movementInput.Normalize();
        }

        currentSpeed = isRunning ? 10f : 5f;

        bool isMoving = movementInput.magnitude > 0.01f;
        anim.SetBool("isMoving", isMoving);

        string oldDirection = lastDirection;

        if (isMoving)
        {
            DetermineLastPressedKey(movementInput);
            SetAnimation();
        }

        bool directionChanged = isMoving && (lastDirection != oldDirection);
        HandleStepSound(isMoving, isRunning, directionChanged);
    }

    void FixedUpdate()
    {
        if (currentState != PlayerState.Free)
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        Vector2 targetVelocity = movementInput * currentSpeed;
        
        Vector2 velocityChange = targetVelocity - rb.linearVelocity;
        
        rb.AddForce(velocityChange, ForceMode2D.Impulse);
    }

    private void HandleStepSound(bool shouldPlay, bool isRunning, bool directionChanged)
    {
        if (shouldPlay)
        {
            if (directionChanged)
            {
                StopSound(); 
                stepTimer = 0f; 
            }

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlaySound(isRunning ? sounds[1] : sounds[0]); 
                stepTimer = isRunning ? 0.718f : 1.067f; 
            }
        }
        else
        {
            if (stepTimer > 0f)
            {
                StopSound();
            }
            stepTimer = 0f;
        }
    }

    public IEnumerator DrawAnimation()
    {
        currentState = PlayerState.Frozen; 
        anim.SetBool("isDraw", true);

        yield return new WaitForSeconds(0.1f); 

        float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength - 0.1f);

        anim.SetBool("isDraw", false);
        currentState = PlayerState.Free; 
        OnDrawAnimationFinished?.Invoke(); 
    }

    void DetermineLastPressedKey(Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            if (moveInput.x > 0) lastDirection = "right";
            if (moveInput.x < 0) lastDirection = "left";
        }
        else
        {
            if (moveInput.y > 0) lastDirection = "up";
            if (moveInput.y < 0) lastDirection = "down";
        }
    }

    void SetAnimation()
    {
        anim.SetBool("isLeft", lastDirection == "left");
        anim.SetBool("isRight", lastDirection == "right");
        anim.SetBool("isUp", lastDirection == "up");
        anim.SetBool("isDown", lastDirection == "down");
    }
}