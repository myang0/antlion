using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
    private float currentRotationSpeed = 2.5f;
    private float baseRotationSpeed = 0.2f;
    private float spitReadyRotationSpeed = 2.5f;
    public float health = 1000;
    private float maxHealth;

    // Start is called before the first frame update
    void Start() {
        antlion = GameObject.Find("Antlion");
        player = GameObject.Find("Player");
        maxHealth = health;

        if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            spriteRenderer.enabled = true;
            polyCollider.enabled = true;
        }
    }

    void FixedUpdate() {
        if (status == Status.Alive) {
            //Vector3 antlionPos = rigidBody.position;
            Vector3 antlionPos = this.transform.position;
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

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("MeleeSwingCrescent")) {
            GameObject meleePoint = other.gameObject;
            Damage(meleePoint.GetComponent<MeleePointBehavior>().getWeaponDamage());
            meleePoint.GetComponent<MeleePointBehavior>().setWeaponDamage(0);
        }
    }

    private void FightPhaseAttack(Vector3 antlionPos) {
        Vector3 vectorToPlayer = GETVectorToPlayer(antlionPos);
        Quaternion angleToPlayer = GETAngleToPlayer(vectorToPlayer);
        RotateToPlayer(angleToPlayer);

        if (isSpitReady && IsFacingPlayer(angleToPlayer)) {
            SpitSand(antlionPos);
        }
    }

    private void RotateToPlayer(Quaternion transformRotation) {
        float rotation = Mathf.Min(currentRotationSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, transformRotation, rotation);
    }

    private Quaternion GETAngleToPlayer(Vector3 dir) {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion transformRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        return transformRotation;
    }

    private Vector3 GETVectorToPlayer(Vector3 antlionPos) {
        Vector3 playerPos = player.transform.position;
        Vector3 dir = (playerPos - antlionPos).normalized;
        return dir;
    }

    private void SpitSand(Vector3 antlionPos) {
        int numOfShots = Random.Range(6, 11) + LowHealthSpitMore();
        for (int i = 0; i < numOfShots; i++) {
            StartCoroutine(SpitBarrage(antlionPos, i*0.05f));
        }
        isSpitReady = false;
        StartCoroutine(SpitAttackTimer());
    }

    private int LowHealthSpitMore() {
        if (health < 200) {
            return 6;
        } else if (health < 400) {
            return 4;
        } else if (health < 600) {
            return 2;
        } else if (health < 800) {
            return 1;
        }

        return 0;
    }

    IEnumerator SpitBarrage(Vector3 antlionPos, float delay) {
        yield return new WaitForSeconds(delay);
        Quaternion angleToPlayer = GETAngleToPlayer(GETVectorToPlayer(antlionPos));
        float randomAngle = Random.Range(-20, 20);
        angleToPlayer *= Quaternion.Euler(Vector3.forward * randomAngle);
        Vector3 spitSpawnPosition = new Vector3(antlionPos.x, antlionPos.y, 0);
        Vector3 mouthOffset = angleToPlayer * (new Vector3(0, 2.5f, 0));
        spitSpawnPosition = spitSpawnPosition + mouthOffset;
        GameObject sandSpitObject =
            Instantiate(sandSpitPrefab, spitSpawnPosition, Quaternion.identity);
        Rigidbody2D sandSpitRigidbody2D = sandSpitObject.GetComponent<Rigidbody2D>();
        sandSpitRigidbody2D.velocity = angleToPlayer * (new Vector2(0, 8f));
    }

    private bool IsFacingPlayer(Quaternion angleToPlayer) {
        float dotProduct = Mathf.Abs(Quaternion.Dot(angleToPlayer, transform.rotation));
        return dotProduct > 0.99;
    }

    IEnumerator SpitAttackTimer() {
        currentRotationSpeed = baseRotationSpeed;
        yield return new WaitForSeconds(5f);
        isSpitReady = true;
        currentRotationSpeed = spitReadyRotationSpeed;
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
                       status == Status.NotSpawned) {
                StartCoroutine(WakeUpBossPhase());
            }
        }
    }

    IEnumerator WakeUpBossPhase() {
        yield return new WaitForSeconds(2f);
        status = Status.Alive;
    }

    public void Damage(float damage) {
        health -= damage;

        Debug.Log("Hit!");

        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}