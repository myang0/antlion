using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the next scene in the queue
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
