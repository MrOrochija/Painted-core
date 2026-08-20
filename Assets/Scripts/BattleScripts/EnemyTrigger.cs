using System.Collections;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject battle;
    public GameObject player;
    public FadeModule fadeModule;
    
    public GameObject battleZone;
    public GameObject zone;

    [HideInInspector] public bool inBattle = false;

    private BattleSystem battleSystem;
    private PlayerMovement plrMovement;
    private GameObject enemy;
    private EnemyHealth enemyHealth;
    private SpriteRenderer enemySpriteRenderer;
    private NPCMovement nPCMovement;

    void Start()
    {
        nPCMovement = gameObject.transform.parent.GetComponent<NPCMovement>();

        if (battle != null) battleSystem = battle.GetComponent<BattleSystem>();
        if (player != null) plrMovement = player.GetComponent<PlayerMovement>();

        Transform enemyTransform = transform.parent;
        if (enemyTransform != null)
        {
            enemy = enemyTransform.gameObject;
            enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
            enemyHealth = enemy.GetComponent<EnemyHealth>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !inBattle)
        {
            inBattle = true;
            StartCoroutine(InteractRoutine());
        }
    }

    private IEnumerator InteractRoutine()
    {
        nPCMovement.canMove = false;

        if (plrMovement != null) plrMovement.currentState = PlayerState.Combat; 

        if (battleZone != null) battleZone.SetActive(false);

        yield return StartCoroutine(fadeModule.Fade(1));

        if (zone != null)
        {
            player.transform.position = zone.transform.position;
        }

        if (enemyHealth != null && battleSystem != null)
        {
            battleSystem.StartBattle(this, enemyHealth);
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(fadeModule.Fade(0));
    }

    public IEnumerator RunAway()
    {
        yield return StartCoroutine(fadeModule.Fade(1));
        player.transform.position = enemy.transform.position;
        
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fadeModule.Fade(0));

        if (plrMovement != null) plrMovement.currentState = PlayerState.Free;

        yield return new WaitForSeconds(5f);
        inBattle = false;

        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        if (myCollider != null && playerCollider != null && myCollider.IsTouching(playerCollider))
        {
            inBattle = true;
            StartCoroutine(InteractRoutine());
        } else
        {
            nPCMovement.canMove = true;
        }
    }

    public IEnumerator EnemyDead()
    {
        yield return StartCoroutine(fadeModule.Fade(1));
        player.transform.position = enemy.transform.position;
        
        if (enemySpriteRenderer != null)
        {
            Color color = enemySpriteRenderer.color;
            color.a = 0;
            enemySpriteRenderer.color = color;
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fadeModule.Fade(0));

        if (plrMovement != null) plrMovement.currentState = PlayerState.Free;

        if (enemy != null)
        {
            Destroy(enemy);
        }
    }
}