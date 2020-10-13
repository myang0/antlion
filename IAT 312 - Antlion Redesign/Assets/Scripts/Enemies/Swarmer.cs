using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float speed;

    [SerializeField] private Rigidbody2D rb;

    private GameObject player;

    private bool isLocked;
    private bool isStunned;
    private bool isFleeing;

    private AudioSource audio;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        isLocked = false;
        isStunned = false;
        isFleeing = false;

        audio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 swarmerPos = transform.position;

        if (playerDist(playerPos, swarmerPos) < 7.5f) {
            StartCoroutine(lockMovement());
        }

        if (IsMovingToPlayer()) {
            moveToPlayer(playerPos, swarmerPos);
            rotateToPlayer(playerPos, swarmerPos);
        }

        if (isStunned) transform.Rotate(0, 0, 3);
    }

    void moveToPlayer(Vector3 playerPos, Vector3 swarmerPos) {
        Vector3 vectorToPlayer = (playerPos - swarmerPos).normalized;
        rb.velocity = vectorToPlayer * speed;
    }

    void inflictKnockback() {
        Vector3 playerPos = player.transform.position;
        Vector3 swarmerPos = transform.position;

        Vector3 vectorAwayFromPlayer = (swarmerPos - playerPos).normalized;

        rb.velocity = vectorAwayFromPlayer * speed;

        StartCoroutine(stunSwarmer());
    } 

    void rotateToPlayer(Vector3 playerPos, Vector3 swarmerPos) {
        // Vector3 vectorToPlayer = (swarmerPos - playerPos).normalized;
        // float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        // Quaternion angleToPlayer = Quaternion.AngleAxis(angle, Vector3.forward);
        // transform.rotation = angleToPlayer;
        Vector3 vectorToPlayer = (playerPos - swarmerPos).normalized;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        Quaternion transformRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = transformRotation;
    }

    float playerDist(Vector3 playerPos, Vector3 swarmerPos) {
        return Vector3.Distance(playerPos, swarmerPos);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("BiteCrescent")) {
            GameObject attackPoint = other.gameObject;
            inflictDamage(attackPoint.GetComponent<BitePointBehavior>().getWeaponDamage());
        }
    }

    public void inflictDamage(float damage) {
        health -= damage;
        audio.Play();

        if (health <= 0) {
            Destroy(gameObject);
        } else {
            inflictKnockback();
        }
    }

    public void SetFleeing() {
        Vector3 playerPos = player.transform.position;
        Vector3 swarmerPos = transform.position;

        Vector3 vectorAwayFromPlayer = (swarmerPos - playerPos).normalized;
        rb.velocity = vectorAwayFromPlayer * speed;

        Vector3 vectorToPlayer = (swarmerPos - playerPos).normalized;
        float angle = -Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        Quaternion angleAwayFromPlayer = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleAwayFromPlayer;

        isFleeing = true;

        StartCoroutine(DestroyOnTimer());
    }

    private bool IsMovingToPlayer() {
        return (!isFleeing && !isLocked && !isStunned);
    }

    IEnumerator lockMovement() {
        isLocked = true;
        yield return new WaitForSeconds(1.5f);
        isLocked = false;
    }

    IEnumerator stunSwarmer() {
        isStunned = true;
        yield return new WaitForSeconds(1.5f);
        isStunned = false;
    }

    IEnumerator DestroyOnTimer() {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
