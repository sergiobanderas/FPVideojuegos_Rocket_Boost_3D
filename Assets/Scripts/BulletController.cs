using UnityEngine;
using UnityEngine.Rendering;

// Pooled projectile that looks like a laser. The collider stays on the root;
// the stretched beam is a child, so scaling the mesh does not deform the trigger.
public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 1.6f;

    int damage = 1;
    bool consumed;
    TrailRenderer trail;
    bool laserReady;

    public void SetDamage(int value)
    {
        damage = value;
    }

    void OnEnable()
    {
        consumed = false;
        EnsureLaserVisuals();
        trail.emitting = false;
        trail.Clear();
        CancelInvoke();
        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    public void BeginFlight()
    {
        if (trail == null)
        {
            EnsureLaserVisuals();
        }

        trail.Clear();
        trail.emitting = true;
    }

    void OnDisable()
    {
        CancelInvoke();
        // Clear the trail when returning to the pool, or the next shot drags the old one.
        if (trail != null)
        {
            trail.emitting = false;
            trail.Clear();
        }
    }

    void EnsureLaserVisuals()
    {
        transform.localScale = Vector3.one * 0.2f;

        SphereCollider hitbox = GetComponent<SphereCollider>();
        if (hitbox != null)
        {
            hitbox.radius = 1.25f;
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.material = CombatVfx.GetLaserMaterial();
        }

        Transform beam = transform.Find("LaserBeam");
        if (beam == null)
        {
            GameObject beamObject = new GameObject("LaserBeam");
            beam = beamObject.transform;
            beam.SetParent(transform, false);

            MeshFilter source = GetComponent<MeshFilter>();
            MeshFilter filter = beamObject.AddComponent<MeshFilter>();
            if (source != null)
            {
                filter.sharedMesh = source.sharedMesh;
            }

            MeshRenderer beamRenderer = beamObject.AddComponent<MeshRenderer>();
            beamRenderer.shadowCastingMode = ShadowCastingMode.Off;
            beamRenderer.receiveShadows = false;
            beamRenderer.material = CombatVfx.GetLaserMaterial();
        }

        beam.localPosition = Vector3.zero;
        beam.localRotation = Quaternion.identity;
        beam.localScale = new Vector3(0.28f, 9f, 0.28f);

        if (!laserReady)
        {
            trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = 0.22f;
            trail.startWidth = 0.32f;
            trail.endWidth = 0.02f;
            trail.minVertexDistance = 0.01f;
            trail.numCapVertices = 6;
            trail.numCornerVertices = 2;
            trail.alignment = LineAlignment.View;
            trail.textureMode = LineTextureMode.Stretch;
            trail.shadowCastingMode = ShadowCastingMode.Off;
            trail.receiveShadows = false;
            trail.material = CombatVfx.GetLaserMaterial();

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.85f, 1f, 1f), 0f),
                    new GradientColorKey(new Color(0.15f, 0.55f, 1f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            trail.colorGradient = gradient;
            laserReady = true;
        }
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
        // An enemy has several colliders. consumed stops the same hit from applying damage more than once.
        if (consumed || other.CompareTag("Player") || other.GetComponent<BulletController>() != null)
        {
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            CombatVfx.PlayImpact(other.ClosestPoint(transform.position));
            enemy.TakeDamage(damage);
        }

        consumed = true;
        DeactivateBullet();
    }


}
