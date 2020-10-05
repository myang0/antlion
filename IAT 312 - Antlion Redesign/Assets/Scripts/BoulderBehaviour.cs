﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer == 8 && !col.gameObject.CompareTag("TintedWall")) {
            Destroy(col.gameObject);
        }
    }
}