using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TintedWallBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] possibleLoot;

    [SerializeField] private GameObject floorTilePrefab;

    private bool isQuitting = false;

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
        int randIndex = Random.Range(0, possibleLoot.Length);
        Instantiate(possibleLoot[randIndex], transform.position, Quaternion.identity);
        Vector3 floorTilePosition = new Vector3(transform.position.x, transform.position.y, 2);
        Instantiate(floorTilePrefab, floorTilePosition, Quaternion.identity);
    }
}
