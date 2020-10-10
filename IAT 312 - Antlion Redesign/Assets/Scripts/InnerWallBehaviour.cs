using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class InnerWallBehaviour : MonoBehaviour {
    [SerializeField] private GameObject brokenInnerWallPrefab;
    [SerializeField] private GameObject brickEffects;
    
    private bool isQuitting = false;

    void OnApplicationQuit() {
        isQuitting = true;
    }

    void OnDestroy() {
        if (isQuitting) return;
        if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
            GameObject mapManager = GameObject.Find("MapManager");
            if (mapManager && !mapManager.GetComponent<MapManager>().isSceneOver) {
                Instantiate(brokenInnerWallPrefab, transform.position, Quaternion.identity);
                Instantiate(brickEffects, transform.position, Quaternion.identity);
            }
        } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            GameObject mapManager = GameObject.Find("FightMapManager");
            Instantiate(brokenInnerWallPrefab, transform.position, Quaternion.identity);
            Instantiate(brickEffects, transform.position, Quaternion.identity);
        }
    }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     throw new NotImplementedException();
    // }

    public void PathGenDestroy() {
        isQuitting = true;
        Destroy(this.gameObject);
    }
}