using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class FightMapManager : MonoBehaviour {
    [SerializeField] private GameObject outerWallPrefab;
    [SerializeField] private GameObject sandTilePrefab;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private BoxCollider camBoundry;

    private GameObject player;
    private GameObject antlion;

    public int[,] grid;

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
        antlion.transform.position = new Vector3(columns-1, rows-1, 0);

        SetupCamera();
        GenerateMap();
    }

    private void GenerateMap() {
        for (int colIdx = 0; colIdx < columns; colIdx++) {
            for (int rowIdx = 0; rowIdx < rows; rowIdx++) {
                int gridValue = IsOuterWall(colIdx, rowIdx) ? 1 : 0;
                grid[colIdx, rowIdx] = gridValue;
                GenerateTile(colIdx, rowIdx, gridValue);
                if ((rowIdx == 4 || rowIdx == 10) && (colIdx == 4 || colIdx == 10)) {
                    GenerateTile(colIdx, rowIdx, 1);
                } else if ((rowIdx > 5 && rowIdx < 9) && (colIdx > 5 && colIdx < 9)) {
                    GenerateTile(colIdx, rowIdx, 2);
                }
            }
        }
    }

    private void SetupCamera() {
        player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "Player is null??? WTF");
        vcam.Follow = player.transform;
        var position = vcam.transform.position;
        position = new Vector3(position.x, position.y, 0);
        vcam.transform.position = position;
        CinemachineBasicMultiChannelPerlin vcamNoise =
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        vcamNoise.m_AmplitudeGain = 7f;
        vcamNoise.m_FrequencyGain = 0f;
        camBoundry.transform.position = new Vector3(columns-1, rows-1.5f, 0);
        camBoundry.size = new Vector3(2 * columns, 2 * rows, 1);
    }

    private bool IsOuterWall(int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void GenerateTile(int column, int row, int value) {
        Instantiate(floorTilePrefab, new Vector3(column * 2, row * 2 - 0.5f, 2),
            Quaternion.identity);
        
        if (value == 1) {
            Instantiate(outerWallPrefab, new Vector3(column * 2, row * 2 - 0.5f, 1),
                Quaternion.identity);
        } else if (value == 2) {
            Instantiate(sandTilePrefab, new Vector3(column * 2, row * 2 - 0.5f, 1),
                Quaternion.identity);
        }
    }
}