using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour {
    public float Energy = 100f;
    public float Efficency = .25f;
    public int[] FoodSources = null; 
    public int[] Wastes = null;
    public int MaturityAge;
    public int RefractoryPeriod;
    public int ReproductionPeriod;

    public float Strength;
    public float Defense;
    public float Health;

    public bool food;




    public bool EnableDebug = false;
    public float ScalingProbability;
    public string Name;
    public GameObject World;
    private Rigidbody rb;
    // Use this for initialization

    void Start () {
        rb = GetComponent<Rigidbody>();
        gameObject.name = Name;
        food = false;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!food)
            Move();
    }

    public void Move()
    {
        float x, y, z;
        if (Random.Range(0f, 100f) < ScalingProbability)
        {
            ScalingProbability = 5f;
            if (Random.Range(0, 2) == 0)
                x = Random.Range(0f, 180f);
            else
                x = 0;
            if (Random.Range(0, 2) == 0)
                y = Random.Range(0f, 180f);
            else
                y = 0;
            if (Random.Range(0, 2) == 0)
                z = Random.Range(0f, 180f);
            else
                z = 0;
            transform.Rotate(0, y, 0);

        }
        else
        {
            // if the snitch hits a wall then its velocity should end up at 0, this ensures that if that happens, the snitch
            // will react and turn around instead of sitting there waiting for a random rotation away from the wall
            if (rb.velocity.magnitude > 0)
            {
                ScalingProbability += (4f / 4f) * (1 / rb.velocity.magnitude);
            }
            else
            {
                ScalingProbability = 100f;
            }
        }
        
        rb.AddForce(transform.forward * 5, ForceMode.Acceleration);
        Energy -= 0.25f;
        if (EnableDebug)
        {
            Debug.Log("Energy remaining ");
            Debug.Log(Energy);
            Debug.Log("-------------");
        }
    }


    void OnCollisionEnter(Collision call)
    {
        
    }

    public void Look()
    {

    }
}
