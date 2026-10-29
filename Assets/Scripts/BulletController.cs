using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 1f;

    void OnEnable()
    {
        //Start the bullet lifetime countdown
        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    /// <summary>
    /// Deactivates the bullet
    /// </summary>
    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        //Deactivate the bullet when it collides with another object
        DeactivateBullet();

        //TODO: Particles
        //TODO: if bullet hits an enemy, reduce enemy health
        //TODO: if bullet hits obstacle, destroy obstacle
        //TODO : if Bullet hits player , reduce player health
        
    }


}
