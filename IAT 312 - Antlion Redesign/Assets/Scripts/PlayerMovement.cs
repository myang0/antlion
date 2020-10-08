using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    public float baseMovementSpeed = 5f;
    [SerializeField] private float currentMovementSpeed = 5f;
    [SerializeField] private GameObject bitePoint;
    [SerializeField] private SpriteRenderer bite;

    public float attackMultiplier = 1f;

    public Rigidbody2D rigidBody;

    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vcamNoise;

    public Transform attackPoint;
    public Transform firePoint;
    public Transform meleePoint;

    public float attackRange = 0.5f;

    public LayerMask wallLayers;
    public LayerMask antlionLayer;

    public bool isStunned = false;

    Vector2 movement;
    Vector2 kbVector;

    public float kbTimer = 0f;

    public float health = 100;

    public bool shielded = false;
    public float shieldedSpeed = 7.5f;
    public bool rotationLock = false;

    public GameObject projectilePrefab;

    void Start() {
        currentMovementSpeed = baseMovementSpeed;

        // SceneManager.activeSceneChanged += sceneTransition;
        SetupCameraNoise();
    }

    void Update() {
        if (Time.timeScale != 0) {
            if (!isStunned) {
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
            }

            // if (Input.GetKeyDown(KeyCode.Space)) Attack();
            Inventory inventory = this.gameObject.GetComponent<Inventory>();
            if (Input.GetMouseButtonDown(1) && !rotationLock) {
                StartCoroutine(ShowBite());
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                MapManager mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
                Vector3 newPosition = new Vector3(transform.position.x, mapManager.getEndOfRunMap(),
                    transform.position.z);
                transform.position = newPosition;
                Debug.Log("test");
            }

            if (kbTimer > 0) {
                rigidBody.AddForce(kbVector * (kbTimer * 0.3f), ForceMode2D.Force);
                kbTimer--;
                isStunned = (kbTimer != 0);
            }

            if (health < 0) {
                vcamNoise.m_FrequencyGain = 0f;
                Destroy(this.gameObject);
            }
        }
    }

    void FixedUpdate() {
        // enable self stun on attack
        // if (rotationLock) return;
        
        if (!shielded ) {
            rigidBody.MovePosition(rigidBody.position +
                                   movement * (currentMovementSpeed * Time.fixedDeltaTime));
        } else {
            rigidBody.MovePosition(rigidBody.position +
                                   movement * (shieldedSpeed * Time.fixedDeltaTime));
        }

        RotateAnt();
    }

    public void ApplyBuffs(float healthBoost, float attackBoost, float speedBoost) {
        baseMovementSpeed *= speedBoost;
        currentMovementSpeed = baseMovementSpeed;

        attackMultiplier *= attackBoost;

        health += healthBoost;
    }

    private void RotateAnt() {
        if ((movement.x != 0 || movement.y != 0) && !rotationLock) {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion rotationAngle = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = rotationAngle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandTile")) {
            currentMovementSpeed = baseMovementSpeed / 2;
        } else if (cName.Contains("BrokenWallTile")) {
            currentMovementSpeed = baseMovementSpeed / 3.5f;
        } else if (cName.Contains("SandSpit")) {
            collider.GetComponent<SandSpitBehavior>().SpawnSandTile();
            TakeDamage(10, 15);
        } else if (collider.CompareTag("FloorTile")) {
            currentMovementSpeed = baseMovementSpeed;
        }
    }

    void OnDrawGizmosSelected() {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (!shielded) {
            if (col.gameObject.CompareTag("Boulder")) {
                Rigidbody2D boulderRb = col.gameObject.GetComponent<Rigidbody2D>();
                Vector2 difference = boulderRb.transform.position - transform.position;
                difference = -difference.normalized;
                kbVector = difference;
                kbTimer = 120;

                isStunned = true;
            } else if (col.gameObject == GameObject.Find("Antlion")) {
                if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
                    TakeDamage(15, 20);
                } else {
                    TakeDamage(5, 10);
                }
            }
        }
    }

    private void TakeDamage(int minDamage, int maxDamage) {
        if (shielded) return;
        int damageTaken = Random.Range(minDamage, maxDamage);
        health = health - damageTaken;
        kbTimer = 0;
        isStunned = false;
        StartCoroutine(ActivateShield());
        StartCoroutine(ActivateScreenShake());
    }

    IEnumerator ActivateScreenShake() {
        if (!vcam || !vcamNoise) {
            SetupCameraNoise();
        }
        vcamNoise.m_AmplitudeGain = 7f;
        vcamNoise.m_FrequencyGain = 4f;
        yield return new WaitForSeconds(0.5f);
        vcamNoise.m_FrequencyGain = 0f;
        Debug.Log("Shakey!");
        Debug.Log(vcam + "::" + vcamNoise);
    }

    IEnumerator ActivateShield() {
        shielded = true;
        isStunned = false;
        yield return new WaitForSeconds(1.5f);
        shielded = false;
    }

    IEnumerator ShowBite() {
        rotationLock = true;
        transform.rotation = GETAngleToMouse();
        bitePoint.SetActive(!bitePoint.activeInHierarchy);
        if (bitePoint.activeInHierarchy) {
            bitePoint.GetComponent<BitePointBehavior>().setWeaponDamage(10f * attackMultiplier);
        }

        // bite.enabled = true;
        yield return new WaitForSeconds(0.1f);
        // bite.enabled = false;
        bitePoint.SetActive(!bitePoint.activeInHierarchy);
        rotationLock = false;
    }

    private Quaternion GETAngleToMouse() {
        Camera cam = Camera.main;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 vectorToMouse = (transform.position - mousePos).normalized;
        float angle = Mathf.Atan2(vectorToMouse.y, vectorToMouse.x) * Mathf.Rad2Deg;
        Quaternion angleToMouse = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        return angleToMouse;
    }

    public Vector2 GETMovement() {
        return movement;
    }

    public bool GETShielded() {
        return shielded;
    }

    public void shootProjectile(float baseDmg) {
        GameObject projectile =
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        // rb.AddForce(firePoint.up * 20f, ForceMode2D.Impulse);
        
        rb.velocity = GETAngleToMouse() * new Vector2(0, 25f);

        Projectile p = projectile.GetComponent<Projectile>();
        p.damage = baseDmg * attackMultiplier;
    }

    public void SceneTransition() {
        currentMovementSpeed = baseMovementSpeed;
        kbTimer = 0;
        isStunned = false;
        transform.position = new Vector3(14, 1.5f, 0);
        // SetupCameraNoise();
    }

    private void SetupCameraNoise() {
        GameObject vcamObject = GameObject.FindWithTag("VirtualCam");
        vcam = vcamObject.GetComponent<CinemachineVirtualCamera>();
        vcamNoise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void showSwingCrescent(float baseDamage) {
        GameObject meleePointGameObject = meleePoint.gameObject;
        meleePointGameObject.SetActive(!meleePointGameObject.activeInHierarchy);
        if (meleePointGameObject.activeInHierarchy) {
            meleePointGameObject.GetComponent<MeleePointBehavior>()
                .setWeaponDamage(baseDamage * attackMultiplier);
        }
    }
}