using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public GameObject collider;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collider != null) collider.SetActive(true);

            Destroy(gameObject);
        }
    }
}
