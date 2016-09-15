using UnityEngine;
using System.Collections;

public class partical_scaler : MonoBehaviour {
    public float scale = 10;
	// Use this for initialization
	void Start () {
        var particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem p in particles)
        {
            p.startSize *= scale;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
