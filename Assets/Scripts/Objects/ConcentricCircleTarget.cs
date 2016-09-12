using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcentricCircleTarget : MonoBehaviour {

    private Vector3 targetScale;
    private Vector3 startingScale = new Vector3(4f,4f,1f);
    private float speedToShrink;
    private float startTime;
    private float scaleCovered = 4;
    public string keyPressed = "a";
    public bool stopShrinking;
    public GameObject targetRing;
    public GameObject scalingRing;
    public float fracScale = 0;

    void Start()
    {
        scalingRing.transform.localScale = startingScale;
        float randomScale = Random.Range(5f, 35f) / 10;
        targetScale = new Vector3(randomScale, randomScale, 1f);
        targetRing.transform.localScale = targetScale;
        speedToShrink = Random.Range(200f, 350f) / 100;
        startTime = Time.time;
    }

    void Update()
    {
        if (stopShrinking)
        {
            speedToShrink = 0;
        }
        else
        {
            float scaledValue = (Time.time - startTime) * speedToShrink;
            fracScale = scaledValue / scaleCovered;
            scalingRing.transform.localScale = Vector3.Lerp(startingScale, new Vector3(0, 0, 1), fracScale);
        }
        if (Input.GetKeyDown(keyPressed))
        {
            stopShrinking = true;
        }
    }
}
