using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    [SerializeField]
    private GameObject innerWallPrefab;

    [SerializeField]
    private GameObject tintedWallPrefab;

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
    private int numRows;

    private int boulderRespawnTime = 8;

    private Vector2 screenBounds;
    private enum Direction {
        Left,
        Right,
        Up
    }

    void Start () {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        numRows = 30;
        rows = cameraHeight * numRows;
        columns = 9;

        grid = new int[columns, rows];

        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {
                if (rowIndex < 8 && !isOuterWall (columnIndex, rowIndex)) { // Starting area
                    grid[columnIndex, rowIndex] = 0;
                    generateTile (columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                } else if (isOuterWall (columnIndex, rowIndex)) { // Outer walls
                    grid[columnIndex, rowIndex] = 1;
                    generateTile (columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                } else { // Everything else
                    generateNonEdge (columnIndex, rowIndex);
                }
            }
        }

        StartCoroutine(boulderWave());
        generatePath ();
    }

    void Update() {
        if (player.transform.position.y > (1.85 * rows)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private bool isOuterWall (int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void generateTile (int column, int row, int value) {
        Instantiate (floorTilePrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);

        if (value == 1) {
            Instantiate (outerWallPrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        }
    }

    private void generateNonEdge (int col, int row) {
        int value = Random.Range (0, 6);

        //important to store value for path gen to identify tiles
        grid[col, row] = value;

        Instantiate (floorTilePrefab, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);

        if (value > 3) {
            int sndRand = Random.Range(0, 51);
            GameObject wallType = (sndRand == 0) ? tintedWallPrefab : innerWallPrefab;

            GameObject wall = Instantiate (wallType, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
            wall.name = "InnerTile" + col + ":" + row + "::" + value;
        } else if (value == 0) {
            Instantiate (sandTilePrefab, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
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

    private void generatePath () {
        int rowIndex = 1;
        int columnIndex = Random.Range (1, 8);
        searchPath (columnIndex, rowIndex, Direction.Up);
    }

    private void searchPath (int column, int row, Direction prevDir) {
        while (grid[column, row] == 5) {
            GameObject tile = GameObject.Find ("InnerTile" + column + ":" + row + "::" + 5);
            grid[column, row] = -1;
            Destroy (tile);
        }

        Direction direction = (Direction) Random.Range (0, 3);
        if (canPathLeft (column, prevDir, direction)) {
            column--;
        } else if (canPathRight (column, prevDir, direction)) {
            column++;
        } else {
            row++;
            direction = Direction.Up;
        }

        if (row != rows - 2) {
            searchPath (column, row, direction);
        }
    }

    private bool canPathRight (int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Right) && (column != 7) && (prevDir == Direction.Up);
    }

    private bool canPathLeft (int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Left) && (column != 1) && (prevDir == Direction.Up);
    }
}