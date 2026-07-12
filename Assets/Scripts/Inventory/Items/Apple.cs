using UnityEngine;

[CreateAssetMenu(fileName = "New Apple Effect", menuName = "Inventory/Effects/Apple")]
public class Apple : ItemEffect
{
    public override void Execute(GameObject player)
    {
        GameObject battle = GameObject.Find("Battle"); 

        if (battle != null)
        {
            battle.GetComponent<BattleSystem>().PlayerHeal(20);
        }
    }
}