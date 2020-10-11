using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItem : MonoBehaviour {
    [SerializeField] private GameObject boostParticles;

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

            Instantiate(boostParticles, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}