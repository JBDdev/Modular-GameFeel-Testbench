using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float defaultSpeed;
    [SerializeField] float[] timerStartThresholds;
    [SerializeField] float timeout;
    [SerializeField] float accelerationFactor;

    int currentTest;

    float[] results;

    [SerializeField] float elapsedTime;
    [SerializeField] float startTimer;
    [SerializeField] bool accelerationTriggered;

    Rigidbody rb;
    float currentSpeed;

    Vector3 startingPosition;


    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        currentSpeed = defaultSpeed;
        elapsedTime = 0;
        startTimer = 0;
        accelerationTriggered = false;
        currentTest = 0;
        startingPosition = player.transform.position;
        results = new float[3];
    }


    // Update is called once per frame
    private void Update()
    {
        if (accelerationTriggered)
        {
            elapsedTime += Time.deltaTime;
        }
        else 
        {
            startTimer += Time.deltaTime;
            if(startTimer > timerStartThresholds[currentTest]) 
            {
                accelerationTriggered = true;
            }
        }

        if (elapsedTime > timeout)
            RecordResult();

        if (Input.GetMouseButtonDown(0)) 
            RecordResult();
        
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.forward * currentSpeed;

        if (accelerationTriggered) 
        {
            currentSpeed += accelerationFactor;
        }
    }

    void RecordResult() 
    {
        results[currentTest] = elapsedTime;

        //Reset for the next test
        elapsedTime = 0;
        startTimer = 0;
        currentSpeed = defaultSpeed;
        player.transform.position = startingPosition;

        currentTest++;

        if(currentTest == 2)
        {
            for (int i = 0; i < results.Length; i++) 
            {
                Debug.Log("Result " + (i+1) + ": " + results[i]);
            }
            //Output to the file then load the next scene
        }
    }
}
