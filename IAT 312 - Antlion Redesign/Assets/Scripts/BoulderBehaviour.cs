using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour
{
    private GameObject player;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (player != null) {
            float playerY = player.transform.position.y;
            float boulderY = transform.position.y;

            if (playerY - boulderY > 10f) {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer == 8 && !col.gameObject.CompareTag("TintedWall")) {
            Destroy(col.gameObject);
        }
    }
}
