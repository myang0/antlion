using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour {
    // [SerializeField] private GameObject swordSwing;
    // [SerializeField] private GameObject axeSwing;
    // [SerializeField] private GameObject crossbowSwing;
    [SerializeField] private GameObject swing;
    private Inventory inventory;
    private PlayerMovement player;

    public SpriteRenderer sprite;

    public Collider2D hitbox;

    public int remainingUses;
    public int baseUses;
    public float baseCooldown;
    private float currentCooldown = 0;

    public float baseDamage;
    // public float attackRange;
    public bool isDropped = true;

    // public bool isRanged;

    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        baseUses = remainingUses;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && isDropped) {
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist < 2f && isDropped) {
                AddToInventory();
            }
        }
    }

    void AddToInventory() {
        if (!player) return;
        
        for (int i = 0; i < inventory.items.Length; i++) {
            if (!inventory.isFull[i]) {
                inventory.selectedItemIndex = inventory.numItemsCarried;
                inventory.selectedItem = gameObject;
                inventory.numItemsCarried += 1;

                inventory.isFull[i] = true;
                inventory.items[i] = gameObject;

                hitbox.enabled = false;
                sprite.enabled = false;
                isDropped = false;

                this.transform.parent = player.gameObject.transform.parent;
                
                i = inventory.items.Length;
            }
        }
    }

    public void Use() {
        if (currentCooldown > 0 || player.rotationLock) return;
        
        Vector3 playerPosition = player.transform.position;
        RotatePlayerToAttack(playerPosition);

        remainingUses--;
        Vector3 position = new Vector3(playerPosition.x, playerPosition.y,
            playerPosition.z);

        GameObject weapon = new GameObject();
        // if (this.gameObject.CompareTag("Crossbow")) {
        //     weapon = Instantiate(crossbowSwing, position, Quaternion.identity);
        //     StartCoroutine(AttackCrossbow());
        // } else if (this.gameObject.CompareTag("Axe")) {
        //     weapon = Instantiate(axeSwing, position, Quaternion.identity);
        // } else if (this.gameObject.CompareTag("Sword")) {
        //     weapon = Instantiate(swordSwing, position, Quaternion.identity);
        // }
        
        weapon = Instantiate(swing, position, Quaternion.identity);
        // if (this.gameObject.CompareTag("Crossbow")) {
        //     StartCoroutine(AttackCrossbow());
        // } 

        weapon.transform.parent = player.gameObject.transform.parent;
        if (weapon.CompareTag("MeleeSwing")) {
            weapon.GetComponent<MeleeSwingBehavior>().baseDamage = this.baseDamage;
            weapon.GetComponent<MeleeSwingBehavior>().damageMultiplier = player.attackMultiplier;
        }
        currentCooldown = baseCooldown;
        StartCoroutine(AttackCooldown(baseCooldown));
    }

    private void RotatePlayerToAttack(Vector3 playerPosition) {
        Camera cam = Camera.main;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 vectorToMouse = (playerPosition - mousePos).normalized;
        float angle = Mathf.Atan2(vectorToMouse.y, vectorToMouse.x) * Mathf.Rad2Deg;
        Quaternion angleToMouse = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        player.transform.rotation = angleToMouse;
    }

    IEnumerator AttackCrossbow() {
        player.rotationLock = true;
        yield return new WaitForSeconds(0.1f);
        player.shootProjectile(baseDamage);
        player.rotationLock = false;
    }
    
    IEnumerator AttackCooldown(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        currentCooldown = 0;
    }
}
