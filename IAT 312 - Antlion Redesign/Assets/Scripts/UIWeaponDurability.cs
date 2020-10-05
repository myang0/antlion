using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponDurability : MonoBehaviour
{
    private Inventory inventory;
    public Text txt;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void Update()
    {
        if (inventory.selectedItem != null) {
            PickupItem item = inventory.selectedItem.GetComponent<PickupItem>();
            txt.text = item.remainingUses + "";
        } else {
            txt.text = "n/a";
        }
    }
}
