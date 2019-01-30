﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelInfo {
    public string name;
    public string sceneName;
    public Color32 backgroundColor;
    public Sprite levelImage;
}

public class LevelSelect : MonoBehaviour {

    public LevelInfo[] levelList;
    public int padding = 128;
    public int height = 512;

    private GameObject levelsCanvas;
    private int currentLevel = 0;

    private void Start() {
        // Set Cursor 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Get LevelListCanvas
        levelsCanvas = GameObject.FindGameObjectWithTag("LevelListCanvas");

        // Destroy unnecessary gameobjects
        foreach (Transform child in levelsCanvas.transform)
            GameObject.Destroy(child.gameObject);
        RemoveInteractables(); 

        // Create LevelButtons
        int i = 0;
        foreach(LevelInfo level in levelList) {
            createLevelButton(levelsCanvas.transform, new Vector3(0, -i * (height + padding), 0), new Vector2(512, 512), level.levelImage);
            i++;
        }

        // Add Button Listeners to LevelButtons
        GameObject.FindGameObjectWithTag("NextButton").GetComponent<Button>().onClick.AddListener(nextLevel);
        GameObject.FindGameObjectWithTag("PrevButton").GetComponent<Button>().onClick.AddListener(prevLevel);

        // Setup Text and Background Color
        setBackgroundColor();
        setCurrentLevelText();
    }

    private void selectLevel() {
        string sceneName = levelList[currentLevel].sceneName;
        SceneManager.LoadScene(sceneName);
    }

    private void nextLevel() {
        if (currentLevel >= levelList.Length - 1) return;

        currentLevel++;
        levelsCanvas.transform.position += new Vector3(0, height + padding);
        setBackgroundColor();
        setCurrentLevelText();
    }

    private void prevLevel() {
        if (currentLevel <= 0) return;

        currentLevel--;
        levelsCanvas.transform.position += new Vector3(0, -(height + padding));
        setBackgroundColor();
        setCurrentLevelText();
    }

    private void setBackgroundColor() {
        GameObject.FindGameObjectWithTag("BackgroundImage").GetComponent<Image>().color = levelList[currentLevel].backgroundColor;
    }

    private void setCurrentLevelText() {
        GameObject.FindGameObjectWithTag("CurrentLevelText").GetComponent<TMPro.TextMeshProUGUI>().text = levelList[currentLevel].name;
    }

    private GameObject createLevelButton(Transform parent, Vector3 position, Vector2 size, Sprite image) {
        GameObject button = new GameObject("Level Button"); 
        button.transform.parent = parent;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.AddComponent<Image>();

        Debug.Log(position);
        button.GetComponent<RectTransform>().localPosition = position;
        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size[0]);
        button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size[1]);
        button.GetComponent<Image>().sprite = image;
        button.GetComponent<Button>().targetGraphic = button.GetComponent<Image>();
        button.GetComponent<Button>().onClick.AddListener(selectLevel);
        return button;
    }

    private void RemoveInteractables() {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("InteractableUI");
        foreach (GameObject child in interactables)
            if (child.layer == LayerMask.NameToLayer("UI"))
                GameObject.Destroy(child);        
    }
}