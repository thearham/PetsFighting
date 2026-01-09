using UnityEngine;

public class DogStart : MonoBehaviour
{
    public float targetXPosition = -2f; // The target x position (-2 in this case)
    public float moveSpeed = 2f;        // Speed at which the character should move
    //private bool isMoving = true;       // Flag to check if the character should still be moving

    private Animator animator;

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Start the walking animation
        
    }

    void Update()
    {
        
    }
}
