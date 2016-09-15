using UnityEngine;
using System.Collections;
using System;

public class attacksign : MonoBehaviour {
	public Vector3 from;
	public Vector3 to;
	// Use this for initialization
	int time = 0;
	void Start () {
		gameObject.transform.position = from;
		gameObject.transform.LookAt(to);
		time = Environment.TickCount;
        iTween.ScaleTo(gameObject, new Vector3(2,2,2), 1);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 from_ = get_over(from);
        Vector3 to_ = get_over(to);
        gameObject.transform.position = (from_ + to_) * 0.5f + (to_ - from_) * 0.5f * (float)Math.Sin(3.1415926f * (Environment.TickCount - time) / 500);
	//	gameObject.transform.Rotate((new Vector3(0, 0, 1)) * Time.deltaTime * 50);
	}


    Vector3 get_over(Vector3 i)
    {
        Vector3 o=Vector3.zero;
        Vector3 scr = Camera.main.WorldToScreenPoint(i);
        scr.z -= 3;
        o = Camera.main.ScreenToWorldPoint(scr);
        return o;
    }
}
