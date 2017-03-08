using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Organism org;
    // Use this for initialization
    void Start () {
        Color speciesColor = new Color();
        for (int i = 0; i < 2; i++)
        {
            speciesColor.r = Random.Range(0.0f, 1.0f);
            speciesColor.g = Random.Range(0.0f, 1.0f);
            speciesColor.b = Random.Range(0.0f, 1.0f);
            CreateSpecies(speciesColor, 2);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}


    void CreateSpecies(Color color, int numberOfOrganisms)
    {
        Organism orgs;

        Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
        Vector3 spawnLocation = Vector3.zero;
        float baseValue = 100.0f;


        for (int i = 0; i < numberOfOrganisms; i++)
        {
            orgs = Instantiate<Organism>(org, spawnLocation, initialRotation);

            //orgs.MaturityAge = baseValue + Random.Range(-5.0f, 5.0f);
            spawnLocation.x += org.transform.localScale.x;
        }
    }
}
