using System.Collections.Generic;
using UnityEngine;
public class OCGCORE_ANNOUNCE_TEXT
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject window;
    UISprite UISprite_window;
    List<GameObject> buttons = new List<GameObject>();
    public OCGCORE_ANNOUNCE_TEXT(CLIENT_SERVANT_OCGCORE f,string hint)
    {
        father = f;
        window = father.create_game_object(father.client.loader.mod_ocgcore_select_common, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        UISprite_window = window.transform.FindChild("under_pic").GetComponent<UISprite>();
        UISprite_window.height = 0;
        UISprite_window.alpha = 0;
        window.transform.FindChild("Label").GetComponent<UILabel>().text = hint;
        father.refresh_functions.Add(update);
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
        UISprite_window.height = 100 + ui_helper.get_zonghangshu(buttons.Count,4) * 70;
        UISprite_window.alpha += (1 - UISprite_window.alpha) * 0.3f;
        for (int i = 0; i < buttons.Count; i++)
        {
            Vector2 v = ui_helper.get_hang_lie(i, 4);
            float hang = v.x;
            float lie = v.y;
            buttons[i].transform.localScale += (new Vector3(1, 1, 1) - buttons[i].transform.localScale) * 0.3f;
            buttons[i].transform.localPosition += (window.transform.localPosition + new Vector3(-180 + lie * 120, 0 + UISprite_window.height / 2 - 100 - hang * 70, 0) - buttons[i].transform.localPosition) * 0.3f;

        }
        if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (father.client.pointed_game_object == buttons[i])
                {
                    father.ui_button_selected(buttons[i]);
                }
            }
        }

    }
    public void add_opt(string opt,int ptr)
    {
        GameObject btn = father.create_game_object(father.client.loader.mod_ocgcore_select_text_option, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        btn.transform.localScale = Vector3.zero;
        btn.transform.FindChild("Label").GetComponent<UILabel>().text = opt;
        btn.name = ptr.ToString();
        buttons.Add(btn);
    }
}

