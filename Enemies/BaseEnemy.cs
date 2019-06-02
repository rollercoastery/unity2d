﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    public GameplayData gd;

    public EnemyStat es;

    [Header("Basic")]
	public uint health;

    [Header("Movement")]
    public float spawnedTimer;          // Set duration after spawning to count down from
    float currentSpawnedTimer;          // The current timer of the period after spawning
    public float moveSpeed;
    public float turnSpeed;
    public MovementType movementType;   // Movement type that will eventually be set after spawning period
    MovementType currentMovementType;   // The movement type that will be set to the enemy

    public enum MovementType
    {
        Straight,       // Moves in a straight line when just spawned
        Seeking,        // Seeks target in a straight line
        Random,         // Moves randomly
        Distance,       // Keeps a small randomized range of distance from the target
        Teleport        // Teleports towards the target in steps
    }

    float randRotationTimer;        // Current random rotation timer for the Random movement type
    float randRotationMaxTimer;     // Max random rotation timer for the Random movement type
    Quaternion randRotation;        // New random rotation

    
    
    bool isCollided;    // For dying animation

    void OnEnable()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);

        // For Random movement type
        randRotationMaxTimer = Random.Range(0f, 0.5f);
        randRotationTimer = 0f;
        randRotation = Random.rotation;

        currentMovementType = MovementType.Straight;
        currentSpawnedTimer = 0f;
        isCollided = false;
    }
    
    void Update()
    {
        float p_turnSpeed = turnSpeed * Time.deltaTime * gd.gameSpeed;

        if (!HelperFunctions.hf.Timer(ref currentSpawnedTimer, spawnedTimer))
        {
            currentMovementType = MovementType.Straight;
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else
        {
            currentMovementType = movementType;
            GetComponent<CircleCollider2D>().enabled = true;
        }

        switch (currentMovementType)
        {
            case MovementType.Straight:
                MoveStraight();
                break;

            case MovementType.Seeking:
                WarpObject();
                transform.rotation = Quaternion.Lerp(transform.rotation, 
                                                     Quaternion.LookRotation(Vector3.forward, ObjectManager.om.head.transform.position - transform.position),
                                                     p_turnSpeed);
                MoveStraight();
                break;

            case MovementType.Random:
                WarpObject();

                // Get a random timer
                HelperFunctions.hf.Timer(ref randRotationTimer, randRotationMaxTimer);
                if (randRotationTimer <= 0f)
                {
                    randRotationMaxTimer = Random.Range(0f, 3f);
                    randRotation = Random.rotation;
                }

                // Rotate towards random direction
                Quaternion newRotation = Quaternion.Lerp(transform.rotation, randRotation, p_turnSpeed);
                newRotation.x = newRotation.y = 0f;
                transform.rotation = newRotation;

                MoveStraight();
                break;

            case MovementType.Distance:
                break;
            case MovementType.Teleport:
                break;
            default:
                break;
        }

        if (isCollided)
        {
            StartCoroutine("DeathAnimation");
            if (transform.localScale.x <= .1f)
                ObjectManager.om.RemoveObject(this.gameObject);
        }
    }

    // Kick start the spawned timer and sets movement to the chosen one after
    private bool StartSpawnedTimer(float timer)
    {
        HelperFunctions.hf.Timer(ref currentSpawnedTimer, timer);
        return currentSpawnedTimer < timer ? false : true;
    }

    private void MoveStraight()
    {
        //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime * gd.gameSpeed;);
        transform.position += transform.up * Time.deltaTime * gd.gameSpeed * moveSpeed;
    }

    // Warp object to opposite screen
    private void WarpObject()
    {
        Vector3 newPosition = transform.position;
        HelperFunctions.hf.WarpObject(ref newPosition, 2.88f, 1.65f);
        transform.position = newPosition;
    }

    IEnumerator DeathAnimation()
    {
        transform.localScale = HelperFunctions.hf.Scaler(transform.localScale, Vector3.zero, 5f);
        yield return new WaitForSeconds(1f);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Insert logic for damaging the player
            float prevHealth = gd.currentHealth;
            gd.currentHealth -= 1f;
            //gd.UpdateHealthBar(ref prevHealth);
            isCollided = true;
        }
    }
}
