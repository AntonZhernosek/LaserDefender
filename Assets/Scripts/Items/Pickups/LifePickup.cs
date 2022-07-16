using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePickup : ItemPickup
{
    public override void UsePickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Lives>().AddALife();
        base.UsePickup();
    }
}
