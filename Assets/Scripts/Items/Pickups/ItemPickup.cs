using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemPickup: MonoBehaviour
{
    [SerializeField] SpriteRenderer pickupIcon;

    public virtual void UsePickup()
    {
        ResetUI();
        Destroy(gameObject);
    }

    protected virtual void ResetUI()
    {
        FindObjectOfType<PickupUI>().ResetPickupIcon();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag != "Player") return;

        Player player = collision.GetComponent<Player>();

        if (player.HasPickup()) return;

        player.SetPickup(this);

        FindObjectOfType<PickupUI>().ChangePickupIcon(pickupIcon.sprite);

        transform.SetParent(collision.transform);

        transform.localPosition = new Vector2(0, 0);

        this.gameObject.SetActive(false);
    }
}
