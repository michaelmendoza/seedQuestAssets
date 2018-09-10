﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class InteractableManager : MonoBehaviour
{

    public float interactDistance = 2.0f;
    public GameObject actionSpotIcon;
    public Interactable[] debugInteractables = null;
    private Interactable activeItem = null;

    static public Interactable ActiveItem
    {
        get { return instance.activeItem; }
        set { instance.activeItem = value; }
    }

    static public InteractableManager __instance = null;
    static public InteractableManager instance {
        get {
            if (__instance == null)
                __instance = GameObject.FindObjectOfType<InteractableManager>();
            return __instance;
        }
    }

    private void Update() {
        debugInteractables = findAllInteractables();
    }

    static Interactable[] findAllInteractables() {
        return GameObject.FindObjectsOfType<Interactable>();
    }

    static void findNearInteractables() {
        Interactable[] list = FindObjectsOfType<Interactable>();

        foreach (Interactable item in list)
        {
            Vector3 playerPosition = PlayerManager.GetPlayer().position;
            float dist = (item.transform.position - playerPosition).magnitude;
            if (dist < instance.interactDistance)
                doNearInteractable(true);
            else
                doNearInteractable(false);
        }  
    }

    static void doNearInteractable(bool isNear) {
        
    }

    static ParticleSystem getEffect() {
        ParticleSystem effect;

        InteractableStateData data = instance.activeItem.stateData;
        if(data == null)
            effect = EffectsManager.createEffect(instance.activeItem.transform);
        else if(data.effect == null)
            effect = EffectsManager.createEffect(instance.activeItem.transform);
        else 
            effect = EffectsManager.createEffect(instance.activeItem.transform, data.effect);

        return effect;
    }

    static public void doInteractableAction(int actionIndex) {
        Debug.Log("Action " + actionIndex);

        ParticleSystem effect = getEffect();
        effect.Play();

        instance.activeItem.doAction(actionIndex);
        instance.activeItem = null;
        
        PauseManager.isPaused = false;
        InteractableUI.hide();
    } 

    public void doInteractableActionButton(int actionIndex) {
        doInteractableAction(actionIndex);
    }

}
