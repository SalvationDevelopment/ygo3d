using System;
using UnityEngine;
public class OCGCORE_UNDER_UI
{
    CLIENT_SERVANT_OCGCORE father;
    OCGCORE_HEALTH_BAR me_bar;
    OCGCORE_HEALTH_BAR op_bar;
    GameObject object_card_hint;
    UILabel UILabel_card_name;
    UILabel UILabel_card_type;
    UITextList UITextList_card_description;
    UITexture UITexture_card_pic;
    GameObject object_select;
    UIToggle UIToggle_shift;
    public GameObject GameObject_shift;
    UIToggle UIToggle_chain;
    GameObject GameObject_chain;
    OCGCORE_TEXT mini_text;
    UISprite UISprite_card_hint;
    public OCGCORE_UNDER_UI(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        try
        {
            Delegater temp;
            me_bar = new OCGCORE_HEALTH_BAR(father);
            me_bar.set_user_name("我方");
            op_bar = new OCGCORE_HEALTH_BAR(father);
            op_bar.set_user_name("对方");
            mini_text = new OCGCORE_TEXT(father,false);
            mini_text.to_right();
            mini_text.set_width(800);
            object_card_hint = father.create_game_object(father.client.loader.mod_ocgcore_ui_card_hint,Vector3.zero,Quaternion.identity,father.ui_back_ground_2d);
            UILabel_card_name = object_card_hint.transform.FindChild("card_name").GetComponent<UILabel>();
            UILabel_card_type = object_card_hint.transform.FindChild("card_type").GetComponent<UILabel>();
            UITextList_card_description = object_card_hint.transform.FindChild("card_description").GetComponent<UITextList>();
           // GameObject_card_description = UILabel_card_description.gameObject;
           // GameObject_card_description.transform.SetParent(father.ui_back_ground_2d.transform, false);
            UITexture_card_pic = object_card_hint.transform.FindChild("card_pic").GetComponent<UITexture>();
            UISprite_card_hint = object_card_hint.transform.FindChild("under_pic").GetComponent<UISprite>();
            object_select = father.create_game_object(father.client.loader.mod_ocgcore_ui_select_chain, Vector3.zero, Quaternion.identity, father.ui_back_ground_2d);
            UIToggle_shift = object_select.transform.FindChild("Checkbox_info").GetComponent<UIToggle>();
            GameObject_shift = UIToggle_shift.gameObject;
            GameObject_shift.transform.SetParent(father.ui_back_ground_2d.transform,false);
            GameObject_shift.GetComponent<UIWidget>().depth = 100;
            UIToggle_chain = object_select.transform.FindChild("Checkbox_chain").GetComponent<UIToggle>();
            UIToggle_chain.value = false;
            GameObject_chain = UIToggle_chain.gameObject;
            temp = UIToggle_shift.gameObject.AddComponent<Delegater>();
            temp.f = on_shift;
            UIToggle_shift.onChange.Add(new EventDelegate(temp,"function"));
            debugger.Log("ui_loaded");
            /////hide_all
            father.camera_game_main.fieldOfView = 60;
            father.camera_container_3d.fieldOfView = 60;
            father.camera_main_3d.fieldOfView = 60;
            me_bar.rush_to_screen_vector(new Vector3(Screen.width + 400, Screen.height));
            op_bar.rush_to_screen_vector(new Vector3(Screen.width + 400, Screen.height));
            GameObject_shift.transform.position = screen_to_world(new Vector3(Screen.width + 400, Screen.height));
            GameObject_shift.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
            object_select.transform.position = screen_to_world(new Vector3(Screen.width + 400, Screen.height));
            mini_text.rush_to_screen_point(new Vector3(Screen.width +100000, Screen.height +10000));
            object_card_hint.transform.position = screen_to_world(new Vector3(- 400, Screen.height/2));
            father.refresh_functions.Add(update);
            change_data(father.card_data_manager.GetById(0));
        }
        catch(Exception e)
        {
            debugger.Log(e);
        }
       

    }

