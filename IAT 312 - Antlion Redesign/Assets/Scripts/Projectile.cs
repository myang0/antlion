using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Antlion")) {
            AntlionBehavior ab = col.gameObject.GetComponent<AntlionBehavior>();
            ab.Damage(damage);
        }

        Destroy(gameObject);
    }
}
