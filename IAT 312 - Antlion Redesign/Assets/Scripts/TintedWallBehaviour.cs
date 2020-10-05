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
    private bool isEquipmentSpawn = false;
    private bool isQuitting = false;
    private int randIndex = 0;

    private void Start() {
        randIndex = Random.Range(0, possibleLoot.Length);
    }

    void OnApplicationQuit() {
        isQuitting = true;
    }

    void OnDestroy() {
        if (isQuitting) return;
        if (string.Equals(SceneManager.GetActiveScene().name, "RunPhaseScene")) {
            GameObject mapManager = GameObject.Find("MapManager");
            if (mapManager && !mapManager.GetComponent<MapManager>().isSceneOver) {
                SpawnItem();
            }
        } else if (string.Equals(SceneManager.GetActiveScene().name, "FightPhase")) {
            SpawnItem();
        }
    }

    private void SpawnItem() {
        if (isEquipmentSpawn) {
            randIndex = Random.Range(0, 3);
        }

        Instantiate(possibleLoot[randIndex], transform.position, Quaternion.identity);
        Vector3 floorTilePosition = new Vector3(transform.position.x, transform.position.y, 2);
        Instantiate(floorTilePrefab, floorTilePosition, Quaternion.identity);
    }

    public void ForceEquipmentSpawn() {
        isEquipmentSpawn = true;
    }
    
    private void OnTriggerEnter2D (Collider2D collider) {
        string cName = collider.name;
        if (cName.Contains("SandSpit")) {
            collider.GetComponent<SandSpitBehavior>().SpawnSandTile();
        }
    }
}