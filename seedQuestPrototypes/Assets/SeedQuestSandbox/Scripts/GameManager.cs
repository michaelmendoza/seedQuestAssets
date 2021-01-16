using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeedQuest.Interactables;

public enum GameMode { Sandbox, Rehearsal, Recall } 
public enum GameState { Play, Pause, Interact, Menu, End }

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    public static GameManager Instance {
        get  {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<GameManager>();

                if(instance != null)
                    DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private void OnApplicationQuit() {
        instance = null;
    }

    public static bool[] GraduatedFlags { get; set; } = new bool[6];
    public static void ResetGraduatedRehearsal()  {
        GraduatedFlags = new bool[6];
    }
    public static bool ReviewMode { get; set; } = false;
    public static bool TutorialMode { get; set; } = false;
    public static bool MobileMode { get; set; } = false;
    public static bool V2Menus { get; set; } = true;

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

    public bool UseReviewMode = true;

    public void Update() {
        ListenForKeyDown();
    }

    static public void ResetCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ListenForKeyDown() {
        if (Input.GetKeyDown("escape") &&  state != GameState.Menu) {
            //ESCMenuUI.ToggleOn();
            //ScenePauseMenu.ToggleOn();
        }

        if (InputManager.GetKeyDown(KeyCode.F) && state != GameState.Menu)
        {
            if (state == GameState.Menu && !FastRecoveryUI.Instance.gameObject.activeSelf)
                return;
            if (Mode == GameMode.Rehearsal)
                return;
            FastRecoveryUI.ToggleActive();
        }
    }

} 