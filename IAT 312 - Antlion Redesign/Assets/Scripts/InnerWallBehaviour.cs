using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerWallBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject brokenInnerWallPrefab;

    void OnDestroy() {
        Instantiate(brokenInnerWallPrefab, transform.position, Quaternion.identity);
    }
}
