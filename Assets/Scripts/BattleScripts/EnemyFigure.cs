using UnityEngine;

public class EnemyFigure : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D playerFigure)
    {
        if (playerFigure.CompareTag("PlayerAttack"))
        {
            Debug.Log(playerFigure.name);
        }
    }
}
