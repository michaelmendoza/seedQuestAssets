﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeedQuest.Interactables;

public class InteractableActionsUI : MonoBehaviour
{
    static private InteractableActionsUI instance = null;
    static private InteractableActionsUI setInstance() { instance = HUDManager.Instance.GetComponentInChildren<InteractableActionsUI>(true); return instance; }
    static public InteractableActionsUI Instance { get { return instance == null ? setInstance() : instance; } }

    private TextMeshProUGUI interactableLabel;
    private Button[] actionButtons = new Button[4];
    private Interactable interactable;

    // Start is called before the first frame update
    void Start() {
        SetComponentRef();
        SetText();
        SetHoverEvents();
    }

    static public void Toggle(bool active) {
        Instance.gameObject.SetActive(active);
        IsometricCamera.instance.ToggleLookAtInteractable(active);
    }

    void SetComponentRef() {
        interactable = InteractableManager.ActiveInteractable;
        actionButtons = GetComponentsInChildren<Button>();
        interactableLabel = GetComponentInChildren<TextMeshProUGUI>();

        actionButtons[0].onClick.AddListener(() => { clickActionButton(0); });
        actionButtons[1].onClick.AddListener(() => { clickActionButton(1); });
        actionButtons[2].onClick.AddListener(() => { clickActionButton(2); });
        actionButtons[3].onClick.AddListener(() => { clickActionButton(3); });
    }

    void SetText() {
        interactableLabel.text = interactable.Name;
        actionButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = interactable.GetActionName(0);
        actionButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = interactable.GetActionName(1);
        actionButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = interactable.GetActionName(2);
        actionButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = interactable.GetActionName(3);
    }

    void hoverActionButton(int actionIndex) {
        interactable.DoAction(actionIndex);
    }

    void clickActionButton(int actionIndex) {
        if (GameManager.Mode == GameMode.Rehearsal) {
            if (actionIndex == InteractablePath.NextInteractable.ID.actionID) {
                InteractableLog.Add(interactable, interactable.ActionIndex);
                InteractablePath.GoToNextInteractable();
            } 
        }
        else if (GameManager.Mode == GameMode.Recall)
            InteractableLog.Add(interactable, interactable.ActionIndex);

        InteractableActionsUI.Toggle(false);
    }

    private void SetHoverEvents() {
        SetHoverForActionButton(0);
        SetHoverForActionButton(1);
        SetHoverForActionButton(2);
        SetHoverForActionButton(3);
    }

    private void SetHoverForActionButton(int index) {
        Button button = actionButtons[index];
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            button.gameObject.AddComponent<EventTrigger>();
            trigger = button.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnHoverEnter(index); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { OnHoverExit(); });
        trigger.triggers.Add(exit);

        index++;        
    }

    private void OnHoverEnter(int actionIndex) {
        hoverActionButton(actionIndex-1);
        AudioManager.Play("UI_Hover");
    }

    private void OnHoverExit() {
        // TODO: Use Default 
    } 

}