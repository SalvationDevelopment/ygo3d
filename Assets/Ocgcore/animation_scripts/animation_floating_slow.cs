using System;
using UnityEngine;
using System.Collections;

public class animation_floating_slow : MonoBehaviour
{
    int time_last = 0;
    float rad = 1;
    void Start()
    {
        int time_last = Environment.TickCount;
        rad = (float)UnityEngine.Random.Range(80, 120) / 100f;
    }
    void Update()
    {
        int time_now = Environment.TickCount;
        float sin_last = (float)Math.Sin(((double)time_last) / 1000d * 3.1415926d * rad/5) * rad/5;
        float sin_now = (float)Math.Sin(((double)time_now) / 1000d * 3.1415926d * rad/5) * rad/5;
        gameObject.transform.position = new Vector3
            (
            gameObject.transform.position.x,
            gameObject.transform.position.y + sin_now - sin_last,
            gameObject.transform.position.z
            );
        time_last = time_now;
    }

}
