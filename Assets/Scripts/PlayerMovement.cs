using UnityEngine;

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
        if (canMove)
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(horizontalMovement, verticalMovement, 0);

            if (movement.magnitude > 1f)
            {
                movement.Normalize();
            }

            transform.position += movement * speed * Time.deltaTime;

            bool isMoving = movement.magnitude > 0.01f;
            anim.SetBool("isMoving", isMoving);

            if (isMoving)
            {
                DetermineLastPressedKey();
                SetAnimation();
            }
        }
    }

    void DetermineLastPressedKey()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastDirection = "left";
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastDirection = "right";
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastDirection = "up";
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastDirection = "down";

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) && lastDirection == "left") ResetToAnyValidKey();
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && lastDirection == "right") ResetToAnyValidKey();
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && lastDirection == "up") ResetToAnyValidKey();
        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow) && lastDirection == "down") ResetToAnyValidKey();
    }

    void ResetToAnyValidKey()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) lastDirection = "left";
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) lastDirection = "right";
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) lastDirection = "up";
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) lastDirection = "down";
    }

    void SetAnimation()
    {
        anim.SetBool("isLeft", lastDirection == "left");
        anim.SetBool("isRight", lastDirection == "right");
        anim.SetBool("isUp", lastDirection == "up");
        anim.SetBool("isDown", lastDirection == "down");
    }
}