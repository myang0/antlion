using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject keyPickupSfxPrefab;

    void FixedUpdate() {
        transform.Rotate(0, 1.5f, 0);

        Vector3 pos = transform.position;
        float yOffset = Mathf.Sin(Time.time * 2f) * 0.005f;

        transform.position = new Vector3(pos.x, pos.y + yOffset, pos.z);
    }

    void OnDestroy() {
        Instantiate(keyPickupSfxPrefab);
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
