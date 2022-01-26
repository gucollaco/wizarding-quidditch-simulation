using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "ScriptableObjects/GameSettingsScriptableObject")]
public class GameSettings : ScriptableObject
{   
    public int targetPoints;
    public int wizardsQuantityPerTeam;

    // Wizard related
    public int wizardFollowSnitchWeight; //1 //2
    public int wizardSeperationRadiusThreshold; //1 //1
    public int wizardSeperationForceWeight; //1 //2
    public int wizardCollisionAvoidanceRadiusThreshold; //1 //20
    public int wizardCollisionAvoidanceForceWeight; //5 //1
    public float wizardMaxSteerForce; //1.5 //10
    public float wizardUnconsciousTime; //1.0f //1.0f
    public float wizardExhaustionTick; //3.0f //3.0f


    // Snitch related
    public float snitchDirectionTimer; //1 //3

    public float snitchMaxSteerForce; //50 //50

    public float snitchMaxVelocity; //10 //10

    public float snitchMinVelocity; //5 //5

    public float snitchSpeed; //0.5 //5.0

    public float snitchRandomDirectionWeight; //0.5 //1

    public float snitchCollisionRadiusDetection; //10 //10

    public float snitchCollisionAvoidanceWeight; //1 //2
}
