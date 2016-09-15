using System.Collections.Generic;
using UnityEngine;
public class ROOM_SELECT_TP
{
    CLIENT_SERVANT_ROOM father;
    GameObject window;
    UISprite UISprite_window;
    UILabel lable;
    List<GameObject> buttons = new List<GameObject>();
    int time_start=0;
    public ROOM_SELECT_TP(CLIENT_SERVANT_ROOM f)
    {
        father = f;
        time_start = father.client.time;
        window = father.create_game_object(father.client.loader.mod_ocgcore_select_common, Vector3.zero, Quaternion.identity, father.client.ui_main_2d);
        UISprite_window = window.transform.FindChild("under_pic").GetComponent<UISprite>();
        UISprite_window.height = 0;
        UISprite_window.alpha = 0;
        //window.transform.FindChild("Label").GetComponent<UILabel>().text = "请选择回合的顺序";
        lable = window.transform.FindChild("Label").GetComponent<UILabel>();
        lable.text = "";
        father.refresh_functions.Add(update);
        add_opt("先攻就由我拿下了！",0);
        add_opt("还是由对方先攻好了。", 1);
    }
    public void kill_oneself()
    {
        iTween.ScaleTo(window, Vector3.zero, 0.6f);
        father.kill_game_object(window, 0.6f);
        father.refresh_functions.Remove(update);
        for (int i = 0; i < buttons.Count; i++)
        {
            iTween.ScaleTo(buttons[i], Vector3.zero, 0.4f);
            father.kill_game_object(buttons[i], 0.4f);
        }
    }
    void update()
    {
        if (father.client.time-time_start>4000)
        {
            lable.text = "请选择回合的顺序";
            UISprite_window.height = 100 + buttons.Count * 70;
            UISprite_window.alpha += (1 - UISprite_window.alpha) * 0.3f;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.localScale += (new Vector3(1, 1, 1) - buttons[i].transform.localScale) * 0.3f;
                buttons[i].transform.localPosition += (window.transform.localPosition + new Vector3(-170, 0 + UISprite_window.height / 2 - 100 - i * 70, 0) - buttons[i].transform.localPosition) * 0.3f;

            }
            if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (father.client.pointed_game_object == buttons[i].transform.FindChild("button").gameObject)
                    {
                        father.tp_windows_selected(int.Parse(buttons[i].name));
                    }
                }
            }
        }
    }
    public void add_opt(string opt, int ptr)
    {
        GameObject btn = father.create_game_object(father.client.loader.mod_ocgcore_select_button_option, Vector3.zero, Quaternion.identity, father.client.ui_main_2d);
        btn.transform.localScale = Vector3.zero;
        btn.transform.FindChild("Label").GetComponent<UILabel>().text = opt;
        btn.name = ptr.ToString();
        buttons.Add(btn);
    }
}

