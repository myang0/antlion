using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitePointBehavior : MonoBehaviour {
    private float weaponDamage;

    public void setWeaponDamage(float damageDealt) {
        weaponDamage = damageDealt;
    }

    public float getWeaponDamage() {
        return weaponDamage;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        string cName = other.name;
        if (cName.Contains("SandSpit") || other.gameObject.CompareTag("InnerWall") ||
            other.gameObject.CompareTag("TintedWall")) {
            Destroy(other.gameObject);
        }
    }
}