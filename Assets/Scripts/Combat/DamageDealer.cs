using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] bool isCollisionDamage = false;
    [SerializeField] int shootingDamage = 10;
    [SerializeField] int collisionDamage = 50;

    public void SetDamage(int amount)
    {
        shootingDamage = amount;
    }

    public void AddDamage(int amount)
    {
        shootingDamage += amount;
    }

    public int GetDamage()
    {
        if (isCollisionDamage) return collisionDamage;
        return shootingDamage;
    }

    public void Hit()
    {
        if (gameObject.tag != "Projectile") return;
        Destroy(gameObject);
    }
}
