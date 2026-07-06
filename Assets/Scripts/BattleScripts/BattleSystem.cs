using Unity.VisualScripting;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private EnemyTrigger enemyTrigger;
    private SelectAction selectAction;
    private SelectFigure selectFigure;

    private GameObject battleZone;

    void Start()
    {
        selectAction = gameObject.GetComponent<SelectAction>();
        selectFigure = gameObject.GetComponent<SelectFigure>();

        Transform battleZoneTransform = transform.Find("BattleZone");
        
        if (battleZoneTransform != null)
        {
            battleZone = battleZoneTransform.gameObject;
        }
    }

    public void StartBattle(EnemyTrigger script)
    {
        if (script != null)
        {
            enemyTrigger = script;
        }

        selectAction.inBattle = true;
    }

    public void SelectAction(int action)
    {
        switch (action)
        {
            case 1:
                StartFight();

                break;
            case 2:
                break;
            case 3:
                int randomInt = Random.Range(0, 2);

                if (randomInt == 0)
                {
                    StartCoroutine(enemyTrigger.RunAway());
                }
                break;
        }
    }

    public void StartFight()
    {
        isFight(true);
        battleZone.SetActive(true);
    }

    private void isFight(bool value)
    {
        selectAction.inFight = value;
        selectFigure.inFight = value;
    }
}
