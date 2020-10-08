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

    [SerializeField]
    private GameObject cameraBoundry;

    public bool isSceneOver = false;

    public int[,] grid;
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
    
    private enum Tile {Floor, OuterWall, Sand, TintedWall, InnerWall}

    private int wallSpawnThreshold = 3;

    void Start () {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;
        screenBounds = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, Camera.main.transform.position.z));
        numRows = 30;
        // rows = cameraHeight * numRows;
        rows = 150;
        columns = 18;
        grid = new int[columns,rows];

        SetupCameraBoundry();
        GenerateMap();
        // GenerateEntrance();
        StartCoroutine (BoulderWave ());
        GeneratePath ();
    }
    
    void Update () {
        if (player) {
            if (player.transform.position.y > (1.9 * rows)) {
                isSceneOver = true;
                SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
                player.GetComponent<PlayerMovement>().SceneTransition();
            }
        }
    }

    public float getEndOfRunMap() {
        return 1.95f * rows;
    }

    private void GenerateMap() {
        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {
                if (rowIndex < 4 && !IsOuterWall(columnIndex, rowIndex)) {
                    // Starting area
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);
                } else if (isEntranceOrExit(columnIndex, rowIndex)) {
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);
                } else if (IsOuterWall(columnIndex, rowIndex)) {
                    // Outer walls
                    GenerateTile(columnIndex, rowIndex, Tile.OuterWall);
                } else if (rowIndex == rows - 5 || rowIndex == 4) {
                    if (columnIndex % 2 == 0) {
                        GenerateTile(columnIndex, rowIndex, Tile.Floor);
                    } else {
                        GameObject antiAntlionOuterWall = Instantiate (outerWallPrefab,
                            new Vector3 (columnIndex * 2 - 8,
                                rowIndex * 2 - (float) 6.5, 1), Quaternion.identity);
                        antiAntlionOuterWall.tag = "Untagged";
                    }
                }  else if (rowIndex > rows - 6) {
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);
                } else {
                    // Everything else
                    GenerateNonEdge(columnIndex, rowIndex);
                }
            }
        }
    }

    private void GenerateEntrance() {
        
    }

    private bool isEntranceOrExit(int columnIndex, int rowIndex) {
        return (columnIndex == (columns / 2) - 1 || columnIndex == columns / 2) &&
               (rowIndex == 0 || rowIndex == rows - 1);
    }

    private void SetupCameraBoundry() {
        float cameraOffset = (float) 7.5;
        grid = new int[columns, rows];
        cameraBoundry.transform.position = new Vector3(columns * 0.5f, rows - cameraOffset, 0);
        cameraBoundry.GetComponent<BoxCollider>().size = new Vector3(columns * 2, rows * 2, 1);
    }

    private bool IsOuterWall (int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void GenerateTile (int column, int row, Tile value) {
        // grid[column, row] = value;
        // Instantiate (floorTilePrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);

        if (value == Tile.OuterWall) {
            Instantiate (outerWallPrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        } else if (value == Tile.Sand) {
            Instantiate (sandTilePrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        } else if (value == Tile.Floor) {
            Instantiate (floorTilePrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);
        }
    }

    private void GenerateNonEdge (int col, int row) {
        int value = Random.Range (0, 10);
        int randomRotation = Random.Range(0, 4);
        // int value = wallSpawnThreshold + 1;

        //important to store value for path gen to identify tiles
        grid[col, row] = value;

        // Instantiate (floorTilePrefab, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);

        if (value > 6) {
            int sndRand = Random.Range(0, 33);
            GameObject wallType = (sndRand == 0) ? tintedWallPrefab : innerWallPrefab;

            GameObject wall = Instantiate (wallType, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
            wall.name = "InnerTile" + col + ":" + row + "::" + value;
        } else if (value > 4) {
            Instantiate (sandTilePrefab, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);
        } else {
            Instantiate (floorTilePrefab, new Vector3 (col * 2 - 8, row * 2 - (float) 6.5, 2), Quaternion.identity);
        }
    }

    private void SpawnBoulder () {
        float playerYPos = player.transform.position.y;
        Instantiate (
            boulderPrefab,
            new Vector3 (Random.Range (-screenBounds.x * 0.75f, screenBounds.x * 0.75f), playerYPos + (screenBounds.y * 4), 0),
            Quaternion.identity
        );

        boulderRespawnTime = Random.Range (8, 13);
    }

    IEnumerator BoulderWave () {
        while (true) {
            yield return new WaitForSeconds (boulderRespawnTime);
            SpawnBoulder ();
        }
    }

    private void GeneratePath () {
        int rowIndex = 1;
        int columnIndex = Random.Range (1, 8);
        SearchPath (columnIndex, rowIndex, Direction.Up);
    }

    private void SearchPath (int column, int row, Direction prevDir) {
        if (grid[column, row] > wallSpawnThreshold) {
            GameObject tile = GameObject.Find ("InnerTile" + column + ":" + row + "::" + grid[column, row]);
            if (tile == null) {
                Debug.Log ("Wall at [" + column + ":" + row + "] is null. Name: [" +
                    "InnerTile" + column + ":" + row + "::" + grid[column, row] + "]");
            }
            grid[column, row] = -1;
            // Destroy (tile);
            // Debug.Log(tile.GetComponent<InnerWallBehaviour>());

            if (tile != null && !tile.CompareTag("TintedWall")) {
                Instantiate(floorTilePrefab, tile.transform.position, Quaternion.identity);
                tile.GetComponent<InnerWallBehaviour> ().PathGenDestroy ();
            }
            // tile.GetComponent<InnerWallBehaviour> ().pathGenDestroy ();
            // Debug.Log ("Destroyed wall at [" + column + ":" + row + "]");
        }

        Direction direction = (Direction) Random.Range (0, 3);
        if (CanPathLeft (column, prevDir, direction)) {
            column--;
        } else if (CanPathRight (column, prevDir, direction)) {
            column++;
        } else {
            row++;
            direction = Direction.Up;
        }

        if (row != rows - 2) {
            SearchPath (column, row, direction);
        }
    }

    private static bool CanPathRight (int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Right) && (column != 7) && (prevDir == Direction.Up);
    }

    private static bool CanPathLeft (int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Left) && (column != 1) && (prevDir == Direction.Up);
    }
}