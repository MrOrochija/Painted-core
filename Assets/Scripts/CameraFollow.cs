using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [HideInInspector] public bool active = true;

    void LateUpdate()
    {
        if (target != null && active)
        {
            transform.position = target.position + new Vector3(0, 0, -10);

            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}