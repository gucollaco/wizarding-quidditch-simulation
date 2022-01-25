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
    private float currentExhaust = 0.0f;
    private bool hasInitialized = false;
    private Vector3 lastVelocity;
    private float unconsciousTimeValue = 1f;
    private WaitForSeconds unconsciousTime;
    private float exhaustTimer = 0.0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        team = GetComponentInParent<Team>();
        
        if (!team)
            return;

        weight = BoxMuller.GaussianFloat(team.teamTraits.weightMean, team.teamTraits.weightStdDeviation);
        maxVelocity = BoxMuller.GaussianFloat(team.teamTraits.maxVelocityMean, team.teamTraits.maxVelocityStdDeviation);
        aggressiveness = BoxMuller.GaussianFloat(team.teamTraits.aggressivenessMean, team.teamTraits.aggressivenessStdDeviation);
        maxExhaust = BoxMuller.GaussianFloat(team.teamTraits.maxExhaustionMean, team.teamTraits.maxExhaustionStdDeviation);

        unconsciousTime = new WaitForSeconds(unconsciousTimeValue);
    }

    public void Initialize()
    {
        rigid.mass = weight;
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
        HandleExhaustion();
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

    private void HandleExhaustion()
    {
        exhaustTimer += Time.deltaTime;
        if (exhaustTimer > 3.0f)
        {
            currentExhaust = UpdateExhaustion(currentExhaust, rigid.velocity.magnitude, maxVelocity);

            Mathf.Clamp(currentExhaust, 0.0f, maxExhaust);
            if (currentExhaust >= maxExhaust)
            {
                ChangeState(true);
            }

            exhaustTimer = 0.0f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BoxSurface"))
        {
            rigid.AddForce(-rigid.velocity);
        }
        else if (other.gameObject.CompareTag("Wizard"))
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
        Wizard wizardTwo = objectTwo.GetComponent<Wizard>();

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
        acceleration += NormalizeSteeringForce(snitchDifference) * 2;

        acceleration += NormalizeSteeringForce(ComputeSeperationForce()) * 2;
        acceleration += NormalizeSteeringForce(CollisionAvoidanceForce());

        float temporaryMaxVelocity = maxVelocity;
        // adjust speed according to the exhaustion
        if ((maxExhaust - currentExhaust) <= 10)
        {
            temporaryMaxVelocity = maxVelocity * Random.Range(0.3f, 0.6f);
        }

        Vector3 newVelocity = rigid.velocity;
        newVelocity += acceleration * Time.deltaTime;
        newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, 0.0f, temporaryMaxVelocity);

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

    private float UpdateExhaustion(float currentExhaust, float currentVelocity, float maxVelocity)
    {
        float velocityRatio = currentVelocity / maxVelocity;
        float randomFloat = Random.Range(0.5f, 1.0f);

        // if player is below half their max velocity, regain some energy
        if (velocityRatio <= 0.5f)
            return currentExhaust + (10 * (-velocityRatio)) * randomFloat;

        return currentExhaust + (10 * velocityRatio) * randomFloat;
    }
}
