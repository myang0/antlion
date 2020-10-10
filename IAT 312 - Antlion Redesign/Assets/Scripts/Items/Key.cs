using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private GameObject particles;

    void OnDestroy() {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
