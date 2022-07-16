using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBomb : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] GameObject explosionParticles;
    [SerializeField] float explosionRadius = 2f;

    [Header("Suck In")]
    [SerializeField] float suckInRadius = 3f;
    [SerializeField] float suckInForce = 4f;

    [Header("Gizmos")]
    [SerializeField] bool showGizmos = false;
    [SerializeField] Color gizmosColor = Color.white;

    Player player;
    Transform playerTransform;
    DamageDealer damageDealer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerTransform = player.transform;
        damageDealer = GetComponent<DamageDealer>();
    }

    private void Update()
    {
        SuckIn();
    }

    private void OnDestroy()
    {
        Explode();
    }

    //Triggered by animation
    public void DestroyBomb()
    {
        Destroy(gameObject);
    }

    private void Explode()
    {
        Instantiate(explosionParticles, transform.position, Quaternion.identity);

        HitNearbyTargets();

        FindObjectOfType<AudioPlayer>().PlayAudioClip("BombExplosion");
    }

    private void HitNearbyTargets()
    {
        var nearbyObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var nearbyObject in nearbyObjects)
        {
            Health health = nearbyObject.GetComponent<Health>();
            if (!health) continue;

            health.TakeDamage(damageDealer.GetDamage());
        }
    }

    void SuckIn()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) < suckInRadius)
        {
            Vector2 relativeDirection = transform.position - playerTransform.position;
            relativeDirection.Normalize();

            Vector2 newRelativePos = relativeDirection * suckInForce * Time.deltaTime;

            player.AffectMovement(newRelativePos);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, suckInRadius);
    }
}
