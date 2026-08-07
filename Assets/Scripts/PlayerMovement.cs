using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum PlayerState { Free, Combat, Frozen }

public class PlayerMovement : SoundsModule
{
    public PlayerHealth playerHealth;
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

        if (Keyboard.current != null && playerHealth != null)
        {
            float x = 0;
            float y = 0;

            if (IsKeyPressed(playerHealth.left) || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (IsKeyPressed(playerHealth.right) || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (IsKeyPressed(playerHealth.down) || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (IsKeyPressed(playerHealth.up) || Keyboard.current.upArrowKey.isPressed) y += 1f;

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

    private bool IsKeyPressed(char c)
    {
        if (Keyboard.current == null) return false;

        c = char.ToLower(c);

        KeyControl keyControl = c switch
        {
            'a' => Keyboard.current.aKey,
            'b' => Keyboard.current.bKey,
            'c' => Keyboard.current.cKey,
            'd' => Keyboard.current.dKey,
            'e' => Keyboard.current.eKey,
            'f' => Keyboard.current.fKey,
            'g' => Keyboard.current.gKey,
            'h' => Keyboard.current.hKey,
            'i' => Keyboard.current.iKey,
            'j' => Keyboard.current.jKey,
            'k' => Keyboard.current.kKey,
            'l' => Keyboard.current.lKey,
            'm' => Keyboard.current.mKey,
            'n' => Keyboard.current.nKey,
            'o' => Keyboard.current.oKey,
            'p' => Keyboard.current.pKey,
            'q' => Keyboard.current.qKey,
            'r' => Keyboard.current.rKey,
            's' => Keyboard.current.sKey,
            't' => Keyboard.current.tKey,
            'u' => Keyboard.current.uKey,
            'v' => Keyboard.current.vKey,
            'w' => Keyboard.current.wKey,
            'x' => Keyboard.current.xKey,
            'y' => Keyboard.current.yKey,
            'z' => Keyboard.current.zKey,
            _ => null
        };

        return keyControl != null && keyControl.isPressed;
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