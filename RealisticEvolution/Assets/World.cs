using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Organism org;
    public Organism basicFood;
    private GameObject ingameTerrain;
    private TerrainData data;
    public Vector3 position = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start () {
        GameObject terrain = new GameObject();
        data = new TerrainData();
        terrain = Terrain.CreateTerrainGameObject(data);
        terrain.name = "Custom Map igguh";
        SetupMap(2f);

        Organism orgs;
        Vector3 spawnLocation = Vector3.zero;
        Quaternion initialRotation = Quaternion.Euler(Vector3.zero);

        spawnLocation.y = org.transform.localScale.y / 2;

        for (int i = 0; i < 2; i++)
        {
            spawnLocation.x += 5f;

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            Color speciesColor = new Color(r, g, b);

            for (int j = 0; j < 2; j++)
            {
                spawnLocation.z += 5f;
                orgs = Instantiate<Organism>(org, spawnLocation, initialRotation);

                //speciesColor.r += Random.Range(-0.2f, 0.2f);
                //speciesColor.g += Random.Range(-0.2f, 0.2f);
                //speciesColor.b += Random.Range(-0.2f, 0.2f);
                orgs.ReproductionPeriod = 100 + Random.Range(-10, 10);
                orgs.RefractoryPeriod = 100 + Random.Range(-10, 10);
                orgs.Efficency = 100 + Random.Range(-10, 10);
                orgs.MaturityAge = 100 + Random.Range(-10, 10);
                orgs.Health = 100 + Random.Range(-10, 10);
                orgs.Strength = 100 + Random.Range(-10, 10);
                orgs.Defense = 100 + Random.Range(-10, 10);
                orgs.GetComponent<Renderer>().material.SetColor("_Color", speciesColor);
            }
        }
        GameObject.Destroy(org.gameObject);
        spawnLocation.y = basicFood.transform.localScale.y / 2;
        for (int i = 0; i < 2; i++)
        {
            spawnLocation.x = Random.Range(0f, 10f);
            spawnLocation.z = Random.Range(0f, 10f);
            orgs = Instantiate<Organism>(basicFood, spawnLocation, initialRotation);
            orgs.Energy = Random.Range(1f, 20f);
            orgs.food = true;
        }

        GameObject.Destroy(basicFood.gameObject);





    }
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

    public void SetupMap(float depth)
    {
        float xWidth = data.size.x;
        float yWidth = data.size.z;
        float[,] heights = new float[(int)xWidth, (int)yWidth];

        for (int j = 0; j < yWidth; j++)
        {
            heights[0, j] = 3f;
            heights[(int)(yWidth - 1), j] = 3f;
        }

        for (int j = 0; j < xWidth; j++)
        {
            heights[j, 0] = 3f;
            heights[j, (int)(xWidth - 1)] = 3f;
        }

        data.SetHeights(1, 1, heights);

        print(data.size);
    }
}
