using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int currentAmmo = 10;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private bool infiniteAmmo = false;

    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletRate = 0.25f;
    [SerializeField] private int bulletDamage = 1;

    private Transform bulletSpawnPoint;
    private ObjectPool bulletPool;

    private float lastShotTime = 0f;

    void Start()
    {
        bulletSpawnPoint = transform.Find("ShootPoint");
        bulletPool = GetComponent<ObjectPool>();
    }

    /// <summary>
    /// Check if the player can shoot based on the bullet rate and ammo availability
    /// </summary>
    /// <returns></returns>
    public bool CanShoot()
    {        
        //Check if enough time has passed since the last shot and if there is ammo available 
        // or infinite ammo is enabled
        return (Time.time - lastShotTime >= bulletRate) && (infiniteAmmo || currentAmmo > 0);
     
    }

    /// <summary>
    /// Shoot a bullet from the object pool and apply 
    /// </summary>
    public void Shoot()
    {
        if (CanShoot())
        {
            //update last shot time
            lastShotTime = Time.time;    

            //reduce the Ammo
            if (!infiniteAmmo)currentAmmo--;

            //Get a bullet from the object pool
            GameObject bullet = bulletPool.GetObjectFromPool();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = bulletSpawnPoint.rotation;

            //Set the bullet's speed and movement forward
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.linearVelocity = bulletSpawnPoint.up * bulletSpeed;
         
        }
     
    }
}
