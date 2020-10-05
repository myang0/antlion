using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour {
    [SerializeField] private GameObject swordSwing;
    [SerializeField] private GameObject axeSwing;
    [SerializeField] private GameObject crossbowSwing;
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
        if (!col.CompareTag("Player")) return;

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
        Vector3 position = new Vector3(player.transform.position.x, player.transform.position.y,
            player.transform.position.z);
        
        if (this.gameObject.CompareTag("Crossbow")) {
            Instantiate(crossbowSwing, position, Quaternion.identity);
            StartCoroutine(AttackCrossbow());
        } else if (this.gameObject.CompareTag("Axe")) {
            Instantiate(axeSwing, position, Quaternion.identity);
            StartCoroutine(AttackMelee());
        } else if (this.gameObject.CompareTag("Sword")) {
            Instantiate(swordSwing, position, Quaternion.identity);
            StartCoroutine(AttackMelee());
        }

        currentCooldown = baseCooldown;
    }

    IEnumerator AttackCrossbow() {
        yield return new WaitForSeconds(0.1f);
        player.shootProjectile(baseDamage);
    }
    
    IEnumerator AttackMelee() {
        yield return new WaitForSeconds(0.1f);
        player.meleeAttack();
    }
}
