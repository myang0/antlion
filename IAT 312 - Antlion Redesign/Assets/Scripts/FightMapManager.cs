using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class FightMapManager : MonoBehaviour {
    [SerializeField] private GameObject outerWallPrefab;
    [SerializeField] private GameObject sandTilePrefab;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject tintedWallPrefab;
    [SerializeField] private GameObject lockedWallPrefab;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private BoxCollider camBoundry;

    private GameObject player;
    private GameObject antlion;

    public int[,] grid;
    private enum Tile {Floor, OuterWall, Sand, TintedWall, LockedWall}

    private int cameraHeight;

    private int rows, columns;
    private int hallLength = 5;
    private int roomSize = 6;
    public bool isEntranceOpen = true;
    public bool isExitOpen = false;
    public bool playerReachedEnd = false;

    void Start() {
        cameraHeight = (int) Camera.main.orthographicSize;

        outerWallPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        floorTilePrefab.transform.localScale = new Vector3(1f, 1f, 1f);

        // rows = cameraHeight * 2;
        columns = 15;
        rows = 15;
        grid = new int[columns, rows];

        antlion = GameObject.Find("Antlion");
        antlion.transform.position = new Vector3(columns - 1, rows - 1, 0);

        SetupCamera();
        GenerateMap();
        GenerateEntranceAndExit();
        GenerateLockedWall();
    }

    private void Update() {
        if (player.transform.position.y > 1.5f && isEntranceOpen) {
            isEntranceOpen = false;
            GenerateTileLayerZero(columns / 2 - 1, 0, Tile.OuterWall);
            GenerateTileLayerZero(columns / 2, 0, Tile.OuterWall);
            VNBehavior vnBehavior = GameObject.FindWithTag("VN").GetComponent<VNBehavior>();
            vnBehavior.UpdateVN(VNBehavior.DialogueChapter.BossStart);
        }

        if (player.transform.position.y > 50.5f && !playerReachedEnd) {
            playerReachedEnd = true;
            GameObject.FindGameObjectWithTag("EscMenu").GetComponent<EscMenuBehavior>().ShowWinScreen();
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
            GameObject floorTileLeft = GenerateTile(leftEntranceFloor, 0 - i, Tile.Floor);
            GenerateTile(rightEntranceWall, 0 - i, Tile.OuterWall);
            GameObject floorTileRight = GenerateTile(rightEntranceFloor, 0 - i, Tile.Floor);
            floorTileLeft.tag = "Untagged";
            floorTileRight.tag = "Untagged";
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
            GameObject floorTileLeft = GenerateTile(leftEntranceFloor, rows-1 + i, Tile.Floor);
            GenerateTile(rightEntranceWall, rows-1 + i, Tile.OuterWall);
            GameObject floorTileRight = GenerateTile(rightEntranceFloor, rows-1 + i, Tile.Floor);
            floorTileLeft.tag = "Untagged";
            floorTileRight.tag = "Untagged";
        }
    }

    private void GenerateLockedWall() {
        GenerateTileLayerZero(columns / 2 - 1, rows - 1, Tile.LockedWall);
        GenerateTileLayerZero(columns / 2, rows - 1, Tile.LockedWall);
    }

    private void GenerateMap() {
        for (int colIdx = 0; colIdx < columns; colIdx++) {
            for (int rowIdx = 0; rowIdx < rows; rowIdx++) {
                int gridValue = IsOuterWall(colIdx, rowIdx) ? 1 : 0;
                grid[colIdx, rowIdx] = gridValue;
                
                if ((rowIdx == 4 || rowIdx == 10) && (colIdx == 4 || colIdx == 10)) {
                    GameObject tintedWall = GenerateTile(colIdx, rowIdx, Tile.TintedWall);
                    if (rowIdx == 4) {
                        tintedWall.GetComponent<TintedWallBehaviour>().ForceEquipmentSpawn();
                    } else {
                        tintedWall.GetComponent<TintedWallBehaviour>().ForcePassiveSpawn();
                    }
                } else if ((rowIdx > 5 && rowIdx < 9) && (colIdx > 5 && colIdx < 9)) {
                    GenerateTile(colIdx, rowIdx, Tile.Sand);
                } else if (IsOuterWall(colIdx, rowIdx) && 
                           !((colIdx == columns / 2 - 1 || colIdx == columns / 2) &&
                             (rowIdx == 0 || rowIdx == rows-1))) {
                    GenerateTile(colIdx, rowIdx, Tile.OuterWall);
                    // GameObject floorTile =
                } else if (!((colIdx == columns / 2 - 1 || colIdx == columns / 2) &&
                             (rowIdx == 0 || rowIdx == rows-1))) {
                    GenerateTile(colIdx, rowIdx, Tile.Floor);
                }
            }
        }
    }
    
    private GameObject GenerateTile(int column, int row, Tile tileType) {

        Vector3 tilePosition = new Vector3(column * 2, row * 2 - 0.5f, 1);
        switch (tileType) {
            case Tile.Floor:
                return Instantiate(floorTilePrefab, tilePosition, Quaternion.identity);
            case Tile.OuterWall:
                return Instantiate(outerWallPrefab, tilePosition, Quaternion.identity);
            case Tile.Sand:
                return Instantiate(sandTilePrefab, tilePosition, Quaternion.identity);
            case Tile.TintedWall:
                GameObject tintedWall = Instantiate(tintedWallPrefab, tilePosition, Quaternion.identity);
                // tintedWall.GetComponent<TintedWallBehaviour>().ForceEquipmentSpawn();
                return tintedWall;
            case Tile.LockedWall:
                return Instantiate(lockedWallPrefab, tilePosition, Quaternion.identity);
        }
        return null;
    }
    
    private GameObject GenerateTileLayerZero(int column, int row, Tile tileType) {

        Vector3 tilePosition = new Vector3(column * 2, row * 2 - 0.5f, 0);
        switch (tileType) {
            case Tile.Floor:
                return Instantiate(floorTilePrefab, tilePosition, Quaternion.identity);
            case Tile.OuterWall:
                return Instantiate(outerWallPrefab, tilePosition, Quaternion.identity);
            case Tile.Sand:
                return Instantiate(sandTilePrefab, tilePosition, Quaternion.identity);
            case Tile.TintedWall:
                GameObject tintedWall = Instantiate(tintedWallPrefab, tilePosition, Quaternion.identity);
                tintedWall.GetComponent<TintedWallBehaviour>().ForceEquipmentSpawn();
                return tintedWall;
            case Tile.LockedWall:
                return Instantiate(lockedWallPrefab, tilePosition, Quaternion.identity);
        }
        return null;
    }

    private void SetupCamera() {
        player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "Player is null??? WTF");
        vcam.Follow = player.transform;
        vcam.m_Lens.OrthographicSize = 10;
        var position = vcam.transform.position;
        position = new Vector3(position.x, position.y, 0);
        vcam.transform.position = position;
        CinemachineBasicMultiChannelPerlin vcamNoise =
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        vcamNoise.m_AmplitudeGain = 7f;
        vcamNoise.m_FrequencyGain = 0f;
        camBoundry.transform.position = new Vector3(columns - 1, rows - 1.5f, 0);
        camBoundry.size = new Vector3(2 * columns,  (rows +(roomSize + hallLength)*2)*2 , 1);
    }

    private bool IsOuterWall(int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }
}