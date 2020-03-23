﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using SeedQuest.Interactables;
using SeedQuest.SeedEncoder;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Runtime.InteropServices;
using QRCoder;
using QRCoder.Unity;
using sharpPDF;
using System.Collections;
using System.Collections.Generic;

public class EndGameUI : MonoBehaviour
{
    #if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void Copy(string copy_str);

        [DllImport("__Internal")]
        private static extern void Download(string file, string content);

        [DllImport("__Internal")]
        private static extern void Print(string str);
    #endif

    static private EndGameUI instance = null;
    static private EndGameUI setInstance() {
        if (GameManager.MobileMode)
            instance = MobileHUDManager.Instance.GetComponentInChildren<EndGameUI>(true);
        else
            instance = HUDManager.Instance.GetComponentInChildren<EndGameUI>(true);
        return instance;
    }
    static public EndGameUI Instance { get { return instance == null ? setInstance() : instance; } }

    public string PrototypeSelectScene = "PrototypeSelect";
    public string RehearsalScene = "PrototypeSelect";
    public string RecallScene = "PrototypeSelect";

    private static string hexSeed;
    private static string bipSeed;
    private static string asciiSeed;

    private GameObject characterButton;
    private GameObject wordsButton;
    private GameObject asciiButton;

    private void getButtonRefs()
    {
        Button[] buttons = Instance.GetComponentsInChildren<Button>();
        characterButton = buttons[4].gameObject;
        wordsButton = buttons[5].gameObject;
        asciiButton = buttons[6].gameObject;
    }

    /// <summary> Toggles On the EndGameUI </summary>
    static public void ToggleOn() {
        if (Instance.gameObject.activeSelf)
            return;

        Instance.gameObject.SetActive(true);
        SeedQuest.Level.LevelManager.Instance.StopLevelMusic();
        AudioManager.Play("KeyOutput");
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        SeedConverter converter = new SeedConverter();
        BIP39Converter bpc = new BIP39Converter();

        if (InteractableConfig.SeedHexLength % 2 == 1)
        {
            string alteredSeedText = converter.DecodeSeed();
            string sentence = getSentence(alteredSeedText);
            string alteredHex = SeedUtility.hexToAsciiLengthCheck(alteredSeedText);
            asciiSeed = AsciiConverter.hexToAscii(alteredHex);
            if (asciiSeed.Substring(asciiSeed.Length -1, 1) == "=")
            {
                char[] asciiArray = asciiSeed.ToCharArray();
                Array.Reverse(asciiArray);
                Queue<char> asciiQueue = new Queue<char>(asciiArray);
                while (asciiQueue.Peek() == '=')
                {
                    asciiQueue.Dequeue();
                }
                asciiArray = asciiQueue.ToArray();
                Array.Reverse(asciiArray);
                asciiSeed = new string(asciiArray);
            }

            char[] array = alteredSeedText.ToCharArray();
            array[array.Length - 2] = array[array.Length - 1];
            alteredSeedText = new string(array);
            if (alteredSeedText.Length > 1)
                alteredSeedText = alteredSeedText.Substring(0, (alteredSeedText.Length - 1));

            hexSeed = alteredSeedText;
            bipSeed = sentence;
            if (GameManager.MobileMode)
            {
                textList[0].text = hexSeed;
            }
            else
                textList[0].text = sentence;
        }
        else
        {
            hexSeed = converter.DecodeSeed();
            bipSeed = getSentence(hexSeed);
            asciiSeed = AsciiConverter.hexToAscii(hexSeed);
            if (GameManager.MobileMode)
            {
                textList[0].text = hexSeed;
            }
            else
            {
                textList[0].text = bipSeed;
            }

        }

        if (GameManager.Mode == GameMode.Rehearsal)
        {
            if (GameManager.MobileMode)
                return;
            textList[3].text = "Practice Again";
        }
        else
        {
            if (GameManager.MobileMode)
                return;
            textList[3].text = "Back to Start Screen";
        }

        setupCharacterMode();
    }

    /// <summary> Handles selecting PrototypeSelect Button </summary>
    public void PrototypeSelect()
    {
        LoadingScreenUI.LoadScene(PrototypeSelectScene, true);
    }

    /// <summary> Handles selecting Rehearsal Button </summary>
    public void Rehearsal()
    {
        LoadingScreenUI.LoadRehearsal(RehearsalScene, true);
    }

    /// <summary> Handles selecting Recall Button </summary>
    public void Recall()
    {
        LoadingScreenUI.LoadRecall(RecallScene, true);
    }

    public void GoToStartScreen() {
        SeedQuest.Level.LevelManager.Instance.StopLevelMusic();
        if (GameManager.V2Menus)
            if (GameManager.MobileMode)
            {
                InteractableLabelUI.ClearInteractableUI();
                MobileMenuScreens.Instance.GoToStart();
            }
            else
                MenuScreenV2.Instance.GoToStart();
        else
            MenuScreenManager.ActivateStart();
        gameObject.SetActive(false);
        GameManager.ResetGraduatedRehearsal();
    }

    public void ResetPlaythrough()
    {
        InteractablePathManager.Reset();
        if (GameManager.MobileMode)
            MobileMenuScreens.Instance.GoToSceneLineUp();
        else
            MenuScreenManager.ActivateSceneLineUp();
        gameObject.SetActive(false);
    }

