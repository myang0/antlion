using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnerWallBehaviour : MonoBehaviour {
    [SerializeField]
    private GameObject brokenInnerWallPrefab;

    private bool isQuitting = false;

    void OnApplicationQuit () {
        isQuitting = true;
    }

    void OnDestroy () {
        if (!isQuitting) {
            Instantiate (brokenInnerWallPrefab, transform.position, Quaternion.identity);
        }
    }

    public void pathGenDestroy () {
        isQuitting = true;
        Destroy (this.gameObject);
    }
}