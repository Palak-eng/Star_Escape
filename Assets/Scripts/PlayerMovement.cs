using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        bool isMoving = movement != Vector2.zero;
        animator.SetBool("IsMoving", isMoving);

        if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
        {
            if (movement.y > 0)
            {
                animator.SetInteger("Direction", 2); // Up
                spriteRenderer.flipX = false;
            }
            else if (movement.y < 0)
            {
                animator.SetInteger("Direction", 0); // Down
                spriteRenderer.flipX = false;
            }
        }
        else if (Mathf.Abs(movement.x) > 0)
        {
            animator.SetInteger("Direction", 1); // Side

            // original side sprite faces left
            if (movement.x < 0)
                spriteRenderer.flipX = false;
            else if (movement.x > 0)
                spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}