    public void copySeed()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        string seed = textList[0].text;

        #if UNITY_WEBGL
            Copy(seed);
        #else
            GUIUtility.systemCopyBuffer = seed;
        #endif

        textList[1].text = "Seed Copied!";
        textList[1].gameObject.SetActive(true); 
    }

    public void copyHexSeed()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        BIP39Converter bpc = new BIP39Converter();
        string seed = bpc.getHexFromSentence(textList[0].text);

        #if UNITY_WEBGL
            Copy(seed);
        #else
            GUIUtility.systemCopyBuffer = seed;
        #endif

        textList[1].text = "Seed Copied!";
        textList[1].gameObject.SetActive(true);
    }

    public static string getSentence(string hex)
    {
        string sentence;
        BIP39Converter bpc = new BIP39Converter();
        if (InteractableConfig.SitesPerGame < 6)
            sentence = bpc.getShortSentenceFromHex(hex, InteractableConfig.SitesPerGame * 2);
        else
            sentence = bpc.getSentenceFromHex(hex);

        return sentence;
    }

    public void downloadSeed()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        BIP39Converter bpc = new BIP39Converter();
        string seed = bipSeed + "\n0x" + hexSeed + "\n" + asciiSeed;

        //QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //QRCodeData qrCodeData = qrGenerator.CreateQrCode(textList[0].text, QRCodeGenerator.ECCLevel.Q);
        //UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        //Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);

        //byte[] bytes = qrCodeAsTexture2D.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/../SavedQRCode.png", bytes);

        #if UNITY_WEBGL
            Download("seed.txt", seed);
        #elif UNITY_EDITOR
            string path = EditorUtility.SaveFilePanel("Save As", "Downloads", "seed", "txt");
            if (path.Length != 0)
            {
                using (StreamWriter outputFile = new StreamWriter(path))
                {
                    outputFile.WriteLine(seed);
                }
            }
        #elif UNITY_IOS || UNITY_ANDROID
            string realPath = Application.persistentDataPath + "/SeedQuest/seed.txt";
            if (!System.IO.File.Exists(realPath))
            {
                if (!System.IO.Directory.Exists(Application.persistentDataPath + "/SeedQuest/"))
                {
                    System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/SeedQuest/");
                }
                WWW reader = new WWW(Application.streamingAssetsPath + "/PATH/" + realPath);
                while ( ! reader.isDone) {}
                System.IO.File.WriteAllBytes(realPath, reader.bytes);
            }
            Application.OpenURL(realPath);
        #else
            string downloads = "";
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string home = System.Environment.GetEnvironmentVariable("HOME");
                downloads = System.IO.Path.Combine(home, "Downloads");
            }
            else
            {
                downloads = System.Convert.ToString(Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty));
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(downloads, "seed.txt")))
            {
                outputFile.WriteLine(seed);
            }
#endif
        //textList[1].text = "Seed Downloaded";
        //textList[1].gameObject.SetActive(true);
    }

    public static void setupCharacterMode()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textList[0].text = hexSeed;

        Button[] buttons = Instance.GetComponentsInChildren<Button>();
        GameObject charButton = buttons[4].gameObject;
        GameObject wordButton = buttons[5].gameObject;
        GameObject asciiButton = buttons[6].gameObject;

        charButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        charButton.GetComponent<Image>().color = new Color32(55, 90, 122, 255);
        wordButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(89, 89, 89, 255);
        wordButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void characterMode()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textList[0].text = hexSeed;

        if (characterButton == null)
            getButtonRefs();

        setButtonColors(characterButton);
    }

    public void wordsMode()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textList[0].text = bipSeed;

        if (characterButton == null)
            getButtonRefs();

        setButtonColors(wordsButton);
    }

    public void asciiMode()
    {
        var textList = Instance.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        textList[0].text = asciiSeed;

        if (characterButton == null)
            getButtonRefs();

        setButtonColors(asciiButton);
    }

    public void pdfTest()
    {
        RawImage rawImage = gameObject.AddComponent<RawImage>();

        string sentence = bipSeed;
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(sentence, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);

        rawImage.texture = qrCodeAsTexture2D;
        byte[] bytes = qrCodeAsTexture2D.EncodeToJPG(); // .EncodeToPNG();

        pdfDocument myDoc = new sharpPDF.pdfDocument("qr_pdf_test", "qr tester");
        pdfPage myPage = myDoc.addPage(500, 500);
        myPage.addImage(bytes, 1, 150, 200, 200);

        myPage.addText("Your seed entropy is: ", 10, 470, sharpPDF.Enumerators.predefinedFont.csCourier, 15);
        myPage.addText(hexSeed, 10, 450, sharpPDF.Enumerators.predefinedFont.csCourier, 15);
        myPage.addText(bipSeed, 10, 425, sharpPDF.Enumerators.predefinedFont.csCourier, 10);

        // need to change this code depending on the current operating system/build type
        myDoc.createPDF("qr_pdf_test.pdf");
        myPage = null;
        myDoc = null;
    }

    private void setButtonColors(GameObject selectedButton)
    {
        characterButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(89, 89, 89, 255);
        characterButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        wordsButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(89, 89, 89, 255);
        wordsButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        asciiButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(89, 89, 89, 255);
        asciiButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        selectedButton.GetComponent<Image>().color = new Color32(55, 90, 122, 255);
    }

}