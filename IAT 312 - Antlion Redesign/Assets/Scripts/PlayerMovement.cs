using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float movementSpeed = 5f;
    public Rigidbody2D rigidBody;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask wallLayers;

    public bool isStunned = false;

    Vector2 movement;
    Vector2 kbVector;

    public float kbTimer = 0f;

    void Start () {

    }

    void Update () {
        if (!isStunned) {
            movement.x = Input.GetAxisRaw ("Horizontal");
            movement.y = Input.GetAxisRaw ("Vertical");
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            Attack ();
        }

        if (kbTimer > 0) {
            rigidBody.AddForce (kbVector * (kbTimer * 0.3f), ForceMode2D.Force);
            kbTimer--;
            isStunned = (kbTimer != 0);
        }
    }

    void FixedUpdate () {
        rigidBody.MovePosition (rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);

        if (!isStunned && (movement.x != 0 || movement.y != 0)) {
            float angle = Mathf.Atan2 (movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
        } else if (isStunned) {
            //for fun
            // transform.rotation = Quaternion.AngleAxis (rigidBody.rotation + 20, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains ("SandTile")) {
            movementSpeed = 2.5f;
        } else if (cName.Contains ("BrokenWallTile")) {
            movementSpeed = 1.75f;
        }
    }

    private void OnTriggerExit2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains ("SandTile") || cName.Contains ("BrokenWallTile")) {
            movementSpeed = 5f;
        }
    }

    void Attack () {
        Collider2D[] hitWalls = Physics2D.OverlapCircleAll (attackPoint.position, attackRange, wallLayers);

        foreach (Collider2D wall in hitWalls) {
            Destroy (wall.gameObject);
        }
    }

    void OnDrawGizmosSelected () {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere (attackPoint.position, attackRange);
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag == "Boulder") {
            Rigidbody2D boulderRb = col.gameObject.GetComponent<Rigidbody2D> ();
            Vector2 difference = boulderRb.transform.position - transform.position;
            difference = -difference.normalized;

            kbVector = difference;
            kbTimer = 240;

            isStunned = true;
        }
    }
}