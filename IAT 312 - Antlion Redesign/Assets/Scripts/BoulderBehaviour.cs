using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    void Update() {
        float cameraHeight = Camera.main.orthographicSize;

        if (playerBoulderDistance() > cameraHeight) {
            Destroy(this);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        

        if (col.gameObject.tag == "InnerWall") {
            Destroy(col.gameObject);
        }
    }

    float playerBoulderDistance() {
        return player.transform.position.y - transform.position.y;
    }
}
