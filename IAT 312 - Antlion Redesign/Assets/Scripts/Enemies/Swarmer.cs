using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float speed;

    [SerializeField] private Rigidbody2D rb;

    private GameObject player;

    private bool isMovingToPlayer;
    private bool isStunned;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        isMovingToPlayer = true;
        isStunned = false;
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 swarmerPos = transform.position;

        if (playerDist(playerPos, swarmerPos) < 5f) {
            StartCoroutine(lockMovement());
        }

        if (isMovingToPlayer && !isStunned) {
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
        Vector3 vectorToPlayer = (swarmerPos - playerPos).normalized;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        Quaternion angleToPlayer = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleToPlayer;
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

        if (health <= 0) {
            Destroy(gameObject);
        } else {
            inflictKnockback();
        }
    }

    IEnumerator lockMovement() {
        isMovingToPlayer = false;
        yield return new WaitForSeconds(1.5f);
        isMovingToPlayer = true;
    }

    IEnumerator stunSwarmer() {
        isStunned = true;
        yield return new WaitForSeconds(1.5f);
        isStunned = false;
    }
}
