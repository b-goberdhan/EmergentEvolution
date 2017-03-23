using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Organism : MonoBehaviour
{
    public float Energy = 100f;
    public float MaxEnergy = 100f;
    public float EatEfficiency = .25f;
    public float AttackEfficiency = .50f; //how good of a fighter this is
    public float Strength = 1.25f; //scales up the raw dmg done
    public float Defense = .25f; //scales down the damage done
    public float Aggression = 5f; //how aggressive an organism is 
    public float TimeToLive = 30f; //default 1 minute
    public float Speed;
    public bool CanLook;

	public int bitmask;

	public Organism OrganismTarget;

	public float AttackSpeed;

	private float LastAttack;
	private bool AttackReady;
	private bool Attacked;

	public float MaturityAge = 10f;

	public float Age{ get; private set;}
	private float Born;


    public const float BASE_DAMAGE = 20f;

    public bool food;
    private bool decay;

    public Vector3 StartPoint;
    public int newtarget;
    private NavMeshAgent nav;
    private Vector3 target;
    private float timer = 0f;
    public int sex;

    private bool attacking = false;
    private Organism enemy = null;

    public bool EnableDebug = false;


    //public GameObject World;
    // Use this for initialization

    void Start()
    {
        if (!food)
        {
            nav = gameObject.GetComponent<NavMeshAgent>();
            target = new Vector3(StartPoint.x, StartPoint.y, StartPoint.z);
        }
		LastAttack = Time.time;
		Born = Time.time;
		Age = Time.time - Born;
        decay = false;
    }
    void FixedUpdate()
    {
		AttackReady = ((Time.time - LastAttack) > AttackSpeed);
		Attacked = false;
		Age = Time.time - Born;
		//print (Age);
		if (Age >= TimeToLive){
			food = true;
			decay = true;
		}

        Vector3 scale = new Vector3(this.transform.localScale.x, 0, this.transform.localScale.z);
        scale.y = 0.2f + ((this.Energy / this.MaxEnergy) * 0.8f);
        this.transform.localScale = scale;
        scale = this.transform.position;
        scale.y = 0.1f + this.transform.localScale.y / 2;
        this.transform.position = scale;

        if (decay)
        {
            Energy -= MaxEnergy / (TimeToLive * 100f);
        }
        else if (!food)
        {
            timer += Time.deltaTime;
            nav.speed = Speed;
			if (timer >= newtarget && OrganismTarget == null) {
				getNewTarget ();
				//Reproduce();
				timer = 0;
			} else if(OrganismTarget != null) {
				nav.SetDestination(OrganismTarget.transform.position);
			}
			Energy -= MaxEnergy / (TimeToLive * 100f);
        }
        else if (food)
        {
            Energy += MaxEnergy / (TimeToLive * 50f);
			if (Energy >= (MaxEnergy - (MaxEnergy * 0.5f)) && Energy <= (MaxEnergy + (MaxEnergy * 0.5f)))
            {
				Vector3 spawnLocation = this.transform.position;
				bool repro = true;

				int bits = 2^8 - 1;

				Collider[] colliders = Physics.OverlapSphere (spawnLocation, this.transform.localScale.x + this.transform.localScale.x/4);
				foreach (Collider col in colliders){
					Organism orgs = col.GetComponent<Organism> ();
					if (orgs != null) {
						if (orgs != this) {
							repro = false;
							spawnLocation = orgs.transform.position;
							if (spawnLocation.x == (this.transform.position.x - this.transform.localScale.x) && spawnLocation.z == (this.transform.position.z - this.transform.localScale.z))
								bits -= 2 ^ 7;
							else if (spawnLocation.x == (this.transform.position.x - this.transform.localScale.x) && spawnLocation.z == this.transform.position.z)
								bits -= 2 ^ 6;
							else if (spawnLocation.x == (this.transform.position.x - this.transform.localScale.x) && spawnLocation.z == (this.transform.position.z + this.transform.localScale.z))
								bits -= 2 ^ 5;
							else if (spawnLocation.x == this.transform.position.x && spawnLocation.z == (this.transform.position.z - this.transform.localScale.z))
								bits -= 2 ^ 4;
							else if (spawnLocation.x == (this.transform.position.x) && spawnLocation.z == (this.transform.position.z + this.transform.localScale.z))
								bits -= 2 ^ 3;
							else if (spawnLocation.x == (this.transform.position.x + this.transform.localScale.x) && spawnLocation.z == (this.transform.position.z - this.transform.localScale.z))
								bits -= 2 ^ 2;
							else if (spawnLocation.x == (this.transform.position.x + this.transform.localScale.x) && spawnLocation.z == (this.transform.position.z))
								bits -= 2;
							else if (spawnLocation.x == (this.transform.position.x + this.transform.localScale.x) && spawnLocation.z == (this.transform.position.z + this.transform.localScale.z))
								bits -= 1;
						}
					}
						
				}

				if (repro)
					Reproduce(bits);
            }
        }
        if (Energy <= 0)
        {
            Die();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (food)
            return;

        if (other.gameObject.GetComponent<Organism>() == null)
            return;
        float attackProb = Aggression / 100 + AttackEfficiency;
        if (EnableDebug)
            print(name + " Attack Prob is " + attackProb);
        float rand = Random.Range(0, 1);
        Organism rivalOrganism = other.gameObject.GetComponent<Organism>();
        bool same = SameSpecies(rivalOrganism);

        if (!Attacked && AttackReady && rivalOrganism.food && !same)
        {
            float attackDamage = (BASE_DAMAGE * Strength * rivalOrganism.Defense);
            this.Energy += attackDamage * EatEfficiency;
            rivalOrganism.Energy -= attackDamage;
			LastAttack = Time.time;
			Attacked = true;
			if (Energy / MaxEnergy > 0.6)
				OrganismTarget = null;
            return;
        }
        else if (same)
        {
            if (!rivalOrganism.food)
            {
				OrganismTarget = null;
                Reproduce(other.GetComponent<Organism>());
            }
            return;
        }

		if (!Attacked && AttackReady && (rand < attackProb && !same))
        {
			OrganismTarget = null;
            //Attack Once per collision, so far there is no chasing*

            float attackDamage = (BASE_DAMAGE * Strength * rivalOrganism.Defense);

            rivalOrganism.Energy -= attackDamage;

            if (Random.Range(0, 100) < 100 * (attackDamage / rivalOrganism.MaxEnergy))
            {
                rivalOrganism.food = true;
                if (!EnableDebug)
                {
                    print("Enemy killed!");
                }
                rivalOrganism.decay = true;
            }

            if (rivalOrganism.Energy <= 0)
            {
                this.Energy += attackDamage * EatEfficiency;
            }
			LastAttack = Time.time;
			Attacked = true;


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
            if (EnableDebug)
                print(name + "Fled, Energy is " + Energy);
        }
    }

    private void getNewTarget()
    {
		float rander = Random.Range (0, 100);
		float findTarget = Energy/MaxEnergy;
		bitmask = 64;
		bitmask = (rander > ((Energy/MaxEnergy) * 100)) ? 1 : bitmask;
		rander = Random.Range (0, 100);
		//max - energy < 20
		bitmask = ((Random.Range(0, 100) > 30) && (Age >= MaturityAge)) ? 2 : bitmask;
		bitmask = ((MaxEnergy - Energy < 20) && (Age >= MaturityAge)) ? 2 : bitmask;

		float[] objectSeen = Look(bitmask);
		if (objectSeen != null && CanLook)
        {
            target = new Vector3(objectSeen[0], objectSeen[1], objectSeen[2]);
        }
        else
        {
			OrganismTarget = null;
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
        }


        nav.SetDestination(target);
    }

    private bool SameSpecies(Organism orga)
    {
        Color other = orga.GetComponent<Renderer>().material.color;
        Color mine = this.GetComponent<Renderer>().material.color;
        float colorDiff = Mathf.Abs(other.r - mine.r);
        colorDiff += Mathf.Abs(other.g - mine.g);
        colorDiff += Mathf.Abs(other.b - mine.b);
        if (colorDiff <= 0.2f)
        {
            return true;
        }
        return false;
    }

	private void Reproduce(int bits)
    {
        Energy /= 2;
        Organism baby = Instantiate<Organism>(this);
        Vector3 newPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        newPos.x += (baby.transform.localScale.x) * (Random.Range(0, 100) > 50 ? 1 : -1);
        newPos.z += (baby.transform.localScale.z) * (Random.Range(0, 100) > 50 ? 1 : -1);
        float rander = Random.Range(0, 100);



        if (rander > 50)
        {
            rander = Random.Range(0, 100);
            if (rander > 50)
            {
                newPos.x = this.transform.position.x + this.transform.localScale.x;
            }
            else
            {
                newPos.z = this.transform.position.z + this.transform.localScale.z;
            }
        }


        if (newPos.x > (100 - this.transform.localScale.x))
            newPos.x = 100 - this.transform.localScale.x;
        else if (newPos.x < (-100 + this.transform.localScale.x))
            newPos.x = -100 + this.transform.localScale.x;
        if (newPos.z > (100 - this.transform.localScale.z))
            newPos.z = 100 - this.transform.localScale.z;
        else if (newPos.z < (-100 + this.transform.localScale.z))
            newPos.z = -100 + this.transform.localScale.z;
        baby.transform.position = newPos;
        //print("x: " + newPos.x + " z: " + newPos.z + " Parent coords-> x: " + this.transform.position.x + " z: " + this.transform.position.z);
        Vector3 scale = new Vector3(baby.transform.localScale.x, 0, baby.transform.localScale.z);
        scale.y = 0.2f + ((baby.Energy / baby.MaxEnergy) * 0.8f);
        baby.transform.localScale = scale;

    }

    private void Reproduce(Organism lover)
    {
        if (lover.food)
            return;
		if ((Age < MaturityAge) || (lover.Age < lover.MaturityAge))
			return;
        if (Energy >= 40 && SameSpecies(lover))
        {
            if (sex == 1 && lover.sex == 0)
            {
                Organism baby = Instantiate<Organism>(this);
                Mutate(baby, this, lover);
                baby.Energy = (lover.Energy + this.Energy) / 2;
                baby.sex = (Random.Range(0, 100) > 50 ? 1 : 0);
                Vector3 scale = new Vector3(baby.transform.localScale.x, 0, baby.transform.localScale.z);
                scale.y = 0.2f + ((baby.Energy / baby.MaxEnergy) * 0.8f);
                baby.transform.localScale = scale;

				TextMesh texter = baby.GetComponentInChildren<TextMesh>();
				texter.font.material.color = Color.black;
				texter.text = "baby";
				texter = this.GetComponentInChildren<TextMesh>();
				texter.font.material.color = Color.black;
				texter.text = "MATED";
				texter = lover.GetComponentInChildren<TextMesh>();
				texter.font.material.color = Color.black;
				texter.text = "MATED";
				Energy -= 20;
				//baby.name = this.name;
            }
        }
    }
    /*
	 * Attribute List
	 * --------------
	 *	Energy
	 *	MaxEnergy
     *	EatEfficency
     *	AttackEffieceny
     *	Strength
     *	Defense
     *	Aggresion 
     *	TimeToLive
	 *	Speed;
	*/
    private float mutationThreshold = 95;
    private void Mutate(Organism baby, Organism mother, Organism father)
    {
        Color babyColor = Color.Lerp(mother.GetComponent<Renderer>().material.color, father.GetComponent<Renderer>().material.color, 0.5f);
        baby.MaxEnergy = (mother.MaxEnergy + father.MaxEnergy) / 2;
        baby.EatEfficiency = (mother.EatEfficiency + father.EatEfficiency) / 2;
        baby.AttackEfficiency = (mother.AttackEfficiency + father.AttackEfficiency) / 2;
        baby.Strength = (mother.Strength + father.Strength) / 2;
        baby.Defense = (mother.Defense + father.Defense) / 2;
        baby.Aggression = (mother.Aggression + father.Aggression) / 2;
        baby.TimeToLive = (mother.TimeToLive + father.TimeToLive) / 2;
        baby.Speed = (mother.Speed + father.Speed) / 2;
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.MaxEnergy += Random.Range(-baby.MaxEnergy / 50, baby.MaxEnergy / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.EatEfficiency += Random.Range(-baby.EatEfficiency / 50, baby.EatEfficiency / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.EatEfficiency += Random.Range(-baby.EatEfficiency / 50, baby.EatEfficiency / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.AttackEfficiency += Random.Range(-baby.AttackEfficiency / 50, baby.AttackEfficiency / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.Strength += Random.Range(-baby.Strength / 50, baby.Strength / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.Defense += Random.Range(-baby.Defense / 50, baby.Defense / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.Aggression += Random.Range(-baby.Aggression / 50, baby.Aggression / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.TimeToLive += Random.Range(-baby.TimeToLive / 50, baby.TimeToLive / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        if (Random.Range(0, 100) > mutationThreshold)
        {
            baby.Speed += Random.Range(-baby.Speed / 50, baby.Speed / 50);
            int colorRandom = Random.Range(0, 100);
            if (colorRandom > 66)
            {
                babyColor.r += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else if (colorRandom > 33)
            {
                babyColor.g += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
            else
            {
                babyColor.b += (Random.Range(0, 100) > 50 ? 0.005f : -0.005f);
            }
        }
        baby.GetComponent<Renderer>().material.color = babyColor;
    }



    private void Attack()
    {
    }

    public void Die()
    {
        Destroy(gameObject);
    }





	public float[] Look(int bitmask)
    {
		//000 | all
		//001 | food
		//010 | mate
		//100 | same

		if (bitmask == 64)
			return null;

        Collider[] collisons = Physics.OverlapSphere(transform.position, 20f);
        foreach (Collider item in collisons)
        {
            if (item.transform.position != transform.position && item.GetComponent<Organism>() != null)
            {
				//print (bitmask);
                //see if what we collided was either food or an organimse
                Organism collItem = item.GetComponent<Organism>();
				if (((bitmask & 1) == 0) && collItem.food) {
					continue;
				}
				if (((bitmask & 2) == 0) && SameSpecies(collItem)){
					continue;
				}
				//print ("Bits: " + (bitmask & 1) + " " + (bitmask & 2));

				Vector3 direction = Vector3.Normalize(item.transform.position - this.transform.position);
                float dot = Vector3.Dot(direction, this.transform.forward);
                if (dot < Mathf.Cos(Mathf.PI / 4))
                {
                    //set nav mesh to follow
                    float[] coords = { collItem.transform.position.x, collItem.transform.position.y, collItem.transform.position.z };
					OrganismTarget = collItem;
                    return coords;
                }
            }

        }
        return null;
    }
}
