using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedWallBehavior : MonoBehaviour {
    [SerializeField] GameObject particles;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (other.gameObject.GetComponent<PlayerMovement>().hasKey) {
                Destroy(this.gameObject);
            }
        }
    }
    
    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandSpit")) {
            collider.GetComponent<SandSpitBehavior>().SpawnSandTile();
        }
    }

    void OnDestroy() {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
