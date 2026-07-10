using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth { get; private set; } = 200;
    public int maxHealth { get; private set; } = 200;

    public int currentMana { get; private set; } = 0;
    public int maxMana { get; private set; } = 100;

    public GameObject currentCheckpoint;

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
    }
}
