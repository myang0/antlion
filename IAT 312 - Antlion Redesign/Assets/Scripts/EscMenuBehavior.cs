using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuBehavior : MonoBehaviour {
    [SerializeField] private GameObject escapeMenuBackground;
    [SerializeField] private GameObject vnBackgroundEdge;
    public bool isEscMenuActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            EscapeMenu();
        }
    }

    public void EscapeMenu() {
        escapeMenuBackground.SetActive(!escapeMenuBackground.activeInHierarchy);
        if (!vnBackgroundEdge.activeInHierarchy) {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
        isEscMenuActive = !isEscMenuActive;
    }

    public void RestartGame() {
        Destroy(GameObject.FindGameObjectWithTag("PlayerContainer"));
        Destroy(GameObject.FindGameObjectWithTag("Antlion"));
        Destroy(GameObject.FindGameObjectWithTag("EscMenu"));
        Destroy(GameObject.FindGameObjectWithTag("UI"));
        Destroy(GameObject.FindGameObjectWithTag("VN"));

        GameObject[] tintedWalls = GameObject.FindGameObjectsWithTag("TintedWall");
        for (int i = 0; i < tintedWalls.Length; i++) {
            TintedWallBehaviour tw = tintedWalls[i].GetComponent<TintedWallBehaviour>();
            tw.SetRestarting();
        }

        GameObject[] regWalls = GameObject.FindGameObjectsWithTag("InnerWall");
        for (int i = 0; i < regWalls.Length; i++) {
            InnerWallBehaviour iw = regWalls[i].GetComponent<InnerWallBehaviour>();
            iw.SetRestarting();
        }

        SceneManager.LoadScene("RunPhaseScene");
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void QuitButton() {
        Application.Quit();
    }
}
