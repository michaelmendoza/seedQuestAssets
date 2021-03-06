﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SeedQuest.Interactables
{
    public enum InteractableUIMode { NextPrevSelect, GridSelect, ListSelect };

    [System.Serializable]
    public class InteractableUI
    { 
        public string name = "";
        public int fontSize = 36;
        public float scaleSize = 1;
        public InteractableUIMode mode;
        public bool useRotateToCamera = true;
        public Vector3 rotationOffset = new Vector3(0, 0, 0);
        public Vector3 positionOffset = new Vector3(0, 0, 0);

        private Interactable parent;
        private GameObject actionUI = null;
        private Button labelButton;
        private Button[] actionButtons;
        private Button checkButton;
        private Image[] checkImages;
        //private bool showCheckButton = false;
        //private Image checkmark;

        /// <summary> Initialize Interactable UI with Prompt Text and Buttons </summary>
        /// <param name="interactable">Parent Interactable Object</param>
        public void Initialize(Interactable interactable)
        {
            parent = interactable;

            if (interactable.flagDeleteUI)
                return;

            int modeIndex = 0;
            modeIndex = mode == InteractableUIMode.GridSelect ? 1 : modeIndex;
            modeIndex = mode == InteractableUIMode.ListSelect ? 2 : modeIndex;

            actionUI = GameObject.Instantiate(InteractableManager.Instance.actionSpotIcons[modeIndex], InteractableManager.Instance.transform);
            SetScale();
            SetPosition();
            SetupLabelButton();
            SetupActionButtons();
            SetupCheckButton();
        }

        public void Update()
        {
            if (isReady())
            {
                SetPosition();
                SetRotation();
            }
        }

        public bool isReady()
        {
            return actionUI != null;
        }

        public void DeleteUI() {
            GameObject.Destroy(actionUI);
        }

        public void SetupLabelButton() {

            var textList = actionUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            textList[0].text = parent.Name;

            Button[] buttons = actionUI.GetComponentsInChildren<Button>();
            labelButton = buttons[0];
            labelButton.onClick.AddListener(onClickLabel);

            setLabelHoverEvents();
        }

        public void SetupActionButtons()
        {
            var textList = actionUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach (TMPro.TextMeshProUGUI text in textList)
                text.fontSize = fontSize;

            Button[] buttons = actionUI.GetComponentsInChildren<Button>();

            if (mode == InteractableUIMode.NextPrevSelect)  {
                actionButtons = new Button[buttons.Length - 2];
            }
            else if (mode == InteractableUIMode.GridSelect || mode == InteractableUIMode.ListSelect) {
                actionButtons = new Button[buttons.Length - 1];
                checkImages = new Image[buttons.Length - 1];
            }

            System.Array.Copy(buttons, 1, actionButtons, 0, actionButtons.Length);

            if (mode == InteractableUIMode.NextPrevSelect) {
                actionButtons[0].onClick.AddListener(parent.NextAction);
                actionButtons[1].onClick.AddListener(parent.PrevAction);
            }
            else if (mode == InteractableUIMode.GridSelect || mode == InteractableUIMode.ListSelect) {
                for (int i = 0; i < 4; i++) {
                    var actionText = actionButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    actionText.text = parent.stateData.getStateName(i);

                    checkImages[i] = actionButtons[i].gameObject.GetComponentsInChildren<Image>()[1];
                }

                foreach (Image image in checkImages) {
                    image.gameObject.SetActive(false);
                }

                actionButtons[0].onClick.AddListener(delegate { ClickActionButton(0); });
                actionButtons[1].onClick.AddListener(delegate { ClickActionButton(1); });
                actionButtons[2].onClick.AddListener(delegate { ClickActionButton(2); });
                actionButtons[3].onClick.AddListener(delegate { ClickActionButton(3); });
            }

            hideActions();

            // Create Triggers for HoverEvents
            foreach (Button button in actionButtons) {
                setButtonHoverEvents(button);
            }
        } 

        public void SetupCheckButton() {
            if (mode == InteractableUIMode.NextPrevSelect) {
                Button[] buttons = actionUI.GetComponentsInChildren<Button>();
                checkButton = buttons[1];
                checkButton.onClick.AddListener(onClickCheck);
                checkButton.gameObject.SetActive(false);
            }
            else if (mode == InteractableUIMode.GridSelect || mode == InteractableUIMode.ListSelect) {
                //Button[] buttons = actionUI.GetComponentsInChildren<Button>();
                //checkButton = buttons[1];
                return;
            }
        }

        public void setButtonHoverEvents(Button button) {
            EventTrigger trigger = button.GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                if (PauseManager.isPaused == true) return;
                GameManager.State = GameState.Interact;
            });
            trigger.triggers.Add(entry);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => {
                if (PauseManager.isPaused == true) return;
                GameManager.State = GameState.Play;
            });
            trigger.triggers.Add(exit);
        }

        public void setLabelHoverEvents() {
            EventTrigger trigger = labelButton.GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                if (PauseManager.isPaused == true) return;
                GameManager.State = GameState.Interact;
            });
            trigger.triggers.Add(entry);

            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((data) => {
                if (PauseManager.isPaused == true) return;
                GameManager.State = GameState.Play;
            });
            trigger.triggers.Add(exit);
        }

        public void hideActions() {
            if (actionButtons == null) return;

            foreach (Button button in actionButtons)
            {
                button.transform.gameObject.SetActive(false);
            }
        }

        public void showActions() {
            if (actionButtons == null) return;

            foreach (Button button in actionButtons)
            {
                button.transform.gameObject.SetActive(true);
            }
        }

        public void showCurrentActions()
        {
            InteractableManager.hideAllInteractableUI();
            showActions();
            SetCheckImageActive();

            string label = GetText();
            InteractableManager.resetInteractableUIText();
            SetText(label);
        }

        public void onClickLabel()
        {
            parent.NextAction();
        }

        public void ClickActionButton(int actionIndex)
        {
            parent.DoAction(actionIndex);

            if (mode == InteractableUIMode.GridSelect || mode == InteractableUIMode.ListSelect) {
                hideActions();
            }

            if (GameManager.Mode == GameMode.Rehearsal) {
                if(actionIndex == InteractablePath.NextInteractable.ID.actionID) {
                    InteractablePath.GoToNextInteractable();
                }
            }
            else if (GameManager.Mode == GameMode.Recall)
                InteractableLog.Add(parent, parent.currentStateID);
        }

        public void onClickCheck() {
            SetCheckButtonActive(false);

            if(GameManager.Mode == GameMode.Rehearsal)
                InteractablePath.GoToNextInteractable();
            else if(GameManager.Mode == GameMode.Recall)
                InteractableLog.Add(parent, parent.currentStateID);
        }

        public void onHoverUI()
        {
            GameManager.State = GameState.Interact;
            showCurrentActions();
            InteractableManager.SetActiveInteractable(parent);
        }

        public void offHoverUI()
        {
            GameManager.State = GameState.Play;
        }

        public void toggleActions()
        {
            bool isShown = actionButtons[0].gameObject.activeSelf;
            if (isShown)
                hideActions();
            else
                showActions();
        }

        public void SetScale()
        {
            actionUI.GetComponent<RectTransform>().localScale = new Vector3(-0.01f * scaleSize, 0.01f * scaleSize, 0.01f * scaleSize);
        }

        public void SetPosition()
        {
            Vector3 labelPositionOffset = Vector3.zero;
            if (parent.stateData != null) labelPositionOffset = parent.stateData.labelPosOffset;
            Vector3 position = parent.transform.position + labelPositionOffset + positionOffset;
            actionUI.GetComponent<RectTransform>().position = position;
        }

        public void SetRotation()
        {
            if (useRotateToCamera)
            {
                BillboardInteractable();
                actionUI.GetComponent<RectTransform>().Rotate(rotationOffset);
            }
            else
            {
                actionUI.GetComponent<RectTransform>().rotation = Quaternion.Euler(rotationOffset);
            }
        }

        public void BillboardInteractable()
        {
            Vector3 targetPosition = Camera.main.transform.position - (100 * Camera.main.transform.forward ) ;
            Vector3 interactablePosition = actionUI.transform.position;
            Vector3 lookAtDir = targetPosition - interactablePosition;

            Quaternion rotate = Quaternion.LookRotation(lookAtDir);
            actionUI.transform.rotation = rotate;
        }

        public void SetText(string text)
        {
            if (actionUI == null) return;
            var textMesh = actionUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.text = text;
        }

        public string GetText() {
            return actionUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        }

        private void SetCheckButtonActive(bool active) {
            if (mode == InteractableUIMode.NextPrevSelect)
                checkButton.gameObject.SetActive(active);
        }

        private void SetCheckImageActive() {
            if (mode == InteractableUIMode.GridSelect || mode == InteractableUIMode.ListSelect) {
                if (InteractablePath.isNextInteractable(parent))
                    checkImages[InteractablePath.NextInteractable.ID.actionID].gameObject.SetActive(true);
                else {
                    checkImages[0].gameObject.SetActive(false);
                    checkImages[1].gameObject.SetActive(false);
                    checkImages[2].gameObject.SetActive(false);
                    checkImages[3].gameObject.SetActive(false);
                }
            }
        }

        public void SetCheckmark(int actionIndex) {
            if (GameManager.Mode == GameMode.Rehearsal) {
                SetCheckImageActive();
                
                if (InteractablePath.isNextInteractable(parent) && actionIndex == InteractablePath.NextInteractable.ID.actionID) 
                    SetCheckButtonActive(true);
                else
                    SetCheckButtonActive(false);
            }
            else if (GameManager.Mode == GameMode.Recall)
            {
                SetCheckButtonActive(true);
            }
        }

        public void SetActionUI(int actionIndex) {
            InteractableState state = parent.stateData.states[actionIndex];
            //SetText(parent.Name + ":\n "+ state.actionName);
            SetText(state.actionName);
            SetCheckmark(actionIndex);
        }

    }
}