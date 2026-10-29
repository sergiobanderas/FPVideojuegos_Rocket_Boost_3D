using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private InputAction shoot;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float rotationForce = 2.5f;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem leftThrustParticleSystem;
    [SerializeField] private ParticleSystem rightThrustParticleSystem;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip thrustAudioClip;
  
    Rigidbody rb;
    AudioSource audioSource;
    PlayerShooting playerShooting;


    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        shoot.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
        ProcessShooting();
    }

    /// <summary>
    /// Processes the shooting input and triggers the shooting action if the shoot input is triggered.
    /// </summary>
    private void ProcessShooting()
    {
        if (shoot.IsPressed()) playerShooting.Shoot();        
    }

    /// <summary>
    /// Processes the rotation input and applies rotation to the player object based on the input value. It also manages the particle systems for left and right thrust effects.
    /// </summary>
    private void ProcessRotation()
    {
        rb.freezeRotation = true; //freezing rotation so we can manually rotate

        float rotationValue = rotation.ReadValue<float>();

        if (rotationValue < 0)
        {
            RotateRight();
        }
        else if (rotationValue > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotation();
        }
    }

    /// <summary>
    /// Stops the rotation action by stopping the left and right thrust particle systems.
    /// </summary>
    private void StopRotation()
    {
        rightThrustParticleSystem.Stop();
        leftThrustParticleSystem.Stop();
    }

    /// <summary>
    /// Rotates the player object to the left and manages the particle systems for left and right thrust effects.
    /// </summary>
    private void RotateLeft()
    {
        transform.Rotate(Vector3.back * rotationForce);
        if (!leftThrustParticleSystem.isPlaying)
        {
            rightThrustParticleSystem.Stop();
            leftThrustParticleSystem.Play();
        }
    }

    /// <summary>
    /// Rotates the player object to the right and manages the particle systems for left and right thrust effects.
    /// </summary>
    private void RotateRight()
    {
        transform.Rotate(Vector3.forward * rotationForce);
        if (!rightThrustParticleSystem.isPlaying)
        {
            leftThrustParticleSystem.Stop();
            rightThrustParticleSystem.Play();
        }

    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    /// <summary>
    /// Stops the thrusting action by stopping the particle system and audio source.
    /// </summary>
    private void StopThrusting()
    {   
        thrustParticleSystem.Stop();
        audioSource.Stop();
    }

    /// <summary>
    /// Starts the thrusting action by playing the particle system and audio source.
    /// </summary>
    private void StartThrusting()
    {
       
        rb.AddRelativeForce(Vector3.up * moveForce);

        if (!thrustParticleSystem.isPlaying)
        {
            thrustParticleSystem.Play();
        }

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustAudioClip);
        }
    }
}
