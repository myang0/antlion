using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    public float baseMovementSpeed = 5f;
    private float currentMovementSpeed = 5f;

    public Rigidbody2D rigidBody;
    [SerializeField]
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vcamNoise;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask wallLayers;

    public bool isStunned = false;

    Vector2 movement;
    Vector2 kbVector;

    public float kbTimer = 0f;

    [SerializeField]
    private float health = 100;
    [SerializeField]
    private bool shielded = false;
    [SerializeField]
    private float shieldedSpeed = 7.5f;

    void Start() {
        currentMovementSpeed = baseMovementSpeed;

        SceneManager.activeSceneChanged += SceneTransition;
    
        if (vcam) {
            vcamNoise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin> ();
        }
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

        if (health < 0) {
            vcamNoise.m_FrequencyGain = 0f;
            Destroy (this.gameObject);
        }
    }

    void FixedUpdate () {
        if (!shielded) {
            rigidBody.MovePosition (rigidBody.position + movement * (currentMovementSpeed * Time.fixedDeltaTime));
        } else {
            rigidBody.MovePosition (rigidBody.position + movement * (shieldedSpeed * Time.fixedDeltaTime));
        }
        RotateAnt();
    }
    
    public void ApplyBuffs(float healthBoost, float attackBoost, float speedBoost) {
        baseMovementSpeed *= speedBoost;
        currentMovementSpeed = baseMovementSpeed;

        // TODO: apply attack boost

        health += healthBoost;
    }

    private void RotateAnt() {
        if ((movement.x != 0 || movement.y != 0)) {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion rotationAngle = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = rotationAngle;
        }
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

    void OnCollisionEnter2D (Collision2D col) {
        if (!shielded) {
            if (col.gameObject.CompareTag("Boulder")) {
                Rigidbody2D boulderRb = col.gameObject.GetComponent<Rigidbody2D> ();
                Vector2 difference = boulderRb.transform.position - transform.position;
                difference = -difference.normalized;
                kbVector = difference;
                kbTimer = 240;

                isStunned = true;
            } else if (col.gameObject == GameObject.Find ("Antlion")) {
                // int damageTaken = Random.Range (5, 10);
                int damageTaken = Random.Range (25, 35);
                health = health - damageTaken;
                StartCoroutine (ActivateShield ());
                StartCoroutine (ActivateScreenShake ());
            }
        }
    }

    IEnumerator ActivateScreenShake () {
        vcamNoise.m_AmplitudeGain = 7f;
        vcamNoise.m_FrequencyGain = 4f;
        yield return new WaitForSeconds (0.5f);
        vcamNoise.m_FrequencyGain = 0f;
    }

    IEnumerator ActivateShield () {
        shielded = true;
        isStunned = false;
        yield return new WaitForSeconds (2);
        shielded = false;
    }

    public Vector2 GETMovement () {
        return movement;
    }

    public bool GETShielded () {
        return shielded;
    }

    void SceneTransition(Scene current, Scene next) {
        transform.position = new Vector3(0, 0, 0);
    }
}
