using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public int numItemsCarried = 0;

    public bool[] isFull;
    public GameObject[] items;

    public int selectedItemIndex;
    public GameObject selectedItem;

    public Camera cam;

    private Vector3 mousePos;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (numItemsCarried > 0) prevItem();
        } else if (Input.GetKeyDown(KeyCode.E)) {
            if (numItemsCarried > 0) nextItem();
        }
        
        GameObject player = GameObject.FindWithTag("Player");
        if (Input.GetMouseButtonDown(0) & isFightScene() && player) {
            useCurrentItem();
        }
        
    }

    private bool isFightScene() {
        return string.Equals(SceneManager.GetActiveScene().name, "FightPhase");
    }

    void prevItem() {
        selectedItemIndex -= 1;
        selectedItemIndex = (selectedItemIndex < 0) ? numItemsCarried - 1 : selectedItemIndex;

        selectedItem = items[selectedItemIndex];
    }

    void nextItem() {
        selectedItemIndex = (selectedItemIndex + 1) % numItemsCarried;

        selectedItem = items[selectedItemIndex];
    }

    void useCurrentItem() {
        if (selectedItem == null) return;

        PickupItem itemComponent = selectedItem.GetComponent<PickupItem>();
        itemComponent.Use();

        if (itemComponent.remainingUses <= 0) removeSelectedItem();
    }

    void removeSelectedItem() {
        // TODO: remove hardcoding of array lengths
        bool[] isFullCopy = new bool[6];
        GameObject[] itemsCopy = new GameObject[6];

        int sweep = 0;
        for (int i = 0; i < numItemsCarried; i++) {
            if (i == selectedItemIndex) {

            } else {
                isFullCopy[sweep] = isFull[i];
                itemsCopy[sweep] = items[i];

                sweep++;
            }
        }

        numItemsCarried--;

        isFull = isFullCopy;
        items = itemsCopy;

        selectedItem = (numItemsCarried > 0) ? items[0] : null;
        selectedItemIndex = 0;
    }
}
