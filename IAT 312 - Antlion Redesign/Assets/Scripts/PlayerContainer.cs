using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
