﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TintedWallBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleLoot;

    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallBreakFxPrefab;
    [SerializeField] private GameObject brickEffects;

    private bool isEquipmentSpawn = false;
    private bool isPassiveSpawn = false;
    private bool isQuitting = false;
    private bool isRestarting = false;
    private int randIndex = 0;

    private void Start() {
        randIndex = Random.Range(0, possibleLoot.Length);
    }

    void OnApplicationQuit() {
        isQuitting = true;
    }

    void OnDestroy() {
        if (isQuitting || isRestarting) return;
        if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
            GameObject mapManager = GameObject.Find("MapManager");
            if (mapManager && !mapManager.GetComponent<MapManager>().isSceneOver) {
                SpawnItem();
                Instantiate(brickEffects, transform.position, Quaternion.identity);
            }
        } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            SpawnItem();
            Instantiate(brickEffects, transform.position, Quaternion.identity);
        }
    }

    private void SpawnItem() {
        if (isEquipmentSpawn) {
            randIndex = Random.Range(0, 6);
        } else if (isPassiveSpawn) {
            randIndex = Random.Range(6, 9);
        }
        Vector3 floorTilePosition = new Vector3(transform.position.x, transform.position.y, 1);
        Instantiate(floorTilePrefab, floorTilePosition, Quaternion.identity);
        Vector3 itemPosition = new Vector3(transform.position.x, transform.position.y, 0);
        Instantiate(possibleLoot[randIndex], itemPosition, Quaternion.identity);
        Instantiate(wallBreakFxPrefab);
    }

    public void ForceEquipmentSpawn() {
        isEquipmentSpawn = true;
    }
    
    public void ForcePassiveSpawn() {
        isPassiveSpawn = true;
    }

    public void SetRestarting() {
        isRestarting = true;
    }
    
    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandSpit")) {
            collider.GetComponent<SandSpitBehavior>().SpawnSandTile();
        }
    }
}
