using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SandSpitBehavior : MonoBehaviour {
    [SerializeField] private GameObject sandTilePrefab;

    void Start() {
    }

    void Update() {
    }

    public void SpawnSandTile() {
        int sandSpawnChance = Random.Range(0, 100);
        if (sandSpawnChance > 50) {
            GameObject floorTile = GETClosestFloorTile();
            Vector3 floorTilePosition = floorTile.transform.position;
            Instantiate(sandTilePrefab, new Vector3(floorTilePosition.x, floorTilePosition.y, 1),
                Quaternion.identity);
            Destroy(floorTile);
        }
        Destroy(this.gameObject);
    }

    private GameObject GETClosestFloorTile() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("FloorTile");
        GameObject closestObject = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (var gObject in gameObjects) {
            Vector3 diff = gObject.transform.position - position;
            float currentDistance = diff.sqrMagnitude;
            if (currentDistance < distance) {
                closestObject = gObject;
                distance = currentDistance;
            }

            if (distance < 0.25) {
                break;
            }
        }

        return closestObject;
    }
}