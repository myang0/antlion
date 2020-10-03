using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    public float baseMovementSpeed = 5f;
    private float currentMovementSpeed = 5f;

    public Rigidbody2D rigidBody;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask wallLayers;

    public bool isStunned = false;

    Vector2 movement;
    Vector2 kbVector;

    public float kbTimer = 0f;

    public void applyBuffs(float healthBoost, float attackBoost, float speedBoost) {
        baseMovementSpeed *= speedBoost;
        currentMovementSpeed = baseMovementSpeed;

        // TODO: apply attack boost

        // TODO: apply heal
    }

    void Start() {
        currentMovementSpeed = baseMovementSpeed;

        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += sceneTransition;
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
            rigidBody.AddForce(kbVector * (kbTimer * 0.3f), ForceMode2D.Force);
            kbTimer--;
            isStunned = (kbTimer != 0);
        }
    }

    void FixedUpdate() {
        rigidBody.MovePosition(rigidBody.position + movement * currentMovementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains ("SandTile")) {
            currentMovementSpeed = baseMovementSpeed / 2;
        } else if (cName.Contains ("BrokenWallTile")) {
            currentMovementSpeed = baseMovementSpeed / 3;
        }
    }

    private void OnTriggerExit2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains ("SandTile") || cName.Contains ("BrokenWallTile")) {
            currentMovementSpeed = baseMovementSpeed;
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

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Boulder") {
            Rigidbody2D boulderRb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 difference = boulderRb.transform.position - transform.position;
            difference = - difference.normalized;

            kbVector = difference;
            kbTimer = 240;

            isStunned = true;
        }
    }

    void sceneTransition(Scene current, Scene next) {
        transform.position = new Vector3(0, 0, 0);
    }
}
