using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    private Team team;
    private Rigidbody rigid;
    private bool unconscious = false;
    private GameObject snitch;
    private float weight;
    private float maxVelocity;
    private float aggressiveness;
    private float maxExhaust;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        team = GetComponentInParent<Team>();

        weight = BoxMuller.GaussianFloat(team.teamTraits.weightMean, team.teamTraits.weightStdDeviation);
        maxVelocity = BoxMuller.GaussianFloat(team.teamTraits.maxExhaustionMean, team.teamTraits.maxVelocityStdDeviation);
        aggressiveness = BoxMuller.GaussianFloat(team.teamTraits.aggressivenessMean, team.teamTraits.aggressivenessStdDeviation);
        maxExhaust = BoxMuller.GaussianFloat(team.teamTraits.maxExhaustionMean, team.teamTraits.maxExhaustionStdDeviation);
    }

    public void Initialize()
    {
        rigid.mass = weight;
        rigid.AddForce(Vector3.right);
        // rigid.velocity = transform.right * 1;
    }

    private void FixedUpdate()
    {
        Conscious();
        // if (!unconscious)
        //     Conscious();
        // else
        //     Unconscious();
    }

    private void Conscious()
    {
        SnitchAttraction();
    }

    private void Unconscious()
    {

    }

    private void SnitchAttraction()
    {
        Vector3 acceleration = Vector3.zero;

        snitch = GameObject.FindGameObjectWithTag("Snitch");
        Vector3 snitchPosition = snitch.transform.position;
        Vector3 thisPosition = transform.position;

        // add force towards the snitch
        Vector3 snitchDifference = snitchPosition - thisPosition;
        acceleration += snitchDifference.normalized;
        acceleration += NormalizeSteeringForce(snitchDifference) * 1;

        // add force to distance from neighbours
        Collider[] neighbours = Physics.OverlapSphere(thisPosition, 20);
        // deal with neighbours

        acceleration += NormalizeSteeringForce(CollisionAvoidanceForce());

        Vector3 newVelocity = rigid.velocity;
        newVelocity += acceleration * Time.deltaTime;
        newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, 0.0f, 3.0f);

        rigid.velocity = newVelocity;
        transform.forward = rigid.velocity.normalized;
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
