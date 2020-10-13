using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBarBehavior : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject hpBar;

    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (!playerMovement) {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            slider.maxValue = playerMovement.maxHealth;
        } else {
            slider.value = playerMovement.health;
        }
    }
}