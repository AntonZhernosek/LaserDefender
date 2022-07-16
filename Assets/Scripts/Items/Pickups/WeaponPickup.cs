using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : ItemPickup
{
    enum WeaponBehaviour
    {
        SwitchTemporarily,
        SwitchPermanently
    }

    enum ReferenceToUse
    {
        UseName,
        UseIndex
    }

    [Header("Weapon Behaviour")]
    [SerializeField] WeaponBehaviour weaponBehaviour;

    [Header("Weapon Reference")]
    [SerializeField] ReferenceToUse referenceToUse;
    [SerializeField] string attackName;
    [SerializeField] int attackIndex;

    [Header("Timer for temporary weapon")]
    [SerializeField] float switchTime = 10f;

    public override void UsePickup()
    {
        switch(weaponBehaviour)
        {
            case WeaponBehaviour.SwitchPermanently:
                HandlePermanentSwitch();
                break;

            case WeaponBehaviour.SwitchTemporarily:
                HandleTemporarySwitch();
                break;
        }

        base.UsePickup();
    }

    private void HandlePermanentSwitch()
    {
        switch(referenceToUse)
        {
            case ReferenceToUse.UseIndex:
                GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>().ChooseAttack(attackIndex);
                break;

            case ReferenceToUse.UseName:
                GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>().ChooseAttack(attackName);
                break;
        }
    }

    private void HandleTemporarySwitch()
    {
        switch (referenceToUse)
        {
            case ReferenceToUse.UseIndex:
                GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>().SwitchWeaponTemporarily(switchTime, attackIndex);
                break;

            case ReferenceToUse.UseName:
                GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>().SwitchWeaponTemporarily(switchTime, attackName);
                break;
        }
    }
}
