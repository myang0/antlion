﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    
    [SerializeField] private GameObject particles;

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Antlion")) {
            // Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
            // rigidbody2D.velocity = Vector3.zero;
            // rigidbody2D.angularVelocity = 0;
            AntlionBehavior ab = col.gameObject.GetComponent<AntlionBehavior>();
            ab.Damage(damage);
        }

        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Swarmer")) {
            Swarmer s = other.GetComponent<Swarmer>();
            s.inflictDamage(damage);

            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
