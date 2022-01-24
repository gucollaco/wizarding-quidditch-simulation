using UnityEngine;
using System;

public class BoxMuller : MonoBehaviour
{
    // https://stackoverflow.com/questions/218060/random-gaussian-variables
    private static System.Random random = new System.Random();

    public static float GaussianFloat(float mean, float stdDeviation)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + stdDeviation * randStdNormal;
        return (float) (randNormal);
    }
}
