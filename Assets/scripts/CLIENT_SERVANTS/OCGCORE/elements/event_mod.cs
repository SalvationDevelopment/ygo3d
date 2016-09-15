using System;
using UnityEngine;
public class event_mod
{
    #region ini
    public CLIENT_SERVANT_OCGCORE father;
    public loader loader;
    private GameObject game_object;
    private GameObject game_object_event;
    public event_mod(CLIENT_SERVANT_OCGCORE f, Vector3 position, Vector3 rotation)
    {
        father = f;
        loader = father.client.loader;
        father.refresh_functions.Add(update);
        game_object = father.create_game_object(loader.mod_ocgcore_card, position, Quaternion.identity);
        try
        {
            game_object_event = game_object.transform.FindChild("event").gameObject;
        }
        catch (Exception e) { debugger.Log(e); }
    }
    private bool mouse_is_down=false;
    private int mouse_down_time = 0;
    void update()
    {
        if (game_object_event != null)
        {
            if (father.client.pointed_game_object == game_object_event && father.client.preview_pointed_game_object != game_object_event)
            {
                on_mouse_move_in();
            }
            if (father.client.pointed_game_object != game_object_event && father.client.preview_pointed_game_object == game_object_event)
            {
                on_mouse_move_out();
            }
            if (father.client.preview_left_mouse_button_is_down == false && father.client.left_mouse_button_is_down == true && father.client.pointed_game_object == game_object_event)
            {
                mouse_is_down = true;
                mouse_down_time = Environment.TickCount;
                on_mouse_down();
            }
            if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false && (father.client.pointed_game_object == game_object_event || mouse_is_down == true))
            {
                mouse_is_down = false;
                on_mouse_up();
                if (father.client.pointed_game_object == game_object_event)
                {
                    on_clicked();
                }
            }
        }
    }
    #endregion
    private void on_mouse_move_in()
    {
    }
    private void on_mouse_move_out()
    {
    }
    private void on_mouse_down()
    {
    }
    private void on_mouse_up()
    {
    }
    private void on_clicked()
    {
    }
}

