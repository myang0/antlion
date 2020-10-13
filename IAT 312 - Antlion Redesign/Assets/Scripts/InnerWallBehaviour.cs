using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class InnerWallBehaviour : MonoBehaviour {
    [SerializeField] private GameObject brokenInnerWallPrefab;
    [SerializeField] private GameObject brickEffects;
    [SerializeField] private GameObject breakFX;
    
    private bool isQuitting = false;
    private bool isRestarting = false;

    void OnApplicationQuit() {
        isQuitting = true;
    }

    void OnDestroy() {
        if (isQuitting || isRestarting) return;
        if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
            GameObject mapManager = GameObject.Find("MapManager");
            if (mapManager && !mapManager.GetComponent<MapManager>().isSceneOver) {
                Instantiate(brokenInnerWallPrefab, transform.position, Quaternion.identity);
                Instantiate(brickEffects, transform.position, Quaternion.identity);
                Instantiate(breakFX);
            }
        } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            GameObject mapManager = GameObject.Find("FightMapManager");
            Instantiate(brokenInnerWallPrefab, transform.position, Quaternion.identity);
            Instantiate(brickEffects, transform.position, Quaternion.identity);
        }
    }

    public void SetRestarting() {
        isRestarting = true;
    }

    public void PathGenDestroy() {
        isQuitting = true;
        Destroy(this.gameObject);
    }
}