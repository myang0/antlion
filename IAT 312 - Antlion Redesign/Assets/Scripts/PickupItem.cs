using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private Inventory inventory;
    private PlayerMovement player;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private BoxCollider2D hitbox;

    public int remainingUses;
    public int baseCooldown;
    private int currentCooldown = 0;

    public float baseDamage;
    public float attackRange;

    public bool isRanged;

    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update() {
        if (currentCooldown > 0) currentCooldown--;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag != "Player") return;

        for (int i = 0; i < inventory.items.Length; i++) {
            if (!inventory.isFull[i]) {
                inventory.selectedItemIndex = inventory.numItemsCarried;
                inventory.selectedItem = gameObject;
                inventory.numItemsCarried += 1;

                inventory.isFull[i] = true;
                inventory.items[i] = gameObject;

                hitbox.enabled = false;
                sprite.enabled = false;

                DontDestroyOnLoad(gameObject);
                
                i = inventory.items.Length;
            }
        }
}

    public void Use() {
        if (currentCooldown > 0) return;

        remainingUses--;

        if (isRanged) {
            player.shootProjectile(baseDamage);
        } else {
            player.meleeAttack();
        }

        currentCooldown = baseCooldown;
    }
}
