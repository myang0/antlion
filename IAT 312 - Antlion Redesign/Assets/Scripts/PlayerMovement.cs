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

    Vector2 movement;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
       movement.x = Input.GetAxisRaw("Horizontal");
       movement.y = Input.GetAxisRaw("Vertical");

       if (Input.GetKeyDown(KeyCode.Space)) {
           Attack();
       }
    }

    void FixedUpdate() {
        // Movement
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
}
