using UnityEngine;

public class FogController : MonoBehaviour
{
    public GameObject fog;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(fog);
    }
}
