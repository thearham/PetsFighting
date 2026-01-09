using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DogAttack : MonoBehaviour
{
    public Animator animator;
    public Transform AttackPoint;
    public LayerMask bear;

    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    public Light2D globalLight;  // Reference to the Global Light 2D
    public Color originalColor;  // Store the original color of the light
    public Color flashColor = Color.red;  // The color to change to (red)
    public GameObject blood;
    public GameObject Aura;
    public GameObject dust;
    public GameObject Bear;

    private playerMovement mov;

    private bool isFacingRight;
    public float moveSpeed = 5f; // Speed at which the character moves towards the target
    public float disappearDelay = 0.1f;
    private SpriteRenderer spriteRenderer;

    public Camera mainCamera; // Reference to the main camera
    public float zoomAmount = 1.18f; // How much to zoom in
    public float zoomDuration = 1f; // How long the zoom effect should last
    public float zoomSpeed = 2f; // Speed of zooming in and out
    public Transform target;
    private float originalSize; // To store the original camera size
    private Vector3 originalPosition;

    private AudioSource audioSource;

    private void Start()
    {
        // Ensure you have assigned the Global Light 2D in the inspector
        if (globalLight != null)
        {
            originalColor = globalLight.color;  // Store the original color
        }
        originalSize = mainCamera.orthographicSize;
        originalPosition = mainCamera.transform.position;
        mov = GetComponent<playerMovement>();
        isFacingRight = mov.facingRight;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                SimpleAttack(attackDamage);
                nextAttackTime = Time.time + 1f / attackRate;
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                
                PerformAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }


    }
    void SimpleAttack(int attackDamage)
    {
        animator.SetTrigger("isAttack");

        Collider2D[] hitBear = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, bear);
        foreach (Collider2D bear in hitBear)
        {
            audioSource.Play();
            bear.GetComponent<HealthDrop>().TakeDamage(attackDamage);
            TriggerLightFlash();
            StartCoroutine(SpillBlood());
        }


    }
    public void SpecialAttack()
    {
        // Check if another player is executing a special attack
        if (SpecialAttackManager.isSpecialAttackInProgress)
        {
            // If true, prevent this player from executing their special attack
            Debug.Log("Another player is performing a special attack. Wait for it to finish.");
            return;
        }

        // Set the special attack flag to true
        SpecialAttackManager.isSpecialAttackInProgress = true;
        StartCoroutine(PerformSpecialAttack());
    }
    private IEnumerator dustInitiate()
    {
        dust.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        dust.SetActive(false);
    }
    private IEnumerator PerformSpecialAttack()
    {
        
        ZoomInOnAttack();
        Aura.SetActive(true);
        
        StartCoroutine(dustInitiate());
        animator.SetTrigger("isRun");
        Vector3 targetPosition = Bear.transform.position;
        Vector3 frontPosition = new Vector3(targetPosition.x - 3, transform.position.y, transform.position.z);
        Vector3 originalPosition = transform.position;
        // Move towards the front of the other character
        while (transform.position.x != frontPosition.x)
        {
            transform.position = Vector3.MoveTowards(transform.position, frontPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        GetComponent<Collider2D>().enabled = false;

        // Deactivate and move to the back position
        yield return new WaitForSeconds(disappearDelay);
        gameObject.SetActive(false);

        Vector3 backPosition = new Vector3(targetPosition.x + 2, transform.position.y, transform.position.z);
        transform.position = backPosition;

        // Flip the character to face the other direction
        Flip();
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
        Bear.GetComponent<HealthDrop>().sparkEffect();

        animator.SetBool("isRun", false);
        
        SimpleAttack(attackDamage + 10);
        // Deactivate and return to the original position
        StartCoroutine(ReturnToOriginalPosition(originalPosition));
        SpecialAttackManager.isSpecialAttackInProgress = false;
    }
    private IEnumerator ReturnToOriginalPosition(Vector3 originalPosition)
    {
        yield return new WaitForSeconds(disappearDelay+0.7f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        StartCoroutine(wait());
        // Move back to the original position
        transform.position = originalPosition;
        Flip();
        spriteRenderer.enabled = true;
        //gameObject.SetActive(true);

        Aura.SetActive(false);

        ResetZoom();
    }
    void PerformAttack()
    {
        // Check if the dog is facing right and if the bear is to the right
        if (isFacingRight && Bear.transform.position.x > transform.position.x)
        {
            SpecialAttack();
            return;
        }
        // Check if the dog is facing left and if the bear is to the left
        else if (!isFacingRight && Bear.transform.position.x < transform.position.x)
        {
            SpecialAttack();
            return;
        }
        else
        {
            // Do not attack because the bear is behind the dog
            Debug.Log("Cannot attack: Bear is behind the dog.");
        }
    }
    public void Flip()
    {
        // Invert the x-axis scale to flip the sprite
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        // Update the facing direction
        //isFacingRight = !isFacingRight;
    }
    public void TriggerLightFlash()
    {
        if (globalLight != null)
        {
            StartCoroutine(FlashLight());
        }
    }

    private IEnumerator FlashLight()
    {
        // Change the light color to red
        globalLight.color = flashColor;

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        // Revert the light color back to the original color
        globalLight.color = originalColor;
    }
    private IEnumerator SpillBlood()
    {
        blood.SetActive(true);

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.5f);


        blood.SetActive(false);
    }
    private IEnumerator wait()
    {
        

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(1f);


       
    }

    public void ZoomInOnAttack()
    {
        StartCoroutine(ZoomIn());
    }
    public void ResetZoom()
    {
        StartCoroutine(ZoomOut());
    }
    private IEnumerator ZoomIn()
    {
        // Focus the camera on the character
        while (mainCamera.orthographicSize > originalSize / zoomAmount)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, originalPosition.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize -= zoomSpeed * Time.deltaTime;
            
            yield return null;
        }
        //CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        //cameraShake.TriggerShake();
    }

    private IEnumerator ZoomOut()
    {
        // Reset the camera to its original size and position
        while (mainCamera.orthographicSize < originalSize)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originalPosition, zoomSpeed * Time.deltaTime);
            mainCamera.orthographicSize += zoomSpeed * Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalPosition;
        mainCamera.orthographicSize = originalSize;
    }
}
