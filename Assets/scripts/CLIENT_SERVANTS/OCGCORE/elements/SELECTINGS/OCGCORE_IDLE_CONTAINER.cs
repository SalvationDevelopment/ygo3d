using System.Collections.Generic;
using UnityEngine;
public class OCGCORE_IDLE_CONTAINER
{
    CLIENT_SERVANT_OCGCORE father;
    List<OCGCORE_BUTTON> OCGCORE_BUTTONs = new List<OCGCORE_BUTTON>();
    GameObject gameobject_container = null;
    UISprite gameobject_container_UISprite = null;
    UILabel label;
    public OCGCORE_IDLE_CONTAINER(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        father.refresh_functions.Add(refresh_2d);
        father.refresh_functions.Add(refresh_3d);
        to_2d();
    }
    public Vector3 position = Vector3.zero;
    string text_string="";
    List<string> strs = new List<string>();
    public void set_text(string text)
    {
        if (text == father.now_hint)
        {
            if (strs.Count > 0)
            {
                strs.RemoveAt(0);
            }
        }
        if (text != father.now_hint)
        {
            strs.Add(text);
        }
        if (strs.Count == 0)
        {
            text_string = "操作按钮";
        }
        else
        {
            text_string = strs[strs.Count - 1];

        }
        if (label != null) label.text = text_string;
    }
    private bool is_2d_ui = true;
    Vector3 screen_2d_position_now;
    public bool have_length=false;
    public void to_2d()
    {
        is_2d_ui = true;
        screen_2d_position_now = father.camera_game_main.WorldToScreenPoint(position);
        have_length = false;
    }
    public void to_3d()
    {
        is_2d_ui = false;
        if(OCGCORE_BUTTONs.Count>0){
            have_length = true;
        }
    }
    public void refresh_2d()
    {
        if (is_2d_ui == true)
        {
            if (OCGCORE_BUTTONs.Count > 0)
            {

                if (gameobject_container == null)
                {
                    gameobject_container = father.create_game_object(father.client.loader.mod_ocgcore_idle_container, position, Quaternion.identity, father.ui_container_3d);
                    gameobject_container.transform.localScale = new Vector3(0, 0, 0);
                    gameobject_container_UISprite = gameobject_container.GetComponent<UISprite>();
                    label = gameobject_container.transform.FindChild("Label").GetComponent<UILabel>();
                    label.text = text_string;
                    screen_2d_position_now = new Vector3(Screen.width - (70) / 1024f * (float)Screen.width, Screen.height - 265-(80) / 600f * (float)Screen.height, 20f);
                }
                screen_2d_position_now += (new Vector3(Screen.width - (70) / 1024f * (float)Screen.width, Screen.height -265- (80) / 600f * (float)Screen.height, 20f) - screen_2d_position_now) * 0.3f;
                gameobject_container_UISprite.height += (int)(((int)(OCGCORE_BUTTONs.Count * (2.8f / 0.03f) + 100f) - gameobject_container_UISprite.height)*0.3f);
                label.gameObject.transform.localPosition = new Vector3(0, gameobject_container_UISprite.height / 2 - 60f, 0);
                gameobject_container_UISprite.width = 100;
                gameobject_container.transform.eulerAngles += (new Vector3(60, 0, 0) - gameobject_container.transform.eulerAngles) * 0.3f;
                gameobject_container.transform.localScale += (new Vector3(0.03f, 0.03f, 0.03f) - gameobject_container.transform.localScale) * 0.3f;
                Vector3 first_vector = Vector3.zero;
                Vector3 last_vector = Vector3.zero;
                Vector3 temp_position = father.camera_game_main.ScreenToWorldPoint(screen_2d_position_now);
                for (int i = 0; i < OCGCORE_BUTTONs.Count; i++)
                {
                    Vector3 want = temp_position;
                    want.y -= i * 2.8f / 2f;
                    want.z -= i * 2.8f / 2f * 1.732f;
                    OCGCORE_BUTTONs[i].very_rush_to_vector(want);
                    if (i == 0)
                    {
                        first_vector.x = want.x;
                        first_vector.y = want.y;
                        first_vector.z = want.z;
                    }
                    if (i == OCGCORE_BUTTONs.Count - 1)
                    {
                        last_vector.x = want.x;
                        last_vector.y = want.y;
                        last_vector.z = want.z;
                    }
                }
                gameobject_container.transform.localPosition = (first_vector + last_vector) / 2 + new Vector3(0, 1f / 2f, 1f / 2f * 1.732f);
            }
            if (OCGCORE_BUTTONs.Count == 0)
            {
                if (gameobject_container != null)
                {
                    iTween.ScaleTo(gameobject_container, new Vector3(0, 0, 0), 0.4f);
                    father.kill_game_object(gameobject_container, 0.4f);
                }
            }
        }
    }
    public void refresh_3d()
    {
        if (is_2d_ui == false)
        {
            if (OCGCORE_BUTTONs.Count > 0)
            {
                have_length = true;
                if (gameobject_container == null)
                {
                    gameobject_container = father.create_game_object(father.client.loader.mod_ocgcore_idle_container, position, Quaternion.identity, father.ui_container_3d);
                    gameobject_container.transform.localScale = new Vector3(0, 0, 0);
                    gameobject_container_UISprite = gameobject_container.GetComponent<UISprite>();
                    label = gameobject_container.transform.FindChild("Label").GetComponent<UILabel>();
                    label.text = text_string;
                }
                gameobject_container_UISprite.height += (int)(((int)(ui_helper.get_zonghangshu(OCGCORE_BUTTONs.Count, 3) * (2.8f / 0.03f) + 100f) - gameobject_container_UISprite.height)*0.3f);
                label.gameObject.transform.localPosition = new Vector3(0, gameobject_container_UISprite.height / 2 - 60f, 0);
                if (ui_helper.get_zonghangshu(OCGCORE_BUTTONs.Count, 3) > 1 || OCGCORE_BUTTONs.Count == 3)
                {
                    gameobject_container_UISprite.width = 320;
                }
                else
                {
                    gameobject_container_UISprite.width = 230;
                }
                gameobject_container.transform.eulerAngles += (new Vector3(60, 0, 0) - gameobject_container.transform.eulerAngles) * 0.3f;
                gameobject_container.transform.localScale += (new Vector3(0.03f, 0.03f, 0.03f) - gameobject_container.transform.localScale) * 0.3f;
                Vector3 first_vector = Vector3.zero;
                Vector3 last_vector = Vector3.zero;
                for (int i = 0; i < OCGCORE_BUTTONs.Count; i++)
                {
                    Vector3 want = position+new Vector3(0,0,-0.7f);
                    if (ui_helper.get_shifouzaizuihouyihang(OCGCORE_BUTTONs.Count, 3, i))
                    {
                        Vector2 vector_temp = ui_helper.get_hang_lie(i, 3);
                        float hang = vector_temp.x;
                        float lie = vector_temp.y;
                        want.x += -2.8f * ui_helper.get_zuihouyihangdegeshu(OCGCORE_BUTTONs.Count, 3) / 2 + 1.4f + lie * 2.8f;
                        want.y -= hang * 2.8f / 2f;
                        want.z -= hang * 2.8f / 2f * 1.732f;
                    }
                    else
                    {
                        Vector2 vector_temp = ui_helper.get_hang_lie(i, 3);
                        float hang = vector_temp.x;
                        float lie = vector_temp.y;
                        want.x += -2.8f * 3 / 2 + 1.4f + lie * 2.8f;
                        want.y -= hang * 2.8f / 2f;
                        want.z -= hang * 2.8f / 2f * 1.732f;
                    }
                    OCGCORE_BUTTONs[i].rush_to_vector(want);
                    if (i == 0)
                    {
                        first_vector.x = want.x;
                        first_vector.y = want.y;
                        first_vector.z = want.z;
                    }
                    if (i == OCGCORE_BUTTONs.Count - 1)
                    {
                        last_vector.x = want.x;
                        last_vector.y = want.y;
                        last_vector.z = want.z;
                    }
                }
                gameobject_container.transform.localPosition += ((first_vector + last_vector) / 2 + new Vector3(0, 1f / 2f, 1f / 2f * 1.732f) - gameobject_container.transform.localPosition) * 0.3f;
            }
            if (OCGCORE_BUTTONs.Count == 0)
            {
                have_length = false;
                if (gameobject_container != null)
                {
                    iTween.ScaleTo(gameobject_container, new Vector3(0, 0, 0), 0.4f);
                    father.kill_game_object(gameobject_container, 0.4f);
                }
            }
        }
    }
    public void add_one_button(OCGCORE_BUTTON button)
    {
        OCGCORE_BUTTONs.Add(button);
    }
    public void clear_all_button()
    {
        for (int i = 0; i < OCGCORE_BUTTONs.Count;i++ )
        {
            OCGCORE_BUTTONs[i].kill_oneself();
        }
        OCGCORE_BUTTONs.Clear();
    }
    public void clear_one_button(OCGCORE_BUTTON btn)
    {
       btn.kill_oneself();
       OCGCORE_BUTTONs.Remove(btn);
    }
    public OCGCORE_BUTTON get_button(string cookie_string)
    {
        OCGCORE_BUTTON re = null;
        for (int i = 0; i < OCGCORE_BUTTONs.Count; i++)
        {
            if (cookie_string == OCGCORE_BUTTONs[i].cookie_string)
            {
                re = OCGCORE_BUTTONs[i];
            }
        }
        return re;
    }
}

