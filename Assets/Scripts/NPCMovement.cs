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

    [HideInInspector] public bool canMove = true; 

    private int currentWaypointIndex = 0;
    private Animator anim;
    private Rigidbody2D rb;

    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove || waypoints.Length == 0)
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
        }
    }

    void FixedUpdate()
    {
        if (!canMove || waypoints.Length == 0 || isWaiting)
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