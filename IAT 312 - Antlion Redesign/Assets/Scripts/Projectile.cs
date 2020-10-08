using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Antlion")) {
            // Rigidbody2D rigidbody2D = this.GetComponent<Rigidbody2D>();
            // rigidbody2D.velocity = Vector3.zero;
            // rigidbody2D.angularVelocity = 0;
            AntlionBehavior ab = col.gameObject.GetComponent<AntlionBehavior>();
            ab.Damage(damage);
        }

        Destroy(gameObject);
    }
}
