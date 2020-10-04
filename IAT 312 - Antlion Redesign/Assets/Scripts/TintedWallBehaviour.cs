using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintedWallBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleLoot;

    private bool isQuitting = false;

    void OnApplicationQuit() {
        isQuitting = true;
    }

    void OnDestroy() {
        if (!isQuitting) {
            int randIndex = Random.Range(0, possibleLoot.Length);
            Instantiate(possibleLoot[randIndex], transform.position, Quaternion.identity);
        }
    }
}
