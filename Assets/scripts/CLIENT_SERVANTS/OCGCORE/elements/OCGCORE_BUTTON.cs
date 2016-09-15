using System;
using UnityEngine;
public class OCGCORE_BUTTON
{
    public CLIENT_SERVANT_OCGCORE father;
    public int ptr = -9999;
    public int cookie_int = 0;
    public string cookie_string = "";
    public string hint = "";
    public OCGCORE_CARD cookie_card = null;
    public loader loader;
    private GameObject mod;
    private GameObject wait_mod;
    private GameObject explode_mod;
    public GameObject game_object;
    private GameObject game_object_waiting_effect;
    public GameObject game_object_event;
        public bool is_ui_button = false;
    public OCGCORE_BUTTON(CLIENT_SERVANT_OCGCORE f, GameObject object_mod, GameObject object_wait_mod, GameObject object_explode_mod)
    {
        father = f;
        loader = father.client.loader;
        mod = object_mod;
        wait_mod = object_wait_mod;
        explode_mod = object_explode_mod;
        father.refresh_functions.Add(update);
    }
    public void kill_oneself(){
        father.refresh_functions.Remove(update);
        father.kill_game_object(game_object_waiting_effect);
        father.kill_game_object(game_object);
    }
    public void show(Vector3 position)
    {
        if (game_object == null)
        {
            game_object = father.create_game_object(mod, position, Quaternion.identity);
            try
            {
                game_object_event = game_object.transform.FindChild("event").gameObject;
            }
            catch (Exception e) { debugger.Log(e); }
            game_object.transform.localScale = Vector3.zero;
            iTween.RotateTo(game_object, new Vector3(60, 0, 0), 0.4f);
            iTween.ScaleTo(game_object, new Vector3(1, 1, 1), 0.4f);
        }
    }
    public void hide()
    {
        animation_wait(false);
        if (game_object != null)
        {
            father.kill_game_object(game_object, 0.4f);
            iTween.ScaleTo(game_object, Vector3.zero, 0.4f);
        }
    }
    private Vector3 get_effect_position(Vector3 v)
    {
        Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(v);
        return Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 3));
    }
    private Vector3 get_effect_position_diatance(Vector3 v)
    {
        Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(v);
        return Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z + 4));
    }
    public void move_to_vector(Vector3 v, float time = 0)
    {
        show(v);
        if (time == 0)
        {
            if (game_object_waiting_effect != null)  game_object_waiting_effect.transform.position = get_effect_position(v);
            if (game_object != null) game_object.transform.position = v;
        }
        else
        {
            if (game_object_waiting_effect != null)
            {
                iTween[] iTweens=game_object_waiting_effect.GetComponents<iTween>();
                for (int i = 0; i < iTweens.Length;i++ )MonoBehaviour.Destroy(iTweens[i]);
                iTween.MoveTo(game_object_waiting_effect, get_effect_position(v), time);
            }
            if (game_object != null)
            {
                iTween[] iTweens = game_object.GetComponents<iTween>();
                for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
                iTween.MoveTo(game_object, v, time);
            }
        }
    }
    public void rush_to_vector(Vector3 v)
    {
        show(v);
        move_to_vector(game_object.transform.position + (v - game_object.transform.position) * 0.2f);
    }
    public void very_rush_to_vector(Vector3 v)
    {
        show(v);
        move_to_vector(v);
    }
    bool mouse_is_down = false;
    void update()
    {
        if (game_object_event != null)
        {
            if (father.client.pointed_game_object == game_object_event && father.client.preview_pointed_game_object != game_object_event)
            {
                father.mouse_move_in_button(this);
            }
            if (father.client.pointed_game_object != game_object_event && father.client.preview_pointed_game_object == game_object_event)
            {
                father.mouse_move_out_button(this);
            }
            if (father.client.preview_left_mouse_button_is_down == false && father.client.left_mouse_button_is_down == true && father.client.pointed_game_object == game_object_event)
            {
                mouse_is_down = true;
                father.mouse_down_button(this);
            }
            if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false && (father.client.pointed_game_object == game_object_event || mouse_is_down == true))
            {
                mouse_is_down = false;
                father.mouse_up_button(this);
                if (father.client.pointed_game_object == game_object_event)
                {
                    father.mouse_click_button(this);
                }
            }
        }
    }
    public void animation_wait(bool on)
    {
        if (on)
        {
            if (game_object_waiting_effect==null) game_object_waiting_effect = father.create_game_object(wait_mod, get_effect_position(game_object.transform.position), Quaternion.identity);
        }
        else
        {
            if (game_object_waiting_effect != null) father.kill_game_object(game_object_waiting_effect);
        }
    }
    public void animation_explode()
    {
        GameObject ex = father.create_game_object(explode_mod, get_effect_position_diatance(game_object.transform.position), Quaternion.identity);
        father.sound_player.Play("explode",0.7f);
        ex.AddComponent<animation_screen_lock>().screen_point = father.camera_game_main.WorldToScreenPoint(ex.transform.position);
    }

}
