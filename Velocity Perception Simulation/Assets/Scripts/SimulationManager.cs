using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float maxSpeed;
    [SerializeField] float[] timerStartThresholds;
    [SerializeField] float timeout;
    [SerializeField] GameObject continueMenu;
    [SerializeField] GameObject timeoutIndicator;
    [SerializeField] GameObject testNumIndicator;

    int currentTest;

    float[] results;

    [SerializeField] float elapsedTime;
    [SerializeField] float startTimer;
    [SerializeField] bool movementTriggered;

    Rigidbody rb;
    float currentSpeed;

    Vector3 startingPosition;

    bool beginSimulation;

    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        currentSpeed = 0;
        elapsedTime = 0;
        startTimer = 0;
        movementTriggered = false;
        currentTest = 0;
        startingPosition = player.transform.position;
        results = new float[3];
        beginSimulation = false;

        //Generate random start values
        for (int i = 0; i < timerStartThresholds.Length; i++)
        {
            timerStartThresholds[i] = UnityEngine.Random.Range(2.5f, 15f);
        }

        GameObject canvas = GameObject.Find("Canvas");
        continueMenu = canvas.transform.GetChild(0).gameObject;
        testNumIndicator = canvas.transform.GetChild(1).gameObject;
        timeoutIndicator = canvas.transform.GetChild(2).gameObject;
    }


    // Update is called once per frame
    private void Update()
    {
        if (beginSimulation)
        {
            //Debug.Log(currentTest);
            if (movementTriggered)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                startTimer += Time.deltaTime;
                if (startTimer > timerStartThresholds[currentTest])
                {
                    movementTriggered = true;
                }
            }

            if (elapsedTime > timeout)
            {
                RecordResult();
                timeoutIndicator.gameObject.SetActive(true);
                Invoke("ClearTimeoutIndicator", 2f);
            }

            if (Input.GetMouseButtonUp(0))
                RecordResult();
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                continueMenu.SetActive(false);
                testNumIndicator.SetActive(true);
                Invoke("BeginRecordingData", 0.1f);
            }
        }
        
    }

    void ClearTimeoutIndicator() 
    {
        timeoutIndicator.gameObject.SetActive(false);
    }

    void BeginRecordingData() 
    {
        beginSimulation = true;
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.forward * currentSpeed;

        if (movementTriggered) 
        {
            currentSpeed += (timerStartThresholds[currentTest] / timeout) * Time.fixedDeltaTime;
            if(currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
    }

    void RecordResult() 
    {
        results[currentTest] = elapsedTime;

        //Reset for the next test
        elapsedTime = 0;
        startTimer = 0;
        currentSpeed = 0;
        player.transform.position = startingPosition;
        movementTriggered = false;

        if(currentTest == 2)
        {
            for (int i = 0; i < results.Length; i++) 
            {
                Debug.Log("Result " + (i+1) + ": " + results[i]);
            }
            //Output to the file then load the next scene

            using (StreamWriter streamWriter = new StreamWriter("output.txt", true)) 
            {
                if (SceneManager.GetActiveScene().buildIndex == 1) 
                {
                    streamWriter.WriteLine("");
                    DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
                    streamWriter.WriteLine("[Results recorded from test at " + now.ToString("MM/dd/yyyy") + "/" + now.ToUnixTimeSeconds() + "]");
                }
                streamWriter.WriteLine(SceneManager.GetActiveScene().name + " results: " + results[0] + ", " + results[1] + ", " + results[2]);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        currentTest++;

        testNumIndicator.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
        testNumIndicator.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + (currentTest + 1);
    }
}
