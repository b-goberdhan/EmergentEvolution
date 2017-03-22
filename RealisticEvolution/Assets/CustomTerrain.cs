using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTerrain : MonoBehaviour {
    private GameObject ingameTerrain;
    private TerrainData data;
    public Vector3 position = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
        GameObject terrain = new GameObject();
        data = new TerrainData();
        terrain = Terrain.CreateTerrainGameObject(data);
        terrain.name = "Custom Map igguh";
        SetupMap(2f);
       
        
    }
	

    
	// Update is called once per frame
	void Update () {
        
	}

    public void SetupMap(float depth)
    {
        float xWidth = data.size.x;
        float yWidth = data.size.z;
        float[,] heights = new float[(int)xWidth, (int)yWidth];

        for (int j = 0; j < yWidth; j++)
        {
            heights[0, j] = 3f;
            heights[(int)(yWidth - 1) , j] = 3f;
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
