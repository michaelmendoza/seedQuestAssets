﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StartScreenStates { Start, ModeSelect, SeedSetup, EncodeSeed }

public class StartScreenManager : MonoBehaviour
{
    static private StartScreenManager instance = null;
    static private StartScreenManager setInstance() { instance = GameObject.FindObjectOfType<StartScreenManager>(); return instance; }
    static public StartScreenManager Instance { get { return instance == null ? setInstance() : instance; } }

    private StartScreenStates state = StartScreenStates.Start;
    private Canvas[] canvas;
    private Canvas motionBackgroundCanvas;
    private Canvas encodeSeedCanvas;

    public void Awake()
    {
        canvas = GetComponentsInChildren<Canvas>(true);
        canvas[1].gameObject.SetActive(true);
        canvas[2].gameObject.SetActive(true);
        motionBackgroundCanvas = canvas[1];
        encodeSeedCanvas = canvas[5];
    }

    public void Start()
    {
        GoToStart();
    }

    private void Update()
    {
        RotateBackground();
    }

    private void ResetCanvas()
    {
        canvas[2].gameObject.SetActive(false);
        canvas[3].gameObject.SetActive(false);
        canvas[4].gameObject.SetActive(false);
        canvas[5].gameObject.SetActive(false);
    }

    public void GoToStart() {
        state = StartScreenStates.Start;
        ResetCanvas();
        canvas[2].gameObject.SetActive(true);
        SetupRotateBackground(0);
    }

    public void GoToModeSelect() {
        state = StartScreenStates.ModeSelect;
        ResetCanvas();
        canvas[3].gameObject.SetActive(true);
        Debug.Log(state);
        SetupRotateBackground(150);
    }

    public void GoToSeedSetup() {
        state = StartScreenStates.SeedSetup;
        ResetCanvas();
        canvas[4].gameObject.SetActive(true);
        SetupRotateBackground(270);
    }

    public void GoToEncodeSeed() {
        state = StartScreenStates.EncodeSeed;
        ResetCanvas();
        encodeSeedCanvas.gameObject.SetActive(true);
        SetupRotateBackground(330);
    }

    static public void SetLevelPanel(int panelIndex, int levelIndex) {
        Canvas levelPanels = Instance.encodeSeedCanvas.GetComponentsInChildren<Canvas>()[2];
        LevelPanel selectedPanel = levelPanels.GetComponentsInChildren<LevelPanel>()[panelIndex]; 
        TextMeshProUGUI text = selectedPanel.GetComponentsInChildren<TextMeshProUGUI>()[1];
        text.text = LevelSetManager.AllLevels[levelIndex].name;
    }

    private Vector3 rotate = new Vector3(0, 0, 0);
    private Vector3 targetRotate = new Vector3(0, 0, 0);
    private float time = 0;
    public void SetupRotateBackground(float angle) {
        if(angle > 180)
            motionBackgroundCanvas.GetComponentsInChildren<Image>(true)[4].gameObject.SetActive(false);
        else
            motionBackgroundCanvas.GetComponentsInChildren<Image>(true)[4].gameObject.SetActive(true);

        rotate.z = targetRotate.z;
        targetRotate.z = angle;
        time = Time.time;

        Debug.Log(targetRotate);
    }

    public void RotateBackground() {
        float timeDuration = 1.0f;
        float t = Mathf.Clamp01((Time.time - time) / timeDuration);
        Vector3 newRotate = Vector3.Lerp(rotate, targetRotate, t);
        Debug.Log(newRotate);
        motionBackgroundCanvas.GetComponentInChildren<RectTransform>().localRotation = Quaternion.Euler(newRotate);
    }

}
