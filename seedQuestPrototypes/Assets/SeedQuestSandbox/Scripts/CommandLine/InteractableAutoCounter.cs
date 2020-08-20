using System.Text;
using System.IO;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SeedQuest.Interactables;

public class InteractableAutoCounter : MonoBehaviour
{
    public bool checkInteractableCount;
    public bool finished;
    public string results;
    public string problemScenes = "";

    private int sceneIndex = 0;
    private int success = 0;
    private int failure = 0;
    private int updateDelay;
    private int waitCheck;

    public WorldSceneList worldSceneList;

    private List<string[]> interactableRowData = new List<string[]>();
    private bool writtenToCSV = false;

    void Start()
    {
        string[] rowHeader = new string[16];
        rowHeader[0] = "Scene ID";
        rowHeader[1] = "Scene Name";
        rowHeader[2] = "Scene Label";
        rowHeader[3] = "Scene File";
        rowHeader[4] = "Interactable ID";
        rowHeader[5] = "Interactable Name";
        rowHeader[6] = "Object Name";
        rowHeader[7] = "Default State";
        rowHeader[8] = "Action One";
        rowHeader[9] = "Action One - Object Name";
        rowHeader[10] = "Action Two";
        rowHeader[11] = "Action Two - Object Name";
        rowHeader[12] = "Action Three";
        rowHeader[13] = "Action Three - Object Name";
        rowHeader[14] = "Action Four";
        rowHeader[15] = "Action Four - Object Name";
        interactableRowData.Add(rowHeader);
    } 

    void Update()
    {

        if (checkInteractableCount && updateDelay >= 15)
        {
            updateDelay = 0;
            countAllInteractables();
        }

        updateDelay++;
    }

    public void loadFirstScene()
    {
        checkInteractableCount = true;
        updateDelay = 0;
        waitCheck = 0;
        LevelSetManager.AddLevel(0);
        if (!GameManager.V2Menus)
            MenuScreenManager.Instance.state = MenuScreenStates.Debug;
        SceneManager.LoadScene(worldSceneList.worldScenes[0].sceneName);
    }

    public void countAllInteractables()
    {
        int count = interactableCount();

        if (count > 0)
            Debug.Log("Current interactable count: " + count);
        
        if (count == 16 && sceneIndex < 16)
        {
            Debug.Log("16 interactables found in this scene!");
            success++;
            waitCheck = 0;
            sceneIndex++;
            if (sceneIndex < 16)
            {
                loadNextScene();
            }
        }
        else if (count < 16 && count > 0 && waitCheck < 5 && sceneIndex < 16)
        {
            Debug.Log("Going to wait for a second to see if more interactables load");
            waitCheck++;
        }
        else if (sceneIndex < 16)
        {
            Debug.Log("Unfortunately, could not find 16 interactables in scene: " +
                      worldSceneList.worldScenes[sceneIndex].sceneName + " Interactable count: " + count);
            problemScenes += "\n" + worldSceneList.worldScenes[sceneIndex].sceneName + " Interactable count: " + count;

            failure++;
            waitCheck = 0;
            sceneIndex++;
            if (sceneIndex < 16)
            {
                loadNextScene();
            }
        }

        if (sceneIndex >= 16)
        {
            Debug.Log("Finished checking all scenes. \nScenes with 16 interactables: " + success);
            Debug.Log("Scenes without 16 interactables: " + failure);

            results = "Scenes with 16 interactables: " + success + "\nScenes without 16 interactables: " + failure + "\n";
            results += "Problematic scenes:" + problemScenes;
            finished = true;


            writeToCSV();
        }
    }

    public void loadNextScene()
    {
        LevelSetManager.AddLevel(sceneIndex);
        if(!GameManager.V2Menus)
            MenuScreenManager.Instance.state = MenuScreenStates.Debug;
        SceneManager.LoadScene(worldSceneList.worldScenes[sceneIndex].sceneName);
    }

    public int interactableCount()
    {
        if (sceneIndex >= 16) return 0;

        int counter = 0;
        Interactable[] items = FindObjectsOfType<Interactable>();
        counter = items.Length;

        foreach(Interactable item in items)
        {
            string[] rowData = new string[16];
            rowData[0] = sceneIndex.ToString();
            rowData[1] = SeedQuest.Level.LevelManager.LevelName;
            rowData[2] = worldSceneList.worldScenes[sceneIndex].name;
            rowData[3] = worldSceneList.worldScenes[sceneIndex].sceneName;
            rowData[4] = item.ID.spotID.ToString();
            rowData[5] = item.Name;
            rowData[6] = item.name;
            rowData[7] = (item.stateData.defaultState.prefab != null) ? item.stateData.defaultState.prefab.name : "null";
            rowData[8] = (item.stateData.states[0] != null) ? "\"" + item.stateData.states[0].actionName.Replace("\"", "\"\"") + "\"" : "null";
            rowData[9] = (item.stateData.states[0].prefab != null) ? item.stateData.states[0].prefab.name : "null";
            rowData[10] = (item.stateData.states[1] != null) ? "\"" + item.stateData.states[1].actionName.Replace("\"", "\"\"") + "\"" : "null";
            rowData[11] = (item.stateData.states[1].prefab != null) ? item.stateData.states[1].prefab.name : "null";
            rowData[12] = (item.stateData.states[2] != null) ? "\"" + item.stateData.states[2].actionName.Replace("\"", "\"\"") + "\"" : "null";
            rowData[13] = (item.stateData.states[2].prefab != null) ? item.stateData.states[2].prefab.name : "null";
            rowData[14] = (item.stateData.states[3] != null) ? "\"" + item.stateData.states[3].actionName.Replace("\"", "\"\"") + "\"" : "null";
            rowData[15] = (item.stateData.states[3].prefab != null) ? item.stateData.states[3].prefab.name : "null";

            interactableRowData.Add(rowData);
        }
        
        return counter;
    }

    public void writeToCSV()
    {
        if (writtenToCSV)
            return;

        writtenToCSV = true;

        Debug.Log("Writing to CSV - " + interactableRowData.Count);
        string[][] output = new string[interactableRowData.Count][];

        for (int i = 0; i < output.Length; i++){
            output[i] = interactableRowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        #if UNITY_EDITOR
            string filePath = Application.dataPath + "/CSV/" + "Saved_data.csv";
        #else
            string filePath = "";
        #endif

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
