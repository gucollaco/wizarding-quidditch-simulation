using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeamTraits", menuName = "ScriptableObjects/TeamTraitsScriptableObject")]
public class TeamTraits : ScriptableObject
{
    public string identifier;
    public Color color;
    public int points;
    public int weightMean;
    public int weightStdDeviation;
    public int maxVelocityMean;
    public int maxVelocityStdDeviation;
    public int aggressivenessMean;
    public int aggressivenessStdDeviation;
    public int maxExhaustionMean;
    public int maxExhaustionStdDeviation;
    public int initialYRotation;
}
