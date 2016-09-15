using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CLIENT_SERVANT_OCGCORE
{
    #region NET
    public CLIENT client;
    public PictureLoader picture_loader;
    private loader loader;
    private List<HASH_MESSAGE> message_to_be_handled = new List<HASH_MESSAGE>();
    public CLIENT_SERVANT_OCGCORE(CLIENT c)
    {
        client = c;
        loader = client.loader;
        picture_loader = client.picture_loader;
        start_script();
        if (is_debugging == true) debug_script();
    }
    int pretime = 0;
    int last_message_continue_time = 0;
    public delegate void refresh_function();
    public List<refresh_function> refresh_functions = new List<refresh_function>();
    bool pre_s = false;
    public void update()
    {
        if(client.pointed_game_object==null&&client.left_mouse_button_is_down==false&&client.preview_left_mouse_button_is_down==true)
        {
            mouse_up_empty();
        }
        if (pre_s == false && Input.GetKey(KeyCode.Space) == true)
        {
            space_is_down();
        }
        if (pre_s == true && Input.GetKey(KeyCode.Space) == false)
        {
            space_is_up();
        }
        bool_space_is_down = Input.GetKey(KeyCode.Space);
        refresh();
        for (int i = 0; i < refresh_functions.Count; i++)
        {
            try
            {
                refresh_functions[i]();
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
        if (message_to_be_handled.Count > 0)
        {
            int delta_time = client.time - pretime;
            while (true)
            {
                if (message_to_be_handled.Count == 0)
                {
                    break;
                }
                if (get_message_time(message_to_be_handled,0) == 0)
                {
                    try
                    {
                        run_message(message_to_be_handled[0]);
                        debugger.Log(((game_function)message_to_be_handled[0].Fuction).ToString() + "被秒杀，当前时间：" + client.time);
                    }
                    catch(Exception e)
                    {
                        debugger.Log(e);
                    }
                    message_to_be_handled.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            if (message_to_be_handled.Count > 0 && delta_time > last_message_continue_time)
            {
                try
                {
                    run_message(message_to_be_handled[0]);
                    debugger.Log(((game_function)message_to_be_handled[0].Fuction).ToString() + "被延时执行，当前时间：" + client.time);
                }
                catch (Exception e)
                {
                    debugger.Log(e);
                }
                last_message_continue_time = get_message_time(message_to_be_handled,0);
                message_to_be_handled.RemoveAt(0);
                pretime = client.time;
            }
        }
    }
    List<GameObject> all_objects = new List<GameObject>();
    public GameObject create_game_object(GameObject mod, Vector3 position, Quaternion quaternion, GameObject ui = null)
    {
        GameObject ob = (GameObject)MonoBehaviour.Instantiate(mod, position, quaternion);
        all_objects.Add(ob);
        if (ui == null)
        {
            ob.layer = 0;
        }
        else
        {
            ob.transform.SetParent(ui.transform, false);
            ob.layer = ui.layer;
        }
        return ob;
    }
    public void kill_game_object(GameObject game_object, float time = 0, bool force_null = true)
    {
        try
        {
            MonoBehaviour.Destroy(game_object, time);
            all_objects.Remove(game_object);
            if (force_null) game_object = null;
        }
        catch (Exception)
        {
        }
    }
    public void kill_oneself()
    {
        refresh_functions.Clear();
        MonoBehaviour.Destroy(under_ui.GameObject_shift);
        for (int i = 0; i < all_objects.Count; i++)
        {
            try
            {
                MonoBehaviour.Destroy(all_objects[i]);
            }
            catch (Exception)
            {

            }
        }
    }
    #endregion
    #region INI
    public GameObject ui_back_ground_2d = null;
    public Camera camera_back_ground_2d = null;

    public GameObject ui_container_3d = null;
    public Camera camera_container_3d = null;

    public Camera camera_game_main = null;

    public GameObject ui_main_2d = null;
    public Camera camera_main_2d = null;

    public GameObject ui_main_3d = null;
    public Camera camera_main_3d = null;


    public GameObject back_ground_texture = null;
    public OCGCORE_GAMEFIELD game_field = null;
    public OCGCORE_IDLE_CONTAINER idle_container = null;
    public OCGCORE_UNDER_UI under_ui = null;
    public CardDataManager card_data_manager;
    public string_reader string_data_manager;
    public OCGCORE_TEXT hint_text;
    List<OCGCORE_HIDDEN_BUTTON> OCGCORE_HIDDEN_BUTTONs = new List<OCGCORE_HIDDEN_BUTTON>();
    public OCGCORE_SELECT_POSITIONS window_select_position = null;
    public OCGCORE_ANNOUNCE_CARD window_announce_card = null;
    public OCGCORE_ANNOUNCE_TEXT window_announce = null;
    public OCGCORE_SELECT_OPTION window_option = null;
    public OCGCORE_SOUND_PLAYER sound_player = null;
    private void start_script()
    {
        card_data_manager = client.card_data_manager;
        string_data_manager = client.string_data_manager;
        ini_cameras();
        hint_text = new OCGCORE_TEXT(this, true);
        under_ui = new OCGCORE_UNDER_UI(this);
        game_field = new OCGCORE_GAMEFIELD(this);
        idle_container = new OCGCORE_IDLE_CONTAINER(this);
        sound_player = new OCGCORE_SOUND_PLAYER(this);
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_DECK, true));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_EXTRA, true));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_GRAVE, true));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_REMOVED, true));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_DECK, false));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_EXTRA, false));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_GRAVE, false));
        OCGCORE_HIDDEN_BUTTONs.Add(new OCGCORE_HIDDEN_BUTTON(this, game_location.LOCATION_REMOVED, false));
        go_to_min_camera();
        under_ui.get_health_bar(true).set_user_light(false);
        under_ui.get_health_bar(false).set_user_light(true);
    }

    private void ini_cameras()
    {
        client.camera_game_main.transform.position = new Vector3(0, 230, -230);
        client.camera_main_3d.transform.position = new Vector3(0, 230, -230);
        client.camera_container_3d.transform.position = new Vector3(0, 230, -230);

        client.camera_game_main.transform.eulerAngles = new Vector3(60, 0, 0);
        client.camera_main_3d.transform.eulerAngles = new Vector3(60, 0, 0);
        client.camera_container_3d.transform.eulerAngles = new Vector3(60, 0, 0);

        camera_back_ground_2d = client.camera_back_ground_2d;
        camera_container_3d = client.camera_container_3d;
        camera_game_main = client.camera_game_main;
        camera_main_2d = client.camera_main_2d;
        camera_main_3d = client.camera_main_3d;
        ui_back_ground_2d = client.ui_back_ground_2d;
        ui_container_3d = client.ui_container_3d;
        ui_main_2d = client.ui_main_2d;
        ui_main_3d = client.ui_main_3d;
        camera_game_main.rect = new Rect(0, 0, 1, 1);
        camera_main_3d.rect = new Rect(0, 0, 1, 1);
        camera_container_3d.rect = new Rect(0, 0, 1, 1);
    }
    Vector3 camera_position = new Vector3(0, 23f, -17.5f);
    public void refresh()
    {
        camera_position.z += client.mouse_wheel_change_value;
        float min = camera_min;
        if (!under_ui.get_if_show_all())
        {
            min -= 7;
        }
        if (camera_position.z < min)
        {
            camera_position.z = min;
        }
        if (camera_position.z > camera_max)
        {
            camera_position.z = camera_max;
        }
        camera_game_main.gameObject.transform.position += (camera_position - camera_game_main.transform.position) * 0.1f;
        fix_camera();
        if(client.pointed_game_object==null){
            if (client.preview_left_mouse_button_is_down == true && client.left_mouse_button_is_down == false)
            {
                //mouse_click_empty();
            }
        }
    }
    public void fit_screen()
    {
        under_ui.on_shift();
    }
    public void fix_camera()
    {
        camera_game_main.transform.position = camera_game_main.transform.position;
        camera_main_3d.transform.localPosition = camera_game_main.transform.position;
        camera_container_3d.transform.localPosition = camera_game_main.transform.position;
    }
    bool is_debugging = true;
    void debug_script()
    {

    }
    #endregion
    #region EVENTSYSTEM
    public bool bool_space_is_down = false;
    public void space_is_down()
    {
        bool_space_is_down = true;
        OCGCORE_BUTTON btn = idle_container.get_button("can");
        if (btn != null)
        {
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Params.writer.Write((int)btn.ptr);
            send_message(message);
        }
    }
    public void space_is_up()
    {
        bool_space_is_down = false;
    }
    public void get_message(HASH_MESSAGE message)
    {
        message_to_be_handled.Add(message);
    }
    public void send_message(HASH_MESSAGE message)
    {
        clear_all_cookies();
        client.sendGameMessgae(message);
    }
    public void mouse_up_empty()
    {
        OCGCORE_BUTTON btn = idle_container.get_button("hide_all_card");
        if (btn != null)
        {
            some_cards_is_locked_in_hand = false;
            hint_text.set_string("");
            idle_container.set_text(now_hint);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].cookie_show_in_my_hand = false;
            }
            refresh_all_cards();
            go_to_min_camera();
        }
    }
    public void ES_card_selected(OCGCORE_CARD card)
    {
        debugger.Log("ES_card_selected");
        if (card.cookie_string == "for_search")
        {
            card.animation_explode();
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Params.writer.Write((int)card.cookie_int);
            send_message(message);
            return;
        }
        if (card.cookie_string == "idle_response")
        {
            card.animation_explode();
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Params.writer.Write((int)card.cookie_int);
            send_message(message);
            return;
        }
        if (card.cookie_string == "chain_response")
        {
            if (card.cookie_chain_effects.Count > 0)
            {
                if (card.cookie_chain_effects.Count == 1)
                {
                    card.animation_explode();
                    HASH_MESSAGE message = new HASH_MESSAGE();
                    message.Params.writer.Write((int)card.cookie_chain_effects[0].ptr);
                    send_message(message);
                }
                else
                {
                    if (window_option != null)
                    {
                        window_option.kill_oneself();
                        window_option = null;
                    }
                    window_option = new OCGCORE_SELECT_OPTION(this);
                    cookie_select_min = 1;
                    window_option.change_title(card.get_data().Name);
                    for (int i = 0; i < card.cookie_chain_effects.Count; i++)
                    {
                        window_option.add_opt(card.cookie_chain_effects[i].desc, card.cookie_chain_effects[i].ptr);
                    }
                    window_option.add_opt("查看场地", 9198);
                }
            }
            return;
        }
        if (response_type == "select_card")
        {
            if (card.cookie_string == "clickable")
            {

                bool selectable = false;

                for (int i = 0; i < cookie_cards_selectable.Count; i++)
                {
                    if (card == cookie_cards_selectable[i])
                    {
                        selectable = true;
                    }
                }
                if (selectable)
                {
                    bool selected = false;
                    for (int i = 0; i < cookie_cards_selected.Count; i++)
                    {
                        if (card == cookie_cards_selected[i])
                        {
                            selected = true;
                        }
                    }
                    if (selected == true)
                    {
                        card.show_number(0);
                        cookie_cards_selected.Remove(card);
                    }
                    else
                    {
                        cookie_cards_selected.Add(card);
                    }
                }
                cookie_select_card_refresh();
            }
        }
        if (response_type == "sort_card")
        {
            if (card.cookie_string == "sortable")
            {

                bool sorted = false;
                for (int i = 0; i < cookie_cards_sorted.Count; i++)
                {
                    if (card == cookie_cards_sorted[i])
                    {
                        sorted = true;
                    }
                }
                if (sorted == true)
                {
                    card.show_number(0);
                    cookie_cards_sorted.Remove(card);
                }
                else
                {
                    cookie_cards_sorted.Add(card);
                }
                if (cookie_cards_sorted.Count == cookie_select_max)
                {
                    HASH_MESSAGE message = new HASH_MESSAGE();
                    for (int i = 0; i < cookie_cards_sorted.Count; i++)
                    {
                        message.Params.writer.Write((byte)cookie_cards_sorted[i].cookie_int);
                    }
                    send_message(message);
                }
                else
                {
                    for (int i = 0; i < cookie_cards_sorted.Count; i++)
                    {
                        cookie_cards_sorted[i].show_number(i + 1);
                    }
                }
            }
        }


        if (response_type == "select_counter")
        {
            if (card.cookie_string == "counter_holder")
            {
                if (card.cookie_int < card.cookie_select_option_1)
                {
                    card.cookie_int++;
                }
                int sum = 0;
                for (int i = 0; i < cookie_cards_for_select.Count; i++)
                {
                    sum += cookie_cards_for_select[i].cookie_int;
                }
                if (sum == cookie_select_max)
                {
                    HASH_MESSAGE message = new HASH_MESSAGE();
                    for (int i = 0; i < cookie_cards_for_select.Count; i++)
                    {
                        message.Params.writer.Write((byte)cookie_cards_for_select[i].cookie_int);
                    }
                    send_message(message);
                }
                else
                {
                    for (int i = 0; i < cookie_cards_for_select.Count; i++)
                    {
                        cookie_cards_for_select[i].show_number(cookie_cards_for_select[i].cookie_int);
                    }
                }

            }
        }


    }
    public void Es_card_become_excited(OCGCORE_CARD card)
    {
        under_ui.change_data(card.get_data());
        List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(card);
        Vector3 qidian = new Vector3(Input.mousePosition.x + 150f, Input.mousePosition.y, 19f);
        Vector3 zhongdian = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 12f);
        for (int x = 0; x < overlayed_cards.Count; x++)
        {
            if (overlayed_cards[x].cookie_show_in_my_hand == false)
            {
                Vector3 screen_vector_to_move =
                       (new Vector3(0, 50f * (float)Math.Sin(((float)x / (float)overlayed_cards.Count) * 2f * 3.1415926f), 0))
                       + qidian + ((float)x / (float)(overlayed_cards.Count)) * (zhongdian - qidian);
                overlayed_cards[x].flash_line_on();
                iTween.MoveTo(overlayed_cards[x].game_object, Camera.main.ScreenToWorldPoint(screen_vector_to_move), 0.5f);
                iTween.RotateTo(overlayed_cards[x].game_object, new Vector3(60, 0, 0), 0.1f);
            }
        }
    }
    public void Es_card_become_not_excited(OCGCORE_CARD card)
    {
        List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(card);
        for (int x = 0; x < overlayed_cards.Count; x++)
        {
            overlayed_cards[x].ES_safe_card_move_to_original_place();
            overlayed_cards[x].flash_line_off();
        }
    }
    public void mouse_move_in_button(OCGCORE_BUTTON button)
    {
        button.animation_wait(true);
        hint_text.set_string(button.hint);
        if (button.cookie_int == 0)
        {
            hint_text.move_to_world_point(button.game_object.transform.position + new Vector3(0, 1, 1.732f));
            idle_container.set_text(now_hint);
        }
        if (button.cookie_int == 1)
        {
            idle_container.set_text(button.hint);
            hint_text.set_string("");
        }
    }
    public void mouse_move_out_button(OCGCORE_BUTTON button)
    {
        button.animation_wait(false);
        hint_text.set_string("");
        idle_container.set_text(now_hint);
    }
    public void mouse_down_button(OCGCORE_BUTTON button)
    {
        button.animation_wait(false);
        hint_text.set_string("");
        idle_container.set_text(now_hint);
    }
    public void mouse_up_button(OCGCORE_BUTTON button)
    {

    }
    public void mouse_click_button(OCGCORE_BUTTON button)
    {
        button.animation_explode();
        if (button.cookie_string == "clear_counter_select")
        {
            for (int i = 0; i < cookie_cards_for_select.Count; i++)
            {
                cookie_cards_for_select[i].cookie_int = 0;
                cookie_cards_for_select[i].show_number(0);
            }
            refresh_all_cards();
            go_to_min_camera();
            return;
        }
        if (button.cookie_string == "search_again")
        {
            clear_all_cookies();
            refresh_all_cards();
            go_to_min_camera();
            window_announce_card = new OCGCORE_ANNOUNCE_CARD(this);
            return;
        }
        if (button.cookie_string == "see_overlay")
        {
            if(button.cookie_card!=null)
            {
                button.cookie_card.ES_exit_excited(true);
                List<OCGCORE_CARD> cas = get_overlayed_cards(button.cookie_card);
                for (int i = 0; i < cas.Count;i++ )
                {
                    cas[i].cookie_show_in_my_hand = !cas[i].cookie_show_in_my_hand;
                    cas[i].flash_line_off();
                    if (cas[i].cookie_show_in_my_hand)
                    {
                        cas[i].set_text("Overlay");
                    }
                    else
                    {
                        cas[i].set_text("");
                    }
                }
                refresh_all_cards();
                go_to_min_camera();
            }
            return;
        }
        if (button.cookie_string == "hide_all_card")
        {
            some_cards_is_locked_in_hand = false;
            hint_text.set_string("");
            idle_container.set_text(now_hint);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].cookie_show_in_my_hand = false;
            }
            refresh_all_cards();
            go_to_min_camera();
            return;
        }
        if (button.cookie_string == "select_finished")
        {
            send_selected();
            return;
        }
        if (response_type == "idle")
        {
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Params.writer.Write((int)button.ptr);
            send_message(message);
        }
    }
    public void mouse_move_in_hidden_button(OCGCORE_HIDDEN_BUTTON hidden_button)
    {
        int count = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == hidden_button.location)
            {
                if (cards[i].p.me == hidden_button.me)
                {
                    count++;
                }
            }
        }
        int count_show = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == hidden_button.location)
            {
                if (cards[i].p.me == hidden_button.me && cards[i].cookie_show_in_my_hand==false)
                {
                    count_show++;
                }
            }
        }
        hint_text.move_to_world_point(hidden_button.game_object_event.transform.position+new Vector3(0,1,1.732f));
        if (hidden_button.me == true && hidden_button.location == game_location.LOCATION_DECK)
        {
            hint_text.set_string("我方卡组共" + count + "张，点击查看。");
        }
        if (hidden_button.me == true && hidden_button.location == game_location.LOCATION_EXTRA)
        {
            hint_text.set_string("我方额外共" + count + "张，点击查看。");
        }
        if (hidden_button.me == true && hidden_button.location == game_location.LOCATION_GRAVE)
        {
            hint_text.set_string("我方墓地共" + count + "张，点击查看。");
        }
        if (hidden_button.me == true && hidden_button.location == game_location.LOCATION_REMOVED)
        {
            hint_text.set_string("我方除外共" + count + "张，点击查看。");
        }
        if (hidden_button.me == false && hidden_button.location == game_location.LOCATION_DECK)
        {
            hint_text.set_string("对方卡组共" + count + "张，点击查看。");
        }
        if (hidden_button.me == false && hidden_button.location == game_location.LOCATION_EXTRA)
        {
            hint_text.set_string("对方额外共" + count + "张，点击查看。");
        }
        if (hidden_button.me == false && hidden_button.location == game_location.LOCATION_GRAVE)
        {
            hint_text.set_string("对方墓地共" + count + "张，点击查看。");
        }
        if (hidden_button.me == false && hidden_button.location == game_location.LOCATION_REMOVED)
        {
            hint_text.set_string("对方除外共" + count + "张，点击查看。");
        }
        int index = 0;
        Vector3 qidian = new Vector3(Screen.width - Input.mousePosition.x, Input.mousePosition.y, 19f);
        Vector3 zhongdian = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 12f); 
        for (int i = 0; i < cards.Count; i++)
        {
           if(cards[i].p.location==hidden_button.location){
               if (cards[i].p.me == hidden_button.me)
               {
                   if (cards[i].cookie_show_in_my_hand == false && cards[i].cookie_show_in_my_hand == false)
                   {
                       cards[i].flash_line_on();
                       Vector3 screen_vector_to_move = Vector3.zero;
                       if (count_show <= 8)
                       {
                           screen_vector_to_move = 
                               (new Vector3(0, 50f * (float)Math.Sin(((float)index / (float)count) * 2f * 3.1415926f), 0))
                           + zhongdian + ((float)index / (float)(8 - 1)) * (qidian - zhongdian);
                       }
                       else
                       {
                           screen_vector_to_move =
                           (new Vector3(0, 50f * (float)Math.Sin(((float)index / (float)count) * 2f * 3.1415926f), 0))
                           + qidian + ((float)index / (float)(count_show - 1)) * (zhongdian - qidian);
                       }
                       iTween.MoveTo(cards[i].game_object, Camera.main.ScreenToWorldPoint(screen_vector_to_move), 0.5f);
                       iTween.RotateTo(cards[i].game_object, new Vector3(60, 0, 0), 0.1f);
                       index++;
                   }
               }
           }
        }
    }
    public void mouse_move_out_hidden_button(OCGCORE_HIDDEN_BUTTON hidden_button)
    {
        for (int i = 0; i < cards.Count;i++ )
        {
            if (cards[i].p.location == hidden_button.location)
            {
                if (cards[i].p.me == hidden_button.me && cards[i].cookie_show_in_my_hand == false)
                {
                    cards[i].ES_safe_card_move_to_original_place();
                    cards[i].flash_line_off();
                }
            }
        }
        hint_text.set_string("");
    }
    bool some_cards_is_locked_in_hand = false;
    public void mouse_click_hidden_button(OCGCORE_HIDDEN_BUTTON hidden_button)
    {
        some_cards_is_locked_in_hand = true;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == hidden_button.location)
            {
                if (cards[i].p.me == hidden_button.me)
                {
                    cards[i].cookie_show_in_my_hand = true;
                }
            }
        }
        refresh_all_cards();
        go_to_min_camera();
    }
    public void search_button_clicked()
    {
        List<CardData> datas = card_data_manager.search(unclearable_cookie_search_card_type, window_announce_card.input.value);
        if (datas.Count > 0)
        {
            int max = datas.Count;
            if (max > 49)
            {
                max = 49;
            }
            for (int i = 0; i < max; i++)
            {
                point p = new point();
                p.me = true;
                p.location = game_location.LOCATION_UNKNOWN;
                p.overlay_index = 0;
                p.index = i;
                p.position = game_position.POS_FACEUP_ATTACK;
                OCGCORE_CARD card = create_card(p);
                card.set_data(datas[i]);
                card.add_one_decoration(loader.mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                card.cookie_int = (int)card.get_data().code;
                card.cookie_string = "for_search";
            }
            window_announce_card.kill_oneself();
            window_announce_card = null;
            OCGCORE_BUTTON button= create_button(null,-1,game_button.no,"重新搜索");
            button.cookie_string = "search_again";
            refresh_all_cards();
            go_to_min_camera();
        }
    }
    public void ui_button_selected(GameObject ui_button)
    {
        if (ui_button.name == "9198")
        {
            if (window_option != null)
            {
                window_option.kill_oneself();
                window_option = null;
            }
            return;
        }
        if (ui_button.name == "2333")
        {
            if (duel_end != null)
            {
                duel_end();
            }
            return;
        }
        bool exist = false;
        for (int i = 0; i < cookie_selected_ui_button.Count;i++ )
        {
            if (cookie_selected_ui_button[i] == ui_button)
            {
                exist = true;
            }
        }
        if (exist)
        {
            ui_button.transform.FindChild("Label").gameObject.GetComponent<UILabel>().gradientTop = Color.white;
            cookie_selected_ui_button.Remove(ui_button);
        }
        else
        {
            ui_button.transform.FindChild("Label").gameObject.GetComponent<UILabel>().gradientTop = Color.blue;
            cookie_selected_ui_button.Add(ui_button);
        }
        if (cookie_selected_ui_button.Count==cookie_select_min)
        {
            Int32 re = 0;
            for (int i = 0; i < cookie_selected_ui_button.Count; i++)
            { 
                try
                {
                    re = (re | Convert.ToInt32(cookie_selected_ui_button[i].name));
                }
                catch(Exception e)
                {
                    debugger.Log(e);
                }
                
            }
            debugger.Log(re);
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Params.writer.Write((int)re);
            send_message(message);
        }
    }
    #endregion

    public void clear_all_cookies()
    {
        if (window_select_position != null)
        {
            window_select_position.kill_oneself();
            window_select_position = null;
        }
        if (window_announce != null)
        {
            window_announce.kill_oneself();
            window_announce = null;
        }
        if (window_option != null)
        {
            window_option.kill_oneself();
            window_option = null;
        }
        if (window_announce_card != null)
        {
            window_announce_card.kill_oneself();
            window_announce_card = null;
        }
        if (cookie_attack_line != null)
        {
            kill_game_object(cookie_attack_line);
            cookie_attack_line = null;
        }
        if (cookie_attack_eff1 != null)
        {
            kill_game_object(cookie_attack_eff1);
            cookie_attack_eff1 = null;
        }
        if (cookie_attack_eff2 != null)
        {
            kill_game_object(cookie_attack_eff2);
            cookie_attack_eff2 = null;
        }
        select_hint = "";
        cookie_cards_sorted.Clear();
        cookie_selected_ui_button.Clear();
        cookie_cards_for_select.Clear();
        cookie_cards_selectable.Clear();
        cookie_cards_selected.Clear();
        cookie_cards_must_select.Clear();
        cookie_select_min = 0;
        cookie_select_max = 0;
        cookie_select_level = 0;
        game_field.set_string("");
        response_type = "";
        idle_container.clear_all_button();
        List<OCGCORE_CARD> removes = new List<OCGCORE_CARD>();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location==game_location.LOCATION_UNKNOWN)
            {
                removes.Add(cards[i]);
            }
            cards[i].cookie_chain_effects.Clear();
            cards[i].flash_line_off();
            cards[i].cookie_select_ptr = 0;
            cards[i].cookie_select_option_1 = 0;
            cards[i].cookie_select_option_2 = 0;
            cards[i].show_number(0);
            cards[i].del_all_decoration();
            cards[i].cookie_be_cared = false;
            cards[i].cookie_int = 0;
            cards[i].cookie_string = "";
            cards[i].remove_all_game_button();
            //if (cards[i].p.location == game_location.LOCATION_DECK)
            //{
            //    cards[i].set_data(card_data_manager.GetById(0));
           // }
        }
        for (int i = 0; i < removes.Count; i++)
        {
            removes[i].kill_oneself();
            cards.Remove(removes[i]);
        }
        removes.Clear();
        // if (some_cards_is_locked_in_hand == false)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].cookie_show_in_my_hand = false;
            }
            refresh_all_cards();
        }
        under_ui.get_health_bar(true).set_user_light(false);
        under_ui.get_health_bar(false).set_user_light(true);
    }
    private point read_point(BinaryReader reader)
    {
        point a = new point();
        try
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            byte b3 = reader.ReadByte();
            byte b4 = reader.ReadByte();
            a.me = true;
            a.location = game_location.LOCATION_UNKNOWN;
            a.index = b3;
            a.position = game_position.POS_FACEDOWN_ATTACK;
            a.overlay_index = 0;
            if (b1 < 2)
            {
                a.me = if_me(b1);
            }
            if (((int)(b2) & (int)(game_location.LOCATION_DECK)) > 0)
            {
                a.location = game_location.LOCATION_DECK;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_HAND)) > 0)
            {
                a.location = game_location.LOCATION_HAND;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_MZONE)) > 0)
            {
                a.location = game_location.LOCATION_MZONE;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_SZONE)) > 0)
            {
                a.location = game_location.LOCATION_SZONE;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_GRAVE)) > 0)
            {
                a.location = game_location.LOCATION_GRAVE;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_REMOVED)) > 0)
            {
                a.location = game_location.LOCATION_REMOVED;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_EXTRA)) > 0)
            {
                a.location = game_location.LOCATION_EXTRA;
            }
            if (((int)(b2) & (int)(game_location.LOCATION_OVERLAY)) > 0)
            {
                a.position = game_position.POS_FACEUP_ATTACK;
                a.overlay_index = b4+1;
            }
            else
            {
                a.overlay_index = 0;
                bool is_deck = false;
                if (a.location == game_location.LOCATION_DECK)
                {
                    is_deck = true;
                }
                if (a.location == game_location.LOCATION_EXTRA)
                {
                    is_deck = true;
                }
                if (a.location == game_location.LOCATION_GRAVE)
                {
                    is_deck = true;
                }
                if (a.location == game_location.LOCATION_REMOVED)
                {
                    is_deck = true;
                }
                if (((int)(b4) & (int)(game_position.POS_FACEUP_DEFENSE)) > 0)
                {
                    a.position = game_position.POS_FACEUP_DEFENSE;
                    if (is_deck)
                    {
                        a.position = game_position.POS_FACEUP_ATTACK;
                    }
                }
                if (((int)(b4) & (int)(game_position.POS_FACEDOWN_DEFENSE)) > 0)
                {
                    a.position = game_position.POS_FACEDOWN_DEFENSE;
                    if (is_deck)
                    {
                        a.position = game_position.POS_FACEDOWN_ATTACK;
                    }
                }
                if (((int)(b4) & (int)(game_position.POS_FACEUP_ATTACK)) > 0)
                {
                    a.position = game_position.POS_FACEUP_ATTACK;
                }
                if (((int)(b4) & (int)(game_position.POS_FACEDOWN_ATTACK)) > 0)
                {
                    a.position = game_position.POS_FACEDOWN_ATTACK;
                }
            }

        }
        catch (Exception e)
        {
            debugger.Log(e);
        }
        return a;
    }
    private bool if_me(int a)
    {
        if (Player == a)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    List<OCGCORE_CARD> cards = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> cookie_cards_for_select = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> cookie_cards_selected = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> cookie_cards_selectable = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> cookie_cards_must_select = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> cookie_cards_sorted = new List<OCGCORE_CARD>();
    List<GameObject> cookie_selected_ui_button = new List<GameObject>();
    GameObject cookie_attack_line = null;
    GameObject cookie_attack_eff1 = null;
    GameObject cookie_attack_eff2 = null;
    GameObject cookie_p_hole_me = null;
    GameObject cookie_p_hole_op = null;
    GameObject cookie_dark_hole = null;
    string event_string = "";
    long event_code=0;
    UInt32 cookie_select_min = 0;
    UInt32 cookie_select_max = 0;
    UInt32 cookie_select_level = 0;
    UInt32 unclearable_cookie_search_card_type = 0;
    List<OCGCORE_CARD> unclearable_cookie_cards_in_chain = new List<OCGCORE_CARD>();
    List<OCGCORE_CARD> unclearable_cookie_cards_in_select = new List<OCGCORE_CARD>();
    public void cookie_select_card_refresh()
    {
        for (int i = 0; i < cookie_cards_selectable.Count;i++ )
        {
            cookie_cards_selectable[i].del_all_decoration();
        }
        cookie_cards_selectable.Clear();
        if (cookie_select_level == 0 || cookie_select_level >255)
        {
            for (int i = 0; i < cookie_cards_for_select.Count; i++)
            {
                cookie_cards_selectable.Add(cookie_cards_for_select[i]);
            }
            for (int i = 0; i < cookie_cards_selectable.Count; i++)
            {
                cookie_cards_selectable[i].add_one_decoration(loader.mod_ocgcore_decoration_card_selecting,2,Vector3.zero,"card_selecting");
                cookie_cards_selectable[i].cookie_show_in_my_hand = true;
            }
            if (cookie_cards_selected.Count==cookie_select_max)
            {
                for (int i = 0; i < cookie_cards_selected.Count; i++)
                {
                    cookie_cards_selected[i].animation_explode();
                }
                send_selected();
            }
            else
            {
                for (int i = 0; i < cookie_cards_selected.Count; i++)
                {
                    cookie_cards_selected[i].show_number(i + 1);
                }
                if (cookie_cards_selected.Count>=cookie_select_min)
                {
                    OCGCORE_BUTTON btn = idle_container.get_button("select_finished");
                    if (btn == null)
                    {
                        btn = new OCGCORE_BUTTON(this,
                           client.loader.mod_ocgcore_button_ok,
                           client.loader.mod_ocgcore_button_wait_blue,
                           loader.mod_ocgcore_explode_ok);
                        btn.hint = "选择完成";
                        btn.cookie_string = "select_finished";
                        idle_container.add_one_button(btn);
                    }
                }
                else
                {
                    OCGCORE_BUTTON btn = idle_container.get_button("select_finished");
                    if (btn != null)
                    {
                        idle_container.clear_one_button(btn);
                    }
                }
            }

        }
        else
        {
            bool sendable = false;
            int all_star_now = 0;
            List<OCGCORE_CARD> selected = new List<OCGCORE_CARD>();
            for (int x = 0; x < cookie_cards_must_select.Count;x++ )
            {
                cookie_cards_must_select[x].show_number((int)(cookie_cards_must_select[x].get_data().Level));
                selected.Add(cookie_cards_must_select[x]);
            }
            for (int x = 0; x < cookie_cards_selected.Count; x++)
            {
                selected.Add(cookie_cards_selected[x]);
            }
            all_star_now = 0;
            for (int x = 0; x < selected.Count; x++)
            {
                all_star_now += selected[x].cookie_select_option_1;
            }
            if (all_star_now == cookie_select_level)
            {
                sendable = true;
            }
            get_selectable(all_star_now);
            all_star_now = 0;
            for (int x = 0; x < selected.Count; x++)
            {
                all_star_now += selected[x].cookie_select_option_2;
            }
            if (all_star_now == cookie_select_level)
            {
                sendable = true;
            }
            get_selectable(all_star_now);
            if (cookie_cards_selectable.Count == 0 || sendable == true)
            {
                for (int i = 0; i < cookie_cards_selected.Count; i++)
                {
                    cookie_cards_selected[i].animation_explode();
                }
                send_selected();
            }
            else
            {
                for (int i = 0; i < cookie_cards_selectable.Count; i++)
                {
                    cookie_cards_selectable[i].add_one_decoration(loader.mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                    cookie_cards_selectable[i].cookie_show_in_my_hand = true;
                }
                for (int i = 0; i < cookie_cards_selected.Count; i++)
                {
                    cookie_cards_selected[i].show_number((int)cookie_cards_selected[i].get_data().Level);
                }
            }
        }
    }
    void get_selectable(int star)
    {
        List<OCGCORE_CARD> cards_remain_unselected = new List<OCGCORE_CARD>();
        for (int x = 0; x < cookie_cards_for_select.Count; x++)
        {
            cards_remain_unselected.Add(cookie_cards_for_select[x]);
        }
        for (int x = 0; x < cookie_cards_selected.Count; x++)
        {
            cards_remain_unselected.Remove(cookie_cards_selected[x]);
        }
        for (int i = 0; i < cards_remain_unselected.Count; i++)
        {
            debugger.Log(cards_remain_unselected[i].get_data().Name);
            debugger.Log("level=" + cards_remain_unselected[i].get_data().Level);
            debugger.Log("left=" + cards_remain_unselected[i].cookie_select_option_1);
            debugger.Log("right=" + cards_remain_unselected[i].cookie_select_option_2);
            List<OCGCORE_CARD> new_cards = new List<OCGCORE_CARD>();
            for (int x = 0; x < cards_remain_unselected.Count; x++)
            {
                if (x != i)
                {
                    new_cards.Add(cards_remain_unselected[x]);
                }
            }
            bool r = sum_selectable_check(new_cards, (int)cookie_select_level-star - cards_remain_unselected[i].cookie_select_option_1);
            if (!r && cards_remain_unselected[i].cookie_select_option_1 != cards_remain_unselected[i].cookie_select_option_2)
            {
                r = sum_selectable_check(new_cards, (int)cookie_select_level-star - cards_remain_unselected[i].cookie_select_option_2);
            }
            if (r)
            {
                cookie_cards_selectable.Remove(cards_remain_unselected[i]);
                cookie_cards_selectable.Add(cards_remain_unselected[i]);
            }
        }
    }
    bool sum_selectable_check(List<OCGCORE_CARD> cards_temp, int sum)
    {
        if (sum == 0)
        {
            return true;
        }
        if (sum < 0)
        {
            return false;
        }

        for (int i = 0; i < cards_temp.Count; i++)
        {
            List<OCGCORE_CARD> new_cards = new List<OCGCORE_CARD>();
            for (int x = 0; x < cards_temp.Count; x++)
            {
                if (x!=i)
                {
                    new_cards.Add(cards_temp[x]);
                }
            }
            bool r = sum_selectable_check(new_cards, sum - cards_temp[i].cookie_select_option_1);
            if (!r && cards_temp[i].cookie_select_option_1 != cards_temp[i].cookie_select_option_2)
            {
                r = sum_selectable_check(new_cards, sum - cards_temp[i].cookie_select_option_2);
            }
            if (r)
            {
                return r;
            }
        }

        return false;
    }
    void send_selected()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Params.writer.Write((byte)(cookie_cards_must_select.Count + cookie_cards_selected.Count));
        for (int i = 0; i < cookie_cards_must_select.Count;i++ )
        {
            message.Params.writer.Write((byte)i);
        }
        for (int i = 0; i < cookie_cards_selected.Count; i++)
        {
            message.Params.writer.Write((byte)(cookie_cards_selected[i].cookie_select_ptr));
        }
        send_message(message);
    }
    public OCGCORE_CARD create_card(point p)
    {
        OCGCORE_CARD card = new OCGCORE_CARD(this, get_point_worldposition(p), get_point_worldrotation(p), get_point_worldcondition(p));
        card.p = p;
        cards.Add(card);
       return card;
    }
    public Vector3 get_point_worldposition(point p)
    {
        Vector3 return_value = Vector3.zero;
        switch(p.location){
            case game_location.LOCATION_DECK:
                {
                    return_value = new Vector3(15, 0, -10);
                    return_value.y += p.index * 0.03f;
                }
                break;
            case game_location.LOCATION_EXTRA:
                {
                    return_value = new Vector3(-15, 0, -10);
                    return_value.y += p.index * 0.03f;
                }
                break;
            case game_location.LOCATION_MZONE:
                {
                    switch (p.index)
                    {
                        case 0: return_value.x = -10; return_value.z = -5;
                            break;
                        case 1: return_value.x = -5; return_value.z = -5;
                            break;
                        case 2: return_value.x = 0; return_value.z = -5;
                            break;
                        case 3: return_value.x = 5; return_value.z = -5;
                            break;
                        case 4: return_value.x = 10; return_value.z = -5;
                            break;
                    }
                }
                break;
            case game_location.LOCATION_SZONE:
                {
                    switch (p.index)
                    {
                        case 0: return_value.x = -10; return_value.z = -10;
                            break;
                        case 1: return_value.x = -5; return_value.z = -10;
                            break;
                        case 2: return_value.x = 0; return_value.z = -10;
                            break;
                        case 3: return_value.x = 5; return_value.z = -10;
                            break;
                        case 4: return_value.x = 10; return_value.z = -10;
                            break;
                        case 5: return_value.x = -15; return_value.z = 0;
                            break;
                        case 6: return_value.x = -15; return_value.z = -5;
                            break;
                        case 7: return_value.x = 15; return_value.z = -5;
                            break;
                    }
                }
                break;
            case game_location.LOCATION_GRAVE:
                {
                    return_value = new Vector3(20, 0, -10);
                    return_value.y += p.index * 0.03f;
                }
                break;
            case game_location.LOCATION_HAND:
                {
                    int lie = (p.index+1) % 7;
                    if (lie == 0) lie = 7;
                    int hang = (p.index+1 - lie) / 7;
                    return_value.z = -hang * 5 - 18;
                    if (p.me) return_value.z = -hang * 5 - 16.5f;
                    switch (lie)
                    {
                        case 1: return_value.x = 0;
                            break;
                        case 2: return_value.x = 5;
                            break;
                        case 3: return_value.x = -5;
                            break;
                        case 4: return_value.x = 10;
                            break;
                        case 5: return_value.x = -10;
                            break;
                        case 6: return_value.x = 15;
                            break;
                        case 7: return_value.x = -15;
                            break;
                    }
                }
                break;
            case game_location.LOCATION_REMOVED:
                {
                    return_value = new Vector3(20, 0, -5);
                    return_value.y += p.index * 0.03f;
                }
                break;
        }

        if(p.me==false){
            return_value.x = -return_value.x;
            return_value.z = -return_value.z;
        }
        return_value.y -= p.overlay_index * 1f;
        return_value.x += p.overlay_index * 0.6f;
        return return_value;
    }
    private Vector3 get_point_worldrotation(point p) {
        Vector3 return_value =new Vector3(60,0,0);
        if (
            p.location == game_location.LOCATION_MZONE
            ||
            p.location == game_location.LOCATION_SZONE
            || 
            p.location == game_location.LOCATION_DECK
            || 
            p.location == game_location.LOCATION_EXTRA
            ||
            p.location == game_location.LOCATION_GRAVE
            || 
            p.location == game_location.LOCATION_REMOVED
            )
        {
            switch (p.position)
            {
                case game_position.POS_FACEDOWN_ATTACK:
                    return_value = new Vector3(-90, 0, 0);
                    break;
                case game_position.POS_FACEDOWN_DEFENSE:
                    return_value = new Vector3(-90, 0, 90);
                    break;
                case game_position.POS_FACEUP_ATTACK:
                    return_value = new Vector3(90, 0, 0);
                    break;
                case game_position.POS_FACEUP_DEFENSE:
                    return_value = new Vector3(90, 0, 90);
                    break;
            }
        }
        if (p.me == false)
        {
            return_value.z += 179f;
        }
        return return_value;
    }
    private OCGCORE_CARD.OCGCORE_CARD_CONDITION get_point_worldcondition(point p)
    {
        OCGCORE_CARD.OCGCORE_CARD_CONDITION return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable;
        switch (p.location)
        {
            case game_location.LOCATION_DECK:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable;
                }
                break;
            case game_location.LOCATION_EXTRA:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable;
                }
                break;
            case game_location.LOCATION_MZONE:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.verticle_clickable;
                    if(p.position==game_position.POS_FACEDOWN_ATTACK)
                    {
                        return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable;
                    }
                    if (p.position == game_position.POS_FACEDOWN_DEFENSE)
                    {
                        return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable;
                    }
                }
                break;
            case game_location.LOCATION_SZONE:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable;
                }
                break;
            case game_location.LOCATION_GRAVE:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable;
                }
                break;
            case game_location.LOCATION_HAND:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable;
                }
                break;
            case game_location.LOCATION_REMOVED:
                {
                    return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable;
                }
                break;
        }
        if(p.overlay_index>0)
        {
            return_value = OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable;
        }
        return return_value;
    }
    private OCGCORE_CARD get_card(point p,bool create=true)
    {
        OCGCORE_CARD c = null;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == p.location)
            {
                if (cards[i].p.me == p.me)
                {
                    if (cards[i].p.index == p.index)
                    {
                        if (cards[i].p.overlay_index == p.overlay_index)
                        {
                            c = cards[i];
                        }
                    }
                }
            }
        }
        if (create==true)
        {
            if(c==null)
            {
                c = create_card(p);
            }
        }
        //if(c.p.position!=p.position){
       //     c.p.position = p.position;
       //     refresh_all_cards();
       // }
        return c;
    }
    private List<OCGCORE_CARD> get_overlayed_cards(OCGCORE_CARD c)
    {
        List<OCGCORE_CARD> cas = new List<OCGCORE_CARD>();
        if (c.p.overlay_index == 0)
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].p.overlay_index > 0)
                    if (cards[i].p.me == c.p.me)
                        if (cards[i].p.location == c.p.location)
                            if (cards[i].p.index == c.p.index)
                                cas.Add(cards[i]);
            }
        return cas;
    }
    private void move_card(point p1, point p2)
    {
        //from card
        OCGCORE_CARD card_from = get_card(p1,false);
        if (card_from == null)
            card_from = create_card(p1);

        //clear
        card_from.cookie_show_in_my_hand = false;
        card_from.clear_all_tail();
        if(p2.location!=p1.location)
            card_from.set_disable(false);

        //to card
        OCGCORE_CARD card_to = get_card(p2,false);

        ///animation
        if (p2.position == game_position.POS_FACEDOWN_DEFENSE || p2.position == game_position.POS_FACEDOWN_ATTACK)
        {
            if (p2.location == game_location.LOCATION_MZONE || p2.location == game_location.LOCATION_SZONE)
            {
                card_from.fast_decoration(loader.mod_ocgcore_decoration_card_setted);
                if ((card_from.get_data().Type & (int)game_type.TYPE_MONSTER)>0) sound_player.Play("set", 1f);
            }
        }
        if (p2.location == game_location.LOCATION_GRAVE)
        {
            if (p1.location == game_location.LOCATION_MZONE) sound_player.Play("destroyed", 1f);
            card_from.fast_decoration(loader.mod_ocgcore_decoration_tograve);
        }
        if (p2.location == game_location.LOCATION_REMOVED)
        {
            sound_player.Play("destroyed", 1f);
            card_from.fast_decoration(loader.mod_ocgcore_decoration_removed);
        }


        //begin analyse
        if (card_to == null)
        {
            if (p2.overlay_index == 0)
            {
                List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(card_from);
                card_from.p = p2;
                for (int i = 0; i < overlayed_cards.Count; i++)
                {
                    overlayed_cards[i].p.me = p2.me;
                    overlayed_cards[i].p.location = p2.location;
                    overlayed_cards[i].p.index = p2.index;
                   // overlayed_cards[i].p.position = p2.position;
                }
            }
            else
            {
                //List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(card_from);
                card_from.p = p2;
                //for (int i = 0; i < overlayed_cards.Count; i++)
                //{
                 //   overlayed_cards[i].p.me = p2.me;
                 //   overlayed_cards[i].p.location = p2.location;
                 //   overlayed_cards[i].p.index = p2.index;
                 //   overlayed_cards[i].p.position = p2.position;
                 //   overlayed_cards[i].p.overlay_index += p2.overlay_index;
               // }
            }
        }
        else
        {
            if(card_from==card_to)
            {
                card_from.p = p2;
            }
            else if (card_to.p.overlay_index == 0)
            {
                if (p2.location == game_location.LOCATION_MZONE || p2.location == game_location.LOCATION_SZONE)
                {
                    List<OCGCORE_CARD> overlayed_cards_from = get_overlayed_cards(card_from);
                    List<OCGCORE_CARD> overlayed_cards_to = get_overlayed_cards(card_to);
                    card_from.p = p2;
                    card_to.p = p1;
                    for (int i = 0; i < overlayed_cards_from.Count; i++)
                    {
                        overlayed_cards_from[i].p.me = p2.me;
                        overlayed_cards_from[i].p.location = p2.location;
                        overlayed_cards_from[i].p.index = p2.index;
                        //overlayed_cards_from[i].p.position = p2.position;
                    }
                    for (int i = 0; i < overlayed_cards_to.Count; i++)
                    {
                        overlayed_cards_to[i].p.me = p1.me;
                        overlayed_cards_to[i].p.location = p1.location;
                        overlayed_cards_to[i].p.index = p1.index;
                       // overlayed_cards_to[i].p.position = p1.position;
                    }
                }
                else
                {
                    card_from.p = p2;
                    card_from.p.index = 999;
                }
                
            }
            else
            {
                card_from.p = p2;
                card_from.cookie_int = -1;
                point xyz_monster_point = new point(p2.me,p2.location,p2.index);
                OCGCORE_CARD xyz_monster = get_card(xyz_monster_point,false);
                if (xyz_monster!=null)
                {
                    List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(xyz_monster);
                    for (int i = 0; i < overlayed_cards.Count; i++)
                    {
                        overlayed_cards[i].p = p2;
                        overlayed_cards[i].cookie_int = -2 - i;
                    }
                }
                
            }
        }


        //sort 
        cards.Sort((left, right) =>
        {
            int a = 1;
            if (left.p.me == true && right.p.me == false)
            {
                a = -1;
            }
            else if (left.p.me == false && right.p.me == true)
            {
                a = 1;
            }
            else
            {
                if (left.p.location == game_location.LOCATION_HAND && right.p.location != game_location.LOCATION_HAND)
                {
                    a = -1;
                }
                else if (left.p.location != game_location.LOCATION_HAND && right.p.location == game_location.LOCATION_HAND)
                {
                    a = 1;
                }
                else
                {
                    if ((int)left.p.location > (int)right.p.location)
                    {
                        a = 1;
                    }
                    else if ((int)left.p.location < (int)right.p.location)
                    {
                        a = -1;
                    }
                    else
                    {
                        if ((int)left.p.index > (int)right.p.index)
                        {
                            a = 1;
                        }
                        else if ((int)left.p.index < (int)right.p.index)
                        {
                            a = -1;
                        }
                        else
                        {
                            if ((int)left.p.overlay_index > (int)right.p.overlay_index)
                            {
                                a = 1;
                            }
                            else if ((int)left.p.overlay_index < (int)right.p.overlay_index)
                            {
                                a = -1;
                            }
                            else
                            {
                                if ((int)left.cookie_int > (int)right.cookie_int)
                                {
                                    a = 1;
                                }
                                else if ((int)left.cookie_int < (int)right.cookie_int)
                                {
                                    a = -1;
                                }
                            }
                        }
                    }
                }
            }
            return a;
        });

        //clear cookie
        List<OCGCORE_CARD> to_clear = new List<OCGCORE_CARD>();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].cookie_int = 0;
            if (cards[i].p.location == game_location.LOCATION_UNKNOWN)
            {
                to_clear.Add(cards[i]);
            }
        }
        for (int i = 0; i < to_clear.Count; i++)
        {
            to_clear[i].kill_oneself();
            cards.Remove(to_clear[i]);
        }
        /////rebuild
        bool pre_me = false;
        game_location pre_location = game_location.LOCATION_UNKNOWN;
        int pre_index = 0;
        ///////////////////////////////////////
        int index = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (
                pre_me != cards[i].p.me ||
                pre_location != cards[i].p.location
                )
            {
                index = 0;
            }
            ////////////////////////
            if (cards[i].p.location != game_location.LOCATION_MZONE)
                if (cards[i].p.location != game_location.LOCATION_SZONE)
                {
                    cards[i].p.index = index;
                }
            if (i + 1 < cards.Count && cards[i + 1].p.overlay_index == 0) index++;
            ////////////////////////
            pre_me = cards[i].p.me;
            pre_location = cards[i].p.location;
            pre_index = cards[i].p.index;
        }
        ///////////////////////////////////////
        ///////////////////////////////////////
        int overlay_index = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (
                pre_me != cards[i].p.me ||
                pre_index != cards[i].p.index ||
                pre_location != cards[i].p.location
                )
            {
                overlay_index = 0;
            }
            ////////////////////////
            cards[i].p.overlay_index = overlay_index;
            overlay_index++;
            ////////////////////////
            pre_me = cards[i].p.me;
            pre_location = cards[i].p.location;
            pre_index = cards[i].p.index;
        }
        //////
    }
    float camera_max = -17.5f;
    float camera_min = -17.5f;
    public void go_to_min_camera()
    {
        camera_position.z = camera_min;
        camera_position.x = 0;
        camera_position.y = 23;
        if (!under_ui.get_if_show_all() && its_my_turn)
        {
            camera_position.z -= 7;
        }
    }
    List<GameObject> thunders = new List<GameObject>();
    public void refresh_all_cards()
    {
        //notice! you must use sort fuction(private void move_card(point p1, point p2)) to make sure card order before use the function

        bool some_things_handed = false;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.overlay_index>0)
            {
                cards[i].p.position = game_position.POS_FACEUP_ATTACK;
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].cookie_be_cared = false;
            if (cards[i].cookie_show_in_my_hand == true)
            {
                if(cards[i].p.overlay_index==0)
                {
                    if (cards[i].p.location == game_location.LOCATION_MZONE)
                    {
                        cards[i].cookie_show_in_my_hand = false;
                    }
                    if (cards[i].p.location == game_location.LOCATION_SZONE)
                    {
                        cards[i].cookie_show_in_my_hand = false;
                    }
                }
                if (cards[i].p.location == game_location.LOCATION_HAND && cards[i].p.me == false)
                {
                    cards[i].cookie_show_in_my_hand = false;
                }
                if (cards[i].p.location != game_location.LOCATION_HAND || cards[i].p.me != true)
                {
                    some_things_handed = true;
                }
            }
            if (cards[i].cookie_show_in_my_hand == false)
            {
                if (cards[i].p.location == game_location.LOCATION_HAND && cards[i].p.me == true)
                {
                    cards[i].cookie_show_in_my_hand = true;
                }
                if (cards[i].p.location==game_location.LOCATION_UNKNOWN)
                {
                    cards[i].cookie_show_in_my_hand = true;
                }
            }
        }

        List<List<OCGCORE_CARD>> lines = new List<List<OCGCORE_CARD>>();
        lines.Add(new List<OCGCORE_CARD>());
        bool pre_me = false;
        game_location pre_location = game_location.LOCATION_UNKNOWN;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cookie_show_in_my_hand == true)
            {
                if (
                pre_me != cards[i].p.me
                ||
                pre_location != cards[i].p.location
                ||
                lines[lines.Count - 1].Count == 7
                    )
                {
                    if (lines[lines.Count - 1].Count != 0)
                        lines.Add(new List<OCGCORE_CARD>());
                }
                lines[lines.Count - 1].Add(cards[i]);
                pre_me = cards[i].p.me;
                pre_location = cards[i].p.location;
            }
        }


        for (int line_index = 0; line_index < lines.Count; line_index++)
        {
            for (int index = 0; index < lines[line_index].Count; index++)
            {
                Vector3 want_position = Vector3.zero;
                want_position.y = 0;
                want_position.z = -line_index * 5 - 16.5f;
                want_position.x = -lines[line_index].Count * 2.5f + 2.5f + index * 5f;
                lines[line_index][index].cookie_be_cared = true;
                lines[line_index][index].UA_give_condition(OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable);
                lines[line_index][index].UA_give_position(want_position);
                lines[line_index][index].UA_give_rotation(new Vector3(60, 0, 0));
                lines[line_index][index].UA_flush_all_gived_witn_lock(0.8f,false);
            }
        }

        List<OCGCORE_CARD> line = new List<OCGCORE_CARD>();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == game_location.LOCATION_HAND && cards[i].p.me == false)
            {
                line.Add(cards[i]);
            }
        }
        for (int index = 0; index < line.Count; index++)
        {
            Vector3 want_position = Vector3.zero;
            want_position.y = 0;
            want_position.z = 18f;
            if (line.Count == 1)
            {
                want_position.x = 0;
            }
            else if (line.Count < 5)
            {
                want_position.x = -2f * line.Count + 4f * line.Count * ((float)index / (float)(line.Count - 1));
            }
            else
            {
                want_position.x = -10f + 20f * ((float)index / (float)(line.Count - 1));
            }
            line[index].cookie_be_cared = true;
            line[index].UA_give_position(want_position);
            if (line[index].p.position == game_position.POS_FACEUP_ATTACK || line[index].p.position == game_position.POS_FACEUP_DEFENSE)
            {
                line[index].UA_give_rotation(new Vector3(60, 0, 0));
            }
            else
            {
                line[index].UA_give_rotation(new Vector3(240, 0, 0));
            }
            line[index].UA_give_condition(OCGCORE_CARD.OCGCORE_CARD_CONDITION.floating_clickable);
            line[index].UA_flush_all_gived_witn_lock(1,false);
        }
        //p effect
        List<OCGCORE_CARD> my_p_cards = new List<OCGCORE_CARD>();
        List<OCGCORE_CARD> op_p_cards = new List<OCGCORE_CARD>();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == game_location.LOCATION_SZONE)
            {
                if (cards[i].p.index == 6 || cards[i].p.index == 7)
                {
                    if(cards[i].p.me)
                    {
                        my_p_cards.Add(cards[i]);
                    }
                    else
                    {
                        op_p_cards.Add(cards[i]);
                    }
                }
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].p_line_off();
        }
        if (my_p_cards.Count == 2)
        {
            game_field.me_left_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[0].get_data().LeftScale,3);
            game_field.me_right_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[1].get_data().LeftScale, 3);
            if (cookie_p_hole_me == null)
            {
                cookie_p_hole_me = create_game_object(loader.mod_ocgcore_ss_p_idle_effect,new Vector3(0,0,-5),Quaternion.identity);
            }
            my_p_cards[0].cookie_be_cared = true;
            my_p_cards[0].UA_give_position(new Vector3(-15,5,-6));
            my_p_cards[0].UA_give_rotation(new Vector3(0, -45, 0));
            my_p_cards[0].UA_flush_all_gived_witn_lock(1, false);
            my_p_cards[1].cookie_be_cared = true;
            my_p_cards[1].UA_give_position(new Vector3(15, 5, -6));
            my_p_cards[1].UA_give_rotation(new Vector3(0, 45, 0));
            my_p_cards[1].UA_flush_all_gived_witn_lock(1, false);
            my_p_cards[0].p_line_on();
            my_p_cards[1].p_line_on();
        }
        else
        {
            game_field.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
            game_field.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            if (cookie_p_hole_me!=null)
            {
                kill_game_object(cookie_p_hole_me);
            }
        }
        if (op_p_cards.Count == 2)
        {
            game_field.op_left_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[0].get_data().LeftScale, 3);
            game_field.op_right_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[1].get_data().LeftScale, 3);
            if (cookie_p_hole_op == null)
            {
                cookie_p_hole_op = create_game_object(loader.mod_ocgcore_ss_p_idle_effect, new Vector3(0, 0, 5), Quaternion.identity);
            }
            op_p_cards[0].cookie_be_cared = true;
            op_p_cards[0].UA_give_position(new Vector3(-15, 5, 4));
            op_p_cards[0].UA_give_rotation(new Vector3(0, -45, 0));
            op_p_cards[0].UA_flush_all_gived_witn_lock(1, false);
            op_p_cards[1].cookie_be_cared = true;
            op_p_cards[1].UA_give_position(new Vector3(15, 5, 4));
            op_p_cards[1].UA_give_rotation(new Vector3(0, 45, 0));
            op_p_cards[1].UA_flush_all_gived_witn_lock(1, false);
            op_p_cards[0].p_line_on();
            op_p_cards[1].p_line_on();
        }
        else
        {
            game_field.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
            game_field.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            if (cookie_p_hole_op != null)
            {
                kill_game_object(cookie_p_hole_op);
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location==game_location.died)
            {
                cards[i].cookie_be_cared = true;
                cards[i].UA_give_condition(OCGCORE_CARD.OCGCORE_CARD_CONDITION.still_unclickable);
                if (cards[i].p.me)
                {
                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, -25f)));

                }
                else
                {
                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(5f, 18f)));

                }
                cards[i].UA_give_rotation(new Vector3(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f)));
                cards[i].UA_flush_all_gived_witn_lock(0.6f,false);
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cookie_be_cared == false)
            {
                cards[i].UA_give_condition(get_point_worldcondition(cards[i].p));
                cards[i].UA_give_position(get_point_worldposition(cards[i].p));
                cards[i].UA_give_rotation(get_point_worldrotation(cards[i].p));
                cards[i].UA_flush_all_gived_witn_lock(1,true);
            }
        }
        //thunders clear
        for (int i = 0; i < thunders.Count; i++)
        {
            kill_game_object(thunders[i]);
        }
        thunders.Clear();

        //overlay_light   overlay_button

        for (int i = 0; i < cards.Count; i++)
        {
            List<OCGCORE_CARD> overlayed_cards = get_overlayed_cards(cards[i]);
            cards[i].set_overlay_light(overlayed_cards.Count);
            cards[i].set_overlay_see_button(overlayed_cards.Count > 0);
            for (int x = 0; x < overlayed_cards.Count; x++)
            {
                if (overlayed_cards[x].cookie_show_in_my_hand)
                {
                    GameObject thunder = create_game_object(loader.mod_ocgcore_decoration_thunder, Vector3.zero, Quaternion.identity);
                    thunder.GetComponent<thunder_locator>().set_objects(overlayed_cards[x].game_object, cards[i].game_object);
                    thunders.Add(thunder);
                }
            }
            if (cards[i].equip_target != null)
            {
                GameObject thunder = create_game_object(loader.mod_ocgcore_decoration_thunder, Vector3.zero, Quaternion.identity);
                thunder.GetComponent<thunder_locator>().set_objects(cards[i].equip_target.game_object, cards[i].game_object);
                thunders.Add(thunder);
            }
        }
        

        

        //camera
        float nearest_z = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (nearest_z > cards[i].UA_get_accurate_position().z)
            {
                nearest_z=cards[i].UA_get_accurate_position().z;
            }
        }
       
        idle_container.position = new Vector3(0, 0, nearest_z-6);
        camera_min = nearest_z -1;
        camera_max = -17.5f;
        if(some_things_handed){
            OCGCORE_BUTTON btn = idle_container.get_button("hide_all_card");
            if (btn==null)
            {
                btn = new OCGCORE_BUTTON(this,
                   client.loader.mod_ocgcore_button_see,
                   client.loader.mod_ocgcore_button_wait_black,
                   loader.mod_ocgcore_explode_see);
                btn.hint = "隐藏卡片";
                btn.cookie_int = 1;
                btn.cookie_string = "hide_all_card";
                idle_container.add_one_button(btn);
            }
        }
        else
        {
            OCGCORE_BUTTON btn = idle_container.get_button("hide_all_card");
            if (btn != null)
            {
                idle_container.clear_one_button(btn);
            }
        }
    }
    private string get_description(UInt32 description)
    {
        string a = "";
        if (description < 10000)
        {
            a = string_data_manager.get("system", (int)description);
            
        }
        else
        {
            a = card_data_manager.get_description(description);
        }
        return a;
    }
    private OCGCORE_BUTTON create_button(OCGCORE_CARD card, int button_ptr, game_button type, string hint,bool ui=false)
    {
        OCGCORE_BUTTON button;
        switch (type)
        {
            case game_button.active:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_act,
                    client.loader.mod_ocgcore_button_wait_green,
                    loader.mod_ocgcore_explode_act);
                break;
            case game_button.main_phrase:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_mp,
                    client.loader.mod_ocgcore_button_wait_black,
                    loader.mod_ocgcore_explode_mp);
                break;
            case game_button.attack:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_atk,
                    client.loader.mod_ocgcore_button_wait_red,
                    loader.mod_ocgcore_explode_atk);
                break;
            case game_button.battle_phrase:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_bp,
                    client.loader.mod_ocgcore_button_wait_red,
                    loader.mod_ocgcore_explode_bp);
                break;
            case game_button.change:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_change,
                    client.loader.mod_ocgcore_button_wait_black,
                    loader.mod_ocgcore_explode_change);
                break;
            case game_button.end_phrase:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_ep,
                    client.loader.mod_ocgcore_button_wait_blue,
                    loader.mod_ocgcore_explode_ep);
                break;
            case game_button.no:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_no,
                    client.loader.mod_ocgcore_button_wait_black,
                    loader.mod_ocgcore_explode_no);
                break;
            case game_button.set:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_set,
                    client.loader.mod_ocgcore_button_wait_blue,
                    loader.mod_ocgcore_explode_set);
                break;
            case game_button.spsummon:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_spsum,
                    client.loader.mod_ocgcore_button_wait_blue,
                    loader.mod_ocgcore_explode_spsum);
                break;
            case game_button.summon:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_sum,
                    client.loader.mod_ocgcore_button_wait_red,
                    loader.mod_ocgcore_explode_sum);
                break;
            case game_button.yes:
                button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_ok,
                    client.loader.mod_ocgcore_button_wait_green,
                    loader.mod_ocgcore_explode_ok);
                break;
            default:
                 button = new OCGCORE_BUTTON(this,
                    client.loader.mod_ocgcore_button_ok,
                    client.loader.mod_ocgcore_button_wait_green,
                    loader.mod_ocgcore_explode_ok);
                break;
        }
        button.hint = hint;
        button.cookie_card = card;
        button.ptr = button_ptr;
        button.is_ui_button = ui;
        if(card!=null){
            card.add_one_button(button);
            button.cookie_int = 0;
        }
        else
        {
            idle_container.add_one_button(button);
            button.cookie_int = 1;
        }
        return button;
    }
    int Player = 0;
    int Operator = 0;
    public bool its_my_turn = false;
    public string now_hint = "";
    void set_now(string hint)
    {
        now_hint = hint;
        idle_container.set_text(now_hint);
    }
    private int get_message_time(List<HASH_MESSAGE> bundle, int i)
    {
        HASH_MESSAGE message=bundle[i];
        int return_value = 0;
        switch ((game_function)message.Fuction)
        {
            case game_function.MSG_MOVE:
                {
                    return_value = 600;
                    if(i + 1<bundle.Count){
                        if (bundle[i + 1].Fuction == (int)(game_function.MSG_MOVE))
                        {
                            return_value = 100;
                            debugger.Log("移动一级加速");
                        }
                    }
                    if (i + 2 < bundle.Count)
                    {
                        if (bundle[i + 2].Fuction == (int)(game_function.MSG_MOVE))
                        {
                            return_value = 100;
                            debugger.Log("移动二级加速");
                        }
                    }
                    if (i + 3 < bundle.Count)
                    {
                        if (bundle[i + 3].Fuction == (int)(game_function.MSG_MOVE))
                        {
                            return_value = 100;
                            debugger.Log("移动三级加速");
                        }
                    }
                }
                break;
            case game_function.MSG_CHAINING: return_value = 400; break;
            case game_function.MSG_CARD_SELECTED: return_value = 800; break;
            case game_function.MSG_CHAIN_DISABLED: return_value = 400; break;
            case game_function.MSG_CHAIN_SOLVED: return_value = 200; break;
            case game_function.MSG_TOSS_COIN: return_value = 4000; break;
            case game_function.MSG_TOSS_DICE: return_value = 4000; break;
            case game_function.MSG_SHUFFLE_HAND: return_value = 500; break;
            case game_function.MSG_SHUFFLE_DECK: return_value = 500; break;
            case game_function.MSG_LPUPDATE: return_value = 800; break;
            case game_function.MSG_DAMAGE: return_value = 1500; break;
            case game_function.MSG_CHINT_TURN: return_value = 500; break;
            case game_function.MSG_NEW_TURN: return_value = 800; break;
            case game_function.MSG_CONFIRM_CARDS: return_value = 800; break;
            case game_function.MSG_QUERY_IS_DISABLED: return_value = 800; break;
            case game_function.MSG_NEW_PHASE:
                {
                    message.Params.reader.BaseStream.Seek(0, 0);
                    int phrase = message.Params.reader.ReadUInt16();
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_BATTLE))
                    {
                        return_value = 800;
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_DRAW))
                    {
                        return_value = 800;
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_END))
                    {
                        return_value = 800;
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_MAIN1))
                    {
                        return_value = 800;
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_MAIN2))
                    {
                        return_value = 800;
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_STANDBY))
                    {
                        return_value = 800;
                    }
                }
                break;
        }
        message.Params.reader.BaseStream.Seek(0,0);
        //return 0;
        return return_value;
    }
    string response_type = "";
    string select_hint = "";
    private void run_message(HASH_MESSAGE message)
    {
        BinaryReader r = message.Params.reader;
        r.BaseStream.Seek(0, 0);
        BYTE_HELPER rr = message.Params;
        int count = 0;
    
        switch ((game_function)message.Fuction)
        {
            case game_function.MSG_CUSTOM_MSG:
                {
                    Player = r.ReadByte();
                    Operator = r.ReadByte();
                }
                break;
            case game_function.MSG_WIN:
                {
                    int winner = r.ReadByte();
                    bool loser_me=!if_me(winner);
                    for (int i = 0; i < cards.Count;i++ )
                    {
                        if (cards[i].p.me == loser_me)
                        {
                            cards[i].p.location = game_location.died;
                        }
                    }
                    Vector3 screenposition = new Vector3(Screen.width/2,Screen.height/2,3f);
                    Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z));
                    GameObject ex = create_game_object(client.loader.mod_ocgcore_explode_atk, worldposition, Quaternion.identity);
                    sound_player.Play("explode", 0.7f);
                    ex.AddComponent<animation_screen_lock>().screen_point = camera_game_main.WorldToScreenPoint(ex.transform.position);
                    refresh_all_cards();
                    go_to_min_camera();

                    window_option = new OCGCORE_SELECT_OPTION(this);
                    cookie_select_min = 1;
                    if (loser_me)
                    {
                        window_option.change_title("惨遭败北！");
                        window_option.add_opt("我一定会回来的！", 2333);
                        window_option.add_opt("人家才不是故意输给你的！", 2333);
                        window_option.add_opt("敢不敢不用主流！", 2333);
                        window_option.add_opt("手动滑稽！", 2333);
                    }
                    else
                    {
                        window_option.change_title("获得胜利！");
                        window_option.add_opt("打得不错！", 2333);
                        window_option.add_opt("真是场有意思的决斗！", 2333);
                        window_option.add_opt("你还是缺少一些人生经验！", 2333);
                        window_option.add_opt("手动滑稽！", 2333);
                    }
                  
                }
                break;
            case game_function.MSG_HINT_EVENT:
                {
                    UInt32 desc = r.ReadUInt32();
                    UInt32 code = r.ReadUInt32();
                    if (code==0)
                    {
                        event_string = get_description(desc);
                    }
                    else
                    {
                        if (desc == (UInt32)game_function.MSG_POS_CHANGE)
                        {
                            event_code = code;
                            event_string =client.card_data_manager.GetById(event_code).Name+
                                " 表示形式被改变";
                            
                        }
                        if (desc == (UInt32)game_function.MSG_SET)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被放置";
                        }
                        if (desc == (UInt32)game_function.MSG_SWAP)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被交换";
                        }
                        if (desc == (UInt32)game_function.MSG_SUMMONING)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 召唤宣言";
                            sound_player.Play("summon", 1f);
                        }
                        if (desc == (UInt32)game_function.MSG_SUMMONED)
                        {
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被召唤";
                        }
                        if (desc == (UInt32)game_function.MSG_SPSUMMONING)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 特殊召唤宣言";
                        }
                        if (desc == (UInt32)game_function.MSG_SPSUMMONED)
                        {
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被特殊召唤";
                        }
                        if (desc == (UInt32)game_function.MSG_FLIPSUMMONING)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 翻转召唤宣言";
                            sound_player.Play("flip", 1f);
                        }
                        if (desc == (UInt32)game_function.MSG_FLIPSUMMONED)
                        {
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被翻转召唤";

                        }
                        if (desc == (UInt32)game_function.MSG_CHAINED)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 被发动";
                        }
                        if (desc == (UInt32)game_function.MSG_DRAW)
                        {
                            event_string = "玩家抽卡";
                        }
                        if (desc == (UInt32)game_function.MSG_DAMAGE)
                        {
                            event_string = "伤害输出";
                        }
                        if (desc == (UInt32)game_function.MSG_RECOVER)
                        {
                            event_string = "玩家生命值恢复";
                        }
                        if (desc == (UInt32)game_function.MSG_ATTACK)
                        {
                            event_code = code;
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 攻击宣言";
                        }
                        if (desc == (UInt32)game_function.MSG_ATTACK_DISABLED)
                        {
                            event_string = client.card_data_manager.GetById(event_code).Name +
                                " 攻击无效";
                        }
                    }
                }
                break;
            case game_function.MSG_SHUFFLE_HAND:
                {
                    sound_player.Play("shuffle", 1f);
                    bool me = if_me(r.ReadByte());
                    animation_shuffle_hand(me);
                }
                break;
            case game_function.MSG_SHUFFLE_DECK:
                {
                    sound_player.Play("shuffle", 1f);
                    bool me = if_me(r.ReadByte());
                    animation_shuffle_deck(me);
                }
                break;
            case game_function.MSG_TOSS_COIN:
                {
                    int data = r.ReadByte();
                    GameObject tempobj = create_game_object(loader.mod_ocgcore_coin,Vector3.zero,Quaternion.identity);
                    tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                    tempobj.GetComponent<coiner>().coin_app();
                    if (data == 0)
                    {
                        tempobj.GetComponent<coiner>().tocoin(false);
                    }
                    else
                    {
                        tempobj.GetComponent<coiner>().tocoin(true);
                    }
                    kill_game_object(tempobj,7);
                }
                break;
            case game_function.MSG_TOSS_DICE:
                {
                    int data = r.ReadByte();
                    GameObject tempobj = create_game_object(loader.mod_ocgcore_dice, Vector3.zero, Quaternion.identity);
                    tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                    tempobj.GetComponent<coiner>().dice_app();
                    tempobj.GetComponent<coiner>().todice(data);
                    kill_game_object(tempobj, 7);
                }
                break;
            case game_function.MSG_FIELD_DISABLED:
                {
                    UInt32 selectable_field = r.ReadUInt32();
                    int filter = 0x1;
                    for (int i = 0; i < 5; ++i, filter <<= 1)
                    {
                        point p = new point(if_me(0), game_location.LOCATION_MZONE, i);
                        if ((selectable_field & filter) > 0)
                        {
                            game_field.set_point_disabled(p, true);
                        }
                        else
                        {
                            game_field.set_point_disabled(p, false);
                        }
                    }
                    filter = 0x100;
                    for (int i = 0; i < 8; ++i, filter <<= 1)
                    {
                        point p = new point(if_me(0), game_location.LOCATION_SZONE, i);
                        if ((selectable_field & filter) > 0)
                        {
                            game_field.set_point_disabled(p, true);
                        }
                        else
                        {
                            game_field.set_point_disabled(p, false);
                        }
                    }
                    filter = 0x10000;
                    for (int i = 0; i < 5; ++i, filter <<= 1)
                    {
                        point p = new point(if_me(1), game_location.LOCATION_MZONE, i);
                        if ((selectable_field & filter) > 0)
                        {
                            game_field.set_point_disabled(p, true);
                        }
                        else
                        {
                            game_field.set_point_disabled(p, false);
                        }
                    }
                    filter = 0x1000000;
                    for (int i = 0; i < 8; ++i, filter <<= 1)
                    {
                        point p = new point(if_me(1), game_location.LOCATION_SZONE, i);
                        if ((selectable_field & filter) > 0)
                        {
                            game_field.set_point_disabled(p, true);
                        }
                        else
                        {
                            game_field.set_point_disabled(p, false);
                        }
                    }
                }
                break;
            case game_function.MSG_REVERSE_DECK:
                {
                    for (int i = 0; i < cards.Count;i++)
                    {
                        if (cards[i].p.location==game_location.LOCATION_DECK)
                        {
                            if (cards[i].p.position==game_position.POS_FACEUP_ATTACK)
                            {
                                cards[i].p.position = game_position.POS_FACEDOWN_ATTACK;
                            }
                            if (cards[i].p.position == game_position.POS_FACEDOWN_ATTACK)
                            {
                                cards[i].p.position = game_position.POS_FACEUP_ATTACK;
                            }
                        }
                    }
                    refresh_all_cards();
                }
                break;
            case game_function.MSG_SWAP_GRAVE_DECK:
                {
                    bool me = if_me(r.ReadByte());
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i].p.me == me)
                        {

                            if (cards[i].p.location == game_location.LOCATION_DECK)
                            {
                                cards[i].p.location = game_location.LOCATION_GRAVE;
                            }
                            else if (cards[i].p.location == game_location.LOCATION_GRAVE)
                            {
                                cards[i].p.location = game_location.LOCATION_DECK;
                            }

                        }
                    }
                    refresh_all_cards();
                }
                break;
            case game_function.MSG_SHUFFLE_SET_CARD:
                {
                    sound_player.Play("shuffle", 1f);
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point loc = read_point(r);
                        OCGCORE_CARD target = get_card(loc);
                        if (target != null)
                        {
                            Vector3 position = Vector3.zero;
                            if (target.p.me == false)
                            {
                                position = new Vector3(0, 5, 5);
                            }
                            if (target.p.me == true)
                            {
                                position = new Vector3(0, 5, -5);
                            }
                            target.animation_confirm_to(position, new Vector3(270, 0, 0), 0.4f, 0.01f);
                            target.set_data(card_data_manager.GetById(0));
                        }
                    }
                }
                break;
            case game_function.MSG_START:
                {
                    int[] lp = new int[2];
                    int[] main_count = new int[2];
                    int[] extra_count = new int[2];
                    lp[0] = r.ReadUInt16();
                    main_count[0] = r.ReadByte();
                    extra_count[0] = r.ReadByte();
                    lp[1] = r.ReadUInt16();
                    main_count[1] = r.ReadByte();
                    extra_count[1] = r.ReadByte();
                    under_ui.get_health_bar(true).set_user_life(lp[0]);
                    under_ui.get_health_bar(false).set_user_life(lp[1]);
                    for (int i = 0; i < main_count[0]; i++)
                    {
                        point p = new point(if_me(0), game_location.LOCATION_DECK, i, game_position.POS_FACEDOWN_ATTACK, 0);
                        create_card(p);
                    }
                    for (int i = 0; i < main_count[1]; i++)
                    {
                        point p = new point(if_me(1), game_location.LOCATION_DECK, i, game_position.POS_FACEDOWN_ATTACK, 0);
                        create_card(p);
                    }
                    for (int i = 0; i < extra_count[0]; i++)
                    {
                        point p = new point(if_me(0), game_location.LOCATION_EXTRA, i, game_position.POS_FACEDOWN_ATTACK, 0);
                        create_card(p);
                    }
                    for (int i = 0; i < extra_count[1]; i++)
                    {
                        point p = new point(if_me(1), game_location.LOCATION_EXTRA, i, game_position.POS_FACEDOWN_ATTACK, 0);
                        create_card(p);
                    }
                    refresh_all_cards();
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_DAMAGE:
                {
                    sound_player.Play("damage",1f);
                    bool me = if_me(r.ReadByte());
                    UInt32 amount = r.ReadUInt32();
                    game_field.animation_show_lp_num(me,false,(int)amount);
                    under_ui.get_health_bar(me).set_user_life(under_ui.get_health_bar(me).get_user_life() - (int)amount);
                    animation_screen_blood(me, (int)amount);
                    count = (int)amount / 50;
                    for (int i = 0; i < count; i++)
                    {
                        if(me){
                            create_game_object(
                            loader.mod_ocgcore_blood,
                            new Vector3(
                                UnityEngine.Random.Range(-15, 15),
                                0,
                                UnityEngine.Random.Range(-5, 5) - 15
                                ),
                                Quaternion.identity
                                );
                        }
                        else
                        {
                            create_game_object(
                            loader.mod_ocgcore_blood,
                            new Vector3(
                                UnityEngine.Random.Range(-15, 15),
                                0,
                                UnityEngine.Random.Range(-5, 5) + 15
                                ),
                                Quaternion.identity
                                );
                        }
                        

                    }
                    lpchanged_to_bgm();
                }
                break;
            case game_function.MSG_RECOVER:
                {
                    sound_player.Play("gainlp", 1f);
                    bool me = if_me(r.ReadByte());
                    UInt32 amount = r.ReadUInt32();
                    game_field.animation_show_lp_num(me, true, (int)amount);
                    under_ui.get_health_bar(me).set_user_life(under_ui.get_health_bar(me).get_user_life() + (int)amount);
                    lpchanged_to_bgm();
                }
                break;
            case game_function.MSG_LPUPDATE:
                {
                    bool me = if_me(r.ReadByte());
                    UInt32 amount_all = r.ReadUInt32();
                    under_ui.get_health_bar(me).set_user_life((int)amount_all);
                    if (under_ui.get_health_bar(me).get_user_life()>amount_all)
                    {
                        sound_player.Play("damage", 1f);
                        int amount = (int)under_ui.get_health_bar(me).get_user_life() - (int)amount_all;

                        game_field.animation_show_lp_num(me, false, (int)amount);

                        animation_screen_blood(me, amount);
                        count = (int)amount / 50;
                        for (int i = 0; i < count; i++)
                        {
                            if (me)
                            {
                                create_game_object(
                                loader.mod_ocgcore_blood,
                                new Vector3(
                                    UnityEngine.Random.Range(-15, 15),
                                    0,
                                    UnityEngine.Random.Range(-5, 5) - 15
                                    ),
                                    Quaternion.identity
                                    );
                            }
                            else
                            {
                                create_game_object(
                                loader.mod_ocgcore_blood,
                                new Vector3(
                                    UnityEngine.Random.Range(-15, 15),
                                    0,
                                    UnityEngine.Random.Range(-5, 5) + 15
                                    ),
                                    Quaternion.identity
                                    );
                            }


                        }

                    }
                    else
                    {
                        sound_player.Play("gainlp", 1f);
                        int amount = -(int)under_ui.get_health_bar(me).get_user_life() + (int)amount_all;

                        game_field.animation_show_lp_num(me, true, (int)amount);
                    }
                    lpchanged_to_bgm();
                }
                break;
            case game_function.MSG_QUERY_CODE:
                {
                    long code = r.ReadUInt32();
                    point loc = read_point(r);
                    Debug.Log("MSG_QUERY_CODE" + "loc:" + loc.location.ToString() + loc.index.ToString() + loc.position.ToString() + " code:" + code.ToString());
                    OCGCORE_CARD target = get_card(loc,false);
                    if (target == null)
                    {
                       // target = create_card(loc);
                       // refresh_all_cards();
                    }
                    else
                    {
                        target.set_data(card_data_manager.GetById(code));
                    }
                }
                break;
            case game_function.MSG_QUERY_EQUIP_CARD:
                {
                    point equip = read_point(r);
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.equip_target = get_card(equip, false);
                        refresh_all_cards();
                        if (target.equip_target != null)
                        {
                            sound_player.Play("equip", 1f);
                            target.fast_decoration(loader.mod_ocgcore_decoration_magic_zhuangbei); 
                        }
                    }
                }
                break;
            case game_function.MSG_QUERY_LSCALE:
                {
                    int lscale = r.ReadByte();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.LeftScale = lscale;
                        d.RightScale = lscale;
                        target.set_data(d);
                        refresh_all_cards();
                    }

                }
                break;
            case game_function.MSG_QUERY_LEVEL:
                {
                    int level = r.ReadByte();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.Level = level;
                        target.set_data(d);
                    }

                }
                break;
            case game_function.MSG_QUERY_ATTRIBUTE:
                {
                    int data = r.ReadInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.Attribute = data;
                        target.set_data(d);
                    }

                }
                break;
            case game_function.MSG_QUERY_RACE:
                {
                    int data = r.ReadInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.Race = data;
                        target.set_data(d);
                    }

                }
                break;
            case game_function.MSG_QUERY_ATTACK:
                {
                    int data = r.ReadInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.Attack = data;
                        target.set_data(d);
                    }

                }
                break;
            case game_function.MSG_QUERY_DEFENSE:
                {
                    int data = r.ReadInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        CardData d = target.get_data();
                        d.Defense = data;
                        target.set_data(d);
                    }

                }
                break;
            case game_function.MSG_CONFIRM_CARDS:
                {
                    long code = r.ReadUInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target == null)
                    {
                        target = create_card(loc);
                        refresh_all_cards();
                    }
                    target.set_data(card_data_manager.GetById(code));
                    confirm_card(target);
                }
                break;
            case game_function.MSG_QUERY_IS_DISABLED:
                {
                    int bol = r.ReadByte();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        if (bol == 0)
                        {
                            target.set_disable(false);
                        }
                        else
                        {
                            target.set_disable(true);
                        }
                    }
                   
                }
                break;
            case game_function.MSG_HINT_OPSELECTED:
                {
                    UInt32 data = r.ReadUInt32();
                    game_field.add_log("效果发动："+get_description(data));
                }
                break;
            case game_function.MSG_HINT_RACE:
                {
                    UInt32 data = r.ReadUInt32();
                    game_field.add_log("种族选择：" + card_string_helper.get_race_string(data));
                }
                break;
            case game_function.MSG_HINT_ATTRIB:
                {
                    UInt32 data = r.ReadUInt32();
                    game_field.add_log("属性选择：" + card_string_helper.get_attribute_string(data));
                }
                break;
            case game_function.MSG_HINT_NUMBER:
                {
                    UInt32 data = r.ReadUInt32();
                    game_field.add_log("数字选择：" + data.ToString());
                }
                break;
            case game_function.MSG_HINT_CODE:
                {
                    UInt32 data = r.ReadUInt32();
                    game_field.animation_show_card_code(data);
                }
                break;
            case game_function.MSG_CHINT_TURN:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.clear_all_tail();
                        target.add_string_tail("数字记录："+data.ToString());
                        confirm_card(target);
                        var number = target.add_one_decoration(loader.mod_ocgcore_number,3,new Vector3(60,0,0),"number");
                        number.game_object.GetComponent<number_loader>().set_number((int)data,3);
                        number.scale_change_ignored = true;

                        number.game_object.transform.localScale = Vector3.zero;
                        iTween.ScaleTo(number.game_object, new Vector3(1,1,1), 0.3f);
                        iTween.RotateTo(number.game_object, new Vector3(60, 0, 0), 0.3f);
                        iTween.ScaleTo(number.game_object, iTween.Hash(
                                           "delay", 1.6f,
                                           "x", 0,
                                           "y", 0,
                                           "z", 0,
                                           "time", 0.6f
                                           ));
                        kill_game_object(number.game_object, 2.2f);


                    }
                }
                break;
            case game_function.MSG_CHINT_CARD:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.add_string_tail("卡片记录：" + card_data_manager.GetById(data).Name);
                    }
                }
                break;
            case game_function.MSG_CHINT_RACE:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.add_string_tail("种族记录：" + card_string_helper.get_race_string(data));
                    }
                }
                break;
            case game_function.MSG_CHINT_ATTRIBUTE:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.add_string_tail("属性记录：" + card_string_helper.get_attribute_string(data));
                    }
                }
                break;
            case game_function.MSG_CHINT_DESC_ADD:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.add_string_tail(get_description(data));
                    }
                }
                break;
            case game_function.MSG_CHINT_DESC_REMOVE:
                {
                    point loc = read_point(r);
                    UInt32 data = r.ReadUInt32();
                    OCGCORE_CARD target = get_card(loc);
                    if (target != null)
                    {
                        target.del_one_tail(get_description(data));
                    }
                }
                break;
            case game_function.MSG_TAG_SWAP:
                {
                    bool tag_swap_me = if_me(r.ReadByte());
                    resize_location(tag_swap_me, game_location.LOCATION_DECK, r.ReadByte());
                    resize_location(tag_swap_me, game_location.LOCATION_EXTRA, r.ReadByte());
                    resize_location(tag_swap_me, game_location.LOCATION_HAND, r.ReadByte());
                }
                break;
            case game_function.MSG_MOVE:
                {
                    point p1 = read_point(r);
                    point p2 = read_point(r);
                    Debug.Log("MSG_MOVE" + "from:" + p1.location.ToString() + p1.index.ToString() + p1.position.ToString() + " to:" + p2.location.ToString() + p2.index.ToString() + p2.position.ToString());
                    move_card(p1, p2);
                    refresh_all_cards();
                    for (int i = 0; i < unclearable_cookie_cards_in_select.Count; i++)
                    {
                        unclearable_cookie_cards_in_select[i].del_all_decoration_by_string("selected");
                    }
                    unclearable_cookie_cards_in_select.Clear();
                    ///animation
                    if(p1.location==game_location.LOCATION_DECK&&p2.location==game_location.LOCATION_HAND){
                        sound_player.Play("draw",1);
                    }
                    if (p2.overlay_index > 0 && p2.location == game_location.LOCATION_EXTRA)
                    {
                        if (cookie_dark_hole == null)
                        {
                            cookie_dark_hole = create_game_object(loader.mod_ocgcore_ss_dark_hole,Vector3.zero,Quaternion.identity);
                            cookie_dark_hole.AddComponent<partical_scaler>().scale = 5;
                            iTween.ScaleTo(cookie_dark_hole, new Vector3(8,8,8), 1f);
                            cookie_dark_hole.transform.eulerAngles = new Vector3(90,0,0);
                        }
                    }
                    else
                    {
                        if (cookie_dark_hole != null)
                        {
                            iTween.ScaleTo(cookie_dark_hole, iTween.Hash(
                                    "delay", 2f,
                                    "x", 0,
                                    "y", 0,
                                    "z", 0,
                                    "time", 1f
                                    ));
                            kill_game_object(cookie_dark_hole, 2.8f, false);
                        }
                    }
                }
                break;
            case game_function.MSG_CARD_SELECTED:
                {
                    point p1 = read_point(r);
                    OCGCORE_CARD card = get_card(p1);
                    unclearable_cookie_cards_in_select.Add(card);
                    card.add_one_decoration(loader.mod_ocgcore_decoration_card_selected, 3, Vector3.zero, "selected", false);
                    bool psum=false;
                    Vector3 pvector = Vector3.zero;
                    if (unclearable_cookie_cards_in_select.Count==2)
                    {
                        if (unclearable_cookie_cards_in_select[0].p.location==game_location.LOCATION_SZONE)
                        {
                            if (unclearable_cookie_cards_in_select[1].p.location == game_location.LOCATION_SZONE)
                            {
                                if (unclearable_cookie_cards_in_select[1].p.index == 6||unclearable_cookie_cards_in_select[1].p.index == 7)
                                {
                                    if (unclearable_cookie_cards_in_select[0].p.index == 6 || unclearable_cookie_cards_in_select[0].p.index == 7)
                                    {
                                        if (unclearable_cookie_cards_in_select[0].p.me ==unclearable_cookie_cards_in_select[0].p.me)
                                        {
                                            psum = true;
                                            if (unclearable_cookie_cards_in_select[0].p.me)
                                            {
                                                pvector = new Vector3(0, 0, -5);
                                            }
                                            else
                                            {
                                                pvector = new Vector3(0, 0, 5);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (psum)
                    {
                        GameObject obj_1 = create_game_object(loader.mod_ocgcore_ss_p_sum_effect, pvector, Quaternion.identity);
                        kill_game_object(obj_1, 5);
                    }
                   
                }
                break;
            case game_function.MSG_SUMMONING:
                {
                    point p1 = read_point(r);
                    OCGCORE_CARD card = get_card(p1);
                    GameObject mod = loader.mod_ocgcore_ss_spsummon_normal;
                    Vector3 pos = get_point_worldposition(p1);
                    GameObject obj_1 = create_game_object(mod, pos, Quaternion.identity);
                    kill_game_object(obj_1, 5);
                    card.animation_show_off(600, true);
                }
                break;
            case game_function.MSG_SPSUMMONING:
                {
                    point p1 = read_point(r);
                    OCGCORE_CARD card = get_card(p1);
                    GameObject mod = loader.mod_ocgcore_ss_summon_light;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
                        mod = loader.mod_ocgcore_ss_summon_earth;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DARK))
                        mod = loader.mod_ocgcore_ss_summon_dark;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
                        mod = loader.mod_ocgcore_ss_summon_light;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
                        mod = loader.mod_ocgcore_ss_summon_fire;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
                        mod = loader.mod_ocgcore_ss_summon_light;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WATER))
                        mod = loader.mod_ocgcore_ss_summon_water;
                    if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WIND))
                        mod = loader.mod_ocgcore_ss_summon_wind;
                    string path_sound = "specialsummon";
                    if (card_string_helper.differ(card.get_data().Type, (long)game_type.TYPE_FUSION))
                    {
                        mod = loader.mod_ocgcore_ss_spsummon_ronghe;
                        path_sound = "specialsummon2";
                    }
                    if (card_string_helper.differ(card.get_data().Type, (long)game_type.TYPE_SYNCHRO))
                    { 
                        mod = loader.mod_ocgcore_ss_spsummon_tongtiao;
                        path_sound = "specialsummon2";
                    }
                    if (card_string_helper.differ(card.get_data().Type, (long)game_type.TYPE_RITUAL))
                    {
                        mod = loader.mod_ocgcore_ss_spsummon_yishi;
                        path_sound = "specialsummon2";
                    }
                    sound_player.Play(path_sound, 1f);
                    Vector3 pos = get_point_worldposition(p1);
                    GameObject obj_1 = create_game_object(mod, pos, Quaternion.identity);
                    kill_game_object(obj_1, 5);
                    card.animation_show_off(600, true);
                }
                break;
            case game_function.MSG_ADD_COUNTER:
                {
                    sound_player.Play("addcounter", 1);
                    point p1 = read_point(r);
                    UInt16 type = r.ReadUInt16();
                    count = r.ReadUInt16();
                    string name = client.string_data_manager.get("counter", type);
                    OCGCORE_CARD card = get_card(p1);
                    for (int i = 0; i < count;i++ )
                    {
                        card.add_string_tail(name);
                    }
                    Vector3 pos = ui_helper.get_close(card.game_object.transform.position, client.camera_game_main, 5);
                    GameObject obj_1 = create_game_object(loader.mod_ocgcore_cs_end, pos, Quaternion.identity);
                    kill_game_object(obj_1, 5);
                }
                break;
            case game_function.MSG_REMOVE_COUNTER:
                {
                    sound_player.Play("removecounter", 1);
                    point p1 = read_point(r);
                    UInt16 type = r.ReadUInt16();
                    count = r.ReadUInt16();
                    string name = client.string_data_manager.get("counter", type);
                    OCGCORE_CARD card = get_card(p1);
                    for (int i = 0; i < count; i++)
                    {
                        card.del_one_tail(name);
                    }
                    Vector3 pos = ui_helper.get_close(card.game_object.transform.position, client.camera_game_main, 5);
                    GameObject obj_1 = create_game_object(loader.mod_ocgcore_cs_end, pos, Quaternion.identity);
                    kill_game_object(obj_1, 5);
                }
                break;
            case game_function.MSG_CHAINING:
                {
                    sound_player.Play("activate",1);
                    ///
                    long code = r.ReadUInt32();
                    point loc = read_point(r);
                    OCGCORE_CARD target = get_card(loc);
                    if (target == null)
                    {
                        target = create_card(loc);
                        refresh_all_cards();
                    }
                    target.safe_data(card_data_manager.GetById(code));
                    if (loc.location == game_location.LOCATION_HAND && loc.me == false)
                    {
                        loc.position = game_position.POS_FACEUP_ATTACK;
                        move_card(loc, loc);
                        refresh_all_cards();
                    }
                    ///
                    debugger.Log("MSG_CHAINING");
                    OCGCORE_CARD card = target;
                    unclearable_cookie_cards_in_chain.Add(card);
                    card.add_one_decoration(loader.mod_ocgcore_cs_chaining,3,Vector3.zero,"chaining",false);
                    card.animation_show_off(600, false);
                    if ((card.get_data().Type & (int)game_type.TYPE_MONSTER) > 0)
                    {
                        card.fast_decoration(loader.mod_ocgcore_decoration_monster_activated);
                    }
                    if ((card.get_data().Type & (int)game_type.TYPE_SPELL) > 0)
                    {
                        card.fast_decoration(loader.mod_ocgcore_decoration_magic_activated);
                    }
                    if ((card.get_data().Type & (int)game_type.TYPE_TRAP) > 0)
                    {
                        card.fast_decoration(loader.mod_ocgcore_decoration_trap_activated);
                    }
                }
                break;
            case game_function.MSG_CHAIN_SOLVED:
                {
                    int id = r.ReadByte()-1;
                    if (id < 0)
                    {
                        id = 0;
                    }
                    debugger.Log("MSG_CHAIN_SOLVED"+id);
                    if (id < unclearable_cookie_cards_in_chain.Count)
                    {
                        OCGCORE_CARD card = unclearable_cookie_cards_in_chain[id];
                        card.del_all_decoration_by_string("chaining");
                        Vector3 pos = ui_helper.get_close(card.game_object.transform.position,client.camera_game_main,5);
                        GameObject obj_1 = create_game_object(loader.mod_ocgcore_cs_end,pos,Quaternion.identity);
                        kill_game_object(obj_1,5);
                        if(id>1){
                            obj_1 = create_game_object(loader.mod_ocgcore_cs_bomb, card.game_object.transform.position, Quaternion.identity);
                            kill_game_object(obj_1, 5);
                        }
                    }
                }
                break;
            case game_function.MSG_CHAIN_DISABLED:
                {
                    int id = r.ReadByte()-1;
                    if (id < 0)
                    {
                        id = 0;
                    }
                    debugger.Log("MSG_CHAIN_DISABLED" + id);
                    if (id < unclearable_cookie_cards_in_chain.Count)
                    {
                        OCGCORE_CARD card = unclearable_cookie_cards_in_chain[id];
                        card.del_all_decoration_by_string("chaining");
                        GameObject obj_1 = create_game_object(loader.mod_ocgcore_cs_negated, card.game_object.transform.position, Quaternion.identity);
                        kill_game_object(obj_1, 5);
                    }
                }
                break;
            case game_function.MSG_CHAIN_END:
                {
                    for (int i = 0; i < unclearable_cookie_cards_in_chain.Count;i++ )
                    {
                        unclearable_cookie_cards_in_chain[i].del_all_decoration_by_string("chaining");
                    }
                    unclearable_cookie_cards_in_chain.Clear();

                    for (int i = 0; i < unclearable_cookie_cards_in_select.Count; i++)
                    {
                        unclearable_cookie_cards_in_select[i].del_all_decoration_by_string("selected");
                    }
                    unclearable_cookie_cards_in_select.Clear();
                    if (cookie_attack_line != null)
                    {
                        kill_game_object(cookie_attack_line);
                    }
                }
                break;
            case game_function.MSG_ATTACK:
                {
                    sound_player.Play("attack", 1);
                    point p1 = read_point(r);
                    point p2 = read_point(r);
                    Vector3 attacker = get_point_worldposition(p1);
                    Vector3 attack_target = Vector3.zero;
                    if(p2.location==game_location.LOCATION_UNKNOWN)
                    {
                        bool attacker_bool_me = p1.me;
                        if (attacker_bool_me)
                        {
                            attack_target = new Vector3(0, 0, 15);
                        }
                        else
                        {
                            attack_target = new Vector3(0, 0, -15);
                        }
                    }
                    else
                    {
                        attack_target = get_point_worldposition(p2);
                    }
                    if (cookie_attack_line != null)
                    {
                        kill_game_object(cookie_attack_line);
                    }
                    cookie_attack_line = create_game_object(loader.mod_ocgcore_bs_atk_sign,Vector3.zero,Quaternion.identity);
                    attacksign compo = cookie_attack_line.GetComponent<attacksign>();
                    compo.from = attacker;
                    compo.to = attack_target;
                }
                break;
            case game_function.MSG_BATTLE:
                {
                    sound_player.Play("explode",1);
                    point p1 = read_point(r);
                    OCGCORE_CARD card_attacker = get_card(p1);
                    OCGCORE_CARD card_attacked = null;
                    point p2 = read_point(r);
                    Vector3 attacker = get_point_worldposition(p1);
                    Vector3 attack_target = Vector3.zero;
                    int amount = (int)(card_attacker.get_data().Attack * 0.8f);
                    iTween.ShakePosition(camera_game_main.gameObject, iTween.Hash(
               "x", (float)amount / 1500f,
               "y", (float)amount / 1500f,
               "z", (float)amount / 1500f,
               "onupdate", (Action)fix_camera,
               "time", (float)amount / 2500f
               ));
                    if (p2.location == game_location.LOCATION_UNKNOWN)
                    {
                        bool attacker_bool_me = p1.me;
                        if (attacker_bool_me)
                        {
                            attack_target = new Vector3(0, 0, 20);
                        }
                        else
                        {
                            attack_target = new Vector3(0, 0, -20);
                        }
                    }
                    else
                    {
                        attack_target = get_point_worldposition(p2);
                        attack_target += (attack_target - attacker) * 0.3f;
                        card_attacked = get_card(p2);
                    }
                    GameObject mod = card_race_to_attack_effect_mod(card_attacker, attacker,card_attacker.get_data().Attack);
                    if (cookie_attack_line != null)
                    {
                        kill_game_object(cookie_attack_line);
                    }
                    if (cookie_attack_eff1 != null)
                    {
                        kill_game_object(cookie_attack_eff1);
                    }
                    if (cookie_attack_eff2 != null)
                    {
                        kill_game_object(cookie_attack_eff2);
                    }
                    cookie_attack_eff1 = create_game_object(mod, Vector3.zero, Quaternion.identity);
                    var particles = cookie_attack_eff1.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (ParticleSystem p in particles)
                    {
                        float k = (float)card_attacker.get_data().Attack;
                        if (k > 3000)
                        {
                            k = 3000;
                        }
                        p.startSize *= 10f * k / 1500f;
                    }
                    attack_camera camer = cookie_attack_eff1.AddComponent<attack_camera>();
                    camer.set(attacker,attack_target,card_attacker.get_data().Attack);
                    if (card_attacked != null)
                    {
                        if (card_attacked.p.position==game_position.POS_FACEUP_ATTACK)
                        {
                            GameObject mod2 = card_race_to_attack_effect_mod(card_attacked, attack_target, card_attacked.get_data().Attack);
                            cookie_attack_eff2 = create_game_object(mod2, Vector3.zero, Quaternion.identity);
                            particles = cookie_attack_eff2.GetComponentsInChildren<ParticleSystem>(true);
                            foreach (ParticleSystem p in particles)
                            {
                                float k = (float)card_attacked.get_data().Attack;
                                if (k > 3000)
                                {
                                    k = 3000;
                                }
                                p.startSize *= 10f * k / 1500f;
                            }
                            attack_camera camer2 = cookie_attack_eff2.AddComponent<attack_camera>();
                            camer2.set(attack_target, attacker, card_attacked.get_data().Attack);
                        }
                    }
                }
                break;
            case game_function.MSG_SELECT_IDLECMD:
                {
                    response_type = "idle";
                    game_field.set_string("现在是" + now_hint + ",请选择需要操作的卡。");
                    select_hint = "";
                    count = r.ReadByte();
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        create_button(get_card(p), ((i << 16) + 0), game_button.summon, "通常召唤");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        if (p.location == game_location.LOCATION_SZONE && (p.index == 6 || p.index == 7))
                        {
                            create_button(null, ((i << 16) + 1), game_button.spsummon, "灵摆召唤!");
                        }
                        else
                        {
                            OCGCORE_CARD car = get_card(p);
                            create_button(car, ((i << 16) + 1), game_button.spsummon, "特殊召唤");
                            if(car.condition!=OCGCORE_CARD.OCGCORE_CARD_CONDITION.verticle_clickable)
                                car.add_one_decoration(loader.mod_ocgcore_decoration_spsummon, 2, Vector3.zero, "chain_selecting", true, true);
                        
                        }
                       
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        create_button(get_card(p), ((i << 16) + 2), game_button.change, "改变表示形式");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        create_button(get_card(p), ((i << 16) + 3), game_button.set, "放置");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        create_button(get_card(p), ((i << 16) + 4), game_button.set, "放置");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        OCGCORE_CARD car = get_card(p);
                        create_button(car, ((i << 16) + 5), game_button.active, "发动 " + get_description(description));
                        if (car.condition != OCGCORE_CARD.OCGCORE_CARD_CONDITION.verticle_clickable) 
                            car.add_one_decoration(loader.mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                    }
                    byte bp = r.ReadByte();
                    byte ep = r.ReadByte();
                    byte shuffle = r.ReadByte();
                    if (bp == 1) 
                    {
                        create_button(null, 6, game_button.battle_phrase, "战斗阶段"); 
                    }
                    if (ep == 1) 
                    {
                        create_button(null, 7, game_button.end_phrase, "结束回合");
                    }
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_SELECT_BATTLECMD:
                {
                    response_type = "idle";
                    game_field.set_string("现在是" + now_hint + ",请选择需要操作的卡。");
                    select_hint = "";
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        OCGCORE_CARD car = get_card(p);
                        create_button(car, ((i << 16) + 0), game_button.active, "发动 " + get_description(description));
                        if (car.condition != OCGCORE_CARD.OCGCORE_CARD_CONDITION.verticle_clickable) 
                            car.add_one_decoration(loader.mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        var c = get_card(p);
                        create_button(c, ((i << 16) + 1), game_button.attack, "攻击！");
                        c.add_one_decoration(loader.mod_ocgcore_bs_atk_decoration,5,Vector3.zero,"atk");
                    }
                    byte m2 = r.ReadByte();
                    byte ep = r.ReadByte();
                    if (m2 == 1)
                    {
                        create_button(null, 2, game_button.main_phrase, "主要阶段");
                    }
                    if (ep == 1)
                    {
                        create_button(null, 3, game_button.end_phrase, "结束回合");
                    }
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_SELECT_YESNO:
                {
                    if (bool_space_is_down)
                    {
                        HASH_MESSAGE message_to_send = new HASH_MESSAGE();
                        message_to_send.Params.writer.Write((int)0);
                        send_message(message_to_send);
                    }
                    else
                    {
                        //response_type = "idle";
                        //under_ui.get_health_bar(true).set_user_light(true);
                        //under_ui.get_health_bar(false).set_user_light(false);
                        //game_field.set_string("(右方按钮) "+get_description(r.ReadUInt32()));
                        //create_button(null, 1, game_button.yes, "确认");
                        //create_button(null, 0, game_button.no, "取消(空格)").cookie_string = "can";
                        under_ui.get_health_bar(true).set_user_light(true);
                        under_ui.get_health_bar(false).set_user_light(false);
                        response_type = "idle";
                        cookie_select_min = 1;
                        window_option = new OCGCORE_SELECT_OPTION(this);
                        window_option.change_title(get_description(r.ReadUInt32()));
                        window_option.add_opt("我要使用这个效果", 1);
                        window_option.add_opt("我不要使用这个效果", 0);
                    }
                }
                break;
            case game_function.MSG_SELECT_CHAIN:
                {
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    response_type = "idle";
                    int spsount = r.ReadByte();
                    int forced = r.ReadByte();
                    count = r.ReadByte();
                    List<OCGCORE_CARD> chain_cards = new List<OCGCORE_CARD>();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        int conti = r.ReadByte();
                        string desc = get_description(description);
                        OCGCORE_CARD car = get_card(p);
                        car.cookie_chain_forced = (conti != 0);

                        OCGCORE_CARD.chain_effect eff = new OCGCORE_CARD.chain_effect();
                        eff.ptr = i;
                        eff.desc = "连锁 " + desc;
                        car.cookie_chain_effects.Add(eff);


                        chain_cards.Add(car);
                    }
                    int handle_flag = 0;
                    if (forced == 0)
                    {
                        //无强制发动的卡
                        if (spsount == 0)
                        {
                            //无关键卡
                            if (bool_space_is_down == true)
                            {
                                //无关键卡 连锁被无视 直接回答---
                                handle_flag = 0;
                            }
                            else if (under_ui.get_if_watch_chains())
                            {
                                //无关键卡但是连锁被监控
                                if (chain_cards.Count == 0)
                                {
                                    //根本没卡 直接回答---
                                    handle_flag = 0;
                                }
                                else
                                {
                                    if (chain_cards.Count == 1)
                                    {
                                        //只有一张要处理的卡 常规处理 一张---
                                        handle_flag = 1;
                                    }
                                    else
                                    {
                                        //常规处理 多张---
                                        handle_flag = 2;
                                    }
                                }
                            }
                            else
                            {
                                //无关键卡而且连锁没有被监控    直接回答---
                                handle_flag = 0;
                            }
                        }
                        else
                        {
                            //有关键卡
                            if (chain_cards.Count == 0)
                            {
                                //根本没卡 直接回答---
                                handle_flag = 0;
                            }
                            else if (bool_space_is_down == true)
                            {
                                //有关键卡 连锁被无视 直接回答---
                                handle_flag = 0;
                            }
                            else
                            {
                                if (chain_cards.Count == 1)
                                {
                                    //只有一张要处理的卡 常规处理 一张---
                                    handle_flag = 1;
                                }
                                else
                                {
                                    //常规处理 多张---
                                    handle_flag = 2;
                                }
                            }
                        }
                    }
                    else
                    {
                        //有强制发动的卡 处理强制发动的卡--
                        handle_flag = 3;
                    }
                    if (handle_flag == 0)
                    {
                        //直接回答
                        HASH_MESSAGE message_to_send = new HASH_MESSAGE();
                        message_to_send.Params.writer.Write((int)-1);
                        send_message(message_to_send);
                    }
                    if (handle_flag == 1)
                    {
                        //处理一张
                        window_option = new OCGCORE_SELECT_OPTION(this);
                        cookie_select_min = 1;
                        game_field.set_string(event_string+" 是否连锁");
                        window_option.change_title(event_string + " 是否连锁");
                        chain_cards[0].add_one_decoration(loader.mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chain_cards[0].cookie_string = "chain_response";
                        create_button(null, -1, game_button.no, "取消(空格)").cookie_string = "can";
                        window_option.add_opt("连锁 " + chain_cards[0].get_data().Name + " 的效果", 0);
                        window_option.add_opt("不连锁这张卡的效果", -1);
                    }
                    if (handle_flag == 2)
                    {
                        //处理多张
                        window_option = new OCGCORE_SELECT_OPTION(this);
                        cookie_select_min = 1;
                        game_field.set_string(event_string + " 是否连锁");
                        window_option.change_title(event_string + " 是否连锁");
                        for (int i = 0; i < chain_cards.Count; i++)
                        {
                            chain_cards[i].add_one_decoration(loader.mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                            chain_cards[i].cookie_string = "chain_response";
                        }
                        create_button(null, -1, game_button.no, "取消(空格)").cookie_string = "can";
                        window_option.add_opt("我要连锁卡的效果", 9198);
                        window_option.add_opt("不连锁任何效果", -1);
                    }
                    if (handle_flag == 3)
                    {
                        //处理强制发动的卡
                        window_option = new OCGCORE_SELECT_OPTION(this);
                        cookie_select_min = 1;
                        game_field.set_string(event_string + " 必须连锁卡片");
                        window_option.change_title(event_string + " 必须连锁卡片");
                        for (int i = 0; i < chain_cards.Count; i++)
                        {
                            chain_cards[i].add_one_decoration(loader.mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                            chain_cards[i].cookie_string = "chain_response";
                            if (chain_cards[i].cookie_chain_forced)
                            {
                                window_option.add_opt("连锁 " + chain_cards[i].get_data().Name + " 的效果", 0);
                            }
                        }
                        window_option.add_opt("查看场地", 9198);
                    }
                }
                break;
            case game_function.MSG_SELECT_EFFECTYN:
                {
                    if (bool_space_is_down)
                    {
                        HASH_MESSAGE message_to_send = new HASH_MESSAGE();
                        message_to_send.Params.writer.Write((int)0);
                        send_message(message_to_send);
                    }
                    else
                    {
                        //response_type = "idle";
                        //under_ui.get_health_bar(true).set_user_light(true);
                        //under_ui.get_health_bar(false).set_user_light(false);
                        //game_field.set_string("现在是" + now_hint + ",请选择需要连锁的卡。");
                        //point p = read_point(r);
                        //create_button(get_card(p), 1, game_button.active, "发动");
                        //get_card(p).add_one_decoration(loader.mod_ocgcore_decoration_chain_selecting, 5, Vector3.zero, "chain_selecting");
                        //create_button(null, 0, game_button.no, "取消(空格)").cookie_string = "can";
                        //get_card(p).cookie_show_in_my_hand = true;
                        //refresh_all_cards();
                        //go_to_min_camera();

                        under_ui.get_health_bar(true).set_user_light(true);
                        under_ui.get_health_bar(false).set_user_light(false);
                        response_type = "idle";
                        cookie_select_min = 1;
                        window_option = new OCGCORE_SELECT_OPTION(this);
                        //for (int i = 0; i < count; i++)
                        //{
                        //    window_option.add_opt(get_description(r.ReadUInt32()), i);
                        //}
                        point p = read_point(r);
                        create_button(get_card(p), 1, game_button.active, "发动");
                        get_card(p).add_one_decoration(loader.mod_ocgcore_decoration_chain_selecting, 5, Vector3.zero, "chain_selecting");
                        create_button(null, 0, game_button.no, "取消(空格)").cookie_string = "can";
                        window_option.add_opt("发动 " + get_card(p).get_data().Name + " 的效果", 1);
                        window_option.add_opt("不发动这张卡的效果", 0);
                        get_card(p).cookie_show_in_my_hand = true;
                        refresh_all_cards();
                        go_to_min_camera();
                    }
                }
                break;
            case game_function.MSG_SELECT_CARD:
                {
                    response_type = "select_card";
                    game_field.set_string(select_hint);
                    select_hint = "";
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    cookie_select_min = r.ReadByte();
                    cookie_select_max = r.ReadByte();
                    cookie_select_level = r.ReadUInt32();
                    int count_2 = r.ReadByte();
                    for (int i = 0; i < count_2; i++)
                    {
                        long code = r.ReadUInt32();
                        point p = read_point(r);
                        OCGCORE_CARD card = get_card(p);
                        card.cookie_select_option_1 = r.ReadUInt16();
                        card.cookie_select_option_2 = r.ReadUInt16();
                        if (card.cookie_select_option_1 == 0)
                        {
                            card.cookie_select_option_1 = (int)card.get_data().Level;
                        }
                        if (card.cookie_select_option_2 == 0)
                        {
                            card.cookie_select_option_2 = (int)card.get_data().Level;
                        }
                        if(code>0)
                        {
                            card.safe_data(card_data_manager.GetById(code));
                        }
                        card.cookie_select_ptr = i;
                        cookie_cards_must_select.Add(card);
                        debugger.Log(card.get_data().Name + ":::" + card.cookie_select_option_1 + ":::" + card.cookie_select_option_2);
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        long code = r.ReadUInt32();
                        point p = read_point(r);
                        OCGCORE_CARD card = get_card(p);
                        card.cookie_select_option_1 = r.ReadUInt16();
                        card.cookie_select_option_2 = r.ReadUInt16();
                        if (card.cookie_select_option_1 == 0)
                        {
                            card.cookie_select_option_1 = (int)card.get_data().Level;
                        }
                        if (card.cookie_select_option_2 == 0)
                        {
                            card.cookie_select_option_2 = (int)card.get_data().Level;
                        }
                        if (code > 0)
                        {
                            card.safe_data(card_data_manager.GetById(code));
                        }
                        card.cookie_select_ptr = i;
                        card.cookie_string = "clickable";
                        cookie_cards_for_select.Add(card);
                    }
                    cookie_select_card_refresh();
                    refresh_all_cards();
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_SORT_CARD:
                {
                    response_type = "sort_card";
                    if (select_hint == "")
                    {
                        select_hint = "请给卡片排序。";
                    }
                    game_field.set_string(select_hint);
                    select_hint = "";
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    cookie_select_max = r.ReadByte();
                    for (int i = 0; i < cookie_select_max; i++)
                    {
                        long code = r.ReadUInt32();
                        point p = read_point(r);
                        OCGCORE_CARD card = get_card(p);
                        if (code > 0)
                        {
                            card.safe_data(card_data_manager.GetById(code));
                        }
                        card.cookie_string = "sortable";
                        card.cookie_int = i;
                        card.add_one_decoration(loader.mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                        card.cookie_show_in_my_hand = true;
                    }
                    refresh_all_cards();
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_SELECT_COUNTER:
                {
                    response_type = "select_counter";
                    select_hint = "";
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    cookie_select_max = r.ReadByte();
                    if (select_hint == "")
                    {
                        select_hint = "请给卡片移除" + cookie_select_max.ToString() + "个指示物。";
                    }
                    game_field.set_string(select_hint);
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        OCGCORE_CARD card = get_card(p);
                        card.cookie_select_option_1 = r.ReadByte();
                        card.cookie_select_option_2 = card.cookie_select_option_1;
                        card.cookie_string = "counter_holder";
                        card.add_one_decoration(loader.mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                        card.cookie_show_in_my_hand = true;
                        card.cookie_int = 0;
                        cookie_cards_for_select.Add(card);
                    }
                    OCGCORE_BUTTON button = create_button(null, 0, game_button.no, "重新选择");
                    button.cookie_string = "clear_counter_select";
                    refresh_all_cards();
                    go_to_min_camera();
                }
                break;
            case game_function.MSG_SELECT_POSITION:
                {
                    response_type = "select_position";
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    UInt32 code = r.ReadUInt32();
                    UInt32 positions = r.ReadUInt32();
                    under_ui.change_data(card_data_manager.GetById(code));
                    window_select_position = new OCGCORE_SELECT_POSITIONS(this);
                    window_select_position.set_code(code);
                    if ((positions & 0x1) > 0)
                    {
                        window_select_position.set_left(1);
                    }
                    if ((positions & 0x2) > 0)
                    {
                        window_select_position.set_left(2);
                    }
                    if ((positions & 0x4) > 0)
                    {
                        window_select_position.set_right(4);
                    }
                    if ((positions & 0x8) > 0)
                    {
                        window_select_position.set_right(8);
                    }
                }
                break;
            case game_function.MSG_ANNOUNCE_CARD:
                {
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    response_type = "annunce_card";
                    unclearable_cookie_search_card_type = r.ReadUInt32();
                    window_announce_card = new OCGCORE_ANNOUNCE_CARD(this);
                }
                break;
            case game_function.MSG_ANNOUNCE_ATTRIB:
                {
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    response_type = "annunce_attrib";
                    cookie_select_min = r.ReadByte();
                    if (select_hint == "")
                    {
                        select_hint = "请宣言属性";
                    }
                    window_announce = new OCGCORE_ANNOUNCE_TEXT(this, select_hint);
                    UInt32 available = r.ReadUInt32();
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_EARTH))
                    {
                        window_announce.add_opt("地", (int)game_attributes.ATTRIBUTE_EARTH);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_WATER))
                    {
                        window_announce.add_opt("水", (int)game_attributes.ATTRIBUTE_WATER);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_FIRE))
                    {
                        window_announce.add_opt("炎", (int)game_attributes.ATTRIBUTE_FIRE);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_WIND))
                    {
                        window_announce.add_opt("风", (int)game_attributes.ATTRIBUTE_WIND);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_LIGHT))
                    {
                        window_announce.add_opt("光", (int)game_attributes.ATTRIBUTE_LIGHT);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_DARK))
                    {
                        window_announce.add_opt("暗", (int)game_attributes.ATTRIBUTE_DARK);
                    }
                    if (card_string_helper.differ(available, (long)game_attributes.ATTRIBUTE_DEVINE))
                    {
                        window_announce.add_opt("神", (int)game_attributes.ATTRIBUTE_DEVINE);
                    }
                }
                break;
            case game_function.MSG_ANNOUNCE_NUMBER:
                {
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    response_type = "annunce_number";
                    count = r.ReadByte();
                    if (select_hint == "")
                    {
                        select_hint = "请宣言数字";
                    }
                    cookie_select_min = 1;
                    window_announce = new OCGCORE_ANNOUNCE_TEXT(this, select_hint);
                    for (int i = 0; i < count;i++ )
                    {
                        window_announce.add_opt(r.ReadUInt32().ToString(), i);
                    }
                }
                break;
            case game_function.MSG_SELECT_OPTION:
                {
                    under_ui.get_health_bar(true).set_user_light(true);
                    under_ui.get_health_bar(false).set_user_light(false);
                    response_type = "annunce_option";
                    count = r.ReadByte();
                    cookie_select_min = 1;
                    window_option = new OCGCORE_SELECT_OPTION(this);
                    for (int i = 0; i < count; i++)
                    {
                        window_option.add_opt(get_description(r.ReadUInt32()), i);
                    }
                }
                break;
            case game_function.MSG_ANNOUNCE_RACE:
                {
                    response_type = "annunce_race";
                    cookie_select_min = r.ReadByte();
                    if (select_hint == "")
                    {
                        select_hint = "请宣言种族";
                    }
                    window_announce = new OCGCORE_ANNOUNCE_TEXT(this, select_hint);
                    UInt32 available = r.ReadUInt32();
                    if (card_string_helper.differ(available, (long)game_race.RACE_WARRIOR))
                    { 
                        window_announce.add_opt("战士", (int)game_race.RACE_WARRIOR);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_SPELLCASTER)) 
                    {
                        window_announce.add_opt("魔法师", (int)game_race.RACE_SPELLCASTER);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_FAIRY)) 
                    {
                        window_announce.add_opt("天使", (int)game_race.RACE_FAIRY);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_FIEND))
                    {
                        window_announce.add_opt("恶魔", (int)game_race.RACE_FIEND);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_ZOMBIE))
                    {
                        window_announce.add_opt("不死", (int)game_race.RACE_ZOMBIE);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_MACHINE)) 
                    {
                        window_announce.add_opt("机械", (int)game_race.RACE_MACHINE);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_AQUA))
                    {
                        window_announce.add_opt("水", (int)game_race.RACE_AQUA);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_PYRO)) 
                    {
                        window_announce.add_opt("炎", (int)game_race.RACE_PYRO);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_ROCK)) 
                    {
                        window_announce.add_opt("岩石", (int)game_race.RACE_ROCK);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_WINDBEAST))
                    {
                        window_announce.add_opt("鸟兽", (int)game_race.RACE_WINDBEAST);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_PLANT))
                    {
                        window_announce.add_opt("植物", (int)game_race.RACE_PLANT);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_INSECT))
                    {
                        window_announce.add_opt("昆虫", (int)game_race.RACE_INSECT);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_THUNDER))
                    {
                        window_announce.add_opt("雷", (int)game_race.RACE_THUNDER);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_DRAGON))
                    {
                        window_announce.add_opt("龙", (int)game_race.RACE_DRAGON);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_BEAST)) 
                    {
                        window_announce.add_opt("兽", (int)game_race.RACE_BEAST);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_BEASTWARRIOR)) 
                    {
                        window_announce.add_opt("兽战士", (int)game_race.RACE_BEASTWARRIOR);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_DINOSAUR))
                    {
                        window_announce.add_opt("恐龙", (int)game_race.RACE_DINOSAUR);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_FISH))
                    {
                        window_announce.add_opt("鱼", (int)game_race.RACE_FISH);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_SEASERPENT))
                    {
                        window_announce.add_opt("海龙", (int)game_race.RACE_SEASERPENT);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_REPTILE)) 
                    {
                        window_announce.add_opt("爬虫", (int)game_race.RACE_REPTILE);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_PSYCHO))
                    {
                        window_announce.add_opt("念动力", (int)game_race.RACE_PSYCHO);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_DEVINE)) 
                    {
                        window_announce.add_opt("幻神兽", (int)game_race.RACE_DEVINE);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_CREATORGOD))
                    {
                        window_announce.add_opt("创造神", (int)game_race.RACE_CREATORGOD);
                    }
                    if (card_string_helper.differ(available, (long)game_race.RACE_PHANTOMDRAGON))
                    {
                        window_announce.add_opt("幻龙", (int)game_race.RACE_PHANTOMDRAGON);
                    }
                }
                break;
            case game_function.MSG_NEW_TURN:
                {
                    sound_player.Play("nextturn", 1f);
                    game_field.animation_show_big_string("New Turn");
                    its_my_turn = if_me(r.ReadByte());
                    if (!under_ui.get_if_show_all()) go_to_min_camera();
                }
                break;
            case game_function.MSG_NEW_PHASE:
                {
                    sound_player.Play("phase", 1f);
                    int phrase = r.ReadUInt16();
                    set_now(card_string_helper.get_phase_string(phrase));
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_BATTLE))
                    {
                        game_field.animation_show_big_string("Battle Phase");
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_DRAW))
                    {
                        game_field.animation_show_big_string("Draw Phase");
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_END))
                    {
                        game_field.animation_show_big_string("End Phase");
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_MAIN1))
                    {
                        game_field.animation_show_big_string("Main Phase one");
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_MAIN2))
                    {
                        game_field.animation_show_big_string("Main Phase two");
                    }
                    if (card_string_helper.differ(phrase, (long)game_phrases.PHASE_STANDBY))
                    {
                        game_field.animation_show_big_string("Standby Phase");
                    }
                }
                break;
            case game_function.MSG_HINT_SELECTMSG:
                {
                    select_hint = get_description(r.ReadUInt32());
                }
                break;
        }
        message.Params.reader.BaseStream.Seek(0, 0);
    }

    private void resize_location(bool tag_swap_me, game_location location_for_resize, int size_now)
    {
        int zize_before = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == location_for_resize)
            {
                if (cards[i].p.me == tag_swap_me)
                {
                    zize_before++;
                    cards[i].set_data(card_data_manager.GetById(0));
                }
            }
        }
        if (zize_before < size_now)
        {
            for (int i = 0; i < size_now - zize_before; i++)
            {
                point p = new point();
                p.me = tag_swap_me;
                p.location = location_for_resize;
                p.index = zize_before + i;
                p.overlay_index = 0;
                p.position = game_position.POS_FACEDOWN_ATTACK;
                create_card(p);
            }
        }

        if (zize_before > size_now)
        {
            for (int i = 0; i < size_now - zize_before; i++)
            {
                point p = new point();
                p.me = tag_swap_me;
                p.location = location_for_resize;
                p.index = zize_before - i - 1;
                p.overlay_index = 0;
                p.position = game_position.POS_FACEDOWN_ATTACK;
                point pp = new point();
                pp.me = tag_swap_me;
                pp.location = location_for_resize;
                pp.index = 0;
                pp.overlay_index = 0;
                pp.position = game_position.POS_FACEDOWN_ATTACK;
                move_card(p, pp);
            }
        }
    }

    private void lpchanged_to_bgm()
    {
        int lp_me=under_ui.get_health_bar(true).get_user_life();
        int lp_op=under_ui.get_health_bar(false).get_user_life();
        if (lp_me - lp_op > 4000)
        {
            client.setting.i_am(true);
        }

        if (lp_op -  lp_me> 4000)
        {
            client.setting.i_am(false);
        }
    }

    private GameObject card_race_to_attack_effect_mod(OCGCORE_CARD card,Vector3 prewarm_world_point,int prewarm_atk)
    {
        GameObject mod = loader.mod_ocgcore_bs_atk_line_earth;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
            mod = loader.mod_ocgcore_bs_atk_line_earth;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WATER))
            mod = loader.mod_ocgcore_bs_atk_line_water;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
            mod = loader.mod_ocgcore_bs_atk_line_fire;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WIND))
            mod = loader.mod_ocgcore_bs_atk_line_wind;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DARK))
            mod = loader.mod_ocgcore_bs_atk_line_dark;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
            mod = loader.mod_ocgcore_bs_atk_line_light;
        if (card_string_helper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
            mod = loader.mod_ocgcore_bs_atk_line_light;
        Vector3 scr = client.camera_game_main.WorldToScreenPoint(prewarm_world_point);
       // scr.z = 5f * 1800f / (float)prewarm_atk;
        mod.transform.GetChild(1).localPosition = client.camera_game_main.ScreenToWorldPoint(scr);
        return mod;
    }

    private void animation_shuffle_deck(bool me)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == game_location.LOCATION_DECK && cards[i].p.me == me)
            {
                Vector3 position;
                if (me)
                {
                    position = new Vector3(0, 0, -15);
                }
                else
                {
                    position = new Vector3(0, 0, 15);
                }
                if(i%2==0)cards[i].animation_shake_to(1.2f);
                cards[i].set_data(card_data_manager.GetById(0));
            }
        }
    }

    private void animation_shuffle_hand(bool me)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].p.location == game_location.LOCATION_HAND && cards[i].p.me == me)
            {
                Vector3 position;
                if (me)
                {
                    position = new Vector3(0, 0, -15);
                }
                else
                {
                    position = new Vector3(0, 0, 15);
                }
                cards[i].animation_rush_to(position, new Vector3(270, 0, 0));
                if(me==false)cards[i].set_data(card_data_manager.GetById(0));
            }
        }
    }

    private void confirm_card(OCGCORE_CARD target)
    {
        Vector3 position = camera_game_main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 8f));
        target.animation_confirm_to(position, new Vector3(60, 0, 0),0.53f,0.23f);
    }

    private void animation_screen_blood(bool me, int amount)
    {
        if (me)
        {
            iTween.ShakePosition(camera_game_main.gameObject, iTween.Hash(
                "x", (float)amount / 1500f,
                "y", (float)amount / 1500f,
                "z", (float)amount / 1500f,
                "onupdate", (Action)fix_camera,
                "time", (float)amount / 2500f
                ));
            GameObject obj_ = create_game_object(loader.mod_ocgcore_blood_screen, Vector3.zero, Quaternion.identity);
            obj_.AddComponent<animation_screen_lock>().screen_point =
                new Vector3(
                    Screen.width / 2f,
                    100f,
                    0.5f + 4000f / (float)amount);
            kill_game_object(obj_, 2.5f);
            for (int i = 0; i < (int)amount / 1000; i++)
            {
                GameObject obj = create_game_object(loader.mod_ocgcore_blood_screen, Vector3.zero, Quaternion.identity);
                obj.AddComponent<animation_screen_lock>().screen_point =
                    new Vector3(
                        (float)Screen.width / (float)UnityEngine.Random.Range(10, 30) * 10f,
                        (float)Screen.height / (float)UnityEngine.Random.Range(10, 30) * 10f,
                        0.5f + 4000f / (float)amount);
                kill_game_object(obj, 2.5f);
            }

        }
    }

    Action duel_end = null;
    internal void set_end(Action d)
    {
        duel_end = d;
    }
}

