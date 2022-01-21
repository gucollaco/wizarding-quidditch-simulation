using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "ScriptableObjects/GameSettingsScriptableObject")]
public class GameSettings : ScriptableObject
{
    public int targetPoints;
    public int wizardsQuantityPerTeam;
}
