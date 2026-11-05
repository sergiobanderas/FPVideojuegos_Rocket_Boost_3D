using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        public ObjectPool enemyPool;
        public Transform spawnPoint;
        public float startX;
        public int enemyCount;
        public float spawnInterval = 1f;

        [HideInInspector] public bool hasStarted;
    }

    [SerializeField] private List<SpawnGroup> spawnGroups;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        foreach (var group in spawnGroups)
        {
            if (!group.hasStarted && playerTransform.position.x >= group.startX)
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
            group.enemyPool.GetObjectFromPool(group.spawnPoint.position);
            yield return new WaitForSeconds(group.spawnInterval);
        }
        
    }
}
