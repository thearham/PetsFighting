using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the player
    public float jumpForce = 17f; // Force applied when jumping
    public Animator animator; // Reference to the Animator component
    public Rigidbody2D rb; // Reference to the Rigidbody2D component

    private Vector2 movement; // Vector to store movement direction
    private bool canMove = true; // Flag to check if the character can move

    public bool facingRight = true; // Track whether the character is facing right
    public Transform groundCheck; // Reference to the GroundCheck object
    public float groundCheckRadius = 0.2f; // Radius of the circle collider for ground check
    public LayerMask groundLayer; // Layer mask to determine what is considered ground

    private bool isGrounded; // Track whether the character is on the ground

    public TextMeshProUGUI countdownText;  // TextMeshProUGUI component for countdown
    public float countdownTime = 3f;  // Countdown duration

    public float targetXPosition = -2f; // The target x position (-2 in this case)

    private bool isMoving = true;
    private bool countComplete = false;
    public GameObject jump;

    void Start()
    {
        // Start the countdown coroutine
        StartCoroutine(CountdownAndMove());
    }

    void Update()
    {
        // Check if the player is grounded using the GroundCheck object
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isMoving)
        {
            // Move the character towards the target x position
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetXPosition, 2 * Time.deltaTime), transform.position.y, transform.position.z);
            animator.SetBool("isWalking", true);

            // Check if the character has reached the target position
            if (transform.position.x == targetXPosition)
            {
                isMoving = false; // Stop moving
                animator.SetBool("isWalking", false); // Stop the walking animation
            }
        }

        if (countComplete)
        {
            // Reset movement
            movement = Vector2.zero;

            if (canMove)
            {
                // Get input for horizontal movement
                if (Input.GetKey(KeyCode.A))
                {
                    movement.x = -1f; // Move left
                    animator.SetBool("isWalking", true);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    movement.x = 1f; // Move right
                    animator.SetBool("isWalking", true);
                }

                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                {
                    animator.SetBool("isWalking", false);
                }

                // Handle Jump with the 'W' key and check if grounded
                if (Input.GetKeyDown(KeyCode.W) && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
                    StartCoroutine(wait());
                    animator.SetTrigger("Jump"); // Play jump animation if available
                }

                // Flip the character based on movement direction
                if (movement.x > 0 && !facingRight)
                {
                    FlipX();
                }
                else if (movement.x < 0 && facingRight)
                {
                    FlipX();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Move the character using Rigidbody2D if canMove is true
        if (canMove)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Stop the character's horizontal movement if they can't move
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void FlipX()
    {
        // Flip the character's sprite along the X axis
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator CountdownAndMove()
    {
        float currentTime = countdownTime;

        // Display countdown
        while (currentTime > 0)
        {
            countdownText.text = currentTime.ToString("0");
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        countdownText.text = "Go!";  // Display "Go!" when countdown is finished
        yield return new WaitForSeconds(1f);

        countdownText.enabled = false;  // Hide the countdown text
        countComplete = true;  // Set countComplete to true
    }
    private IEnumerator wait()
    {
        jump.SetActive(true);

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(1f);


        jump.SetActive(false);
    }
}
