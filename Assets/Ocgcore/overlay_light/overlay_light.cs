using UnityEngine;
using System.Collections;

public class overlay_light : MonoBehaviour {
    Vector3 v = Vector3.right;
    GameObject light;
    void Start()
    {
        light = gameObject.transform.FindChild("light").gameObject;
        v = new Vector3(get_rand(), get_rand(), get_rand());
        Vector3 chuizhi = (new Vector3(1, 1, -(v.x + v.y) / v.z)) / Vector3.Distance(Vector3.zero, new Vector3(1, 1, -(v.x + v.y) / v.z));
        light.transform.localPosition =  chuizhi * 5;

    }
    float get_rand()
    {
        float r = (float)Random.Range(-100, 100) / 10f;
        return r;
    }
    // Update is called once per frame
    void Update()
    {
        light.transform.RotateAround(gameObject.transform.position, v, 90 * Time.deltaTime);
    }
}
