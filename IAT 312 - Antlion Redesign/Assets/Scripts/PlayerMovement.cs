using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Rigidbody2D rigidBody;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask wallLayers;

    public bool isStunned = false;

    Vector2 movement;
    Vector2 kbVector;

    public int kbTimer = 0;

    void Start() {

    }

    void Update() {
        if (!isStunned) {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
       
        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack();
        }

        if (kbTimer > 0) {
            rigidBody.AddForce(kbVector * kbTimer, ForceMode2D.Force);
            kbTimer--;
            isStunned = (kbTimer != 0);
        }
    }

    void FixedUpdate() {
        rigidBody.MovePosition(rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandTile")) {
            movementSpeed = 2.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandTile")) {
            movementSpeed = 5f;
        }
    }

    void Attack() {
        Collider2D[] hitWalls = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, wallLayers);

        foreach(Collider2D wall in hitWalls) {
            Destroy(wall.gameObject);
        }
    }

    void OnDrawGizmosSelected() {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Boulder") {
            Rigidbody2D boulderRb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 difference = boulderRb.transform.position - transform.position;
            difference = - difference.normalized;

            kbVector = difference;
            kbTimer = 120;

            isStunned = true;
        }
    }
}
