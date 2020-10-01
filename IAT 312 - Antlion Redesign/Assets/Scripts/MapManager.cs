using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    [SerializeField]
    private GameObject innerWallPrefab;

    [SerializeField]
    private GameObject floorTilePrefab;

    [SerializeField]
    private GameObject sandTilePrefab;

    [SerializeField]
    private GameObject outerWallPrefab;

    [SerializeField]
    private GameObject boulderPrefab;

    [SerializeField]
    private GameObject player;

    public int[, ] grid;
    public Sprite sprite;
    private int cameraHeight, cameraWidth;

    private int rows, columns;

    private int boulderRespawnTime = 8;

    private Vector2 screenBounds;

    void Start () {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        rows = cameraHeight * 25;
        columns = 9;

        grid = new int[columns, rows];

        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {
                if (rowIndex < 8 && !isOuterWall(columnIndex, rowIndex)) { // Starting area
                    grid[columnIndex, rowIndex] = 0;
                    generateTile(columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                } else if (isOuterWall(columnIndex, rowIndex)) { // Outer walls
                    grid[columnIndex, rowIndex] = 1;
                    generateTile(columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                } else { // Everything else
                    generateNonEdge(columnIndex, rowIndex);
                }
            }
        }

        StartCoroutine(boulderWave());
    }

    private bool isOuterWall(int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void generateTile (int column, int row, int value) {
        Instantiate(floorTilePrefab, new Vector3(column * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);

        if (value == 1) {
            Instantiate(outerWallPrefab, new Vector3(column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        }
    }

    private void generateNonEdge(int col, int row)
    {
        int value = Random.Range(0, 6);

        Instantiate(floorTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);

        if (value == 5) {
            GameObject wall = Instantiate(innerWallPrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
            wall.name = "InnerTile" + col + ":" + row + "::" + value;
        } else if (value == 0) {
            Instantiate(sandTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        } else {

        }
    }

    private void spawnBoulder() {
        float playerYPos = player.transform.position.y;
        Instantiate(
            boulderPrefab,
            new Vector3(Random.Range(-screenBounds.x * 0.75f, screenBounds.x * 0.75f), playerYPos + (screenBounds.y * 4), 0), 
            Quaternion.identity
        );

        boulderRespawnTime = Random.Range(8, 13);
    }

    IEnumerator boulderWave() {
        while (true) {
            yield return new WaitForSeconds(boulderRespawnTime);
            spawnBoulder();
        }
    }
}