using UnityEngine;

[CreateAssetMenu(fileName = "New Chocolate Effect", menuName = "Inventory/Effects/Chocolate")]
public class Chocolate : ItemEffect
{
    public override void Execute(GameObject player)
    {
        GameObject battle = GameObject.Find("Battle"); 

        if (battle != null)
        {
            battle.GetComponent<BattleSystem>().PlayerHeal(50);
        }
    }
}