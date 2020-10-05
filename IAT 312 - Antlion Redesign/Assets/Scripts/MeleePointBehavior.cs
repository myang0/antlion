using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MeleePointBehavior : MonoBehaviour {
    private float weaponDamage;

    public void setWeaponDamage(float damageDealt) {
        weaponDamage = damageDealt;
    }

    public float getWeaponDamage() {
        return weaponDamage;
    }
    
    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandSpit")) {
            Destroy(collider.gameObject);
        }
    }
}
