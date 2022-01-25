using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnitchController : MonoBehaviour
{
    public UnityEvent<Team> OnRoundEnd;

    private Rigidbody rigid;
    private float speed = 5.0f;
    private float timer = 0.0f;
    private float directionChangeTime = 3.0f;
    private bool hasInitialized = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BoxSurface"))
            rigid.AddForce(-rigid.velocity);
        // else if (other.gameObject.CompareTag("Wizard"))
        // {
        //     Team team = other.gameObject.GetComponentInParent<Team>();
        //     OnRoundEnd?.Invoke(team);
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WizardPart"))
        {
            Team team = other.gameObject.GetComponentInParent<Team>();
            OnRoundEnd?.Invoke(team);
        }
    }

    public void Initialize()
    {
        Vector3 acceleration = Vector3.zero;
        Vector3 randomSpherePoint = Random.onUnitSphere;
        Vector3 randomDirection = (randomSpherePoint - transform.position) * speed;

        acceleration += randomDirection;
        acceleration += NormalizeSteeringForce(randomDirection);
        acceleration += NormalizeSteeringForce(CollisionAvoidanceForce()) * 2;
        
        Vector3 newVelocity = rigid.velocity;
        newVelocity += acceleration * Time.deltaTime;

        newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, 1, 2);

        rigid.velocity = newVelocity;
        transform.forward = rigid.velocity.normalized;
        hasInitialized = true;
    }

    private void FixedUpdate()
    {
        if (hasInitialized)
        {
            Vector3 acceleration = Vector3.zero;

            timer += Time.deltaTime;

            if (timer >= directionChangeTime)
            {
                Vector3 randomSpherePoint = Random.onUnitSphere;
                Vector3 randomDirection = (randomSpherePoint - transform.position) * speed;

                acceleration += randomDirection;
                acceleration += NormalizeSteeringForce(randomDirection);

                timer = 0.0f;
            }

            acceleration += NormalizeSteeringForce(CollisionAvoidanceForce()) * 2;

            Vector3 newVelocity = rigid.velocity;
            newVelocity += acceleration * Time.deltaTime;

            newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, 5.0f, 10.0f);

            rigid.velocity = newVelocity;
            transform.forward = rigid.velocity.normalized;
        }
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 NormalizeSteeringForce(Vector3 force)
    {
        return force.normalized * Mathf.Clamp(force.magnitude, 0, 50);
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 CollisionAvoidanceForce()
    {
        // Check if heading to collision
        if (!Physics.SphereCast(transform.position, 10, transform.forward, out RaycastHit hitInfo, 10))
            return Vector3.zero;

        // Compute force
        return transform.position - hitInfo.point;
    }
}
