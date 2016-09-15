using System;
using UnityEngine;
public class OCGCORE_HEALTH_BAR
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject object_health_bar=null;
    GameObject obj_light = null;
    UILabel user_life_label = null;
    UILabel user_name_label = null;
    UILabel user_time_label = null;
    UITexture user_face_texture = null;
    public OCGCORE_HEALTH_BAR(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        try
        {
            object_health_bar = father.create_game_object(father.client.loader.mod_ocgcore_ui_health, Vector3.zero, Quaternion.identity, father.ui_back_ground_2d);
            object_health_bar.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            user_life_label = object_health_bar.transform.FindChild("user_life").GetComponent<UILabel>();
            user_name_label = object_health_bar.transform.FindChild("user_name").GetComponent<UILabel>();
            user_time_label = object_health_bar.transform.FindChild("user_time").GetComponent<UILabel>();
            user_face_texture = object_health_bar.transform.FindChild("user_face").GetComponent<UITexture>();
            user_face_texture.mainTexture = ui_helper.get_rand_face();
        }catch(Exception e){
            debugger.Log(e);
        }
        father.refresh_functions.Add(hander);
    }
    public int get_user_life()
    {
       // return int.Parse(user_life_label.text);
        return life;
    }
    public void set_user_name(string str)
    {
        user_name_label.text = str;
    }
    public void set_user_color(Color c)
    {
        user_name_label.gradientTop = c;
    }
    public void set_user_time(int time)
    {
        user_time_label.text = time.ToString();
    }
    public void set_user_face(Texture2D face)
    {
        user_face_texture.mainTexture = face;
    }
    int life = 0;
    int life_now = 0;
    public void set_user_life(int l)
    {
        //user_life_label.text = life.ToString();
        life = l;
    }
    public void set_user_light(bool on)
    {
        if (on)
        {
            if (obj_light==null)
            {
                obj_light = father.create_game_object(father.client.loader.mod_ocgcore_ui_time_effect,Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
            if (obj_light != null)
            {
                father.kill_game_object(obj_light);
            }
        }
    }
    void hander()
    {
        life_now += (int)((float)(life - life_now) * 0.1f);
        if (Math.Abs(life-life_now)<10)
        {
            life_now = life;
        }
        user_life_label.text = life_now.ToString();
        if (obj_light != null)
        {
            Vector3 screen_point = father.camera_back_ground_2d.WorldToScreenPoint(user_time_label.gameObject.transform.position);
            screen_point.y += 30;
            screen_point.z = 15;
            obj_light.transform.position = father.camera_game_main.ScreenToWorldPoint(screen_point);
        }
        if (time_now>0)
        {
            if (obj_light != null)
            {
                if (father.client.time - time_temp > 1000)
                {

                    time_temp = father.client.time;
                    time_now--;
                    user_time_label.text = time_now.ToString();
                }
            }
        }
    }
    public void rush_to_screen_vector(Vector3 v)
    {
        Vector3 screenpoint = v;
        screenpoint.z = 0;
        Vector3 worldpoint = father.camera_back_ground_2d.ScreenToWorldPoint(screenpoint);
        object_health_bar.transform.position = worldpoint;
    }
    public void fade_to_vector(Vector3 v)
    {
        object_health_bar.transform.position += (v - object_health_bar.transform.position)*0.3f;
    }


    int time_now = 240;
    int time_temp = 0;
    public void start_time(int time)
    {
        time_now = time;
        time_temp = father.client.time;
    }
}
