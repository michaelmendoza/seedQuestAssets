﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SeedQuest.Interactables;

public class SeedSetupCanvas : MonoBehaviour
{

    private BIP39Converter bpc = new BIP39Converter();
    private bool showPassword = true;

    public Image greenCheck;
    public Image redWarning;
    public Image greenOutline;
    public Image redOutline;
    public Sprite hide;
    public Sprite show;
    public Button hidePasswordButton;
    public Button HideKeyButton;
    public TMP_InputField seedInputField;
    public TextMeshProUGUI warningTextTMP;
    public PasswordEntropyUI passwordBar;

    private bool passwordMode = false;
    private SeedStrSelection seedStrSelection;
    private GameObject randomGenerators;

    static private SeedSetupCanvas instance = null;
    static private SeedSetupCanvas setInstance() {instance = Resources.FindObjectsOfTypeAll< SeedSetupCanvas >()[0]; return instance; }
    static public SeedSetupCanvas Instance { get { return instance == null ? setInstance() : instance; } }

    static public bool PasswordMode
    {
        get { return Instance.passwordMode; }
        set { Instance.passwordMode = value; }
    }

    private void Awake()
    {
        passwordBar = GetComponentInChildren<PasswordEntropyUI>();
        seedStrSelection = GameObject.FindObjectOfType<SeedStrSelection>();
        Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
        randomGenerators = buttons[2].transform.parent.gameObject;
    }

    private void Update()
    {

        if (!passwordMode)
        {
            togglePassword(false);
        }
        else
        {
            togglePassword(true);
        }

        SeedStrSelection seedStr = GetComponentInChildren<SeedStrSelection>(true);

        if (seedStr != null)
        {
            SeedStrUpdate(seedStr);
        }
        else
        {
            if (InteractableConfig.SitesPerGame != 6)
                InteractableConfig.SitesPerGame = 6;
            checkInputSeed();
        }
        
    }

    public void SeedStrUpdate(SeedStrSelection seedStr)
    {
        bool doUpdate = seedStr.updateFlag;

        if (doUpdate)
        {
            seedInputField.text = "";
            checkInputSeed();
            GetComponentInChildren<SeedStrSelection>().updateFlag = false;
        }
    }

    public void Back() {
        MenuScreenV2.Instance.GoToStart();
    }

    public void FindKey() {
        EncodeSeed();
        MenuScreenV2.Instance.GoToEncodeSeed();
    }

    public void FindKeyMobile()
    {
        EncodeSeed();
        MobileMenuScreens.Instance.GoToSceneSelection();
    }

    public void SetRandomSeed() {
        InteractablePathManager.SetRandomSeed();
        seedInputField.text = InteractablePathManager.SeedString;
    }

    public void SetRandomBIP39Seed() {
        InteractablePathManager.SetRandomSeed();
        if(InteractableConfig.SitesPerGame == 6)
            seedInputField.text = bpc.getSentenceFromHex(InteractablePathManager.SeedString);
        else
        {
            int wordCount = InteractableConfig.SitesPerGame * 2;
            if (wordCount <= 0)
                Debug.Log("Error: word count should not be less than or equal to zero");
            Debug.Log("Word count: " + wordCount);

            seedInputField.text = bpc.getShortSentenceFromHex(InteractablePathManager.SeedString, wordCount);
        }

    }

