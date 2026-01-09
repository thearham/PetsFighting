using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // How long the camera should shake for
    public float shakeDuration = 0.5f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeMagnitude = 0.2f;

    // How smooth the shake will be
    public float dampingSpeed = 1.0f;

    // Store the initial position of the camera
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Method to call to start the camera shake
    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    // Coroutine for the shaking effect
    IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random X offset for camera position
            float randomXOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake only in the X direction
            transform.localPosition = new Vector3(initialPosition.x + randomXOffset, initialPosition.y, initialPosition.z);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Smooth out the shaking effect using damping
            yield return null;
        }

        // Reset camera to original position after the shake
        transform.localPosition = initialPosition;
    }
}
