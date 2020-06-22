﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour {
    public bool useCursor = false;
    public bool useLevelSelect = false;
    public bool usePreview = false;

    public string sceneName = "SceneSelect";
    static GameObject cursor;
    static private bool showCursor;
    static public bool ShowCursor
    {
        get { return showCursor; }
        set { showCursor = value; cursor.SetActive(showCursor); }
    }

    private void Start() {
        cursor = GameObject.FindGameObjectWithTag("Cursor");
        if(cursor != null)
            cursor.SetActive(showCursor);
    }

    public void GoToSceneSelect() {
        SceneManager.LoadScene(sceneName);
    }
}