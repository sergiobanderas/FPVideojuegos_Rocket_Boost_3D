using UnityEngine;
using DG.Tweening;
using System;

public class FlyingEnemy : MonoBehaviour
{

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseRange = 7f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float floatingHeight = 1f;
    [SerializeField] private float floatingDuration = 1f;

    private Transform playerTransform;

    private Vector3 startPosition;
    private Tween floatingTween;

    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    private EnemyState currentState = EnemyState.Idle;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;

        StartFloating();
        
    }

    void Update()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {            
            StartChasing(distanceToPlayer);
        }
        else if (currentState == EnemyState.Chasing && distanceToPlayer > loseRange)
        {
            ReturnToStartPosition();
        }        
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw lose range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }

    private void ReturnToStartPosition()
    {
        
        floatingTween.Kill();

        // Move back to the starting position
        float distanceToStart = Vector3.Distance(transform.position, startPosition);
        
        float duration = distanceToStart / moveSpeed;

        floatingTween = transform
                    .DOMove(startPosition, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => StartFloating());


    }

    private void StartChasing(float distanceToPlayer)
    {
        currentState = EnemyState.Chasing;
        floatingTween.Kill();

        // Move towards the player
        float duration = Mathf.Max(0.1f, distanceToPlayer / moveSpeed);

        floatingTween = transform
                    .DOMove(playerTransform.position, duration)
                    .SetEase(Ease.Linear);

    }

    private void StartFloating()
    {
        currentState = EnemyState.Idle;
        // Create a floating motion using DOTween
        floatingTween = transform.DOMoveY(startPosition.y + floatingHeight, floatingDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
