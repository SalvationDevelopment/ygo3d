using UnityEngine;
using System.Collections;
using System;

public class select_floating : MonoBehaviour
{
	int open = 0;
	// Use this for initialization
	void Start () {
		open = Environment.TickCount-UnityEngine.Random.Range(0,500);
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localPosition = new Vector3(0,1+ 0.5f * (float)Math.Sin(3.1415936f * (float)(Environment.TickCount - open) / 500f), 1.732f);
	}
}
