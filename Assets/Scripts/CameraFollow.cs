using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [HideInInspector] public bool active = true;

    void LateUpdate()
    {
        if (target != null && active)
        {
            Vector3 desiredPosition = target.position + new Vector3(0, 2, -10);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 0.125f);
            transform.position = smoothedPosition;

            transform.LookAt(target);
        }
    }
}