using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int health;
    public int damage;    
}
