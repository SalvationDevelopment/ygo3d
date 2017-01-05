using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OCGCORE_CARD
{
    public point p;
    public int cookie_int = 0;
    public string cookie_string = "";
    public bool cookie_show_in_my_hand = false;
    public bool cookie_be_cared = false;
    public int cookie_select_option_1 = 0;
    public int cookie_select_option_2 = 0;
    public int cookie_select_ptr;
    public bool cookie_chain_forced = false;
    public class chain_effect
    {
        public int ptr;
        public string desc;
    }
    public List<chain_effect> cookie_chain_effects = new List<chain_effect>();
    public OCGCORE_CARD equip_target = null;
    private CardData data;

    public CLIENT_SERVANT_OCGCORE father;
    public loader loader;
    public GameObject game_object;
    private GameObject game_object_face;
    private GameObject game_object_back;
    private GameObject game_object_event_main;
    private GameObject game_object_event_button_bed;
    private GameObject game_object_event_card_bed;
    private TMPro.TextMeshPro text_mesh;
    public OCGCORE_CARD(CLIENT_SERVANT_OCGCORE f, Vector3 position,Vector3 rotation,OCGCORE_CARD_CONDITION condition)
    {
        accurate_position = position;
        accurate_rotation = rotation;
        father = f;
        loader = father.client.loader;
        father.refresh_functions.Add(decoration_handler);
        father.refresh_functions.Add(ES_refresh);
        game_object = father.create_game_object(loader.mod_ocgcore_card, position, Quaternion.identity);
        game_object.transform.eulerAngles = rotation;
        try
        {
            game_object_face = game_object.transform.FindChild("card").FindChild("face").gameObject;
            game_object_back = game_object.transform.FindChild("card").FindChild("back").gameObject;
            game_object_event_main = game_object.transform.FindChild("card").FindChild("event").gameObject;
            text_mesh = game_object.transform.FindChild("card").FindChild("text").GetComponent<TMPro.TextMeshPro>();
            game_object_event_main.AddComponent<card_monoer>().card = this;
        }
        catch (Exception e) { debugger.Log(e); }
        UA_give_condition(condition);
        data = father.card_data_manager.GetById(0);
    }

    public void kill_oneself(){
        father.refresh_functions.Remove(ES_refresh);
        father.refresh_functions.Remove(decoration_handler);
        father.refresh_functions.Remove(card_picture_handler);
        father.refresh_functions.Remove(card_verticle_drawing_handler);
        father.refresh_functions.Remove(monster_cloude_handler);
        father.refresh_functions.Remove(number_handler);
        iTween.ScaleTo(game_object,Vector3.zero,0.6f);
        father.kill_game_object(game_object,0.6f);
        father.kill_game_object(game_object_verticle_drawing);
        father.kill_game_object(game_object_verticle_number);
        father.kill_game_object(game_object_monster_cloude);
        father.kill_game_object(flash_line);
        del_all_decoration();
    }

    //ES_system
    private bool ES_mouse_check()
    {
        bool re = false;
        if (game_object_event_main != null)
        {
            if (father.client.pointed_game_object == game_object_event_main)
            {
                re = true;
            }
        }
        if (game_object_event_button_bed != null)
        {
            if (father.client.pointed_game_object == game_object_event_button_bed)
            {
                re = true;
            }
        }
        if (game_object_event_card_bed != null)
        {
            if (father.client.pointed_game_object == game_object_event_card_bed)
            {
                re = true;
            }
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].game_object_event != null)
            {
                if (father.client.pointed_game_object == buttons[i].game_object_event)
                {
                    re = true;
                }
            }
        }
        if (condition == OCGCORE_CARD_CONDITION.still_unclickable)
        {
            re = false;
        }
        return re;
    }

    public void ES_lock(float time)
    {
        ES_exit_excited(false);
        MonoBehaviour.Destroy(game_object.AddComponent<card_locker>(),time);
    }

    private bool ES_check_locked()
    {
        bool return_value = false;
        if(game_object.transform.GetComponent<card_locker>()!=null){
            return_value = true;
        }
        return return_value;
    }

    private bool ES_excited_unsafe_should_not_be_changed_dont_touch_this = false;

    private void ES_refresh()
    {
        if (father.client.preview_left_mouse_button_is_down == true && father.client.left_mouse_button_is_down == false && ES_mouse_check())
        {
            father.ES_card_selected(this);
        }

        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            //当前在excited态
            if (ES_mouse_check())
            {
                //刷新excited的数据
                ES_excited_handler();
            }
            else
            {
                //退出excited态
                ES_exit_excited(true);
            }
        }
        else
        {
            //当前不在excited态
            if (ES_mouse_check())
            {
                if (ES_check_locked() == false)
                {
                    //进入excited态
                    ES_enter_excited();
                }
                else
                {
                    //无作为
                }
            }
            else
            {
                //无作为
            }
        }
    }

    private void ES_excited_handler()
    {
        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            ES_excited_handler_close_up_handler();
            ES_excited_handler_button_shower();
            ES_excited_handler_event_cookie_card_bed();
            ES_excited_handler_event_cookie_button_bed();
        }
    }

    private void ES_excited_handler_close_up_handler()
    {
        Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(accurate_position);
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 10));
        game_object.transform.position += (worldposition - game_object.transform.position) * 0.4f;
        if (game_object_verticle_drawing != null)
        {
            card_verticle_drawing_handler();
        }
    }

    private void ES_excited_handler_button_shower()
    {
        if (condition == OCGCORE_CARD_CONDITION.verticle_clickable)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float x = -2.8f * buttons.Count / 2 + 1.4f + i * 2.8f;
                Vector3 want = Vector3.zero;
                float showscale = 2f + (float)(verticle_drawing_scale_number - 1000) / 1000f;
                if (showscale > 4) showscale = 4;
                if (showscale < 2) showscale = 2;
                showscale *= 1.8f;
                float length = (showscale * 0.6f + 2.8f * (0 + 1));
                want = game_object_face.transform.position + (new Vector3(x, length / 2f, length / 2f * 1.732f));
                buttons[i].rush_to_vector(want);
            }
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float x = -2.8f * buttons.Count / 2 + 1.4f + i * 2.8f;
                Vector3 want = Vector3.zero;
                float length = (1.1f + 2.8f * (0 + 1));
                want = game_object_face.transform.position + (new Vector3(x, length / 2f, length / 2f * 1.732f));
                buttons[i].rush_to_vector(want);
            }
        }
    }

    private void ES_excited_handler_event_cookie_button_bed()
    {
        if (game_object_event_button_bed != null)
        {
            Vector3 left_button_vector = Vector3.zero;
            Vector3 right_button_vector = Vector3.zero;
            Vector3 middle_button_vector = Vector3.zero;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == 0)
                {
                    left_button_vector = buttons[i].game_object.transform.position;
                }
                if (i == buttons.Count - 1)
                {
                    right_button_vector = buttons[i].game_object.transform.position;
                }
            }
            middle_button_vector = left_button_vector + right_button_vector;
            middle_button_vector /= 2f;
            Vector3 obj_event_vector = Vector3.zero;
            float length = 0;
            obj_event_vector = (middle_button_vector + game_object_face.transform.position) / 2f + new Vector3(0, 0.75f / 2f, 0.75f / 2f * 1.732f);
            length = Vector3.Distance(middle_button_vector, game_object_face.transform.position) + 1.4f;
            game_object_event_button_bed.transform.position = obj_event_vector;
            game_object_event_button_bed.transform.localScale = new Vector3(2.8f * buttons.Count, length, 0.1f);
        }
        else
        {
            game_object_event_button_bed = father.create_game_object(father.client.loader.mod_ocgcore_hidden_button, Vector3.zero, Quaternion.identity);
            game_object_event_button_bed.transform.eulerAngles = new Vector3(60, 0, 0);
        }
    }

    private void ES_excited_handler_event_cookie_card_bed()
    {
        if (condition != OCGCORE_CARD_CONDITION.verticle_clickable)
        {
            if (game_object_event_card_bed == null)
            {
                game_object_event_card_bed 
                    = father.create_game_object(
                    father.client.loader.mod_ocgcore_hidden_button, 
                    game_object.transform.position, 
                    Quaternion.identity);
            }
        }
        else
        {
            if (game_object_event_card_bed != null)
            {
                father.kill_game_object(game_object_event_card_bed);
            }
        }
    }

    private void ES_enter_excited()
    {
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        if (condition == OCGCORE_CARD_CONDITION.floating_clickable)
        {
            flash_line_on();
            iTween.RotateTo(game_object, new Vector3(60, 0, 0), 0.3f);
        }
        ES_excited_unsafe_should_not_be_changed_dont_touch_this = true;
        father.Es_card_become_excited(this);
    }

    public void ES_exit_excited(bool move_to_original_place)
    {
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        flash_line_off();
        ES_excited_unsafe_should_not_be_changed_dont_touch_this = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].hide();
        }
        father.kill_game_object(game_object_event_button_bed);
        father.kill_game_object(game_object_event_card_bed);
        if (move_to_original_place)
        {
            ES_safe_card_move_to_original_place();
        }
        father.Es_card_become_not_excited(this);
    }

    public void ES_safe_card_move_to_original_place()
    {
        ES_safe_card_move(
                        iTween.Hash(
                        "x", accurate_position.x,
                        "y", accurate_position.y,
                        "z", accurate_position.z,
                        "time", 0.4f
                        ),
                        iTween.Hash
                        (
                        "x", accurate_rotation.x,
                        "y", accurate_rotation.y,
                        "z", accurate_rotation.z,
                        "time", 0.4f
                        ));
    }

    private void ES_safe_card_move(Hashtable move_hash, Hashtable rotate_hash)
    {
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) 
        {
            MonoBehaviour.Destroy(iTweens[i]);
        }
        iTween.MoveTo(game_object, move_hash);
        iTween.RotateTo(game_object, rotate_hash);
    }

    //UA_system
    Vector3 gived_position = Vector3.zero;
    Vector3 gived_rotation = Vector3.zero;
    Vector3 accurate_position = Vector3.zero;
    Vector3 accurate_rotation = Vector3.zero;

    public void UA_give_position(Vector3 p)
    {
        gived_position = p;
    }

    public Vector3 UA_get_accurate_position()
    {
        return accurate_position;
    }

    public void UA_give_rotation(Vector3 r)
    {
        gived_rotation = r;
    }

    public void UA_flush_all_gived_witn_lock(float time,bool jump)
    {
        if (Vector3.Distance(gived_position, accurate_position) > 0.1f || Vector3.Distance(gived_rotation, accurate_rotation) > 0.1f)
        {
            ES_lock(time*0.8f);
            //game_object.transform.position = accurate_position;
            game_object.transform.eulerAngles = accurate_rotation;//unity的锅
            accurate_position = gived_position;
            accurate_rotation = gived_rotation;
            refresh_datas();
            if (jump)
            {
                Vector3[] path = new Vector3[30];
                Vector3 from = game_object.transform.position;
                Vector3 to = accurate_position;
                for (int i = 0; i < 30; i++)
                {
                    path[i] = from + (to - from) * (float)i / 29f + (new Vector3(0, 2, 0)) * (float)Math.Sin(3.1415926 * (double)i / 29d);
                }
                ES_safe_card_move(
                           iTween.Hash(
                           "x", accurate_position.x,
                           "y", accurate_position.y,
                           "z", accurate_position.z,
                           "path", path,
                           "time", time
                           ),
                           iTween.Hash
                           (
                           "x", accurate_rotation.x,
                           "y", accurate_rotation.y,
                           "z", accurate_rotation.z,
                           "time", time * 0.8f
                           ));
            }
            else
            {
                ES_safe_card_move(
                          iTween.Hash(
                          "x", accurate_position.x,
                          "y", accurate_position.y,
                          "z", accurate_position.z,
                          "time", time
                          ),
                          iTween.Hash
                          (
                          "x", accurate_rotation.x,
                          "y", accurate_rotation.y,
                          "z", accurate_rotation.z,
                          "time", time * 0.8f
                          ));
            }
            
          
        }
    }

    public void UA_give_condition(OCGCORE_CARD_CONDITION c)
    {
        if (condition != c)
        {
            condition = c;
            if (condition == OCGCORE_CARD_CONDITION.floating_clickable)
            {
                try
                {
                    game_object_event_main.GetComponent<MeshCollider>().enabled = true;
                    game_object.transform.FindChild("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (Exception e)
                {
                    debugger.Log(e);
                }
                father.kill_game_object(game_object_monster_cloude);
                father.kill_game_object(game_object_verticle_drawing);
                father.kill_game_object(game_object_verticle_number);
                father.refresh_functions.Remove(this.card_verticle_drawing_handler);
                father.refresh_functions.Remove(this.monster_cloude_handler);

            }
            if (condition == OCGCORE_CARD_CONDITION.still_unclickable)
            {
                try
                {
                    game_object_event_main.GetComponent<MeshCollider>().enabled = false;
                    game_object.transform.FindChild("card").GetComponent<animation_floating_slow>().enabled = false;
                    father.kill_game_object(game_object_event_button_bed);
                    father.kill_game_object(game_object_event_card_bed);
                }
                catch (Exception e)
                {
                    debugger.Log(e);
                }
                father.kill_game_object(game_object_monster_cloude);
                father.kill_game_object(game_object_verticle_drawing);
                father.kill_game_object(game_object_verticle_number);
                father.refresh_functions.Remove(this.card_verticle_drawing_handler);
                father.refresh_functions.Remove(this.monster_cloude_handler);
                set_text("");
                game_object.transform.FindChild("card").transform.localPosition = Vector3.zero;
            }
            if (condition == OCGCORE_CARD_CONDITION.verticle_clickable)
            {
                try
                {
                    game_object_event_main.GetComponent<MeshCollider>().enabled = true;
                    game_object.transform.FindChild("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (Exception e)
                {
                    debugger.Log(e);
                }
                father.refresh_functions.Add(this.card_verticle_drawing_handler);
                father.refresh_functions.Add(this.monster_cloude_handler);
                set_text("");
            }
            refresh_datas();
        }
    }



    //handler

    void decoration_handler()
    {
        for (int i = 0; i < CARD_DECORATIONs.Count; i++)
        {
            if (CARD_DECORATIONs[i].game_object != null)
            {
                Vector3 screenposition = Vector3.zero;
                if(CARD_DECORATIONs[i].up_of_card){
                    screenposition = father.camera_game_main.WorldToScreenPoint(game_object_face.transform.position+new Vector3(0,1.2f,1.2f*1.732f));
                }
                else
                {
                    screenposition = father.camera_game_main.WorldToScreenPoint(game_object_face.transform.position);
                }
                Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - CARD_DECORATIONs[i].relative_position));
                CARD_DECORATIONs[i].game_object.transform.eulerAngles = CARD_DECORATIONs[i].rotation;
                CARD_DECORATIONs[i].game_object.transform.position = worldposition;
                if (CARD_DECORATIONs[i].scale_change_ignored == false)
                    CARD_DECORATIONs[i].game_object.transform.localScale += (new Vector3(1, 1, 1) - CARD_DECORATIONs[i].game_object.transform.localScale) * 0.3f;
            }
        }
        over_light_handler();
    }

    void card_picture_handler()
    {
        Texture2D texture = father.picture_loader.get(data.code, ocgcore_picture_type.card_picture);
        if (texture != null)
        {
            father.refresh_functions.Remove(this.card_picture_handler);
            try
            {
                game_object_face.GetComponent<Renderer>().material.mainTexture = texture;
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
    }

    void card_verticle_drawing_handler()
    {
        if (game_object_verticle_drawing == null)
        {
            Texture2D texture = father.picture_loader.get(data.code, ocgcore_picture_type.card_verticle_drawing);
            if (texture != null)
            {
                if (game_object_verticle_drawing == null) game_object_verticle_drawing = father.create_game_object(loader.mod_simple_quad, game_object.transform.position, Quaternion.identity);
                game_object_verticle_drawing.transform.eulerAngles = new Vector3(60, 0, 0);
                game_object_verticle_drawing.transform.localScale = Vector3.zero;
                try
                {
                    game_object_verticle_drawing.GetComponent<Renderer>().material.mainTexture = texture;
                }
                catch (Exception e)
                {
                    debugger.Log(e);
                }
            }
        }
        else
        {
            Vector3 want_scale = Vector3.zero;

            float showscale = 2f + (float)(verticle_drawing_scale_number - 1000) / 1000f;
            if (showscale > 4) showscale = 4;
            if (showscale < 2) showscale = 2;
            showscale *= 1.8f;
            want_scale = new Vector3(showscale, showscale, 1);

            game_object_verticle_drawing.transform.position = get_verticle_drawing_vector(game_object_face.transform.position);
            game_object_verticle_drawing.transform.localScale += (want_scale - game_object_verticle_drawing.transform.localScale) * 0.3f;
            if(game_object_verticle_number==null){
                game_object_verticle_number = father.create_game_object(father.client.loader.mod_ocgcore_number, Vector3.zero, Quaternion.identity);
                game_object_verticle_number.GetComponent<number_loader>().set_number((int)data.Level, get_color_num_int());
                game_object_verticle_number.transform.eulerAngles = new Vector3(60,0,0);
            }
            Vector3 screen_number_pos = father.client.camera_game_main.WorldToScreenPoint(game_object_verticle_drawing.transform.position + new Vector3(-showscale * 0.9f / 2, (showscale * 0.9f / 4), showscale * 0.9f / 4 * 1.732f));
            screen_number_pos.z -= 8;
            game_object_verticle_number.transform.position = father.client.camera_game_main.ScreenToWorldPoint(screen_number_pos);

        }
    }

    private int get_color_num_int()
    {
        int re = 0;
        //
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
        {
            re = 0;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_WATER))
        {
            re = 3;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
        {
            re = 5;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_WIND))
        {
            re = 2;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_DARK))
        {
            re = 4;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
        {
            re = 1;
        }
        if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
        {
            re = 1;
        }
        //
        return re;
    }

    void monster_cloude_handler()
    {
        if (game_object_monster_cloude == null)
        {
            try
            {
                game_object_monster_cloude = father.create_game_object(loader.mod_ocgcore_card_cloude, game_object.transform.position, Quaternion.identity);
                game_object_monster_cloude_ParticleSystem = game_object_monster_cloude.GetComponent<ParticleSystem>();
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
        else
        {
            Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(game_object.transform.position);
            game_object_monster_cloude.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z + 3));
            game_object_monster_cloude_ParticleSystem.startSize = UnityEngine.Random.Range(3f, 3f + (20f - 3f) * (float)verticle_drawing_scale_number / 3000f);
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                    new Color(
                        200f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        80f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_WATER))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                   new Color(
                       0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                       0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                       255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                  new Color(
                      255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                      0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                      0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_WIND))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                  new Color(
                      0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                      140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                      0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_DARK))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                   new Color(
                       158f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                       0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                       158f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                    new Color(
                        255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
            if (card_string_helper.differ(data.Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
            {
                game_object_monster_cloude_ParticleSystem.startColor =
                    new Color(
                        255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                        0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
            }
        }
    }

    void number_handler()
    {
        if (obj_number != null)
        {
            Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(game_object_face.transform.position + new Vector3(0, 1f * 2.4f, 1.732f * 2.4f));
            Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 5));
            obj_number.transform.position = worldposition;
        }
    }


    //others

    private void set_back_texture(Texture2D texture)
    {
        try
        {
            game_object_back.GetComponent<Renderer>().material.mainTexture = texture;
        }
        catch (Exception e)
        {
            debugger.Log(e);
        }
    }

    public void set_data(CardData d){
        if (data.code != d.code)
        {
            father.refresh_functions.Add(this.card_picture_handler);
        }
        data = d;
        refresh_datas();
        father.kill_game_object(game_object_verticle_drawing);
    }

    public void safe_data(CardData d)
    {
        if (data.code != d.code)
        {
            father.refresh_functions.Add(this.card_picture_handler);
            data = d;
            refresh_datas();
            father.kill_game_object(game_object_verticle_drawing);
        }
       
    }

    public CardData get_data()
    {
        return data;
    }

    private void refresh_datas()
    {
        if (p.position == game_position.POS_FACEUP_ATTACK || p.position == game_position.POS_FACEDOWN_ATTACK)
        {
            verticle_drawing_scale_number = data.Attack;
        }
        else
        {
            verticle_drawing_scale_number = data.Defense;
        }
        if (game_object_verticle_number != null)
        {
            game_object_verticle_number.GetComponent<number_loader>().set_number((int)data.Level, get_color_num_int());
        }
        if (condition == OCGCORE_CARD_CONDITION.verticle_clickable)
        {
            string raw = "";
            CardData data_raw = father.card_data_manager.GetById(data.code);
            if (data.Attack > data_raw.Attack)
            {
                raw += "<#7fff00>" + data.Attack.ToString() + "</color>";
            }
            if (data.Attack < data_raw.Attack)
            {
                raw += "<#dda0dd>" + data.Attack.ToString() + "</color>";
            }
            if (data.Attack == data_raw.Attack)
            {
                raw += data.Attack.ToString();
            }
            raw += "/";
            if (data.Defense > data_raw.Defense)
            {
                raw += "<#7fff00>" + data.Defense.ToString() + "</color>";
            }
            if (data.Defense < data_raw.Defense)
            {
                raw += "<#dda0dd>" + data.Defense.ToString() + "</color>";
            }
            if (data.Defense == data_raw.Defense)
            {
                raw += data.Defense.ToString();
            }
            //raw += "/";
            //raw += data.Level.ToString();
            set_text(raw);
        }
        else if (condition == OCGCORE_CARD_CONDITION.floating_clickable)
        {
            set_text("");
            if (p.me == true && p.location == game_location.LOCATION_DECK)
            {
                set_text("Deck");
            }
            if (p.me == true && p.location == game_location.LOCATION_EXTRA)
            {
                set_text("Extra");
            }
            if (p.me == true && p.location == game_location.LOCATION_GRAVE)
            {
                set_text("Grave");
            }
            if (p.me == true && p.location == game_location.LOCATION_REMOVED)
            {
                set_text("Removed");
            }
            if (p.me == false && p.location == game_location.LOCATION_DECK)
            {
                set_text("OPdeck");
            }
            if (p.me == false && p.location == game_location.LOCATION_EXTRA)
            {
                set_text("OPextra");
            }
            if (p.me == false && p.location == game_location.LOCATION_GRAVE)
            {
                set_text("OPgrave");
            }
            if (p.me == false && p.location == game_location.LOCATION_REMOVED)
            {
                set_text("OPremoved");
            }
        }
        else
        {
            set_text("");
        }
        if (p.location == game_location.LOCATION_MZONE && p.overlay_index == 0)
        {
            text_mesh.fontSize = 40;
            text_mesh.characterSpacing = 0;
            if (p.me)
            {
                if (p.position == game_position.POS_FACEUP_ATTACK || p.position == game_position.POS_FACEDOWN_ATTACK)
                {
                    text_mesh.gameObject.transform.localPosition = new Vector3(0, -2.5f, 0);
                    text_mesh.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    text_mesh.gameObject.transform.localPosition = new Vector3(-2.5f, 0, 0);
                    text_mesh.gameObject.transform.localEulerAngles = new Vector3(0, 0, -90);
                }
            }
            else
            {
                if (p.position == game_position.POS_FACEUP_ATTACK || p.position == game_position.POS_FACEDOWN_ATTACK)
                {
                    text_mesh.gameObject.transform.localPosition = new Vector3(0, 2.5f, 0);
                    text_mesh.gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    text_mesh.gameObject.transform.localPosition = new Vector3(2.5f, 0, 0);
                    text_mesh.gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }

        }
        else
        {
            text_mesh.gameObject.transform.localPosition = new Vector3(0, -2.5f, 0);
            text_mesh.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            text_mesh.fontSize = 45;
            text_mesh.characterSpacing = 15;
        }
    }

    public OCGCORE_CARD_CONDITION condition = OCGCORE_CARD_CONDITION.floating_clickable;

    public enum OCGCORE_CARD_CONDITION
    {
        floating_clickable=1,
        still_unclickable=2,
        verticle_clickable=3,
    }

    GameObject game_object_verticle_drawing = null;
    GameObject game_object_verticle_number = null;

    GameObject game_object_monster_cloude = null;

    ParticleSystem game_object_monster_cloude_ParticleSystem = null;

    int verticle_drawing_scale_number = 2500;

    Vector3 get_verticle_drawing_vector(Vector3 facevector)
    {
        Vector3 want_position = Vector3.zero;

        float showscale = 2f + (float)(verticle_drawing_scale_number - 1000) / 1000f;
        if (showscale > 4) showscale = 4;
        if (showscale < 2) showscale = 2;
        showscale *= 1.8f;
        want_position = facevector;
        want_position.y += showscale / 2f * 0.5f;
        want_position.z += (showscale / 2f * 1.732f * 0.5f) - (showscale * 1.3f / 3.6f - 0.8f);

        return want_position;
    }

    List<OCGCORE_BUTTON> buttons = new List<OCGCORE_BUTTON>();

    public void add_one_button(OCGCORE_BUTTON b)
    {
        buttons.Add(b);
    }

    public void remove_all_game_button()
    {
        List<OCGCORE_BUTTON> buttons_to_remove = new List<OCGCORE_BUTTON>();
        for (int i = 0; i < buttons.Count;i++)
        {
            if (buttons[i].is_ui_button==false)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }
        }
        for (int i = 0; i < buttons_to_remove.Count; i++)
        {
            buttons.Remove(buttons_to_remove[i]);
        }
        buttons_to_remove.Clear();
    }

    public void remove_all_ui_button()
    {
        List<OCGCORE_BUTTON> buttons_to_remove = new List<OCGCORE_BUTTON>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].is_ui_button == true)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }
        }
        for (int i = 0; i < buttons_to_remove.Count; i++)
        {
            buttons.Remove(buttons_to_remove[i]);
        }
        buttons_to_remove.Clear();
    }

    public class CARD_DECORATION
    {
       public GameObject game_object;
       public float relative_position;
       public Vector3 rotation;
       public string desctiption;
       public bool scale_change_ignored = false;
       public bool cookie = true;
       public bool up_of_card = false;
    }

    List<CARD_DECORATION> CARD_DECORATIONs = new List<CARD_DECORATION>();

    public CARD_DECORATION add_one_decoration(GameObject mod, float relative_position, Vector3 rotation, string desctiption,bool cookie=true,bool up=false)
    {
        CARD_DECORATION c = new CARD_DECORATION();
        c.desctiption = desctiption;
        c.up_of_card = up;
        c.cookie=cookie;
        c.relative_position = relative_position;
        c.rotation = rotation;
        c.game_object = father.create_game_object(mod,game_object_face.transform.position,Quaternion.identity);
        c.game_object.transform.eulerAngles = rotation;
        c.game_object.transform.localScale = Vector3.zero;
        CARD_DECORATIONs.Add(c);
        return c;
    }

    public void fast_decoration(GameObject mod)
    {
        father.kill_game_object(add_one_decoration(mod,-0.5f,Vector3.zero,"").game_object,5);
    }

    public void fast_decoration_prelocation(GameObject mod)
    {
        father.kill_game_object(father.create_game_object(mod,father.get_point_worldposition(this.p)+new Vector3(0,-0.5f,0),Quaternion.identity), 5);
    }

    public void del_all_decoration_by_string(string desctiption)
    {
        List<CARD_DECORATION> to_remove = new List<CARD_DECORATION>();
        for (int i = 0; i < CARD_DECORATIONs.Count; i++)
        {
            if (CARD_DECORATIONs[i].desctiption == desctiption)
            {
                to_remove.Add(CARD_DECORATIONs[i]);
                father.kill_game_object(CARD_DECORATIONs[i].game_object);
            }
        }
        for (int i = 0; i < to_remove.Count; i++)
        {
            CARD_DECORATIONs.Remove(to_remove[i]);
        }
    }

    public void del_all_decoration()
    {
        List<CARD_DECORATION> to_remove = new List<CARD_DECORATION>();
        for (int i = 0; i < CARD_DECORATIONs.Count; i++)
        {
            if (CARD_DECORATIONs[i].game_object != null && CARD_DECORATIONs[i].cookie)
            {
                to_remove.Add(CARD_DECORATIONs[i]);
                iTween.ScaleTo(CARD_DECORATIONs[i].game_object, Vector3.zero, 0.6f);
                father.kill_game_object(CARD_DECORATIONs[i].game_object, 0.6f);
            }
        }
        for (int i = 0; i < to_remove.Count; i++)
        {
            CARD_DECORATIONs.Remove(to_remove[i]);
        }
    }

    List<GameObject> overlay_lights = new List<GameObject>();

    public void add_one_overlay_light()
    {
        GameObject obj = father.create_game_object(father.client.loader.mod_ocgcore_overlay_light, game_object_face.transform.position, Quaternion.identity);
        overlay_lights.Add(obj);
        over_light_handler();
    }

    void over_light_handler()
    {
        for (int i = 0; i < overlay_lights.Count;i++ )
        {
            overlay_lights[i].transform.position = game_object_face.transform.position + new Vector3(0, 1.8f, 0); 
        }
    }

    public void del_one_overlay_light()
    {
        if (overlay_lights.Count>0)
        {
            father.kill_game_object(overlay_lights[0]);
            overlay_lights.RemoveAt(0);
        }
    }

    public void set_overlay_light(int number)
    {
        if (number > overlay_lights.Count)
        {
            for (int i=0 ; i < number - overlay_lights.Count;i++ )
            {
                add_one_overlay_light();
            }
        }
        if (number < overlay_lights.Count)
        {
            for (int i = 0; i < overlay_lights.Count - number; i++)
            {
                del_one_overlay_light();
            }
        }
    }

    public void set_overlay_see_button(bool on)
    {
        OCGCORE_BUTTON re = null;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].cookie_string == "see_overlay")
            {
                re = buttons[i];
            }
        }
        if (on)
        {
            if (re==null)
            {
                OCGCORE_BUTTON button =  new OCGCORE_BUTTON(father,
                    father.client.loader.mod_ocgcore_button_see,
                    father.client.loader.mod_ocgcore_button_wait_green,
                    loader.mod_ocgcore_explode_see);
                button.is_ui_button = true;
                button.cookie_string = "see_overlay";
                button.cookie_card = this;
                button.hint = "超量素材";
                add_one_button(button);
            }
        }
        else
        {
            if (re != null)
            {
                remove_all_ui_button();
            }
        }
    }

    GameObject obj_number = null;

    int number_showing=0;

    public void show_number(int number)
    {
        if (number==0)
        {
            if (obj_number != null)
            {
                iTween.ScaleTo(obj_number,Vector3.zero,0.6f);
                father.kill_game_object(obj_number, 0.6f);
                father.refresh_functions.Remove(number_handler);
            }
        }
        else
        {
            if (obj_number == null)
            {
                obj_number = father.create_game_object(father.client.loader.mod_ocgcore_card_number_shower,Vector3.zero,Quaternion.identity);
                obj_number.transform.GetComponent<TMPro.TextMeshPro>().text = number.ToString();
                father.refresh_functions.Add(number_handler);
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1,1,1), 0.6f);
                iTween.RotateTo(obj_number,new Vector3(60,0,0),0.6f);
                number_handler();
            }
            else if(number_showing!=number)
            {
                iTween.ScaleTo(obj_number, Vector3.zero, 0.6f);
                father.kill_game_object(obj_number, 0.6f);
                obj_number = father.create_game_object(father.client.loader.mod_ocgcore_card_number_shower, Vector3.zero, Quaternion.identity);
                obj_number.transform.GetComponent<TMPro.TextMeshPro>().text = number.ToString();
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1, 1, 1), 0.6f);
                iTween.RotateTo(obj_number, new Vector3(60, 0, 0), 0.6f);
                number_handler();
            }
        }
        number_showing = number;
    }

    public void set_text(string s)
    {
        text_mesh.text = s;
        if(s=="")
        {
            //text_mesh.enabled = false;
        }
        else
        {
            //text_mesh.enabled = true;
        }
    }

    GameObject flash_line = null;

    public void flash_line_on()
    {
        father.camera_game_main.GetComponent<HighlightingEffect>().enabled = true;
        if (flash_line!=null) father.kill_game_object(flash_line);
        flash_line = father.create_game_object(loader.mod_ocgcore_card_figure_line,Vector3.zero,Quaternion.identity);
        flash_line.transform.SetParent(game_object_face.transform,false);
        flash_line.transform.localPosition = Vector3.zero;
        if ((data.Type&(int)game_type.TYPE_MONSTER)>0)
        {
            Color tcl = Color.yellow;
            ColorUtility.TryParseHtmlString("ff8000", out tcl);
            flash_line.GetComponent<FlashingController>().flashingStartColor = tcl;
        }
        if ((data.Type & (int)game_type.TYPE_SPELL) > 0)
        {
            Color tcl = Color.yellow;
            ColorUtility.TryParseHtmlString("7fff00", out tcl);
            flash_line.GetComponent<FlashingController>().flashingStartColor = tcl;
        }
        if ((data.Type & (int)game_type.TYPE_TRAP) > 0)
        {
            Color tcl = Color.yellow;
            ColorUtility.TryParseHtmlString("dda0dd", out tcl);
            flash_line.GetComponent<FlashingController>().flashingStartColor = tcl;
        }
    }

    public void flash_line_off()
    {
        if (flash_line!=null) father.kill_game_object(flash_line);
    }

    GameObject p_line = null;

    public void p_line_on()
    {
        father.camera_game_main.GetComponent<HighlightingEffect>().enabled = true;
        if (p_line != null) father.kill_game_object(p_line);
        p_line = father.create_game_object(loader.mod_ocgcore_card_figure_line, Vector3.zero, Quaternion.identity);
        p_line.transform.SetParent(game_object_face.transform, false);
        p_line.transform.localPosition = Vector3.zero;
        p_line.GetComponent<FlashingController>().flashingStartColor = Color.blue;
        p_line.GetComponent<FlashingController>().flashingEndColor = Color.gray;
        p_line.GetComponent<FlashingController>().flashingFrequency = 0.5f;
    }

    public void p_line_off()
    {
        if (p_line != null) father.kill_game_object(p_line);
    }

    public void animation_explode()
    {
        Vector3 screenposition = father.camera_game_main.WorldToScreenPoint(game_object_face.transform.position);
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z +4));
        GameObject ex = father.create_game_object(father.client.loader.mod_ocgcore_explode_change, worldposition, Quaternion.identity);
        father.sound_player.Play("explode",0.7f);
        ex.AddComponent<animation_screen_lock>().screen_point = father.camera_game_main.WorldToScreenPoint(ex.transform.position);
    }

    GameObject cage = null;

    public void set_disable(bool dis)
    {
        if(dis==true){
            if (cage==null)
            {
                cage = father.create_game_object(loader.mod_ocgcore_decoration_cage,game_object.transform.position,Quaternion.identity);
            }
        }else{
            if (cage != null)
            {
                MonoBehaviour.Destroy(cage.transform.FindChild("Cube").gameObject);
                father.kill_game_object(cage, 5f);
            }
        }
    }

    private class tail
    {
        public string str;
        public int count;
    }

    List<tail> tails = new List<tail>();

    public void add_string_tail(string str)
    {
        bool exist = false;
        for (int i = 0; i < tails.Count;i++ )
        {
            if (tails[i].str == str)
            {
                exist = true;
                tails[i].count++;
            }
        }
        if (exist==false)
        {
            tail t=new tail();
            t.count=1;
            t.str=str;
            tails.Add(t);
        }
        data.tail = "";
        for (int i = 0; i < tails.Count; i++)
        {
            if (tails[i].count == 1)
            {
                data.tail += tails[i].str + "\n";
            }
            else
            {
                data.tail += tails[i].str + "*" + tails[i].count.ToString() + "\n";
            }
        }
    }

    public void clear_all_tail()
    {
        tails.Clear();
        data.tail = "";
    }

    public void del_one_tail(string str)
    {
        tail t = null;
        for (int i = 0; i < tails.Count; i++)
        {
            if (tails[i].str == str)
            {
                t = tails[i];
            }
        }
        if(t!=null){
            if(t.count==1){
                tails.Remove(t);
            }
            else
            {
                t.count--;
            }
        }
        data.tail = "";
        for (int i = 0; i < tails.Count; i++)
        {
            if (tails[i].count == 1)
            {
                data.tail += tails[i].str + "\n";
            }
            else
            {
                data.tail += tails[i].str + "*" + tails[i].count.ToString() + "\n";
            }
        }
    }

    public void animation_confirm_to(Vector3 position,Vector3 rotation,float time_move,float time_still)
    {
        ES_lock(time_move + time_move + time_still);
        confirm_step_time_still = time_still;
        confirm_step_time_move = time_move;
        confirm_step_r = rotation;
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.MoveTo(game_object, iTween.Hash(
                            "x", position.x,
                            "y", position.y,
                            "z", position.z,
                            "onupdate",(Action)decoration_handler,
                            "oncomplete", (Action)confirm_step_2,
                            "time", confirm_step_time_move
                            ));
        iTween.RotateTo(game_object, iTween.Hash(
                            "x", confirm_step_r.x,
                            "y", confirm_step_r.y,
                            "z", confirm_step_r.z,
                            "time", confirm_step_time_move
                            ));
    }

    Vector3 confirm_step_r = Vector3.zero;

    float confirm_step_time_still = 0;

    float confirm_step_time_move = 0;

    void confirm_step_2()
    {
        iTween.RotateTo(game_object, iTween.Hash(
                            "x", confirm_step_r.x,
                            "y", confirm_step_r.y,
                            "z", confirm_step_r.z,
                            "onupdate", (Action)decoration_handler,
                            "oncomplete", (Action)confirm_step_3,
                            "time", confirm_step_time_still
                            ));
    }

    void confirm_step_3()
    {
        iTween.RotateTo(game_object, iTween.Hash(
                            "x", accurate_rotation.x,
                            "y", accurate_rotation.y,
                            "z", accurate_rotation.z,
                            "time", confirm_step_time_move + 0.2f
                            ));
        iTween.MoveTo(game_object, iTween.Hash(
                            "x", accurate_position.x,
                            "y", accurate_position.y,
                            "z", accurate_position.z,
                            "onupdate", (Action)decoration_handler,
                            "oncomplete", (Action)ES_safe_card_move_to_original_place,
                            "time", confirm_step_time_move + 0.2f
                            ));
    }

    public void animation_shake_to(float time)
    {
        ES_lock(time);
        game_object.transform.position = accurate_position;
        game_object.transform.eulerAngles = accurate_rotation;
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.ShakePosition(game_object, iTween.Hash(
                            "x", 1,
                            "y", 1,
                            "z", 1,
                            "time", time,
                            "oncomplete", (Action)ES_safe_card_move_to_original_place
                            ));
    }

    public void animation_rush_to(Vector3 position, Vector3 rotation)
    {
        ES_lock(0.4f);
        iTween[] iTweens = game_object.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.MoveTo(game_object, iTween.Hash(
                            "x", position.x,
                            "y", position.y,
                            "z", position.z,
                            "time", 0.4f
                            ));
        iTween.RotateTo(game_object, iTween.Hash(
                            "x", rotation.x,
                            "y", rotation.y,
                            "z", rotation.z,
                             "onupdate", (Action)decoration_handler,
                            "oncomplete", (Action)ES_safe_card_move_to_original_place,
                            "time", 0.25f
                            ));
    }

    public void animation_show_off(int delay,bool shokewave)
    {
        father.refresh_functions.Add(show_off_handler);
        show_off_delay_time = delay;
        show_off_begin_time = father.client.time;
        show_off_shokewave = shokewave;
    }

    bool show_off_shokewave = false;
    int show_off_begin_time = 0;
    int show_off_delay_time = 0;
    void show_off_handler()
    {
        if (father.client.time - show_off_begin_time > show_off_delay_time)
        {
            Texture2D tex = father.client.picture_loader.get(data.code,ocgcore_picture_type.card_feature);
            if (tex != null)
            {
                father.refresh_functions.Remove(show_off_handler);
                Vector3 screen_point = father.client.camera_game_main.WorldToScreenPoint(accurate_position);
                screen_point.z = 10;
                if ((data.Type & (int)game_type.TYPE_MONSTER) > 0)
                {
                    screen_point.z = 10 - ((float)(data.Attack - 1500)) / 2500f * 6f;
                }
                float width = 6f;
                float height = 6f / (float)tex.width * (float)tex.height;
                Vector3 zuoxia = father.client.camera_game_main.ScreenToWorldPoint(new Vector3(0, 0, screen_point.z - 1));
                Vector3 youshang = father.client.camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screen_point.z - 1));
                zuoxia.x += width / 2f;
                zuoxia.y += height / 4f;
                zuoxia.z += height / 4f * 1.732f;
                youshang.x -= width / 2f;
                youshang.y -= height / 4f;
                youshang.z -= height / 4f * 1.732f;
                Vector3 zuoxia_screen_point = father.client.camera_game_main.WorldToScreenPoint(zuoxia);
                Vector3 youshang_screen_point = father.client.camera_game_main.WorldToScreenPoint(youshang);
                if (screen_point.x < zuoxia_screen_point.x)
                {
                    screen_point.x = zuoxia_screen_point.x;
                }
                if (screen_point.y < zuoxia_screen_point.y)
                {
                    screen_point.y = zuoxia_screen_point.y;
                }
                if (screen_point.x > youshang_screen_point.x)
                {
                    screen_point.x = youshang_screen_point.x;
                }
                if (screen_point.y > youshang_screen_point.y)
                {
                    screen_point.y = youshang_screen_point.y;
                }
                Vector3 world_point = father.client.camera_game_main.ScreenToWorldPoint(screen_point);
                GameObject show_board = father.create_game_object(loader.mod_simple_quad, world_point, Quaternion.identity);
                show_board.AddComponent<animation_screen_lock>().screen_point = screen_point;
                Vector3 scale = new Vector3(width, height, 1);
                show_board.transform.localScale = Vector3.zero;
                iTween.ScaleTo(show_board, scale, 1f);
                iTween.FadeTo(show_board,0, 1f);
                show_board.GetComponent<Renderer>().material.mainTexture = tex;
                show_board.transform.eulerAngles = new Vector3(60,0,0);
                father.kill_game_object(show_board,5f);
                if (show_off_shokewave)
                {
                    screen_point.z -= 0.5f;
                    world_point = father.client.camera_game_main.ScreenToWorldPoint(screen_point);
                    show_board = father.create_game_object(loader.mod_ocgcore_explode_ep, world_point, Quaternion.identity);
                    show_board.AddComponent<animation_screen_lock>().screen_point = screen_point;
                    father.kill_game_object(show_board, 5f);
                }
            }
        }
    }
}

