using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntlionBehavior : MonoBehaviour {
    private enum Status {
        NotSpawned,
        Alive,
        Dead
    }

    private Status status = Status.NotSpawned;
    public float movementSpeed = 4f;
    public float rageMovementSpeed = 6f;
    private GameObject antlion;
    private GameObject player;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polyCollider;
    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start () {
        antlion = GameObject.Find ("Antlion");
        player = GameObject.Find ("Player");
    }

    void FixedUpdate () {
        if (status == Status.Alive) {
            Vector3 playerPos = player.transform.position;
            Vector3 antlionPos = rigidBody.transform.position;
            Vector3 dir = (playerPos - antlionPos).normalized;

            if (Vector3.Distance (playerPos, antlionPos) > 1) {
                //If player is not behind antlion and antlion isn't too far behind
                if (playerPos.y - antlionPos.y > 0 && playerPos.y - antlionPos.y < 8) {
                    rigidBody.MovePosition (antlionPos + dir * movementSpeed * Time.fixedDeltaTime);
                } else {
                    rigidBody.MovePosition (antlionPos + dir * rageMovementSpeed * Time.fixedDeltaTime);
                }

                float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
            }
        }
    }

    private void OnCollisionEnter2D (Collision2D collider) {
        if (collider.gameObject.layer == 8 || collider.gameObject.tag == "Boulder") {
            Destroy (collider.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        if ((player.transform.position.y > 12) && status == Status.NotSpawned) {
            spriteRenderer.enabled = true;
            // boxCollider.enabled = true;
            polyCollider.enabled = true;
            status = Status.Alive;
        }
    }
}