using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public bool canMove = true;
    private Animator anim;

    private string lastDirection = "down"; 

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            anim.SetBool("isMoving", false);
            return;
        }

        Vector2 movementInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;

            movementInput = new Vector2(x, y);
        }

        if (movementInput.magnitude > 1f)
        {
            movementInput.Normalize();
        }

        Vector3 movement = new Vector3(movementInput.x, movementInput.y, 0);
        transform.position += movement * speed * Time.deltaTime;

        bool isMoving = movementInput.magnitude > 0.01f;
        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            DetermineLastPressedKey(movementInput);
            SetAnimation();
        }
    }

    void DetermineLastPressedKey(Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            if (moveInput.x > 0) lastDirection = "right";
            else if (moveInput.x < 0) lastDirection = "left";
        }
        else
        {
            if (moveInput.y > 0) lastDirection = "up";
            else if (moveInput.y < 0) lastDirection = "down";
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