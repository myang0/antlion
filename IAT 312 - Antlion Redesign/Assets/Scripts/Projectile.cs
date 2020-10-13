using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private AudioSource audio;
    public float damage;
    [SerializeField] private GameObject particles;

    private void Start() {
        audio = gameObject.GetComponent<AudioSource>();
        audio.Play();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Antlion")) {
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