    // Check the user's input to verify that it's a valid seed
    public void checkInputSeed()
    {
        //Debug.Log("Hello from checkInputSeed()");

        string seed = SeedUtility.removeHexPrefix(seedInputField.text);
        bool validSeed = validSeedString(seed);

        if (SeedUtility.validAscii(seedInputField.text) && passwordMode)
        {
            Debug.Log("Valid ascii seed: " + seed);
            warningTextTMP.text = "Character seed detected!";
            warningTextTMP.color = new Color32(81, 150, 55, 255);
            setGreenCheck();
            passwordBar.SetNonPassword(false);
        }
        else if (SeedUtility.validAscii(seedInputField.text) && !passwordMode)
        {
            Debug.Log("Invalid hex seed: " + seed);
            warningTextTMP.text = "Go to password mode for passwords.";
            warningTextTMP.color = new Color32(255, 20, 20, 255);
            setRedWarning();
            passwordBar.SetNonPassword(false);
        }
        else if (validSeed)
        {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for hex seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                Debug.Log("Valid hex seed: " + seed);
                warningTextTMP.text = "Hex seed detected!";
                warningTextTMP.color = new Color32(81, 150, 55, 255);
                setGreenCheck();
                passwordBar.SetNonPassword(true);
            }
        }
        else if (SeedUtility.validBip(seedInputField.text))
        {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for bip-39 seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                Debug.Log("Valid bip39 seed: " + seed);
                warningTextTMP.text = "Word seed detected!";
                warningTextTMP.color = new Color32(81, 150, 55, 255);
                setGreenCheck();
                passwordBar.SetNonPassword(true);
            }
        }
        else
        {
            if (seed.Length < 1)
            {
                return;
            }
            else if (!passwordMode)
            {
                Debug.Log("Invalid hex seed: " + seed);
                warningTextTMP.text = "Go to password mode for passwords.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
                passwordBar.SetNonPassword(false);
                return;
            }
            else if (seed.Length > InteractableConfig.BitEncodingCount / 8)
            {
                warningTextTMP.text = "Too many characters!";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
                return;
            }
            int encodingLength = InteractableConfig.BitEncodingCount / 8;
            string paddedSeed = seed;
            if (seed.Length < encodingLength)
            {
                int paddingLength = encodingLength - seedInputField.text.Length;
                for (int i = 0; i < paddingLength; i++)
                {
                    paddedSeed += "=";
                }
            }
            Debug.Log("Valid ascii seed: " + paddedSeed);
            warningTextTMP.text = "Character seed detected!";
            warningTextTMP.color = new Color32(81, 150, 55, 255);
            setGreenCheck();
            passwordBar.SetNonPassword(false);
        }

    }

    public void EncodeSeed()
    {
        if (GameManager.Mode == GameMode.Rehearsal)
        {
            string seedFromInput = seedInputField.text;
            string hexSeed = "";
            int encodingLength = InteractableConfig.BitEncodingCount / 8;
            string paddedSeed = seedFromInput;

            if (seedFromInput.Length < encodingLength)
            {
                int paddingLength = encodingLength - seedInputField.text.Length;
                for (int i = 0; i < paddingLength; i++)
                {
                    paddedSeed += "=";
                }
            }

            if (!SeedUtility.detectHex(seedFromInput) && !SeedUtility.validAscii(seedFromInput) && SeedUtility.validBip(seedFromInput) && InteractableConfig.SitesPerGame < 6)
            {
                hexSeed = bpc.getHexFromShortSentence(seedFromInput, InteractableConfig.SitesPerGame * 2);
            }
            else if (!SeedUtility.detectHex(seedFromInput) && !SeedUtility.validAscii(seedFromInput) && SeedUtility.validBip(seedFromInput))
            {
                hexSeed = bpc.getHexFromSentence(seedFromInput);
            }
            else if (SeedUtility.validAscii(seedFromInput))
            {
                hexSeed = AsciiConverter.asciiToHex(seedFromInput);
                hexSeed = SeedUtility.asciiToHexLengthCheck(hexSeed);
            }
            else if (SeedUtility.validAscii(paddedSeed))
            {
                hexSeed = AsciiConverter.asciiToHex(paddedSeed);
                hexSeed = SeedUtility.asciiToHexLengthCheck(hexSeed);
            }
            else
            {
                hexSeed = seedFromInput;
                if (InteractableConfig.SeedHexLength % 2 == 1)
                {
                    if (seedFromInput.Length == InteractableConfig.SeedHexLength)
                    {
                        string seedText = seedFromInput + "0";
                        char[] array = seedText.ToCharArray();
                        array[array.Length - 1] = array[array.Length - 2];
                        array[array.Length - 2] = '0';
                        hexSeed = new string(array);
                    }
                    else if (seedFromInput.Length == InteractableConfig.SeedHexLength + 1)
                    {
                        char[] array = seedFromInput.ToCharArray();
                        array[array.Length - 2] = '0';
                        hexSeed = new string(array);
                    }
                    else
                        Debug.Log("Seed: " + hexSeed);
                }
            }
            Debug.Log("Seed: " + hexSeed);

            InteractablePathManager.SeedString = hexSeed;
            int[] siteIDs = InteractablePathManager.GetPathSiteIDs();

        }
    }

    // Check a given string to see if it's either a valid seed phrase or hex seed
    public bool validSeedString(string seedString)
    {
        bool validHex = SeedUtility.validHex(seedString);
        bool detectAscii = SeedUtility.detectAscii(seedString);
        int asciiLength = ((InteractableConfig.BitEncodingCount) / 8);

        string[] wordArray = seedString.Split(null);

        if (seedString == "" || seedString.Length < 1) {
            deactivateCheckSymbols();
            deactivateHideKeyButton();
            warningTextTMP.text = "";
            validHex = false;
        }
        else if (!validHex && !detectAscii && wordArray.Length > 1 && wordArray.Length != ((InteractableConfig.SitesPerGame * 2 )) && InteractableConfig.SitesPerGame < 6)
        {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for bip-39 seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                Debug.Log("array length: " + wordArray.Length + " word req: " + InteractableConfig.SitesPerGame * 2);
                warningTextTMP.text = "Remember to add spaces between the words.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }
        else if (!validHex && !detectAscii && wordArray.Length > 1 && wordArray.Length < 12 && InteractableConfig.SitesPerGame == 6) {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for bip-39 seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                Debug.Log("array length: " + wordArray.Length);
                warningTextTMP.text = "Remember to add spaces between the words.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }
        else if (!validHex && !detectAscii && wordArray.Length > 1 && !SeedUtility.validBip(seedString)) {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for bip-39 seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                warningTextTMP.text = "Make sure the words are spelled correctly.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }
        else if (detectAscii && seedString.Length < asciiLength && !validHex)
        {
            /*warningTextTMP.text = "Not enough characters!";
            warningTextTMP.color = new Color32(255, 20, 20, 255);
            setRedWarning();*/
        }
        else if (detectAscii && !passwordMode)
        {
            warningTextTMP.text = "Go to password mode for passwords.";
            warningTextTMP.color = new Color32(255, 20, 20, 255);
            setRedWarning();
        }
        else if (detectAscii && seedString.Length > asciiLength && !validHex)
        {
            warningTextTMP.text = "Too many characters!";
            warningTextTMP.color = new Color32(255, 20, 20, 255);
            setRedWarning();
        }
        else if (!validHex) {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for hex seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                warningTextTMP.text = "Character seeds must only contain hex characters.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }
        else if (validHex && seedString.Length < InteractableConfig.SeedHexLength) {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to hex mode for bip-39 seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                validHex = false;
                warningTextTMP.text = "Not enough characters!";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }
        else if (validHex && seedString.Length > InteractableConfig.SeedHexLength + 1) {
            if (passwordMode)
            {
                warningTextTMP.text = "Go to key mode for hex seeds.";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
            else
            {
                validHex = false;
                warningTextTMP.text = "Too many characters!";
                warningTextTMP.color = new Color32(255, 20, 20, 255);
                setRedWarning();
            }
        }

        return validHex;
    }

    // The end game UI removes a superfluous character from hex strings, this 
    //  fixes a UI issue that arises when restarting from end game UI
    public static string checkHexLength(string hexString)
    {
        if (hexString.Length == 34 && hexString[32] == '0')
        {
            hexString = hexString.Substring(0, 32) + hexString.Substring(33, 1);
        }
        Debug.Log("Fixed string: " + hexString);

        return hexString;
    }

    public void deactivateHideKeyButton()
    {
        HideKeyButton.interactable = false;
    }

    public void activateHideKeyButton()
    {
        HideKeyButton.interactable = true;
    }

    public void setGreenCheck()
    {
        redWarning.gameObject.SetActive(false);
        redOutline.gameObject.SetActive(false);
        greenCheck.gameObject.SetActive(true);
        greenOutline.gameObject.SetActive(true);
        activateHideKeyButton();
    }

    public void setRedWarning()
    {
        redWarning.gameObject.SetActive(true);
        redOutline.gameObject.SetActive(true);
        greenCheck.gameObject.SetActive(false);
        greenOutline.gameObject.SetActive(false);
        deactivateHideKeyButton();
    }

    public void deactivateCheckSymbols()
    {
        redWarning.gameObject.SetActive(false);
        redOutline.gameObject.SetActive(false);
        greenCheck.gameObject.SetActive(false);
        greenOutline.gameObject.SetActive(false);
    }

    public void togglePassword(bool value)
    {
        passwordBar.gameObject.SetActive(value);
        if (!GameManager.MobileMode)
        {
            if (value == true)
            {
                seedStrSelection.gameObject.SetActive(false);
                randomGenerators.SetActive(false);
            }
            else
            {
                seedStrSelection.gameObject.SetActive(true);
                randomGenerators.SetActive(true);
            }
        }
    }

    public void hidePassword()
    {
        if (showPassword)
        {
            hidePasswordButton.GetComponent<Image>().sprite = hide;
            seedInputField.contentType = TMP_InputField.ContentType.Password;
            seedInputField.ForceLabelUpdate();
        }

        else
        {
            hidePasswordButton.GetComponent<Image>().sprite = show;
            seedInputField.contentType = TMP_InputField.ContentType.Standard;
            seedInputField.ForceLabelUpdate();
        }

        showPassword = !showPassword;
    }

    public void clearInput()
    {
        seedInputField.text = "";
        deactivateCheckSymbols();
        warningTextTMP.text = "";

    }
} 