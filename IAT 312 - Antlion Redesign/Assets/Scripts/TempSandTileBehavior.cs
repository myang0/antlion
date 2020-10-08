using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSandTileBehavior : MonoBehaviour {
    [SerializeField] private SpriteRenderer renderer;
    public GameObject floorTilePrefab;
    public GameObject fakeFloorTilePrefab;
    private bool expired = false;
    private Vector3 tilePosition;
    private GameObject fakeFloorTile;

    void Start() {
        tilePosition = this.transform.position;
        fakeFloorTile = Instantiate(fakeFloorTilePrefab, new Vector3(tilePosition.x, tilePosition.y, 2),
            Quaternion.identity);
        StartCoroutine(Dissipate());
    }

    private void OnDestroy() {
        if (expired) {
            Instantiate(floorTilePrefab, new Vector3(tilePosition.x, tilePosition.y, 1),
                Quaternion.identity);
        }
    }

    private IEnumerator Dissipate() {
        for (float timeLeft = 5; timeLeft > 0; timeLeft -= Time.deltaTime) {
            var color = renderer.color;
            color = new Color(color.r, color.g, color.b, 0.5f + 0.1f * timeLeft);
            
            renderer.color = color;
            yield return null;
        }

        expired = true;
        Destroy(this.fakeFloorTile.gameObject);
        Destroy(this.gameObject);
    }
}
