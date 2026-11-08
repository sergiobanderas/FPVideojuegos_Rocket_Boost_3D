using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int currentAmmo = 10;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private bool infiniteAmmo = false;

    [Header("Shooting Settings")]
    [SerializeField] private float bulletSpeed = 35f;
    [SerializeField] private float bulletRate = 0.15f;
    [SerializeField] private int bulletDamage = 1;

    [Header("Shoot Audio")]
    [SerializeField] private AudioClip laserClip1;
    [SerializeField] private AudioClip laserClip2;

    private Transform bulletSpawnPoint;
    private ObjectPool bulletPool;
    private Rigidbody playerBody;
    private AudioSource shootAudio;

    private float lastShotTime = 0f;

    void Start()
    {
        bulletSpawnPoint = transform.Find("ShootPoint");
        bulletPool = GetComponent<ObjectPool>();
        playerBody = GetComponent<Rigidbody>();

        // Separate source from the engine. Releasing boost calls Stop() on that AudioSource and would cut the laser.
        shootAudio = gameObject.AddComponent<AudioSource>();
        shootAudio.playOnAwake = false;
        shootAudio.loop = false;
        shootAudio.spatialBlend = 0f;
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

            CombatVfx.PlayMuzzle(bulletSpawnPoint.position, bulletSpawnPoint.up);

            GameObject bullet = bulletPool.GetObjectFromPool();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = bulletSpawnPoint.rotation;

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDamage(bulletDamage);
            // Start the trail once the shot is at the muzzle. Starting it in OnEnable connects the pool to the shot.
            bulletController.BeginFlight();

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            // Fired along the muzzle up axis and inherits the ship's velocity.
            Vector3 inheritedVelocity = playerBody != null ? playerBody.linearVelocity : Vector3.zero;
            bulletRigidbody.linearVelocity = bulletSpawnPoint.up * bulletSpeed + inheritedVelocity;
            PlayShootSound();
        }
     
    }

    void PlayShootSound()
    {
        if (shootAudio == null)
        {
            return;
        }

        AudioClip clip = Random.Range(0, 2) == 0 ? laserClip1 : laserClip2;
        if (clip == null)
        {
            clip = laserClip1 != null ? laserClip1 : laserClip2;
        }

        if (clip == null)
        {
            return;
        }

        // Each shot cuts the previous one so a burst is heard back to back.
        // The clip is sfx_laser1 or sfx_laser2, chosen at random.
        shootAudio.Stop();
        shootAudio.clip = clip;
        shootAudio.Play();
    }
}
