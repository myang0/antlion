using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItem : MonoBehaviour {
    // [SerializeField]
    // private PlayerMovement player;

    [SerializeField] private float healthBoost;

    [SerializeField] private float attackBoost;

    [SerializeField] private float speedBoost;

    void Start() {
        // player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            col.gameObject.GetComponent<PlayerMovement>()
                .ApplyBuffs(healthBoost, attackBoost, speedBoost);

            Destroy(gameObject);
        }
    }
}