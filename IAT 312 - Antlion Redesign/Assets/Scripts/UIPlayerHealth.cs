using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    private PlayerMovement player;
    public Text txt;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (player != null) {
            txt.text = player.health.ToString();
        } else {
            txt.text = "DEAD!";
        }
    }
}
