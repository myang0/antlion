using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject outerWallPrefab;

    [SerializeField]
    private GameObject floorTilePrefab;

    public int[, ] grid;

    private int cameraHeight;

    private int rows, columns;

    void Start() {
        cameraHeight = (int) Camera.main.orthographicSize;

        outerWallPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        floorTilePrefab.transform.localScale = new Vector3(1f, 1f, 1f);

        rows = cameraHeight * 2;
        columns = 11;

        grid = new int[columns, rows];

        for (int colIdx = 0; colIdx < columns; colIdx++) {
            for (int rowIdx = 0; rowIdx < rows; rowIdx++) {
                int gridValue = isOuterWall(colIdx, rowIdx) ? 1 : 0;

                grid[colIdx, rowIdx] = gridValue;
                generateTile(colIdx, rowIdx, gridValue);
            }
        }
    }

    private bool isOuterWall (int column, int row) {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void generateTile (int column, int row, int value) {
        Instantiate (floorTilePrefab, new Vector3 (column * 2 - 10, row * 2 - (float) 9, 2), Quaternion.identity);

        if (value == 1) {
            Instantiate (outerWallPrefab, new Vector3 (column * 2 - 10, row * 2 - (float) 9, 1), Quaternion.identity);
        }
    }
}
