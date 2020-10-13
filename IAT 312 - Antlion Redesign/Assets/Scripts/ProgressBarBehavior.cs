using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressBarBehavior : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMax(float maxDist) {
        slider.maxValue = maxDist;
        slider.value = 0;
    }

    public void SetDist(float dist) {
        slider.value = (dist < 0) ? 0 : dist;
    }

    void Update() {
        if (SceneManager.GetActiveScene().name == "FightPhase") {
            Destroy(gameObject);
        }
    }
}
