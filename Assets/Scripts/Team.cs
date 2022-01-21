using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public GameObject wizardTemplate;
    public List<Wizard> wizards;
    public Material material;
    public GameSettings gameSettings;
    public TeamTraits teamTraits;

    private MeshRenderer wizardBody;
    
    public void Initialize()
    {
        for (int i = 0; i < gameSettings.wizardsQuantityPerTeam; i++)
            CreateWizard(i);
    }

    public void ResetScore()
    {
        teamTraits.points = 0;
    }

    private void CreateWizard(int index)
    {
        if (wizards == null)
            wizards = new List<Wizard>();
        
        GameObject wizard = GameObject.Instantiate(wizardTemplate, this.gameObject.transform);
        wizard.transform.parent = this.gameObject.transform;
        wizard.transform.localPosition = new Vector3(0, Random.Range(-5, 5), Random.Range(-8, 8));
        wizard.transform.localEulerAngles = new Vector3(0, teamTraits.initialYRotation, 0);
        wizardBody = wizard.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        wizardBody.material = material;
    }
}
