using UnityEngine;
using System;
using System.Collections.Generic;

public class CLIENT_SERVANT_mod
{
    #region ini
    CLIENT father;

    List<GameObject> all_objects = new List<GameObject>();

    public delegate void refresh_function();

    public List<refresh_function> refresh_functions = new List<refresh_function>();

    public CLIENT_SERVANT_mod(CLIENT c)
    {
        father = c;
        initialize();
    }

    void initialize()
    {
        start_script();
    }

    public void fit_screen()
    {
        screen_size_changed();
    }

    void kill_oneself()
    {
        refresh_functions.Clear();
        for (int i = 0; i < all_objects.Count; i++)
        {
            MonoBehaviour.Destroy(all_objects[i]);
        }
    }

    public void update()
    {
        for (int i = 0; i < refresh_functions.Count; i++)
        {
            try
            {
                refresh_functions[i]();
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
    }

    public GameObject create_game_object(GameObject mod, Vector3 position, Quaternion quaternion, GameObject ui = null)
    {
        GameObject ob = (GameObject)MonoBehaviour.Instantiate(mod, position, quaternion);
        all_objects.Add(ob);
        if (ui == null)
        {
            ob.layer = 0;
        }
        else
        {
            ob.transform.SetParent(ui.transform, false);
            ob.layer = ui.layer;
        }
        return ob;
    }

    #endregion

    void start_script()
    {
        debugger.Log("start_script");
    }

    void screen_size_changed()
    {
        debugger.Log("screen_size_changed");
    }

}
