using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int[, ] grid;
    public Sprite sprite;
    private int cameraHeight, cameraWidth;

    private int rows, columns;

    void Start () {
        cameraHeight = (int) Camera.main.orthographicSize;
        cameraWidth = cameraHeight * (int) Camera.main.aspect;

        rows = cameraHeight * 20;
        columns = 9;

        grid = new int[columns, rows];

        for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
            for (int rowIndex = 0; rowIndex < rows; rowIndex++) {
                // Starting Area
                if (rowIndex < 8 && !isOuterWall(columnIndex, rowIndex)) {
                    grid[columnIndex, rowIndex] = 0;
                    generateTile(columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                }
                // Outer walls
                else if (isOuterWall(columnIndex, rowIndex))
                {
                    grid[columnIndex, rowIndex] = 1;
                    generateTile(columnIndex, rowIndex, grid[columnIndex, rowIndex]);
                }
                // Anything else
                else
                {
                    generateNonEdge(columnIndex, rowIndex);
                }
            }
        }
    }

    private bool isOuterWall(int column, int row)
    {
        return (column == 0 || column == columns - 1 || row == 0 || row == rows - 1);
    }

    private void generateTile (int column, int row, int value) {
        string tileTitle = (value == 1 ? "OuterTile" : "StartingTile");
        GameObject g = new GameObject (tileTitle + column + ":" + row + "::" + value);

        g.transform.position = new Vector3(
            column * 2 - 8,
            row * 2 - (float) 6.5,
            1
        );

        var spriteComponent = g.AddComponent<SpriteRenderer> ();

        spriteComponent.color = new Color (value, 255, 255);
        spriteComponent.sprite = sprite;

        if (value == 1) {
            var colliderComponent = g.AddComponent<BoxCollider2D> ();
            colliderComponent.size = new Vector2 (2, 2);
        }
    }

    private void generateNonEdge(int col, int row)
    {
        int value = Random.Range(0, 6);

        GameObject g = (value == 1) ? new GameObject("SandTile" + col + ":" + row + "::" + value) : new GameObject("InnerTile" + col + ":" + row + "::" + value);

        g.transform.position = new Vector3(
            col * 2 - 8,
            row * 2 - (float) 6.5,
            1
        );

        var spriteComponent = g.AddComponent<SpriteRenderer>();

        float colorVal = 0f;

        if (value == 5)
        {
            var colliderComponent = g.AddComponent<BoxCollider2D>();
            colliderComponent.size = new Vector2(2, 2);

            colorVal = 1f;
        }
        else if (value == 1)
        {
            var colliderComponent = g.AddComponent<BoxCollider2D>();
            colliderComponent.isTrigger = true;
            colliderComponent.size = new Vector2(0.85f, 0.85f);

            colorVal = 0.5f;
        }

        spriteComponent.color = new Color(colorVal, 255, 255);
        spriteComponent.sprite = sprite;
    }
}