    float destinate_fieldOfView = 0;
    Vector3 destinate_me_bar_vector = Vector3.zero;
    Vector3 destinate_op_bar_vector = Vector3.zero;
    Vector3 destinate_shift_vector = Vector3.zero;
    Vector3 destinate_select_vector = Vector3.zero;
    Vector3 destinate_mini_text_vector = Vector3.zero;
    Vector3 destinate_card_hint_vector = Vector3.zero;
    //Vector3 destinate_card_hint_text_vector = Vector3.zero;
    Vector3 destinate_ui_buttons = Vector3.zero;
    Vector3 screen_to_world(Vector3 s)
    {
        Vector3 screen = s;
        screen.z = 0;
        return father.camera_back_ground_2d.ScreenToWorldPoint(screen);
    }
    public void on_shift()
    {
        set_text_mini();
        UISprite_card_hint.height = (int)(Screen.height/0.85) - 16;
        if (UIToggle_shift.value)
        {
            debugger.Log("显示主界面");
            father.idle_container.to_2d();
            destinate_fieldOfView = 75;
            mini_text.move_to_screen_point(new Vector3(Screen.width - 410, Screen.height + 50));
            destinate_me_bar_vector = screen_to_world(new Vector3(Screen.width -150, Screen.height-60));
            destinate_op_bar_vector = screen_to_world(new Vector3(Screen.width -150, Screen.height-160));
            destinate_shift_vector = screen_to_world(new Vector3(Screen.width - 130, Screen.height - 235));
            destinate_select_vector = screen_to_world(new Vector3(Screen.width - 135, Screen.height-235));
            destinate_card_hint_vector = screen_to_world(new Vector3(UISprite_card_hint.width/2+190, Screen.height / 2+106));
            destinate_ui_buttons = screen_to_world(new Vector3(Screen.width - 50, 40));
        }
        else
        {
            debugger.Log("隐藏主界面");
            father.idle_container.to_3d();
            destinate_fieldOfView = 65;
            mini_text.move_to_screen_point(new Vector3(Screen.width - 410, Screen.height - 20));
            destinate_me_bar_vector = screen_to_world(new Vector3(Screen.width + 400, Screen.height));
            destinate_op_bar_vector = screen_to_world(new Vector3(Screen.width + 400, Screen.height));
            destinate_shift_vector = screen_to_world(new Vector3(Screen.width -105, Screen.height-40));
            destinate_select_vector = screen_to_world(new Vector3(Screen.width + 400, Screen.height));
            destinate_card_hint_vector = screen_to_world(new Vector3(-400, Screen.height / 2));
            destinate_ui_buttons = screen_to_world(new Vector3(Screen.width + 130, 40));
        }
        father.go_to_min_camera();
    }
    public void set_text_mini()
    {
        mini_text.set_string("8000:8000 我方操作 主要阶段");
    }
    public void change_data(CardData data)
    {
        card_data = data;
        UILabel_card_name.text = card_string_helper.get_name_string(data);
        UILabel_card_type.text = card_string_helper.get_type_string((int)data.Type);
        UITextList_card_description.Clear();
        UITextList_card_description.Add(card_string_helper.get_string(data));
    }
    public bool get_if_watch_chains()
    {
        return UIToggle_chain.value;
    }
    public bool get_if_show_all()
    {
        return UIToggle_shift.value;
    }
    public OCGCORE_HEALTH_BAR get_health_bar(bool me){
        if(me){
            return me_bar;
        }else{
            return op_bar;
        }
    }
    CardData card_data;
    long pic_code=-1;
    public void update()
    {
        father.camera_game_main.fieldOfView += (destinate_fieldOfView - father.camera_game_main.fieldOfView)*0.2f;
        father.camera_container_3d.fieldOfView += (destinate_fieldOfView - father.camera_container_3d.fieldOfView)*0.2f;
        father.camera_main_3d.fieldOfView += (destinate_fieldOfView - father.camera_main_3d.fieldOfView)*0.2f;
        me_bar.fade_to_vector(destinate_me_bar_vector);
        op_bar.fade_to_vector(destinate_op_bar_vector);
        GameObject_shift.transform.position += (destinate_shift_vector - GameObject_shift.transform.position) * 0.2f;
        object_select.transform.position += (destinate_select_vector - object_select.transform.position) * 0.2f;
        object_card_hint.transform.position += (destinate_card_hint_vector - object_card_hint.transform.position) * 0.2f;
        if (pic_code != card_data.code)
        {
            Texture2D pic = father.picture_loader.get(card_data.code, ocgcore_picture_type.card_picture);
            if (pic!=null)
            {
                pic_code = card_data.code;
                UITexture_card_pic.mainTexture = pic;
            }
        }
    }
}
