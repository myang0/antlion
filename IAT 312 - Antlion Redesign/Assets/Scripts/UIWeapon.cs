using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour
{
    private Inventory inventory;

    public Image img;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void Update()
    {
        if (inventory.selectedItem != null) {
            PickupItem item = inventory.selectedItem.GetComponent<PickupItem>();
            img.sprite = item.sprite.sprite;
        } else {
            img.sprite = null;
        }
    }
}
