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
    private GameObject antlion;
    private GameObject player;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start () {
        antlion = GameObject.Find ("Antlion");
        player = GameObject.Find ("Player");
    }

    void FixedUpdate () {
        if (status == Status.Alive) {
            Vector3 dir = (player.transform.position - rigidBody.transform.position).normalized;
            if (Vector3.Distance (player.transform.position, rigidBody.transform.position) > 1) {
                rigidBody.MovePosition (rigidBody.transform.position + dir * movementSpeed * Time.fixedDeltaTime);
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
            boxCollider.enabled = true;
            status = Status.Alive;
        }
    }
}