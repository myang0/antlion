using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
