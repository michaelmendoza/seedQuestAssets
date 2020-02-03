﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect {
    public string name;
    public GameObject effectPrefab;
}

public class EffectsManager : MonoBehaviour {

    public GameObject effectPrefab;
    public Effect[] effects;

    static EffectsManager __instance = null;

    static public EffectsManager instance {

        get {
            if (__instance == null)
                __instance = GameObject.FindObjectOfType<EffectsManager>();
            return __instance;
        }
    }

    static public void PlayEffect(string name, Transform parent) {
        Effect effect = System.Array.Find(instance.effects, Effect => Effect.name == name);
        ParticleSystem particleSystem = parent.GetComponentInChildren<ParticleSystem>();

        if (effect == null)
            Debug.LogWarning("Effect : " + name + " was not found.");
        //else if(particleSystem != null)
        //   particleSystem.Play();
        else {
            var effectSystem = Instantiate(effect.effectPrefab);
            effectSystem.transform.parent = parent;
            effectSystem.transform.localPosition = Vector3.zero;
            effectSystem.GetComponentInChildren<ParticleSystem>().Play(true);
            //effectSystem.transform.localScale = parent.InverseTransformVector(parent.transform.localScale);
            //effectSystem.transform.localRotation = Quaternion.Inverse(parent.localRotation);
        } 
    }

    static public void PlayEffect(GameObject effectPrefab, Transform parent) {

        //ParticleSystem effect = parent.GetComponentInChildren<ParticleSystem>();
        //if (effect != null)
        //    effect.Play();

        var effectSystem = Instantiate(effectPrefab, parent);
        effectSystem.GetComponentInChildren<ParticleSystem>().Play(true);
    }

    static public void StopEffect(Transform parent) {
        if (parent.GetComponentInChildren<ParticleSystem>() != null)
            parent.GetComponentInChildren<ParticleSystem>().Stop(true);
    }


} 