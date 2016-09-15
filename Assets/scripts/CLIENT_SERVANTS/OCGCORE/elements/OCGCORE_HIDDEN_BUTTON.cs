using System;
using UnityEngine;
public class OCGCORE_HIDDEN_BUTTON
{
    #region ini
    public CLIENT_SERVANT_OCGCORE father;
    public GameObject game_object_event;
    public game_location location;
    public bool me;
    public OCGCORE_HIDDEN_BUTTON(CLIENT_SERVANT_OCGCORE f, game_location l,bool m)
    {
        me = m;
        father = f;
        location = l;
        father.refresh_functions.Add(update);
        game_object_event = father.create_game_object(father.client.loader.mod_ocgcore_hidden_button, Vector3.zero, Quaternion.identity);
        switch (l)
        {
            case game_location.LOCATION_DECK:
                {
                    game_object_event.transform.position = new Vector3(15, 0, -10);
                }
                break;
            case game_location.LOCATION_EXTRA:
                {
                    game_object_event.transform.position = new Vector3(-15, 0, -10);
                }
                break;
            case game_location.LOCATION_GRAVE:
                {
                    game_object_event.transform.position = new Vector3(20, 0, -10);
                }
                break;
            case game_location.LOCATION_REMOVED:
                {
                    game_object_event.transform.position = new Vector3(20, 0, -5);
                }
                break;
        }
        if(me==false){
            game_object_event.transform.position = -game_object_event.transform.position;
        }
    }
    private bool mouse_is_down = false;
    void update()
    {
        if (game_object_event != null)
        {
            if (father.client.pointed_game_object == game_object_event && father.client.preview_pointed_game_object != game_object_event)
            {
                father.mouse_move_in_hidden_button(this);
            }
            if (father.client.pointed_game_object != game_object_event && father.client.preview_pointed_game_object == game_object_event)
            {
                father.mouse_move_out_hidden_button(this);
            }
            if (father.client.preview_left_mouse_button_is_down == false && father.client.left_mouse_button_is_down == true && father.client.pointed_game_object == game_object_event)
            {
                mouse_is_down = true;
            }
            if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false && (father.client.pointed_game_object == game_object_event || mouse_is_down == true))
            {
                mouse_is_down = false;
                if (father.client.pointed_game_object == game_object_event)
                {
                    father.mouse_click_hidden_button(this);
                }
            }
        }
    }
    #endregion
}

