using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WeaponSwingBehavior : MonoBehaviour {
    private GameObject player;
    private Animator anim;

    void Start() {
        anim = this.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        if (!player) {
            Destroy(this.gameObject);
        }
    }

    void Update() {
        this.gameObject.transform.position = player.transform.position;
        this.gameObject.transform.rotation = player.transform.rotation;
    }

    void DeleteOnAnimationEnd() {
        Destroy(this.gameObject);
    }
}