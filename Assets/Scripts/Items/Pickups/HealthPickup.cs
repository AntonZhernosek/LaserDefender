using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : ItemPickup
{
    [SerializeField] int healAmount = 20;

    public override void UsePickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().Heal(healAmount);
        base.UsePickup();
    }
}
