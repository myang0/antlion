using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AntlionBehavior : MonoBehaviour {
    private enum Status {
        NotSpawned,
        Alive,
        Dead
    }

    private Status status = Status.NotSpawned;
    public float movementSpeed = 4f;
    public float rageMovementSpeed = 8f;
    private GameObject antlion;
    private GameObject player;
    [SerializeField] private GameObject sandSpitPrefab;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polyCollider;
    public Rigidbody2D rigidBody;
    private bool isSpitReady = true;

    // Start is called before the first frame update
    void Start() {
        antlion = GameObject.Find("Antlion");
        player = GameObject.Find("Player");

        if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            spriteRenderer.enabled = true;
            polyCollider.enabled = true;
        }
    }

    void FixedUpdate() {
        if (status == Status.Alive) {
            Vector3 antlionPos = rigidBody.transform.position;
            if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
                if (player) {
                    RunPhaseMovement(antlionPos);
                } else {
                    RunOffScreen(antlionPos);
                }
            } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
                if (player) {
                    FightPhaseAttack(antlionPos);
                } else {

                }
            }
        }
    }

    private void FightPhaseAttack(Vector3 antlionPos) {
        Vector3 playerPos = player.transform.position;
        Vector3 dir = (playerPos - antlionPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion transformRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = transformRotation;

        SpitSand(antlionPos, transformRotation);
    }

    private void SpitSand(Vector3 antlionPos, Quaternion transformRotation) {
        if (isSpitReady) {
            Vector3 spitSpawnPosition = new Vector3(antlionPos.x, antlionPos.y, 0);
            Vector3 mouthOffset = transformRotation * (new Vector3(0, 2.5f, 0));
            spitSpawnPosition = spitSpawnPosition + mouthOffset;
            GameObject sandSpitObject = Instantiate(sandSpitPrefab, spitSpawnPosition, Quaternion.identity);
            Rigidbody2D sandSpitRigidbody2D = sandSpitObject.GetComponent<Rigidbody2D>();
            sandSpitRigidbody2D.velocity = transformRotation * (new Vector2(0, 8f));
            isSpitReady = false;
            StartCoroutine(SpitAttackTimer());
            
        }
    }

    IEnumerator SpitAttackTimer() {
        yield return new WaitForSeconds(5f);
        isSpitReady = true;
    }

    private void RunOffScreen(Vector3 antlionPos) {
        //Player is dead, antlion runs off screen
        Vector3 dir = new Vector3(0f, 2f, 0f);
        rigidBody.MovePosition(antlionPos +
                               dir * (movementSpeed * Time.fixedDeltaTime));
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void RunPhaseMovement(Vector3 antlionPos) {
        Vector3 playerPos = player.transform.position;
        if (playerPos.y - antlionPos.y > 2) {
            playerPos = new Vector3(playerPos.x, playerPos.y + 8, playerPos.z);
        } else if (playerPos.y - antlionPos.y < -2) {
            playerPos = new Vector3(playerPos.x, playerPos.y - 8, playerPos.z);
        }

        Vector3 dir = (playerPos - antlionPos).normalized;

        if (Vector3.Distance(playerPos, antlionPos) > 1) {
            //If player is not behind antlion and antlion isn't too far behind
            if (playerPos.y - antlionPos.y > 0 && playerPos.y - antlionPos.y < 16) {
                rigidBody.MovePosition(antlionPos +
                                       dir * (movementSpeed * Time.fixedDeltaTime));
            } else {
                //If antlion needs to catch up, it speeds up
                rigidBody.MovePosition(antlionPos +
                                       dir * (rageMovementSpeed * Time.fixedDeltaTime));
            }

            //Rotation
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer == 8 || col.gameObject.CompareTag("Boulder")) {
            Destroy(col.gameObject);
        }
    }

    void Update() {
        if (player) {
            if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene") &&
                (player.transform.position.y > 12) && status == Status.NotSpawned) {
                spriteRenderer.enabled = true;
                polyCollider.enabled = true;
                status = Status.Alive;
            } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase") &&
                       (player.transform.position.y > 5.5) && status == Status.NotSpawned) {
                status = Status.Alive;
            }
        }
    }
}