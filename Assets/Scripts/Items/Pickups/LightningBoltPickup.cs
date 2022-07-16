using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltPickup : ItemPickup
{
    [SerializeField] int damage = 50;
    [SerializeField] GameObject explosionParticles;

    public override void UsePickup()
    {
        DealDamage();
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        FindObjectOfType<AudioPlayer>().PlayAudioClip("Lightning");
        Camera.main.GetComponent<CameraShake>().Play();
        base.UsePickup();
    }

    private void DealDamage()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0) return;

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
