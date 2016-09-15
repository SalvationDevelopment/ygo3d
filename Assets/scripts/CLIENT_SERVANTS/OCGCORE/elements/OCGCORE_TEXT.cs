using System.Collections.Generic;
using UnityEngine;
public class OCGCORE_TEXT
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject game_object;
    UILabel label;
    public OCGCORE_TEXT(CLIENT_SERVANT_OCGCORE f,bool centered=false)
    {
        father = f;
        game_object = father.create_game_object(father.client.loader.mod_simple_ngui_text,Vector3.zero,Quaternion.identity,father.ui_main_2d);
        label = game_object.GetComponent<UILabel>();
        label.alpha = 0;
        label.depth = 0;
        label.text = "";
        if (centered)
        {
            label.alignment = NGUIText.Alignment.Center;
        }
        else
        {
            label.alignment = NGUIText.Alignment.Left;
        }
        father.refresh_functions.Add(handler);
        set_width(1500);
    }
    public void to_right()
    {
        label.alignment = NGUIText.Alignment.Right;
    }
    public void kill_oneself(){
        father.refresh_functions.Remove(handler);
        father.kill_game_object(game_object);
    }
    public int get_height()
    {
        return label.height;
    }
    public int get_width()
    {
        return label.width;
    }
    public void set_height(int i)
    {
         label.height=i;
    }
    public void set_width(int i)
    {
         label.width=i;
    }
    public string get_string()
    {
        if (dest_alpha == 0) return "";
        return label.text;
    }
    float dest_alpha = 0;
    List<string> strs = new List<string>();
    public void set_string(string raw)
    {
        if (raw == "" )
        {
            if (strs.Count>0)
            {
                strs.RemoveAt(0);
            }
        }
        if (raw != "" )
        {
            strs.Add(raw);
        }
        if (strs.Count==0)
        {
            dest_alpha = 0;
        }
        else
        {
            dest_alpha = 1;
            label.text = strs[strs.Count - 1];
        }
      //  debugger.Log(dest_alpha);
    }
    public void move_to_screen_point(Vector3 v)
    {
        Vector3 vvv = v;
        vvv.z = 0;
        in_world = false;
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.MoveTo(game_object, father.camera_main_2d.ScreenToWorldPoint(vvv), 0.3f);
    }
    public void rush_to_screen_point(Vector3 v)
    {
        Vector3 vvv = v;
        vvv.z = 0;
        in_world = false;
        game_object.transform.position = father.camera_main_2d.ScreenToWorldPoint(vvv);
    }
    public void fade_to_screen_point(Vector3 v)
    {
        Vector3 vvv = v;
        vvv.z = 0;
        in_world = false;
        game_object.transform.position += (father.camera_main_2d.ScreenToWorldPoint(vvv) - game_object.transform.position)*0.3f;
    }
    bool in_world=false;
    Vector3 lock_world_point=Vector3.zero;
    void handler()
    {
        if (in_world)
        {
            Vector3 screen_point = father.camera_game_main.WorldToScreenPoint(lock_world_point);
            screen_point.z = 0;
            Vector3 vvv = father.camera_main_2d.ScreenToWorldPoint(screen_point);
            game_object.transform.position = vvv;
        }
        label.alpha += (dest_alpha - label.alpha) * 0.1f;
    }

    public void move_to_world_point(Vector3 v)
    {
        lock_world_point = v;
        in_world = true;
        handler();
    } 


    

}

