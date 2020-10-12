using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarBehavior : MonoBehaviour {
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject hpBar;

    private AntlionBehavior antlionBehavior;
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (!antlionBehavior) {
            antlionBehavior = GameObject.FindGameObjectWithTag("Antlion").GetComponent<AntlionBehavior>();
            slider.maxValue = antlionBehavior.maxHealth;
        } else {
            slider.value = antlionBehavior.health;
        }

        hpBar.SetActive(antlionBehavior.status != AntlionBehavior.Status.NotSpawned);
    }
}
