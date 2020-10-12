using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WeaponSwingBehavior : MonoBehaviour {
    private GameObject player;
    private PlayerMovement playerMovement;
    private Animator anim;

    void Start() {
        anim = this.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        if (!player) {
            Destroy(this.gameObject);
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.rotationLock = true;
    }

    void Update() {
        gameObject.transform.position = player.transform.position;
        gameObject.transform.rotation = player.transform.rotation;
    }

    void DeleteOnAnimationEnd() {
        playerMovement.rotationLock = false;
        if (gameObject.CompareTag("Crossbow")) {
            playerMovement.shootProjectile(15f);
        }
        Destroy(this.gameObject);
    }
}