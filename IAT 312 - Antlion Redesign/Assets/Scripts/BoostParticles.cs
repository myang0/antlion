using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostParticles : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}
