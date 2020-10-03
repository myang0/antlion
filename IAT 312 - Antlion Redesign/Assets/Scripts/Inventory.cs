using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int numItemsCarried = 0;

    public bool[] isFull;
    public GameObject[] items;

    public int selectedItemIndex;
    public GameObject selectedItem;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (numItemsCarried > 0) {
                prevItem();
            }
        } else if (Input.GetKeyDown(KeyCode.E)) {
            if (numItemsCarried > 0) {
                nextItem();
            }
        }
    }

    void prevItem() {
        selectedItemIndex -= 1;

        if (selectedItemIndex < 0) {
            selectedItemIndex = numItemsCarried - 1;
        }

        selectedItem = items[selectedItemIndex];
    }

    void nextItem() {
        selectedItemIndex = (selectedItemIndex + 1) % numItemsCarried;

        selectedItem = items[selectedItemIndex];
    }
}
