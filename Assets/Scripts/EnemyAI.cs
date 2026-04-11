using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float stoppingDistance = 0.1f;

    private Transform player;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private AudioSource audioSource;
    private bool hasCaughtPlayer = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player found: " + player.name);
        }
        else
        {
            Debug.LogError("Player not found! Make sure player tag is 'Player'");
        }
    }

    void Update()
    {
        if (player == null || hasCaughtPlayer) return;

        if (isChasing)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            patrolSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPoint.position) <= stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    public void StartChase()
    {
        if (hasCaughtPlayer) return;

        isChasing = true;
        Debug.Log("Enemy started chasing");
    }

    public void StopChase()
    {
        if (hasCaughtPlayer) return;

        isChasing = false;
        Debug.Log("Enemy stopped chasing");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);

        if (hasCaughtPlayer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasCaughtPlayer = true;
            Debug.Log("Player caught! Playing sound and calling Game Over.");

            if (audioSource != null)
            {
                audioSource.Play();
            }

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.2f, 0.3f);
            }

            Invoke(nameof(TriggerGameOver), 0.2f);
        }
    }

    void TriggerGameOver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager.Instance is NULL");
        }
    }
}