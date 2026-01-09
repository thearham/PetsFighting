using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BearMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the player
    public float jumpForce = 17f; // Force applied when jumping
    public Animator animator; // Reference to the Animator component
    public Rigidbody2D rb; // Reference to the Rigidbody2D component

    public GameObject powerEffectPrefab;  // Assign the prefab in the Inspector
    public Transform feetPosition;        // An empty GameObject at the player's feet

    private Vector2 movement; // Vector to store movement direction
    private bool canMove = true; // Flag to check if the character can move


    public bool facingRight = true; // Track whether the character is facing right
    public Transform groundCheck; // Reference to the GroundCheck object
    public float groundCheckRadius = 0.2f; // Radius of the circle collider for ground check
    public LayerMask groundLayer; // Layer mask to determine what is considered ground

    private bool isGrounded; // Track whether the character is on the ground

    public GameObject jump;
    public float countdownTime = 3f;  // Countdown duration
    public float targetXPosition = 2f; // The target x position (-2 in this case)

    private bool isMoving = true;
    private bool countComplete = false;

    void Start()
    {
        // Start the countdown coroutine
        StartCoroutine(Countdown());
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isMoving)
        {
            // Move the character towards the target x position
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetXPosition, 2 * Time.deltaTime), transform.position.y, transform.position.z);
            animator.SetBool("isWalk", true);

            // Check if the character has reached the target position
            if (transform.position.x == targetXPosition)
            {
                isMoving = false; // Stop moving
                animator.SetBool("isWalk", false); // Stop the walking animation
            }
        }
        if (transform.position.x > 8f)
        {
            transform.position = new Vector3(8f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -8.35f)
        {
            transform.position = new Vector3(-8.35f, transform.position.y, transform.position.z);
        }
        if (countComplete == true)
        {
     
            // Reset movement
            movement = Vector2.zero;

            if (canMove)
            {
                // Get input for horizontal movement
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    movement.x = -1f; // Move left
                    animator.SetBool("isWalk", true);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    movement.x = 1f; // Move right
                    animator.SetBool("isWalk", true);
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    //movement.x = -1f; // Move left
                    animator.SetBool("isWalk", false);
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    //movement.x = 1f; // Move right
                    animator.SetBool("isWalk", false);
                }
                // Handle Jump with the 'W' key and check if grounded
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                {
                    
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
                    GameObject powerEffect = Instantiate(powerEffectPrefab, feetPosition.position, Quaternion.identity);
                    Destroy(powerEffect, 1f);
                    StartCoroutine(wait());
                    animator.SetTrigger("Jump"); // Play jump animation if available
                    
                }

                if (movement.x < 0 && !facingRight)
                {
                    FlipX();
                }
                else if (movement.x > 0 && facingRight)
                {
                    FlipX();
                }
            }
        }

    }

    void FixedUpdate()
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

 

    void FlipX()
    {
        // Flip the character's sprite along the X axis
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator Countdown()
    {
        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
       
        yield return new WaitForSeconds(1f);
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
