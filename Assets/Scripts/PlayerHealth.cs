using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int Health { get; private set; } = 200;

    public void GetDamage(int damage)
    {
        Health -= damage;
    }
}
