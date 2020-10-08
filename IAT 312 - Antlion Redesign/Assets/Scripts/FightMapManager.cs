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
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private BoxCollider camBoundry;

    private GameObject player;
    private GameObject antlion;

    public int[,] grid;
    private enum Tile {Floor, OuterWall, Sand, TintedWall}

    private int cameraHeight;

    private int rows, columns;

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
    }

    private void GenerateMap() {
        for (int colIdx = 0; colIdx < columns; colIdx++) {
            for (int rowIdx = 0; rowIdx < rows; rowIdx++) {
                int gridValue = IsOuterWall(colIdx, rowIdx) ? 1 : 0;
                grid[colIdx, rowIdx] = gridValue;
                
                //
                if ((rowIdx == 4 || rowIdx == 10) && (colIdx == 4 || colIdx == 10)) {
                    GenerateTile(colIdx, rowIdx, Tile.TintedWall);
                } else if ((rowIdx > 5 && rowIdx < 9) && (colIdx > 5 && colIdx < 9)) {
                    GenerateTile(colIdx, rowIdx, Tile.Sand);
                } else if (IsOuterWall(colIdx, rowIdx)) {
                    GenerateTile(colIdx, rowIdx, Tile.OuterWall);
                } else {
                    GenerateTile(colIdx, rowIdx, Tile.Floor);
                }
            }
        }
    }
    
    private void GenerateTile(int column, int row, Tile tileType) {

        Vector3 tilePosition = new Vector3(column * 2, row * 2 - 0.5f, 1);
        switch (tileType) {
            case Tile.Floor:
                Instantiate(floorTilePrefab, tilePosition, Quaternion.identity);
                break;
            case Tile.OuterWall:
                Instantiate(outerWallPrefab, tilePosition, Quaternion.identity);
                break;
            case Tile.Sand:
                Instantiate(sandTilePrefab, tilePosition, Quaternion.identity);
                break;
            case Tile.TintedWall:
                GameObject tintedWall = Instantiate(tintedWallPrefab, tilePosition, Quaternion.identity);
                tintedWall.GetComponent<TintedWallBehaviour>().ForceEquipmentSpawn();
                break;
        }
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
        camBoundry.size = new Vector3(2 * columns, 2 * rows, 1);
    }

    private bool IsOuterWall(int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }
}