using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Organism : MonoBehaviour {
    public float Energy = 100f;
    public float EatEfficency = .25f;
    public float AttackEffieceny = .50f; //how good of a fighter this is
    public float Strength = 1.25f; //scales up the raw dmg done
    public float Defense = .25f; //scales down the damage done
    public float Aggresion = 5f; //how aggressive an organism is 
    public float TimeToLive = 60f; //default 1 minute
    
    public const float BASE_DAMAGE = 20f;

    public bool food;

    public Vector3 StartPoint; 
    public int newtarget;
    private NavMeshAgent nav;
    private Vector3 target;
    public float Speed;
    private float timer = 0f;

    public bool EnableDebug = false;
    

    public GameObject World;
    // Use this for initialization

    void Start () {
        nav = gameObject.GetComponent<NavMeshAgent>();
        target = new Vector3(StartPoint.x, StartPoint.y, StartPoint.z);
	}
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        nav.speed = Speed;
        if(timer >= newtarget)
        {
            getNewTarget();
            //Reproduce();
            timer = 0;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Organism>() == null)
            return;
        float attackProb = Aggresion / 100 + AttackEffieceny;
        if(EnableDebug)
            print(name + " Attack Prob is " + attackProb);
        float rand = Random.Range(0, 1);
        
        if (rand < attackProb)
        {
            //Attack Once per collision, so far there is no chasing*
            Organism rivalOrganism = other.gameObject.GetComponent<Organism>();
            rivalOrganism.Energy -= (BASE_DAMAGE * Strength * rivalOrganism.Defense);
            if (EnableDebug)
            {
                print(name + " Dmg done is " + (BASE_DAMAGE * Strength * rivalOrganism.Defense));
                print(name + "Attacked, Energy is " + Energy);
            }

        }
        else
        {
            //flee
            //turn arounnd 180
            timer = -newtarget;
            target = new Vector3(-target.x, target.y, -target.z);
            if(EnableDebug)
                print(name + "Fled, Energy is " + Energy);
        }
    }

    private void getNewTarget()
    {
        float x = transform.position.x;
        float z = transform.position.z;

        float xPos = x + Random.Range(-20, 20);
        float zPos = z + Random.Range(-20, 20);
        target = new Vector3(xPos, transform.position.y, zPos);
        if (EnableDebug)
        {
            print("Target Destination");
            print(target.ToString());
            print("-------------");
        }
            
        nav.SetDestination(target);
    }

    private void Reproduce()
    {
        //for now an exact copy with no chance of mutations
        Instantiate(gameObject);
    }

    private void Attack()
    {

    }

    
    


    public void Look()
    {

    }
}
