using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public bool unconscious = false;

    private Team team;
    private Rigidbody rigid;
    private GameObject snitch;
    private float weight;
    private float maxVelocity;
    private float aggressiveness;
    private float maxExhaust;
    private float currentExhaust;
    private bool hasInitialized = false;
    private Vector3 lastVelocity;
    private float unconsciousTimeValue = 1f;
    private WaitForSeconds unconsciousTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        team = GetComponentInParent<Team>();
        
        if (!team)
            return;

        weight = BoxMuller.GaussianFloat(team.teamTraits.weightMean, team.teamTraits.weightStdDeviation);
        maxVelocity = BoxMuller.GaussianFloat(team.teamTraits.maxExhaustionMean, team.teamTraits.maxVelocityStdDeviation);
        aggressiveness = BoxMuller.GaussianFloat(team.teamTraits.aggressivenessMean, team.teamTraits.aggressivenessStdDeviation);
        maxExhaust = BoxMuller.GaussianFloat(team.teamTraits.maxExhaustionMean, team.teamTraits.maxExhaustionStdDeviation);

        unconsciousTime = new WaitForSeconds(unconsciousTimeValue);
    }

    public void Initialize()
    {
        rigid.mass = weight;
        rigid.AddForce(Vector3.forward);
        hasInitialized = true;
    }

    private void FixedUpdate()
    {
        if (hasInitialized)
        {
            if (!unconscious)
                Conscious();
            else
            {
                Unconscious();
            }
        }
    }

    private void Conscious()
    {
        SnitchAttraction();
    }

    private void Unconscious()
    {
        StartCoroutine(UnconsciousState());
    }

    private IEnumerator UnconsciousState()
    {
        yield return StartCoroutine(WaitUnconscious());
        ChangeState(false);
    }

    private IEnumerator WaitUnconscious()
    {
        yield return unconsciousTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BoxSurface"))
        {
            // checks if the surface is the ground
            if (other.gameObject.ToString().Contains("Bottom"))
            {
                rigid.AddForce(-rigid.velocity);
            }
            else
            {
                rigid.AddForce(-rigid.velocity);
            }
        }
        else if (other.gameObject.CompareTag("WizardPart"))
        {
            Team collidedTeam = other.gameObject.GetComponentInParent<Team>();

            // collided with wizard from the same team
            if (collidedTeam.teamTraits.name == team.teamTraits.name)
            {
                SameTeamCollision(this.gameObject, other.gameObject);
            }
            else // collided with wizard from different team
            {
                DifferentTeamCollision(this.gameObject, other.gameObject);
            }
        }
    }

    private void SameTeamCollision(GameObject objectOne, GameObject objectTwo)
    {
        // calculates the chance of it triggering a DifferentTeamCollision
        float randFloat = UnityEngine.Random.Range(0.0f, 1.0f);
        if (randFloat <= 0.05f)
        {
            DifferentTeamCollision(objectOne, objectTwo);    
        }
    }

    private void DifferentTeamCollision(GameObject objectOne, GameObject objectTwo)
    {
        Wizard wizardOne = objectOne.GetComponent<Wizard>();
        Wizard wizardTwo = objectTwo.GetComponentInParent<Wizard>();

        // both wizards should be conscious
        if (!wizardOne.unconscious && !wizardTwo.unconscious)
        {
            double wizardOneValue = wizardOne.aggressiveness * (BoxMuller.random.NextDouble() * (1.2 - 0.8) + 0.8) * (1 - (wizardOne.currentExhaust / wizardOne.maxExhaust));
            double wizardTwoValue = wizardTwo.aggressiveness * (BoxMuller.random.NextDouble() * (1.2 - 0.8) + 0.8) * (1 - (wizardTwo.currentExhaust / wizardTwo.maxExhaust));

            if (wizardOneValue > wizardTwoValue)
                wizardTwo.ChangeState(true);
            else        
                wizardOne.ChangeState(true);
        }
    }

    public void ChangeState(bool isNowUnconscious)
    {
        // conscious to unconscious
        if (!unconscious && isNowUnconscious)
        {
            unconscious = true;
            lastVelocity = rigid.velocity;
            rigid.useGravity = true;
            rigid.velocity = new Vector3(0, -1, 0);
        }
        // unconscious to conscious
        else if (unconscious && !isNowUnconscious)
        {
            unconscious = false;
            rigid.useGravity = false;
            rigid.velocity = lastVelocity;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
    }

    private void SnitchAttraction()
    {
        Vector3 acceleration = Vector3.zero;

        snitch = GameObject.FindGameObjectWithTag("Snitch");

        if (!snitch)
            return;
         
        Vector3 snitchPosition = snitch.transform.position;
        Vector3 thisPosition = transform.position;

        // add force towards the snitch
        Vector3 snitchDifference = snitchPosition - thisPosition;
        acceleration += snitchDifference.normalized;
        acceleration += NormalizeSteeringForce(snitchDifference) * 3;

        // // add force to distance from neighbours
        // Collider[] neighbours = Physics.OverlapSphere(thisPosition, 20);
        // // deal with neighbours

        acceleration += NormalizeSteeringForce(ComputeSeperationForce());// * Flock.FlockSettings.SeperationForceWeight;
        acceleration += NormalizeSteeringForce(CollisionAvoidanceForce());

        Vector3 newVelocity = rigid.velocity;
        newVelocity += acceleration * Time.deltaTime;
        newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, 3.0f, 6.0f);

        rigid.velocity = newVelocity;
        transform.forward = rigid.velocity.normalized + Vector3.right;
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 ComputeSeperationForce()
    {
        // Initialize seperation force
        Vector3 force = Vector3.zero;

        // Find nearby wizards
        foreach (GameObject wizard in team.wizards)
        {
            if (wizard == this.gameObject || (wizard.transform.position - transform.position).magnitude > 1)
                continue;

            // Repel away
            force += transform.position - wizard.transform.position;
        }

        return force;
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 NormalizeSteeringForce(Vector3 force)
    {
        return force.normalized * Mathf.Clamp(force.magnitude, 0, 10);
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 CollisionAvoidanceForce()
    {
        // Check if heading to collision
        if (!Physics.SphereCast(transform.position, 20, transform.forward, out RaycastHit hitInfo, 20))
            return Vector3.zero;

        // Compute force
        return transform.position - hitInfo.point;
    }
}
