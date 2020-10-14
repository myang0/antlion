using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySetting : MonoBehaviour {
    [SerializeField] private Toggle toggle;
    public bool isHardMode;
    
    void Update()
    {
        if (toggle) {
            isHardMode = !toggle.isOn;
        }
    }
}
