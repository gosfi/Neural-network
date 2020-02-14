using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector3 startPos, startRotation;
    private NeuralNetwork _neuralNetwork;
    [Range(-1, 1)] public float a, t;

    public float timeSinceStart = 0f;

    [Header("Network Options")] public int Layers = 1;
    
    
    [Header("Fitness")] public float ovrlFitness;
    public float distanceMultiplier;
    public float avrgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    private Vector3 lastPos;
    private float totalDistanceTravel;

    private float avgSpeed;

    private float aSensor, bSensor, cSensor;

    // Start is called before the first frame update
    private void Awake()
    {
        startPos = transform.position;
        startRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    public void Restart()
    {
        timeSinceStart = 0f;
        totalDistanceTravel = 0f;
        avgSpeed = 0f;
        lastPos = startPos;
        transform.position = startPos;
        transform.eulerAngles = startRotation;
    }

    private void FixedUpdate()
    {
        InputSensors();
        lastPos = transform.position;

        MoveCar(a, t);
        timeSinceStart += Time.fixedDeltaTime;

        calculateFitness();
    }

    private void calculateFitness()
    {
        totalDistanceTravel += Vector3.Distance(transform.position, lastPos);
        avgSpeed = totalDistanceTravel / timeSinceStart;

        ovrlFitness = (totalDistanceTravel * distanceMultiplier) + (avgSpeed * avrgSpeedMultiplier) +
                      (((aSensor + bSensor + cSensor) / 3) * sensorMultiplier);

        if (timeSinceStart > 20 && ovrlFitness < 40)
        {
            Restart();
        }

        if (ovrlFitness >= 1000)
        {
            Restart();
        }
    }

    private void InputSensors()
    {
        Vector3 a = (transform.forward + transform.right);
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            aSensor = hit.distance / 20;
            print("A: " + aSensor);
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = b;
        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 20;
            print("B: " + bSensor);
            Debug.DrawLine(r.origin, hit.point, Color.blue);
        }

        r.direction = c;
        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 20;
            print("C: " + cSensor);
            Debug.DrawLine(r.origin, hit.point, Color.green);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Restart();
    }

    private Vector3 inp;

    public void MoveCar(float v, float h)
    {
        inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;
        transform.eulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);
    }
}