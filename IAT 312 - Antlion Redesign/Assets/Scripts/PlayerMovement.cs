using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Rigidbody2D rigidBody;

    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       movement.x = Input.GetAxisRaw("Horizontal");
       movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Movement
        rigidBody.MovePosition(rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        string cName = collider.name;
        if (cName.Contains("SandTile"))
        {
            movementSpeed = 2.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        string cName = collider.name;
        if (cName.Contains("SandTile"))
        {
            movementSpeed = 5f;
        }
    }
}
