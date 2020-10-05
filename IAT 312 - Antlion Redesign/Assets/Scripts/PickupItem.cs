using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour {
    [SerializeField] private GameObject swordSwing;
    [SerializeField] private GameObject axeSwing;
    [SerializeField] private GameObject crossbowSwing;
    private Inventory inventory;
    private PlayerMovement player;

    public SpriteRenderer sprite;

    [SerializeField]
    private BoxCollider2D hitbox;

    public int remainingUses;
    public float baseCooldown;
    private float currentCooldown = 0;

    public float baseDamage;
    public float attackRange;

    public bool isRanged;

    void Start() {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update() {
        // if (currentCooldown > 0) currentCooldown--;
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
        if (currentCooldown > 0 || player.rotationLock) return;
        
        Vector3 playerPosition = player.transform.position;
        RotatePlayerToAttack(playerPosition);

        remainingUses--;
        Vector3 position = new Vector3(playerPosition.x, playerPosition.y,
            playerPosition.z);
        
        if (this.gameObject.CompareTag("Crossbow")) {
            Instantiate(crossbowSwing, position, Quaternion.identity);
            StartCoroutine(AttackCrossbow());
        } else if (this.gameObject.CompareTag("Axe")) {
            Instantiate(axeSwing, position, Quaternion.identity);
            StartCoroutine(AttackAxe());
        } else if (this.gameObject.CompareTag("Sword")) {
            Instantiate(swordSwing, position, Quaternion.identity);
            StartCoroutine(AttackSword());
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
    
    IEnumerator AttackSword() {
        player.rotationLock = true;
        player.showSwingCrescent(baseDamage);
        yield return new WaitForSeconds(0.1f);
        // player.meleeAttack(attackRange, baseDamage);
        player.showSwingCrescent(baseDamage);
        player.rotationLock = false;
    }
    IEnumerator AttackAxe() {
        player.rotationLock = true;
        player.showSwingCrescent(baseDamage);
        yield return new WaitForSeconds(0.15f);
        // player.meleeAttack(attackRange, baseDamage);
        player.showSwingCrescent(baseDamage);
        player.rotationLock = false;
    }

    IEnumerator AttackCooldown(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        currentCooldown = 0;
    }
}
