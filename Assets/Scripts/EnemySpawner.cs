using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Three enemy groups spawn from their pools once the ship passes startX.
// The area is an XY ellipse: the same Z as the player, less height than width,
// and a minimum height so they never spawn under the ground.
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        public ObjectPool enemyPool;
        public Transform spawnCenter;
        public Color tint = Color.white;
        public float startX;
        public int enemyCount;
        public float spawnInterval = 3f;

        [HideInInspector] public bool hasStarted;
    }

    [Header("Spawn Area")]
    [SerializeField] private float radius = 9f;
    [SerializeField] private float verticalRadius = 3.5f;
    [SerializeField] private float minSpawnHeight = 2f;
    [SerializeField] private float minSeparation = 3.5f;

    [SerializeField] private List<SpawnGroup> spawnGroups;

    private Transform playerTransform;
    private readonly List<Vector3> reservedPositions = new List<Vector3>();

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        foreach (var group in spawnGroups)
        {
            if (group.spawnCenter == null || group.hasStarted)
            {
                continue;
            }

            if (playerTransform.position.x >= group.startX)
            {
                group.hasStarted = true;
                StartCoroutine(SpawnEnemies(group));
            }
        }
    }

    private IEnumerator SpawnEnemies(SpawnGroup group)
    {
        for (int i = 0; i < group.enemyCount; i++)
        {
            Vector3 spawnPosition = PickSpawnPosition(group);
            reservedPositions.Add(spawnPosition);
            GameObject enemy = group.enemyPool.GetObjectFromPool(spawnPosition);
            // Prefabs are authored at scale 1; in the level they spawn at 0.15.
            enemy.transform.localScale = Vector3.one * 0.15f;
            ApplyTint(enemy, group.tint);
            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    private Vector3 PickSpawnPosition(SpawnGroup group)
    {
        Vector3 center = group.spawnCenter.position;
        // 2.5D game: the circle is XY and the depth matches the ship.
        center.z = playerTransform.position.z;

        Vector3 bestCandidate = center;
        float bestDistance = -1f;

        for (int attempt = 0; attempt < 40; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle;
            Vector3 candidate = center + new Vector3(offset.x * radius, offset.y * verticalRadius, 0f);
            // The horizontal radius is large; without this clamp the ellipse crosses the ground.
            if (candidate.y < minSpawnHeight)
            {
                candidate.y = minSpawnHeight;
            }
            float nearest = DistanceToNearest(candidate);

            if (nearest > bestDistance)
            {
                bestDistance = nearest;
                bestCandidate = candidate;
            }

            if (nearest >= minSeparation)
            {
                return candidate;
            }
        }

        return bestCandidate;
    }

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    // One tint per group, without duplicating the model's shared material.
    private static void ApplyTint(GameObject enemy, Color tint)
    {
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>(true);
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor(BaseColorId, tint);
            block.SetColor(ColorId, tint);
            renderers[i].SetPropertyBlock(block);
        }
    }

    private float DistanceToNearest(Vector3 candidate)
    {
        float nearest = float.MaxValue;

        for (int i = 0; i < reservedPositions.Count; i++)
        {
            nearest = Mathf.Min(nearest, Vector3.Distance(candidate, reservedPositions[i]));
        }

        FlyingEnemy[] enemies = FindObjectsByType<FlyingEnemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            nearest = Mathf.Min(nearest, Vector3.Distance(candidate, enemies[i].transform.position));
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnGroups == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        for (int groupIndex = 0; groupIndex < spawnGroups.Count; groupIndex++)
        {
            Transform center = spawnGroups[groupIndex].spawnCenter;
            if (center == null)
            {
                continue;
            }

            const int segments = 40;
            Vector3 previous = center.position + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                Vector3 next = center.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * verticalRadius, 0f);
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }
    }
}
