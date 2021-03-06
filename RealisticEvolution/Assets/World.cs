﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class World : MonoBehaviour {
    public Organism org;
	public float landX;
	public float landZ;
	public int numberOfSpecies = 2;
	public int numberOfOrganisms = 2;
	public float bladeRespawn;

    // Use this for initialization
    void Start () {
		Vector3 spawnLocation = Vector3.zero;
        Color speciesColor = new Color();
		TextMesh texter = org.GetComponentInChildren<TextMesh>();
		texter.font.material.color = Color.black;
		texter.text = "";

		//spawnLocation.z = -org.transform.localScale.z * numberOfSpecies;

		for (int i = 0; i < numberOfSpecies; i++)
        {
			spawnLocation.z = Random.Range(0, landZ/2) * (Random.Range(0, 100) > 50 ? 1 : -1);
			spawnLocation.x = Random.Range(0, landX/2) * (Random.Range(0, 100) > 50 ? 1 : -1);

			//spawnLocation.x = org.transform.position.x;
            speciesColor.r = Random.Range(0.0f, 1.0f);
            speciesColor.g = Random.Range(0.0f, 1.0f);
            speciesColor.b = Random.Range(0.0f, 1.0f);
			CreateSpecies(speciesColor, numberOfOrganisms, spawnLocation);
			//spawnLocation.z += 2 * org.transform.localScale.z;
        }
		org.GetComponent<Renderer> ().enabled = false;
		org.GetComponent<BoxCollider> ().enabled = false;
		org.enabled = false;;

		for (int i = 0; i < 160; i++){
			spawnFood ();	
		}
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}


	void CreateSpecies(Color color, int numberOfOrganisms, Vector3 spawnLocation)
    {
        Organism orgs;

        Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
        float baseValue = 100.0f;

		//spawnLocation.x = -org.transform.localScale.x * numberOfOrganisms;
		string name;
		float rander = Random.Range (0, 100);
		if (rander < 25)
			name = "Inky";
		else if (rander < 50)
			name = "Blinky";
		else if (rander < 75)
			name = "Pinky";
		else
			name = "Clyde";


        for (int i = 0; i < numberOfOrganisms; i++)
        {
            orgs = Instantiate<Organism>(org, spawnLocation, initialRotation);
			orgs.sex = i%2;

			orgs.GetComponent<Renderer>().material.color = color;
			orgs.food = false;

            //orgs.MaturityAge = baseValue + Random.Range(-5.0f, 5.0f);
            //spawnLocation.x += 2 * org.transform.localScale.x;
			orgs.name = name;
        }
    }

	public void spawnFood(float x, float z){
		Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
		Vector3 spawnLocation = new Vector3(x, 0.1f, z);
		int rem = (int) (spawnLocation.x / (org.transform.localScale.x));

		spawnLocation.x = org.transform.localScale.x * rem;
		rem = (int) (spawnLocation.z / (org.transform.localScale.z));
		spawnLocation.z = org.transform.localScale.z * rem;

		Collider[] colliders = Physics.OverlapSphere (spawnLocation, 0.25f);
		foreach (Collider col in colliders){
			if (col.GetComponent<Organism> () != null)
				return;
		}

		Organism food = Instantiate<Organism> (org, spawnLocation, initialRotation);
		food.enabled = true;
		Destroy(food.GetComponent<NavMeshAgent> ());
		food.transform.position = spawnLocation;
		food.food = true;
		food.MaxEnergy = 150f;
		food.Energy = food.MaxEnergy * 0.75f;
		food.GetComponent<Renderer> ().enabled = true;
		food.GetComponent<BoxCollider> ().enabled = true;
		food.GetComponent<Renderer> ().material.color = Color.green;
		food.name = "Food";
		Vector3 scale = new Vector3();
		scale = food.transform.position;
		scale.y = 0.1f + food.transform.localScale.y / 2;
		food.transform.position = scale;
		food.gameObject.isStatic = true;
		food.TimeToLive = 120f;

	}

	void spawnFood(){
		Quaternion initialRotation = Quaternion.Euler(Vector3.zero);
		Vector3 spawnLocation = new Vector3(0, 0.1f, 0);
		float xz = Random.Range (0, 100);

		float xOut = (landX)/2  - (landX / 5);
		float zOut = (landZ)/2 - (landZ / 5);

		if (xz > 50) {
			xOut = 0;
		} else {
			zOut = 0;
		}

		spawnLocation.x = Random.Range (xOut, landX/2 - org.transform.localScale.x);
		spawnLocation.x *= (Random.Range (0, 100) > 50 ? 1 : -1);
		spawnLocation.z = Random.Range (zOut, landZ/2 - org.transform.localScale.x);
		spawnLocation.z *= (Random.Range (0, 100) > 50 ? 1 : -1);
		/*
		int rem = (int) (spawnLocation.x / (org.transform.transform.localScale.x));

		spawnLocation.x = org.transform.localScale.x * rem;
		rem = (int) (spawnLocation.z / (org.transform.transform.localScale.z));
		spawnLocation.z = org.transform.localScale.z * rem;
*/
		spawnFood (spawnLocation.x, spawnLocation.z);
		/*

		Collider[] colliders = Physics.OverlapSphere (spawnLocation, 0.25f);
		foreach (Collider col in colliders){
			if (col.GetComponent<Organism> () != null)
				return;
		}
			



		Organism food = Instantiate<Organism> (org, spawnLocation, initialRotation);
		Destroy(food.GetComponent<NavMeshAgent> ());
		food.transform.position = spawnLocation;
		food.food = true;
		food.MaxEnergy = 150f;
		food.Energy = food.MaxEnergy * 0.75f;
		food.GetComponent<Renderer> ().enabled = true;
		food.GetComponent<BoxCollider> ().enabled = true;
		food.GetComponent<Renderer> ().material.color = Color.green;
		food.name = "Food";
		Vector3 scale = new Vector3();
		scale = food.transform.position;
		scale.y = 0.1f + food.transform.localScale.y / 2;
		food.transform.position = scale;*/
	}
}
