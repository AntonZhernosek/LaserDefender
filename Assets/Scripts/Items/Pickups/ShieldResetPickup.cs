using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldResetPickup : ItemPickup
{
    public override void UsePickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shield>().HandleShieldPickupUse();
        base.UsePickup();
    }
}
