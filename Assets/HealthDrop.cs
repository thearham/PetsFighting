using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthDrop : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Animator animator;
    public TextMeshProUGUI textMeshProUGUI;

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public GameObject spark;

    public playerMovement playerMovementScript;
    public DogAttack DogAttackScript;
    public BearMovement bearMovementScript;
    public BearAttack BearAttackScript;

    public float idleHealthRegenRate = 1f; // Amount to regenerate when idle
    public float idleTimeThreshold = 3f; // Time after which health starts regenerating
    private float lastAttackTime;
    private bool dead = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth; // Ensure slider is set correctly
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(1f);

        lastAttackTime = Time.time;
    }

    void Update()
    {
        // Call health regeneration in each frame
        RegenerateHealth();
    }

    public void sparkEffect()
    {
        spark.SetActive(true);
        StartCoroutine(StopSpark());
    }

    private IEnumerator StopSpark()
    {
        yield return new WaitForSeconds(0.5f);
        spark.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        
        animator.SetTrigger("isAttacked");
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);

        lastAttackTime = Time.time; // Reset idle timer

        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    void RegenerateHealth()
    {
        // Check if enough time has passed since last attack and character is still alive
        if (Time.time - lastAttackTime >= idleTimeThreshold && currentHealth < maxHealth && dead == false)
        {
            currentHealth += Mathf.CeilToInt(idleHealthRegenRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    void Die()
    {
        dead = true;
        Debug.Log("Died" + gameObject.name);
        animator.SetBool("isDead", true);

        if (gameObject.name == "Dog")
        {
            textMeshProUGUI.text = "Bear WINS!!!";
        }
        else
        {
            textMeshProUGUI.text = "Dog WINS!!!";
        }

        textMeshProUGUI.enabled = true;

        // Disable movement scripts
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
            DogAttackScript.enabled = false;
        }

        if (bearMovementScript != null)
        {
            bearMovementScript.enabled = false;
            BearAttackScript.enabled = false;
        }

       
        this.enabled = false;
    }
}
