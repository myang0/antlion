using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour {
    [SerializeField]
    private GameObject player;
    private Vector2 movement;
    private Rigidbody2D rigidBody;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    [SerializeField]

    private float shieldBrightness;
    [SerializeField]
    private bool state = false;

    void Start () {
        playerMovement = player.GetComponent<PlayerMovement> ();
        spriteRenderer = this.GetComponent<SpriteRenderer> ();
    }

    void Update () { }

    private void FixedUpdate () {
        movement = playerMovement.GETMovement ();
        rigidBody = playerMovement.rigidBody;
        if (player) {
            MoveShield ();
            RotateShield ();
            DisplayShield ();
        } else {
            Destroy(this.gameObject);
        }
    }

    private void DisplayShield () {
        if (!state && playerMovement.GETShielded ()) {
            shieldBrightness = 1f;
            state = true;
        } else if (state) {
            shieldBrightness = shieldBrightness - 0.01f;
            spriteRenderer.color = new Color (255, 255, 255, shieldBrightness);

            if (!playerMovement.GETShielded ()) {
                shieldBrightness = 0f;
                state = false;
                spriteRenderer.color = new Color (255, 255, 255, shieldBrightness);
            }
        }
    }

    private void RotateShield () {
        if ((movement.x != 0 || movement.y != 0)) {
            float angle = Mathf.Atan2 (movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion rotationAngle = Quaternion.AngleAxis (angle - 90, Vector3.forward);
            transform.rotation = rotationAngle;
        }
    }

    private void MoveShield () {
        float xOffset = 0;
        float yOffset = 0;
        if (movement.x > 0) {
            xOffset = 0.1f;
        } else if (movement.x < 0) {
            xOffset = -0.1f;
        }
        if (movement.y > 0) {
            yOffset = 0.15f;
        } else if (movement.y < 0) {
            yOffset = -0.15f;
        }

        this.transform.position = new Vector3 (rigidBody.position.x + xOffset,
            rigidBody.position.y + yOffset, 0);
    }
}