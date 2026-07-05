using UnityEngine;
using UnityEngine.UI;

public class HeartMonitoring : MonoBehaviour
{
    [HideInInspector] public bool isFighting = false;

    private Image player;
    private Image enemy;

    void Start()
    {
        Transform playerTransform = transform.Find("Player/HeartBar/Image");
        Transform enemyTransform = transform.Find("Enemy/HeartBar/Image");

        if (playerTransform != null && enemyTransform != null)
        {
            player = playerTransform.GetComponent<Image>();
            enemy = enemyTransform.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (isFighting)
        {
            
        }
    }
}