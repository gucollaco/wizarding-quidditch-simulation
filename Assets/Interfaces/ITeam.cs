using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam
{
    int WeightMean { get; set; }
    int WeightStdDeviation { get; }
    int MaxVelocityMean { get; }
    int MaxVelocityStdDeviation { get; }
    int AggressivenessMean { get; }
    int AggressivenessStdDeviation { get; }
    int MaxExhaustionMean { get; }
    int MaxExhaustionStdDeviation { get; }
}
