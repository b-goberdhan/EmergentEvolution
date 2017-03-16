using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Organism org;
    // Use this for initialization
    void Start () {
		Vector3 spawnLocation = Vector3.zero;
        Color speciesColor = new Color();
        for (int i = 0; i < 3; i++)
        {
			spawnLocation.x = org.transform.position.x;
            speciesColor.r = Random.Range(0.0f, 1.0f);
            speciesColor.g = Random.Range(0.0f, 1.0f);
            speciesColor.b = Random.Range(0.0f, 1.0f);
			CreateSpecies(speciesColor, 2, spawnLocation);
			spawnLocation.z += 2 * org.transform.localScale.z;
        }
		org.GetComponent<Renderer> ().enabled = false;
		org.GetComponent<BoxCollider> ().enabled = false;
		//Destroy(org.gameObject);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}


	void CreateSpecies(Color color, int numberOfOrganisms, Vector3 spawnLocation)
    {
        Organism orgs;

        Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
        float baseValue = 100.0f;


        for (int i = 0; i < numberOfOrganisms; i++)
        {
            orgs = Instantiate<Organism>(org, spawnLocation, initialRotation);
			orgs.sex = i;

			orgs.GetComponent<Renderer>().material.color = color;
			orgs.food = false;

            //orgs.MaturityAge = baseValue + Random.Range(-5.0f, 5.0f);
            spawnLocation.x += 2 * org.transform.localScale.x;
        }
    }
	void spawnFood(){
	
	}
}
