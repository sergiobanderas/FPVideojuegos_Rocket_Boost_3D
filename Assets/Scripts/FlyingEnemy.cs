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
    private bool snapFacing;
    private float facingAngle;

    private void Awake()
    {
        // Moved by DOTween, so a kinematic body is required for the ship to receive the hit.
        Rigidbody body = GetComponent<Rigidbody>();
        if (body == null)
        {
            body = gameObject.AddComponent<Rigidbody>();
        }

        body.isKinematic = true;
        body.useGravity = false;
    }

    private void OnEnable()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // The pool reuses the object. Start does not run again, so the origin is stored here.
        startPosition = transform.position;
        snapFacing = true;
        StartFloating();
    }

    private void OnDisable()
    {
        // If the tween is still alive, reactivating the enemy continues the previous movement.
        if (floatingTween != null && floatingTween.IsActive())
        {
            floatingTween.Kill();
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {            
            StartChasing(distanceToPlayer);
        }
        else if (currentState == EnemyState.Chasing && distanceToPlayer > loseRange)
        {
            ReturnToStartPosition();
        }

        // Also while chasing: DOMove does not rotate the transform.
        FacePlayer();
    }

    // The model's head (local +Z) looks at the player on the XY plane.
    // Up stays toward world up so they do not flip upside down when they turn around.
    private void FacePlayer()
    {
        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.z = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        if (snapFacing)
        {
            facingAngle = targetAngle;
            snapFacing = false;
        }
        else
        {
            facingAngle = Mathf.LerpAngle(facingAngle, targetAngle, 12f * Time.deltaTime);
        }

        Vector3 head = new Vector3(Mathf.Cos(facingAngle * Mathf.Deg2Rad), Mathf.Sin(facingAngle * Mathf.Deg2Rad), 0f);
        Vector3 upHint = Mathf.Abs(head.y) > 0.95f ? Vector3.right : Vector3.up;
        transform.rotation = Quaternion.LookRotation(head, upHint);
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
        
        if (floatingTween != null && floatingTween.IsActive())
        {
            floatingTween.Kill();
        }

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
        if (floatingTween != null && floatingTween.IsActive())
        {
            floatingTween.Kill();
        }

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
