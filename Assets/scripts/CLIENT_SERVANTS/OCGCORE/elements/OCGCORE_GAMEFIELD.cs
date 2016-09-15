using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class OCGCORE_GAMEFIELD
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject game_object = null;
    GameObject game_object_label = null;
    UILabel label = null;
    OCGCORE_TEXT printed_text;
    public GameObject me_left_p_num;
    public GameObject me_right_p_num;
    public GameObject op_left_p_num;
    public GameObject op_right_p_num;
    public OCGCORE_GAMEFIELD(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        game_object = father.create_game_object(father.client.loader.mod_simple_quad,Vector3.zero,Quaternion.identity,father.ui_container_3d);
        game_object.transform.eulerAngles = new Vector3(90, 0, 0);
        game_object_label = father.create_game_object(father.client.loader.mod_simple_ngui_text, Vector3.zero, Quaternion.identity, father.ui_container_3d);
        label = game_object_label.GetComponent<UILabel>();
        label.fontSize = 40;
        label.overflowMethod = UILabel.Overflow.ShrinkContent;
        label.width = 800;
        label.height = 40;
        game_object_label.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        game_object_label.transform.eulerAngles = new Vector3(60, 0, 0);
        game_object_label.transform.localPosition = new Vector3(0,0,-13.5f);
        FileStream file = new FileStream("table.png", FileMode.Open, FileAccess.Read);
        file.Seek(0, SeekOrigin.Begin);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        file.Close();
        file.Dispose();
        file = null;
        try
        {
            Texture2D pic = new Texture2D(1024, 600);
            pic.LoadImage(data);
            game_object.GetComponent<Renderer>().material.mainTexture = pic;
        }
        catch (Exception e)
        {
            debugger.Log(e);
        }
        game_object.transform.localScale = new Vector3(44.5f, 31.3245f, 1f);
        game_object.transform.localPosition = Vector3.zero;
        father.refresh_functions.Add(handler);
        printed_text = new OCGCORE_TEXT(father,false);
        me_left_p_num = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
       // me_left_p_num.GetComponent<number_loader>().set_number(3, 3);
        me_left_p_num.transform.localScale = new Vector3(2,2,2);
        me_left_p_num.transform.eulerAngles = new Vector3(0,-45,0);
        me_left_p_num.transform.position = new Vector3(0, 0, -5) + new Vector3(-13, 7, -1.2f);
        me_right_p_num = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
       // me_right_p_num.GetComponent<number_loader>().set_number(3, 3);
        me_right_p_num.transform.localScale = new Vector3(2, 2, 2);
        me_right_p_num.transform.eulerAngles = new Vector3(0, 45, 0);
        me_right_p_num.transform.position = new Vector3(0, 0, -5) + new Vector3(13, 7, -1.2f);
        op_left_p_num = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
        //op_left_p_num.GetComponent<number_loader>().set_number(3, 3);
        op_left_p_num.transform.localScale = new Vector3(2, 2, 2);
        op_left_p_num.transform.eulerAngles = new Vector3(0, -45, 0);
        op_left_p_num.transform.position = new Vector3(0, 0, 5) + new Vector3(-13, 7, -1.2f);
        op_right_p_num = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
        //op_right_p_num.GetComponent<number_loader>().set_number(3, 3);
        op_right_p_num.transform.localScale = new Vector3(2, 2, 2);
        op_right_p_num.transform.eulerAngles = new Vector3(0, 45, 0);
        op_right_p_num.transform.position = new Vector3(0, 0, 5) + new Vector3(13, 7, -1.2f);
    }
    public void set_string(string raw)
    {
        
        if (raw=="")
        {
            dest_alpha = 0;
        }
        else
        {
            label.text = raw;
            dest_alpha = 1;
        }
    }
    float dest_alpha = 0;
    int last_log_time = 0;
    List<string> logs = new List<string>();
    public void add_log(string str)
    {
        last_log_time = father.client.time;
        logs.Add(str);
        if (logs.Count > 4)
        {
            logs.RemoveAt(0);
        }
        string att = "";
        foreach (string ss in logs)
        {
            att += ss + "\n";
        }
        printed_text.set_width(Screen.width-650);
        printed_text.set_string("");
        printed_text.set_string(att);
    }
    public void handler()
    {
        if (father.client.time - last_log_time > 4000)
        {
            if (logs.Count>0)
            {
                logs.RemoveAt(0);
            }
            string att = "";
            foreach (string ss in logs)
            {
                att += ss + "\n";
            }
            printed_text.set_width(Screen.width - 650);
            printed_text.set_string("");
            printed_text.set_string(att);
            last_log_time = father.client.time;
        }
        label.alpha += (dest_alpha - label.alpha) * 0.1f;
        if (logs.Count > 0)
            printed_text.rush_to_screen_point(new Vector3(230 + printed_text.get_width() / 2, (printed_text.get_height() / 2)));
        

    }
    public void animation_show_big_string(string str)
    {
        GameObject obj = father.create_game_object(father.client.loader.mod_ocgcore_card_number_shower,Vector3.zero,Quaternion.identity);
        TMPro.TextMeshPro text_mesh = obj.GetComponent<TMPro.TextMeshPro>();
        TMPro.TextContainer text_container = obj.GetComponent<TMPro.TextContainer>();
        text_container.width = 60;
        text_container.height = 10;
        text_mesh.text = str;
        text_mesh.alignment = TMPro.TextAlignmentOptions.Center;
        obj.transform.position = father.camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width/2f,Screen.height/2f+50f,15));
        obj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2f, Screen.height / 2f + 50f, 15);
        obj.transform.localScale = Vector3.zero;
        iTween.ScaleTo(obj,new Vector3(1,1,1),0.3f);
        iTween.RotateTo(obj,new Vector3(60,0,0),0.3f);
        iTween.ScaleTo(obj, iTween.Hash(
                           "delay", 0.6f,
                           "x", 0,
                           "y", 0,
                           "z", 0,
                           "time", 0.6f
                           ));
        father.kill_game_object(obj,3f);
    }
    public void animation_show_lp_num(bool me,bool up,int count)
    {
        int color = 0;
        if(up){
            color = 3;
        }
        Vector3 position;
        Vector3 screen_p;
        if(me){
            screen_p = new Vector3(Screen.width / 2f, 100f, 5);
            position = father.camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 100f, 5));
        }
        else
        {
            screen_p = new Vector3(Screen.width / 2f, Screen.height - 100f, 5);
            position = father.camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height -100f, 5));
        }




        GameObject obj = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
        obj.GetComponent<number_loader>().set_number(count, color);
        obj.AddComponent<animation_screen_lock>().screen_point = screen_p;
        obj.transform.position = position;
        obj.transform.localScale = Vector3.zero;
        iTween.ScaleTo(obj, new Vector3(1, 1, 1), 0.3f);
        iTween.RotateTo(obj, new Vector3(60, 0, 0), 0.3f);
        iTween.ScaleTo(obj, iTween.Hash(
                           "delay", 1.6f,
                           "x", 0,
                           "y", 0,
                           "z", 0,
                           "time", 0.6f
                           ));
        father.kill_game_object(obj, 5f);


    }
    public void animation_show_card_code(long code)
    {
        code_for_show = code;
        father.refresh_functions.Add(animation_show_card_code_handler);
    }
    long code_for_show = 0;
    void animation_show_card_code_handler()
    {
        Texture2D texture = father.picture_loader.get(code_for_show, ocgcore_picture_type.card_picture);
        if (texture != null)
        {
            father.refresh_functions.Remove(this.animation_show_card_code_handler);
            Vector3 position = father.camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 10));
            GameObject obj = father.create_game_object(father.client.loader.mod_simple_quad, Vector3.zero, Quaternion.identity);
            obj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2f, Screen.height / 2f, 10);
            obj.transform.eulerAngles = new Vector3(60,0,0);
            obj.GetComponent<Renderer>().material.mainTexture = texture;
            obj.transform.localPosition = position;
            obj.transform.localScale = Vector3.zero;
            iTween.ScaleTo(obj, new Vector3(3, 4, 1), 0.3f);
            iTween.RotateTo(obj, new Vector3(60, 0, 0), 0.3f);
            iTween.ScaleTo(obj, iTween.Hash(
                               "delay", 1.6f,
                               "x", 0,
                               "y", 0,
                               "z", 0,
                               "time", 0.6f
                               ));
            father.kill_game_object(obj, 5f);

        }
    }
    class field_disabled_container
    {
        public point p;
        public Vector3 position;
        public GameObject game_object;
        public field_disabled_container(CLIENT_SERVANT_OCGCORE father,point p_)
        {
            p = p_;
            position = father.get_point_worldposition(p);
        }
    }
    List<field_disabled_container> field_disabled_containers = new List<field_disabled_container>();
    public void set_point_disabled(point p, bool disabled)
    {
        field_disabled_container container = null;

        foreach (field_disabled_container cont in field_disabled_containers)
        {
            if (cont.p.me == p.me)
            {
                if (cont.p.location == p.location)
                {
                    if (cont.p.index == p.index)
                    {
                        container = cont;
                        break;
                    }
                }
            }
        }

        if (container == null)
        {
            container = new field_disabled_container(father, p);
            field_disabled_containers.Add(container);
        }


        if (disabled)
        {
            if (container.game_object == null)
            {
                container.game_object =
                    father.create_game_object(father.client.loader.mod_ocgcore_decoration_cage_of_field, container.position, Quaternion.identity);
            }
        }
        else
        {
            if (container.game_object != null)
            {
                MonoBehaviour.Destroy(container.game_object.transform.FindChild("Cube").gameObject);
                father.kill_game_object(container.game_object, 5f);
            }
        }
    }

}

