using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    [SerializeField] private GameObject innerWallPrefab;
    [SerializeField] private GameObject tintedWallPrefab;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject sandTilePrefab;
    [SerializeField] private GameObject outerWallPrefab;
    [SerializeField] private GameObject boulderPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraBoundry;

    [SerializeField] private GameObject swarmerPrefab;

    public bool isSceneOver = false;

    public int[,] grid;
    public Sprite sprite;
    private int cameraHeight, cameraWidth;

    private int rows, columns;
    private int numRows;

    private int boulderRespawnTime = 8;
    private int swarmerRespawnTime = 0;

    private Vector2 screenBounds;

    private enum Direction {
        Left,
        Right,
        Up
    }

    public bool isEntranceBlocked = false;
    public bool isEntranceWallBlocked = false;
    public bool isExitWallBlocked = false;
    public bool isTransitionWallBlocked = false;
    private int hallLength = 5;
    private int roomSize = 6;

    private enum Tile {
        Floor,
        OuterWall,
        Sand,
        TintedWall,
        InnerWall
    }

    private int wallSpawnThreshold = 3;

    void Start() {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;
        screenBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                Camera.main.transform.position.z));
        numRows = 30;
        // rows = cameraHeight * numRows;
        rows = 150;
        columns = 18;
        grid = new int[columns, rows];

        SetupCameraBoundry();
        GenerateMap();
        GenerateEntranceAndExit();
        GeneratePath();
    }

    void Update() {
        if (player) {
            if (player.transform.position.y > GETEndOfRunMap()) {
                isSceneOver = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                player.GetComponent<PlayerMovement>().SceneTransition();
            }

            if (player.transform.position.y > 3.5 && !isEntranceWallBlocked) {
                isEntranceWallBlocked = true;
                for (int i = 0; i < columns; i++) {
                    GenerateTile(i, 4, Tile.OuterWall);
                }

                StartCoroutine(BoulderWave());
            }

            if (player.transform.position.y > -4.5 && !isEntranceBlocked) {
                isEntranceBlocked = true;
                GenerateTile(columns / 2 - 1, 0, Tile.OuterWall);
                GenerateTile(columns / 2, 0, Tile.OuterWall);
            }

            if (player.transform.position.y > (rows-5)*2f-4 && !isExitWallBlocked) {
                isExitWallBlocked = true;
                for (int i = 0; i < columns; i++) {
                    GenerateTile(i, rows-5, Tile.OuterWall);
                }
                VNBehavior vnBehavior = GameObject.FindWithTag("VN").GetComponent<VNBehavior>();
                vnBehavior.UpdateVN(VNBehavior.DialogueChapter.EndRunPhase);
            }

            if (player.transform.position.y > (rows/2-1)*2f-4 && !isTransitionWallBlocked) {
                isTransitionWallBlocked = true;
                for (int i = 0; i < columns; i++) {
                    GenerateTile(i, rows / 2 - 1, Tile.OuterWall);
                }

                StartCoroutine(SwarmerWave());
                VNBehavior vnBehavior = GameObject.FindWithTag("VN").GetComponent<VNBehavior>();
                vnBehavior.UpdateVN(VNBehavior.DialogueChapter.Desert);
            }
        }
    }

    public float GETEndOfRunMap() {
        return 2f * rows + 5f;
    }

    private void GenerateMap() {
        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {

                if (rowIndex < 4 && !IsOuterWall(columnIndex, rowIndex)) {
                    // Starting area
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);

                } else if (IsEntranceOrExit(columnIndex, rowIndex)) {
                    // Ensures entrance is unblocked
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);

                } else if (IsOuterWall(columnIndex, rowIndex)) {
                    // Outer walls
                    GenerateTile(columnIndex, rowIndex, Tile.OuterWall);

                    // Construct walls seal chase area
                } else if (rowIndex == rows - 5 || rowIndex == 4) {
                    if (columnIndex % 2 == 0) {
                        GenerateTile(columnIndex, rowIndex, Tile.Floor);
                    } else {
                        GameObject antiAntlionOuterWall = Instantiate(outerWallPrefab,
                            new Vector3(columnIndex * 2 - 8,
                                rowIndex * 2 - (float) 6.5, 1), Quaternion.identity);
                        antiAntlionOuterWall.tag = "Untagged";
                    }
                    
                } else if (rowIndex > rows/2 - 4 && rowIndex < rows/2 + 2) {
                    //Antlion to Mini flier transition walls
                    if (rowIndex == rows / 2 - 1) {
                        for (int i = 0; i < columns; i++) {
                            if (i % 2 == 0) {
                                GameObject antiAntlionOuterWall = Instantiate(outerWallPrefab,
                                    new Vector3(i * 2 - 8,
                                        rowIndex * 2 - (float) 6.5, 1), Quaternion.identity);
                                antiAntlionOuterWall.tag = "Untagged";
                            } else {
                                GenerateTile(i, rowIndex, Tile.Floor);
                            }
                        }
                    } else {
                        GenerateTile(columnIndex, rowIndex, Tile.Floor);
                    }
                    
                } else if (rowIndex > rows - 6) {
                    // Ending Area
                    GenerateTile(columnIndex, rowIndex, Tile.Floor);
                    
                } else {
                    // Everything else
                    if (rowIndex < rows / 2) {
                        //Antlion Chase
                        GenerateNonEdge(columnIndex, rowIndex, true);
                    } else {
                        //Mini Flying Antlion Chase
                        GenerateNonEdge(columnIndex, rowIndex, false);
                    }
                }
            }
        }
    }

    private void GenerateEntranceAndExit() {
        int leftEntranceWall = columns / 2 - 2;
        int leftEntranceFloor = columns / 2 - 1;
        int rightEntranceWall = columns / 2 + 1;
        int rightEntranceFloor = columns / 2;

        // generate hall
        for (int i = 0; i < hallLength; i++) {
            GenerateTile(leftEntranceWall, 0 - i, Tile.OuterWall);
            GenerateTile(leftEntranceFloor, 0 - i, Tile.Floor);
            GenerateTile(rightEntranceWall, 0 - i, Tile.OuterWall);
            GenerateTile(rightEntranceFloor, 0 - i, Tile.Floor);
        }

        int leftMostColumn = leftEntranceFloor - roomSize / 2;
        int rightMostColumn = rightEntranceWall + roomSize / 2;

        // generate starting room
        for (int columnIndex = leftMostColumn; columnIndex < rightMostColumn; columnIndex++) {
            for (int rowIndex = 0; rowIndex < roomSize; rowIndex++) {
                //If outer wall of room
                if (
                    columnIndex == leftMostColumn || columnIndex == rightMostColumn - 1 ||
                    rowIndex == 0 || rowIndex == roomSize - 1) {
                    //if entrance, do not block with walls
                    if ((columnIndex == leftEntranceFloor || columnIndex == rightEntranceFloor) &&
                        rowIndex == 0) {
                        GenerateTile(columnIndex, -hallLength - rowIndex, Tile.Floor);
                    } else {
                        //if not entrance, resume
                        GenerateTile(columnIndex, -hallLength - rowIndex, Tile.OuterWall);
                    }
                } else {
                    //room floor
                    GenerateTile(columnIndex, -hallLength - rowIndex, Tile.Floor);
                }
            }
        }
        
        // generate end hall
        for (int i = 0; i < hallLength*5; i++) {
            GenerateTile(leftEntranceWall, rows-1 + i, Tile.OuterWall);
            GenerateTile(leftEntranceFloor, rows-1 + i, Tile.Floor);
            GenerateTile(rightEntranceWall, rows-1 + i, Tile.OuterWall);
            GenerateTile(rightEntranceFloor, rows-1 + i, Tile.Floor);
        }
    }

    private bool IsEntranceOrExit(int columnIndex, int rowIndex) {
        return (columnIndex == (columns / 2) - 1 || columnIndex == columns / 2) &&
               (rowIndex == 0 || rowIndex == rows - 1);
    }

    private void SetupCameraBoundry() {
        float cameraOffset = (float) 7.5;
        grid = new int[columns, rows];
        cameraBoundry.transform.position = new Vector3(columns * 0.5f, rows - cameraOffset-10f, 0);
        cameraBoundry.GetComponent<BoxCollider>().size = new Vector3(columns * 2,
            (rows + (roomSize + hallLength)*2) * 2, 1);
    }

    private bool IsOuterWall(int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void GenerateTile(int column, int row, Tile value) {
        // grid[column, row] = value;
        // Instantiate (floorTilePrefab, new Vector3 (column * 2 - 8, row * 2 - (float) 6.5, 1), Quaternion.identity);

        if (value == Tile.OuterWall) {
            Instantiate(outerWallPrefab, new Vector3(column * 2 - 8, row * 2 - (float) 6.5, 1),
                Quaternion.identity);
        } else if (value == Tile.Sand) {
            Instantiate(sandTilePrefab, new Vector3(column * 2 - 8, row * 2 - (float) 6.5, 1),
                Quaternion.identity);
        } else if (value == Tile.Floor) {
            Instantiate(floorTilePrefab, new Vector3(column * 2 - 8, row * 2 - (float) 6.5, 2),
                Quaternion.identity);
        }
    }

    private void GenerateNonEdge(int col, int row, bool isFirstPhase) {
        int value = Random.Range(0, 10);

        //important to store value for path gen to identify tiles
        grid[col, row] = value;

        if (isFirstPhase) {
            if (value > 6) {
                int sndRand = Random.Range(0, 33);
                GameObject wallType = (sndRand == 0) ? tintedWallPrefab : innerWallPrefab;

                GameObject wall = Instantiate(wallType, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1),
                    Quaternion.identity);
                wall.name = "InnerTile" + col + ":" + row + "::" + value;
            } else if (value > 4) {
                Instantiate(sandTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1),
                    Quaternion.identity);
            } else {
                Instantiate(floorTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 2),
                    Quaternion.identity);
            }
            
        } else {
            if (value > 8) {
                int sndRand = Random.Range(0, 33);
                GameObject wallType = (sndRand == 0) ? tintedWallPrefab : innerWallPrefab;

                GameObject wall = Instantiate(wallType, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1),
                    Quaternion.identity);
                wall.name = "InnerTile" + col + ":" + row + "::" + value;
            } else if (value > 2) {
                Instantiate(sandTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 1),
                    Quaternion.identity);
            } else {
                Instantiate(floorTilePrefab, new Vector3(col * 2 - 8, row * 2 - (float) 6.5, 2),
                    Quaternion.identity);
            }
        }
    }

    private void SpawnBoulder() {
        float playerYPos = player.transform.position.y;
        Instantiate(
            boulderPrefab,
            new Vector3(Random.Range(-screenBounds.x * 0.75f, screenBounds.x * 0.75f),
                playerYPos + (screenBounds.y * 4), 0),
            Quaternion.identity
        );

        boulderRespawnTime = Random.Range(8, 13);
    }

    private void SpawnSwarmer() {
        float playerY = player.transform.position.y;
        float playerX = player.transform.position.x;

        float swarmerX, swarmerY;

        swarmerX = playerX + Random.Range(-screenBounds.x, screenBounds.x);
        swarmerY = playerY - (screenBounds.y * 4);

        Instantiate(
            swarmerPrefab,
            new Vector3(swarmerX, swarmerY, 0),
            Quaternion.identity
        );

        swarmerRespawnTime = Random.Range(4, 7);
    }

    IEnumerator BoulderWave() {
        while (true) {
            yield return new WaitForSeconds(boulderRespawnTime);
            SpawnBoulder();
        }
    }

    IEnumerator SwarmerWave() {
        while (true) {
            yield return new WaitForSeconds(swarmerRespawnTime);
            SpawnSwarmer();
        }
    }

    private void GeneratePath() {
        int rowIndex = 1;
        int columnIndex = Random.Range(1, 8);
        SearchPath(columnIndex, rowIndex, Direction.Up);
    }

    private void SearchPath(int column, int row, Direction prevDir) {
        if (grid[column, row] > wallSpawnThreshold) {
            GameObject tile = GameObject.Find("InnerTile" + column + ":" + row + "::" + grid[column, row]);
            if (tile == null) {
                Debug.Log("Wall at [" + column + ":" + row + "] is null. Name: [" +
                          "InnerTile" + column + ":" + row + "::" + grid[column, row] + "]");
            }

            grid[column, row] = -1;
            // Destroy (tile);
            // Debug.Log(tile.GetComponent<InnerWallBehaviour>());

            if (tile != null && !tile.CompareTag("TintedWall")) {
                Instantiate(floorTilePrefab, tile.transform.position, Quaternion.identity);
                tile.GetComponent<InnerWallBehaviour>().PathGenDestroy();
            }

            // tile.GetComponent<InnerWallBehaviour> ().pathGenDestroy ();
            // Debug.Log ("Destroyed wall at [" + column + ":" + row + "]");
        }

        Direction direction = (Direction) Random.Range(0, 3);
        if (CanPathLeft(column, prevDir, direction)) {
            column--;
        } else if (CanPathRight(column, prevDir, direction)) {
            column++;
        } else {
            row++;
            direction = Direction.Up;
        }

        if (row != rows - 2) {
            SearchPath(column, row, direction);
        }
    }

    private static bool CanPathRight(int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Right) && (column != 7) && (prevDir == Direction.Up);
    }

    private static bool CanPathLeft(int column, Direction prevDir, Direction direction) {
        return (direction == Direction.Left) && (column != 1) && (prevDir == Direction.Up);
    }
}