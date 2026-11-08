using UnityEngine;
using UnityEngine.Rendering;

// Muzzle flash, impact, and explosion created in code. They destroy themselves when finished.
// The additive material also draws the laser beam and its trail.
public static class CombatVfx
{
    static Material sharedMaterial;
    static Texture2D circleTexture;

    public static void PlayMuzzle(Vector3 position, Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector3.up;
        }

        ParticleSystem particles = CreateBurst(
            "Muzzle Flash",
            position,
            Quaternion.LookRotation(direction.normalized),
            count: 10,
            lifetime: 0.12f,
            speed: 3.5f,
            size: 0.18f,
            colorA: new Color(0.85f, 1f, 1f),
            colorB: new Color(0.2f, 0.7f, 1f),
            shape: ParticleSystemShapeType.Cone,
            shapeAngle: 18f);

        var main = particles.main;
        main.duration = 0.08f;
        particles.Play();
    }

    public static void PlayImpact(Vector3 position)
    {
        ParticleSystem particles = CreateBurst(
            "Bullet Impact",
            position,
            Quaternion.identity,
            count: 16,
            lifetime: 0.28f,
            speed: 2.2f,
            size: 0.14f,
            colorA: new Color(1f, 0.85f, 0.35f),
            colorB: new Color(1f, 0.35f, 0.1f),
            shape: ParticleSystemShapeType.Sphere,
            shapeAngle: 0f);

        particles.Play();
    }

    public static void PlayExplosion(Vector3 position)
    {
        ParticleSystem particles = CreateBurst(
            "Enemy Explosion",
            position,
            Quaternion.identity,
            count: 36,
            lifetime: 0.5f,
            speed: 3.4f,
            size: 0.42f,
            colorA: new Color(1f, 0.75f, 0.25f),
            colorB: new Color(0.85f, 0.15f, 0.05f),
            shape: ParticleSystemShapeType.Sphere,
            shapeAngle: 0f);

        particles.Play();
    }

    static ParticleSystem CreateBurst(
        string name,
        Vector3 position,
        Quaternion rotation,
        int count,
        float lifetime,
        float speed,
        float size,
        Color colorA,
        Color colorB,
        ParticleSystemShapeType shape,
        float shapeAngle)
    {
        GameObject effectObject = new GameObject(name);
        effectObject.transform.SetPositionAndRotation(position, rotation);

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = particles.main;
        main.duration = lifetime;
        main.loop = false;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = lifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.45f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.45f, size);
        main.startColor = new ParticleSystem.MinMaxGradient(colorA, colorB);
        main.gravityModifier = 0f;
        main.stopAction = ParticleSystemStopAction.Destroy;
        main.maxParticles = count + 4;

        var emission = particles.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var particleShape = particles.shape;
        particleShape.enabled = true;
        particleShape.shapeType = shape;
        particleShape.angle = shapeAngle;
        particleShape.radius = 0.05f;

        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = gradient;

        ParticleSystemRenderer renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetMaterial();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        return particles;
    }

    public static Material GetLaserMaterial()
    {
        return GetMaterial();
    }

    static Material GetMaterial()
    {
        if (sharedMaterial != null)
        {
            return sharedMaterial;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        sharedMaterial = new Material(shader);
        sharedMaterial.SetFloat("_Surface", 1f);
        sharedMaterial.SetFloat("_Blend", 2f);
        sharedMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        sharedMaterial.SetInt("_DstBlend", (int)BlendMode.One);
        sharedMaterial.SetInt("_ZWrite", 0);
        sharedMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        sharedMaterial.renderQueue = (int)RenderQueue.Transparent;
        sharedMaterial.SetTexture("_BaseMap", GetCircleTexture());
        sharedMaterial.SetTexture("_MainTex", GetCircleTexture());
        sharedMaterial.SetColor("_BaseColor", Color.white);
        return sharedMaterial;
    }

    static Texture2D GetCircleTexture()
    {
        if (circleTexture != null)
        {
            return circleTexture;
        }

        const int size = 64;
        circleTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float radius = size * 0.5f;
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - radius + 0.5f;
                float dy = y - radius + 0.5f;
                float distance = Mathf.Sqrt(dx * dx + dy * dy) / radius;
                float alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha;
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        circleTexture.SetPixels(pixels);
        circleTexture.Apply();
        return circleTexture;
    }
}
