using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardConstants : MonoBehaviour
{
        public int exhastionTickFreq;

        public float inactiveTime;

        public float exhastSlowdownWeight;
        
        [Header("Boid Values")]
        
        [Tooltip("Radius that agents will detect neighbours within")]
        public float neighbourDetectionRadius;

        [Tooltip("Radius that agents will actually avoid neighbours")]
        public float neighbourAvoidanceRadius;
        
        [Tooltip("Weighting for how hard an agent should try to avoid neighbour")]
        public float neighbourAvoidanceWeight;

        [Tooltip("Weighting for how hard the force towards the snitch is")]
        public float snitchFollowWeight;

        [Tooltip("Weighting for how hard the force to avoid the environment is.")]
        public float environmentAvoidanceWeight;
        public float environmentAvoidanceRadius;
        
        
        [Tooltip("Speed modifier to speed up or slow down all agents")]
        public float speedTuningValue;

        public float maxSteeringForce;
}
