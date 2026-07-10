using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int currentHealth { get; private set; } = 125;
    public int maxHealth { get; private set; } = 125;

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void HealMax()
    {
        currentHealth = maxHealth;
    }
}
