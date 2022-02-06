using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]    // allows you to add only 1 script 
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;
    [Range(0,1)][SerializeField] float movementFactor;
    Vector3 startingPos;

	// Use this for initialization
	void Start () 
    {
        startingPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //set movement factor
        if (period <= Mathf.Epsilon) { return; }  //return = stop
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;   // Will not change const //about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
	}
}
