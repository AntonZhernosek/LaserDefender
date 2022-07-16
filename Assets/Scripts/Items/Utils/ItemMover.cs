using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    private void Start()
    {
        moveSpeed = -moveSpeed;
        Move();
    }

    private void Move()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * moveSpeed;
    }
}
