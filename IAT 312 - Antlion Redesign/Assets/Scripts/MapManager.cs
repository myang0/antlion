using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int[, ] grid;
    public Sprite sprite;
    private int cameraHeight, cameraWidth;
    // Start is called before the first frame update
    void Start () {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;
        int rows = cameraHeight * 10;
        int columns = 9;
        grid = new int[columns, rows];

        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {
                //Starting Area
                if (rowIndex < 5) {
                    grid[columnIndex, rowIndex] = 0;

                } else {
                    grid[columnIndex, rowIndex] = Random.Range (0, 2);
                }

                //Outer Walls
                if (columnIndex == 0 || columnIndex == 8 ||
                    rowIndex == 0 || rowIndex == rows - 1) {
                    grid[columnIndex, rowIndex] = 1;
                }
                generateTile (columnIndex, rowIndex, grid[columnIndex, rowIndex]);
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }

    private void generateTile (int column, int row, int value) {
        GameObject g = new GameObject (column + ":" + row + "::" + value);
        g.transform.position = new Vector3 (column * 2 - 8,
            row * 2 - (float) 6.5);

        var spriteComponent = g.AddComponent<SpriteRenderer> ();
        spriteComponent.color = new Color (value, 255, 255);
        spriteComponent.sprite = sprite;

        if (value == 1) {
            var colliderComponent = g.AddComponent<BoxCollider2D> ();
            colliderComponent.size = new Vector2 (2, 2);
        }
    }
}