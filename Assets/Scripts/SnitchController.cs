using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnitchController : MonoBehaviour
{
    public UnityEvent<Team> OnRoundEnd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WizardPart"))
        {
            Team team = other.gameObject.GetComponentInParent<Team>();
            OnRoundEnd?.Invoke(team);

        }
    }
}
