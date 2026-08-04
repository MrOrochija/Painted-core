using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Transform point;
    public float waitTime;
}

public class NPCMovement : MonoBehaviour
{
    public Waypoint[] waypoints;

    public Transform visionPivot;

    [HideInInspector] public bool canMove = true; 

    private int currentWaypointIndex = 0;
    private Animator anim;
    private Rigidbody2D rb;

    private bool isWaiting = false;
    private float waitTimer = 0f;

    private Transform playerTarget;
    private bool isChasing = false;
    private bool isLostPlayerWaiting = false;
    private float lostTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            StopAnimation();
            return;
        }

        if (isChasing && playerTarget != null)
        {
            RotatePivotTo(playerTarget.position);

            Vector2 dirToPlayer = (playerTarget.position - transform.position).normalized;
            anim.SetFloat("moveX", dirToPlayer.x);
            anim.SetFloat("moveY", dirToPlayer.y);
            return;
        }

        if (isLostPlayerWaiting)
        {
            lostTimer -= Time.deltaTime;
            StopAnimation();

            if (lostTimer <= 0f)
            {
                isLostPlayerWaiting = false;
            }
            return;
        }

        if (waypoints.Length == 0)
        {
            StopAnimation();
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            StopAnimation();

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                GoToNextWaypoint();
            }
            
            return;
        }

        Transform targetPoint = waypoints[currentWaypointIndex].point;
        if (targetPoint == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, targetPoint.position);

        if (distanceToTarget < 0.1f)
        {
            float timeToWait = waypoints[currentWaypointIndex].waitTime;
            
            if (timeToWait > 0f)
            {
                isWaiting = true;
                waitTimer = timeToWait;
            }
            else
            {
                GoToNextWaypoint();
            }
        }
        else
        {
            Vector2 direction = (targetPoint.position - transform.position).normalized;
            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.y);

            RotatePivotTo(targetPoint.position);
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isChasing && playerTarget != null)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, playerTarget.position, 3f * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            return;
        }

        if (isLostPlayerWaiting || isWaiting || waypoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = waypoints[currentWaypointIndex].point;
        if (targetPoint == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = targetPoint.position;
        
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, 3f * Time.fixedDeltaTime);
        
        rb.MovePosition(newPosition);
    }

    private void RotatePivotTo(Vector3 targetPosition)
    {
        if (visionPivot == null) return;

        Vector2 dir = targetPosition - visionPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        angle -= 90f;

        visionPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = other.transform;
            isChasing = true;
            isLostPlayerWaiting = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            playerTarget = null;
            isLostPlayerWaiting = true;
            lostTimer = 3f;
        }
    }

    private void GoToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0;
        }
    }

    private void StopAnimation()
    {
        anim.SetFloat("moveX", 0f);
        anim.SetFloat("moveY", 0f);
    }
}