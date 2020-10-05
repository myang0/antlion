using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAntlionHealth : MonoBehaviour
{
    private AntlionBehavior antlion;
    public Text txt;

    void Start()
    {
        antlion = GameObject.FindGameObjectWithTag("Antlion").GetComponent<AntlionBehavior>();
    }

    void Update()
    {
        if (antlion != null) {
            txt.text = antlion.health.ToString();
        } else {
            txt.text = "DEAD!";
        }
    }
}
