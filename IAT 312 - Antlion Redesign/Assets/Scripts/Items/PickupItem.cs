using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private Inventory inventory;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private BoxCollider2D hitbox;

    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            for (int i = 0; i < inventory.items.Length; i++) {
                if (!inventory.isFull[i]) {
                    inventory.isFull[i] = true;
                    inventory.items[i] = gameObject;

                    hitbox.enabled = false;
                    sprite.enabled = false;
                    
                    i = inventory.items.Length;
                }
            }
        }
    }
}
