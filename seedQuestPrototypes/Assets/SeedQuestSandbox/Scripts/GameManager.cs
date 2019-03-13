﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using SeedQuest.Interactables;

public enum GameMode { Sandbox, Rehearsal, Recall } 
public enum GameState { Play, Pause, Interact, Menu, End }

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    public static GameManager Instance {
        get  {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private void OnApplicationQuit() {
        instance = null;
    }

    public GameMode mode = GameMode.Sandbox;
    public static GameMode Mode { 
        get { return Instance.mode; }
        set { Instance.mode = value; }
    }

    public GameState state = GameState.Play;
    public GameState prevState = GameState.Play;
    public static GameState State {
        get { return Instance.state; }
        set { if (value == Instance.state) return; Instance.prevState = Instance.state; Instance.state = value; }
    }
    public static GameState PrevState {
        get { return Instance.prevState; }
    }

    public GameSoundData gameSound = null;
    public static GameSoundData GameSound { get { return Instance.gameSound; } }

    public GameObject HUDEndGamePrefab;
    public GameObject HUDLevelClearPrefab;
    public GameObject HUDMenuPrefab;

    private string scene = null;
    private GameObject menu = null;

    public static string Scene
    {
        get { return Instance.scene; }
        set { Instance.scene = value; }
    }

    public void Update() {
        CheckButtonClick();
        ListenForKeyDown();
        CheckForEndGame();
        CheckForNewScene();
    }

    public void CheckForEndGame() {

    }

    public void CheckForNewScene()
    {
        if (scene != SceneManager.GetActiveScene().name)
        {
            scene = SceneManager.GetActiveScene().name;
            if (HUDMenuPrefab != null)
            {
               menu = Instantiate(HUDMenuPrefab);
            }
        }
    }

    static public void ResetCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ListenForKeyDown() {
        if (Input.GetKeyDown("escape") && Mode != GameMode.Sandbox) {
            //SceneManager.LoadScene("PrototypeSelect");
            Debug.Log(state);
            if (menu)
            {
                if (menu.activeSelf)
                {
                    state = MenuUI.PrevState;
                    MenuUI.StateSaved = false;
                    menu.SetActive(false);
                }
                else
                {
                    menu.SetActive(true);
                }
            }
        }
    }

    public void CheckButtonClick() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            ClickButtons();
        }
    }

    public void ClickButtons()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f)) {
                IPointerClickHandler clickHandler = hit.transform.gameObject.GetComponent<IPointerClickHandler>();
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                if (clickHandler != null) {
                    clickHandler.OnPointerClick(pointerEventData);
                }
            }
        }
    }
}
