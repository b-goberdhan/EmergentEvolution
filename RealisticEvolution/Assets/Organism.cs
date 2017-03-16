﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Organism : MonoBehaviour {
    public float Energy = 100f;
	public float MaxEnergy = 100f;
    public float EatEfficiency = .25f;
    public float AttackEfficiency = .50f; //how good of a fighter this is
    public float Strength = 1.25f; //scales up the raw dmg done
    public float Defense = .25f; //scales down the damage done
    public float Aggression = 5f; //how aggressive an organism is 
    public float TimeToLive = 60f; //default 1 minute
	public float Speed;


    public const float BASE_DAMAGE = 20f;

    public bool food;

    public Vector3 StartPoint; 
    public int newtarget;
    private NavMeshAgent nav;
    private Vector3 target;
    private float timer = 0f;
	public int sex;

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
		if (Energy <= 0) {
			Destroy (gameObject);
		}
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Organism>() == null)
            return;
		Reproduce (other.GetComponent<Organism>());
		float attackProb = Aggression / 100 + AttackEfficiency;
        if(EnableDebug)
            print(name + " Attack Prob is " + attackProb);
        float rand = Random.Range(0, 1);
		Organism rivalOrganism = other.gameObject.GetComponent<Organism>();
		bool same = SameSpecies(rivalOrganism);
        if (rand < attackProb && !same)
        {
            //Attack Once per collision, so far there is no chasing*

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

	private bool SameSpecies(Organism orga){
		Color other = orga.GetComponent<Renderer> ().material.color;
		Color mine = this.GetComponent<Renderer> ().material.color;
		float colorDiff = Mathf.Abs (other.r - mine.r);
		colorDiff += Mathf.Abs (other.g - mine.g);
		colorDiff += Mathf.Abs (other.b - mine.b);
		if (colorDiff <= 0.2f) {
			return true;
		}
		return false;
	}


	private void Reproduce(Organism lover)
    {
		if (Energy >= 20 && SameSpecies(lover)) {
			if (sex == 1 && lover.sex == 0) {
				Organism baby;
				baby = Instantiate<Organism> (this);
				Mutate (baby, this, lover);
				baby.Energy = baby.MaxEnergy;
				baby.sex = (Random.Range (0, 100) > 50 ? 1 : 0);
			}
			Energy -= 20;
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
	private void Mutate(Organism baby, Organism mother, Organism father){
		Color babyColor = Color.Lerp(mother.GetComponent<Renderer> ().material.color, father.GetComponent<Renderer> ().material.color, 0.5f);
		baby.MaxEnergy = (mother.MaxEnergy + father.MaxEnergy) / 2;
		baby.EatEfficiency = (mother.EatEfficiency + father.EatEfficiency) / 2;
		baby.AttackEfficiency = (mother.AttackEfficiency + father.AttackEfficiency) / 2;
		baby.Strength = (mother.Strength + father.Strength) / 2;
		baby.Defense = (mother.Defense + father.Defense) / 2;
		baby.Aggression = (mother.Aggression + father.Aggression) / 2;
		baby.TimeToLive = (mother.TimeToLive + father.TimeToLive) / 2;
		baby.Speed = (mother.Speed + father.Speed) / 2;
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.MaxEnergy += Random.Range(-baby.MaxEnergy/50, baby.MaxEnergy/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.EatEfficiency += Random.Range(-baby.EatEfficiency/50, baby.EatEfficiency/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.EatEfficiency += Random.Range(-baby.EatEfficiency/50, baby.EatEfficiency/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.AttackEfficiency += Random.Range(-baby.AttackEfficiency/50, baby.AttackEfficiency/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.Strength += Random.Range(-baby.Strength/50, baby.Strength/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.Defense += Random.Range(-baby.Defense/50, baby.Defense/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.Aggression += Random.Range(-baby.Aggression/50, baby.Aggression/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.TimeToLive += Random.Range(-baby.TimeToLive/50, baby.TimeToLive/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		if (Random.Range (0, 100) > mutationThreshold) {
			baby.Speed += Random.Range(-baby.Speed/50, baby.Speed/50);
			int colorRandom = Random.Range (0, 100);
			if (colorRandom > 66){
				babyColor.r += 0.005f;
			}
			else if (colorRandom > 33){
				babyColor.g += 0.005f;
			}
			else{
				babyColor.b += 0.005f;
			}
		}
		baby.GetComponent<Renderer> ().material.color = babyColor;
	}

    private void Attack()
    {
    }

    
    


    public void Look()
    {

    }
}
