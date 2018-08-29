﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//[ExecuteInEditMode] 
public class Interactable : MonoBehaviour {

    public GameObject actionSpot = null;
    public Vector3 positionOffset = new Vector3(0, 0.0f, 0);

    // Use this for initialization
    void Start () {
        Vector3 position = transform.position;
        position += positionOffset;
        Quaternion rotate = Quaternion.identity;
        actionSpot = Instantiate(InteractableManager.instance.actionSpotIcon, position, rotate, InteractableManager.instance.transform);
        var text = actionSpot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = this.name;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPosition = Camera.main.transform.position; 
        Vector3 interactablePosition = actionSpot.transform.position;
        Vector3 lookAtDir = targetPosition - interactablePosition;
        lookAtDir.y = 0;

        Quaternion rotate =  Quaternion.LookRotation(lookAtDir);
        actionSpot.transform.rotation = rotate;
        //actionSpot.transform.Rotate(new Vector3(90.0f, 0, 0));
	} 
      
    public void toggleHighlight(bool highlight) {
        Renderer rend = transform.GetComponent<Renderer>();
        Shader shaderDefault = Shader.Find("Standard");
        Shader shader = Shader.Find("Custom/Outline + Rim");

        if (highlight)
            rend.material.shader = shader;
        else
            rend.material.shader = shaderDefault;
    }

    private void OnDrawGizmos() { 
        if(actionSpot != null)
            Gizmos.DrawWireSphere(actionSpot.transform.position, 0.1f);
    }

} 