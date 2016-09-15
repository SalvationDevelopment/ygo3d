using UnityEngine;
using System.Collections;

public class ban_icon : MonoBehaviour {
    public Texture2D tex_0;
    public Texture2D tex_1;
    public Texture2D tex_2;
    public Texture2D tex_3;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void show(int i){
        if (i == 0)
        {
            gameObject.GetComponent<UITexture>().mainTexture = tex_0;
        }
        if (i == 1)
        {
            gameObject.GetComponent<UITexture>().mainTexture = tex_1;
        }
        if (i == 2)
        {
            gameObject.GetComponent<UITexture>().mainTexture = tex_2;
        }
        if (i == 3)
        {
            gameObject.GetComponent<UITexture>().mainTexture = tex_3;
        }
    }
}
