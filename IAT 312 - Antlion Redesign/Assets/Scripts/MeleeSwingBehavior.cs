using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSwingBehavior : MonoBehaviour {
    private PolygonCollider2D polyCollider;
    public float damageMultiplier;
    public float baseDamage;
    private List<Collider2D> objectsHit;

    void Start() {
        polyCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        objectsHit = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!objectsHit.Contains(other)) {
            objectsHit.Add(other);
            string cName = other.name;
            if (cName.Contains("SandSpit")) {
                Destroy(other.gameObject);
            } else if (other.CompareTag("Antlion")) {
                other.gameObject.GetComponent<AntlionBehavior>().Damage(baseDamage * damageMultiplier);
            }

            if (other.CompareTag("Swarmer")) {
                other.gameObject.GetComponent<Swarmer>().inflictDamage(baseDamage * damageMultiplier);
            }
        }
    }
}
