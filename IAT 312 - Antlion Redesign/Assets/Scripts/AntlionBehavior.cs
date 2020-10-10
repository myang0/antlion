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
    public float baseMovementSpeed = 4;
    public float movementSpeed = 4f;
    public float rageMovementSpeed = 10f;
    private GameObject antlion;
    private GameObject player;
    [SerializeField] private GameObject sandSpitPrefab;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polyCollider;
    public Rigidbody2D rigidBody;
    private bool isAttackReady = true;
    private float currentRotationSpeed = 3f;
    private float baseRotationSpeed = 0.2f;
    private float spitReadyRotationSpeed = 3f;
    public float health = 1000;
    private float maxHealth;
    [SerializeField] private bool isInvulnerable = false;
    private Vector3 antlionPos;
    private const string FightPhaseStr = "FightPhase";
    private const string RunPhaseSceneStr = "RunPhaseScene";

    // Start is called before the first frame update
    void Start() {
        antlion = GameObject.Find("Antlion");
        player = GameObject.Find("Player");
        maxHealth = health;

        if (CompareCurrentSceneTo(FightPhaseStr)) {
            spriteRenderer.enabled = true;
            polyCollider.enabled = true;
        }
    }

    void FixedUpdate() {
        if (status == Status.Alive) {
            antlionPos = this.transform.position;
            if (CompareCurrentSceneTo(RunPhaseSceneStr)) {
                if (player && 
                    !GameObject.FindWithTag("MapManager").GetComponent<MapManager>().isTransitionWallBlocked) {
                    isInvulnerable = false;
                    RunPhaseMovement();
                } else {
                    isInvulnerable = true;
                    RunOffScreen();
                }
            } else if (CompareCurrentSceneTo(FightPhaseStr)) {
                if (player) {
                    FightPhaseAttack();
                } else {
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // if (IsFightPhase()) {
            // if (other.CompareTag("MeleeSwingCrescent")) {
            //     GameObject attackPoint = other.gameObject;
            //     Damage(attackPoint.GetComponent<MeleePointBehavior>().getWeaponDamage());
            // } else if (other.CompareTag("BiteCrescent")) {
            //     GameObject attackPoint = other.gameObject;
            //     Damage(attackPoint.GetComponent<BitePointBehavior>().getWeaponDamage());
            // }
        // }
        if (other.CompareTag("BiteCrescent")) {
            GameObject attackPoint = other.gameObject;
            Damage(attackPoint.GetComponent<BitePointBehavior>().getWeaponDamage());
        }
    }

    private void FightPhaseAttack() {
        Vector3 vectorToPlayer = GETVectorToPlayer();
        Quaternion angleToPlayer = GETAngleToPlayer();
        RotateToPlayer(GETAngleToPlayer());

        if (isAttackReady && IsFacingPlayer(GETAngleToPlayer())) {
            if (Random.Range(0, 100) > 66) {
                SpitStreamStart();
            } else {
                SpitBarrageStart();
            }
        }
    }

    private void RotateToPlayer(Quaternion transformRotation) {
        float rotation = Mathf.Min(currentRotationSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, transformRotation, rotation);
    }

    private Quaternion GETAngleToPlayer() {
        Vector3 vectorToPlayer = GETVectorToPlayer();
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        Quaternion transformRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        return transformRotation;
    }

    private Vector3 GETVectorToPlayer() {
        Vector3 playerPos = player.transform.position;
        Vector3 dir = (playerPos - antlionPos).normalized;
        return dir;
    }

    private void SpitBarrageStart() {
        int numOfShots = Random.Range(6, 11) + LowHealthSpitMore();
        for (int i = 0; i < numOfShots; i++) {
            StartCoroutine(SpitBarrage(i * 0.05f));
        }

        isAttackReady = false;
        StartCoroutine(AttackDelay(6f));
    }
    
    private void SpitStreamStart() {
        int numOfShots = Random.Range(18, 33) + LowHealthSpitMore()*3;
        for (int i = 0; i < numOfShots; i++) {
            StartCoroutine(SpitStream(i * 0.1f));
        }

        isAttackReady = false;
        StartCoroutine(AttackDelay(7.5f));
    }

    private int LowHealthSpitMore() {
        if (health < 200) {
            return 16;
        } else if (health < 400) {
            return 8;
        } else if (health < 600) {
            return 4;
        } else if (health < 800) {
            return 2;
        }

        return 0;
    }

    IEnumerator SpitBarrage(float delay) {
        yield return new WaitForSeconds(delay);
        Quaternion angleToPlayer = GETAngleToPlayer();
        float randomAngle = Random.Range(-20, 20);
        angleToPlayer *= Quaternion.Euler(Vector3.forward * randomAngle);
        Vector3 spitSpawnPosition = GETSpitSpawnPosition(angleToPlayer);
        GameObject sandSpitObject =
            Instantiate(sandSpitPrefab, spitSpawnPosition, Quaternion.identity);
        Rigidbody2D sandSpitRigidbody2D = sandSpitObject.GetComponent<Rigidbody2D>();
        sandSpitRigidbody2D.velocity = angleToPlayer * (new Vector2(0, 8f));
    }

    IEnumerator SpitStream(float delay) {
        yield return new WaitForSeconds(delay);
        Quaternion angleToPlayer = GETAngleToPlayer();
        this.transform.rotation = angleToPlayer;
        float randomAngle = Random.Range(-5, 5);
        angleToPlayer *= Quaternion.Euler(Vector3.forward * randomAngle);
        Vector3 spitSpawnPosition = GETSpitSpawnPosition(angleToPlayer);
        GameObject sandSpitObject =
            Instantiate(sandSpitPrefab, spitSpawnPosition, Quaternion.identity);
        Rigidbody2D sandSpitRigidbody2D = sandSpitObject.GetComponent<Rigidbody2D>();
        sandSpitRigidbody2D.velocity = angleToPlayer * (new Vector2(0, 12f));
    }

    private Vector3 GETSpitSpawnPosition(Quaternion angleToPlayer) {
        Vector3 spitSpawnPosition = new Vector3(antlionPos.x, antlionPos.y, 0);
        Vector3 mouthOffset = angleToPlayer * (new Vector3(0, 2.5f, 0));
        spitSpawnPosition = spitSpawnPosition + mouthOffset;
        return spitSpawnPosition;
    }

    private bool IsFacingPlayer(Quaternion angleToPlayer) {
        float dotProduct = Mathf.Abs(Quaternion.Dot(angleToPlayer, transform.rotation));
        return dotProduct > 0.99;
    }

    IEnumerator AttackDelay(float delay) {
        currentRotationSpeed = baseRotationSpeed;
        yield return new WaitForSeconds(delay);
        isAttackReady = true;
        currentRotationSpeed = spitReadyRotationSpeed;
    }

    private void RunOffScreen() {
        //Player is dead, antlion runs off screen
        Vector3 dir = new Vector3(0f, -2f, 0f);
        rigidBody.MovePosition(antlionPos +
                               dir * (movementSpeed * Time.fixedDeltaTime));
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void RunPhaseMovement() {
        Vector3 playerPos = player.transform.position;
        Vector3 dir = (playerPos - antlionPos).normalized;

        if (Vector3.Distance(playerPos, antlionPos) > 1) {
            //If player is not behind antlion and antlion isn't too far behind
            if (playerPos.y - antlionPos.y > 0 && playerPos.y - antlionPos.y < 10) {
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
        if (col.gameObject.layer == 8) {
            Destroy(col.gameObject);
        } else if (col.gameObject.CompareTag("Boulder")) {
            Damage(20);
            StartCoroutine(BoulderStun());
            Destroy(col.gameObject);
        }

        if (col.gameObject.CompareTag("OuterWall") && CompareCurrentSceneTo(RunPhaseSceneStr)) {
            Physics2D.IgnoreCollision(polyCollider, col.gameObject.GetComponent<Collider2D>());
        }
    }

    private IEnumerator BoulderStun() {
        movementSpeed = baseMovementSpeed / 2;
        rageMovementSpeed = baseMovementSpeed / 2;
        yield return new WaitForSeconds(2f);
        movementSpeed = baseMovementSpeed;
        rageMovementSpeed = baseMovementSpeed * 2;
    }

    void Update() {
        if (player) {
            if (CompareCurrentSceneTo(RunPhaseSceneStr) &&
                (player.transform.position.y > 22) && status == Status.NotSpawned) {
                spriteRenderer.enabled = true;
                polyCollider.enabled = true;
                status = Status.Alive;
            } else if (CompareCurrentSceneTo(FightPhaseStr) &&
                       status == Status.NotSpawned && player.transform.position.y > 1.5f) {
                StartCoroutine(WakeUpBossPhase());
            }
        }
    }

    private bool CompareCurrentSceneTo(string scene) {
        return string.Equals(SceneManager.GetActiveScene().name, scene);
    }

    IEnumerator WakeUpBossPhase() {
        yield return new WaitForSeconds(2f);
        status = Status.Alive;
        isInvulnerable = false;
    }

    public void Damage(float damage) {
        if (!isInvulnerable) {
            Debug.Log("Hit!");
            health -= damage;
            if (health <= 0) {
                Destroy(gameObject);
            }
        } else {
            Debug.Log("Invulnerable!");
        }
        
        
    }
}