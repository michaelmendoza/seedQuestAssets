using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeedQuest.Interactables;

[CreateAssetMenu(menuName = "Config/ConfigData")]
public class ConfigData : ScriptableObject {
    public int SiteBits = 4;
    public int InteractableBits = 4;
    public int ActionBits = 2;
    public int ActionsPerSite = 3;
    public int SitesPerGame = 6;

    public void ApplyConfiguration()
    {
        InteractableConfig.SiteBits = SiteBits;
        InteractableConfig.InteractableBits = InteractableBits;
        InteractableConfig.ActionBits = ActionBits;
        InteractableConfig.ActionsPerSite = ActionsPerSite;
        InteractableConfig.SitesPerGame = SitesPerGame;
    }
}