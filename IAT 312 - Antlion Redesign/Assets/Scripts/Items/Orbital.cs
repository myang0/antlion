using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbital : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private float currentScale;
    [SerializeField] private float contactDamage;

    [SerializeField] private int level;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

        transform.localScale = new Vector3(currentScale, currentScale, 1);

        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate() {
        RotateAroundPlayer();

        transform.Rotate(0, 0, 10 * Mathf.Sin(Time.time));
    }

    public void Upgrade() {
        currentScale *= 1.25f;
        transform.localScale = new Vector3(currentScale, currentScale, 1);

        contactDamage *= 1.5f;

        level++;
    }

    private void RotateAroundPlayer() {
        Quaternion rotationAngle = Quaternion.AngleAxis(Time.time * (50f * currentScale), Vector3.forward);

        Vector3 norm = new Vector3(0f, currentScale * 1.5f, 0);
        Vector3 rotatedNorm = rotationAngle * norm;

        transform.position = player.transform.position + rotatedNorm;
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (col.CompareTag("Antlion")) {
            AntlionBehavior a = col.GetComponent<AntlionBehavior>();
            a.Damage(contactDamage);
        }

        if (col.CompareTag("Swarmer")) {
            Swarmer s = col.GetComponent<Swarmer>();
            s.inflictDamage(contactDamage);
        }

        if (col.CompareTag("SandSpit") || col.CompareTag("Boulder")) {
            Destroy(col.gameObject);
        }
    }
}
