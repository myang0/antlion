using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItem : MonoBehaviour
{
    private PlayerMovement player;

    [SerializeField]
    private float healthBoost;

    [SerializeField]
    private float attackBoost;

    [SerializeField]
    private float speedBoost;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            player.ApplyBuffs(healthBoost, attackBoost, speedBoost);

            Destroy(gameObject);
        }
    }
}
