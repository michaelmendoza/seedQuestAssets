using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeedQuest.Interactables;

public class SettingsManager : MonoBehaviour {
    
    static public bool useDebug = false;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float soundEffectVolume = 1.0f;
    public float cameraSensitivity = 1.0f;
    public bool muteVolume = false;
    public InteractableConfigData interactableDefaultConfig;

    static private SettingsManager __instance = null;
    static public SettingsManager Instance
    {
        get
        {
            if (__instance == null)
                __instance = GameObject.FindObjectOfType<SettingsManager>();
            return __instance;
        }
    }

    public void Start() {
        ApplyDefaultInteractableSettings();
    }

    static public float MasterVolume {
        get { return Instance.masterVolume; }
        set { Instance.masterVolume = value; AudioManager.UpdateAudioSettings(); }
    }

    static public float MusicVolume {
        get { return Instance.musicVolume; }
        set { Instance.musicVolume = value; AudioManager.UpdateAudioSettings(); }
    }

    static public float SoundEffectVolume {
        get { return Instance.soundEffectVolume; }
        set { Instance.soundEffectVolume = value; AudioManager.UpdateAudioSettings(); }
    }

    static public bool IsVolumeMuted {
        get { return Instance.muteVolume; }
        set { Instance.muteVolume = value; AudioManager.UpdateAudioSettings(); }
    }

    static public float CameraSensitivity
    {
        get { return Instance.cameraSensitivity; }
        set { Instance.cameraSensitivity = value; }
    }

    static public bool ApplyDefaultInteractableSettings() {
        if (Instance.interactableDefaultConfig != null) {
            Instance.interactableDefaultConfig.ApplyConfiguration();
            Debug.Log("Apply Interactable Default Settings --- Seed Hex Size:" + InteractableConfig.SeedHexSize);
            return true;
        }
        else {
            return false;
        }
    }
}
