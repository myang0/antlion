using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderIndicator : MonoBehaviour
{
    public GameObject player;
    public GameObject boulder;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (boulder == null || player == null) {
            Destroy(gameObject);
            return;
        }

        Quaternion angleToBoulder = RotateToBoulder();

        Vector3 norm = new Vector3(0f, 1f, 0);
        Vector3 rotatedNorm = angleToBoulder * norm;

        transform.position = player.transform.position + rotatedNorm;
    }

    private Quaternion RotateToBoulder() {
        Vector3 arrowPos = transform.position;
        Vector3 boulderPos = boulder.transform.position;

        Vector3 vectorToBoulder = (arrowPos - boulderPos).normalized;
        float angle = Mathf.Atan2(vectorToBoulder.y, vectorToBoulder.x) * Mathf.Rad2Deg;
        Quaternion angleToBoulder = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        transform.rotation = angleToBoulder;

        return angleToBoulder;
    }
}
