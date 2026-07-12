using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject battle;
    private BattleSystem battleSystem;
    private Animator animator;

    void Start()
    {
        battleSystem = battle.GetComponent<BattleSystem>();
        animator = GetComponent<Animator>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            animator.SetBool("isOpen", true);

            battleSystem.PlayerGetDamage(20);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            animator.SetBool("isOpen", false);
        }
    }
}