using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    [SerializeField] private GameObject item;

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            GameObject orbital = GameObject.FindGameObjectWithTag("Orbital");
            if (orbital == null) {
                Instantiate(item, transform.position, Quaternion.identity);
            } else {
                Orbital o = orbital.GetComponent<Orbital>();
                o.Upgrade();
            }

            Destroy(gameObject);
        }
    }
}
