using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnitchController : MonoBehaviour
{
    public UnityEvent<Team> OnRoundEnd;
    public GameSettings gameSettings;

    private Rigidbody rigid;
    private float timer = 0.0f;
    private bool hasInitialized = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BoxSurface"))
            rigid.AddForce(-rigid.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wizard"))
        {
            Team team = other.gameObject.GetComponentInParent<Team>();
            OnRoundEnd?.Invoke(team);
        }
    }

    public void Initialize()
    {
        hasInitialized = true;
    }

    private void FixedUpdate()
    {
        if (hasInitialized)
        {
            Vector3 acceleration = Vector3.zero;

            timer += Time.deltaTime;

            if (timer >= gameSettings.snitchDirectionTimer)
            {
                Vector3 randomSpherePoint = Random.onUnitSphere;
                Vector3 randomDirection = (randomSpherePoint - transform.position) * gameSettings.snitchSpeed;

                acceleration += randomDirection;
                acceleration += NormalizeSteeringForce(randomDirection) * gameSettings.snitchRandomDirectionWeight;

                timer = 0.0f;
            }

            acceleration += NormalizeSteeringForce(CollisionAvoidanceForce()) * gameSettings.snitchCollisionAvoidanceWeight;

            Vector3 newVelocity = rigid.velocity;
            newVelocity += acceleration * Time.deltaTime;

            newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, gameSettings.snitchMinVelocity, gameSettings.snitchMaxVelocity);

            rigid.velocity = newVelocity;
            transform.forward = rigid.velocity.normalized;
        }
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 NormalizeSteeringForce(Vector3 force)
    {
        return force.normalized * Mathf.Clamp(force.magnitude, 0, gameSettings.snitchMaxSteerForce);
    }

    // https://github.com/omaddam/Boids-Simulation
    private Vector3 CollisionAvoidanceForce()
    {
        // Check if heading to collision
        if (!Physics.SphereCast(transform.position, gameSettings.snitchCollisionRadiusDetection, transform.forward, out RaycastHit hitInfo, gameSettings.snitchCollisionRadiusDetection))
            return Vector3.zero;

        // Compute force
        return transform.position - hitInfo.point;
    }
}
