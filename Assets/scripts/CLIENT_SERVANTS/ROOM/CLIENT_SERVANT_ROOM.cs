using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CLIENT_SERVANT_ROOM
{
    public CLIENT client;

    List<GameObject> all_objects = new List<GameObject>();

    public delegate void refresh_function();

    public List<refresh_function> refresh_functions = new List<refresh_function>();

    GameObject gameObjectUIbuttons;

    UIButton UIButton_setting;

    Vector3 vector_for_UIButton_setting=Vector3.zero;

    UIInput chat_input;

    UILabel printed_logs;

    GameObject LazyRoom = null;

    lazy_room LazyRoom_ui = null;

    Vector3 vector_for_window = Vector3.zero;

    UIButton gg_btn;

    bool i_can_edit_deck = true;

    public CLIENT_SERVANT_ROOM(CLIENT c)
    {
        client = c;
        
    }

    private void startScript()
    {
        if (LazyRoom == null)
        {
            temp_delegate_prep = false;
            count_watch = 0;
            refresh_functions.Add(refresh);
            Delegater temp;
            gameObjectUIbuttons = create_game_object(client.loader.mod_room_ui_button, new Vector3(1000, -1000, 0), Quaternion.identity, client.ui_back_ground_2d);
            UIButton_setting = gameObjectUIbuttons.transform.FindChild("setting").GetComponent<UIButton>();
            var btn = UIButton_setting;
            var function = (CLIENT_SERVANT_OCGCORE.refresh_function)on_setting_clicked;
            GiveButtonFunction(UIButton_setting, on_setting_clicked);
            chat_input = gameObjectUIbuttons.transform.FindChild("talk_input").GetComponent<UIInput>();
            temp = chat_input.gameObject.AddComponent<Delegater>();
            temp.f = on_chat;
            chat_input.onSubmit.Add(new EventDelegate(temp, "function"));
            gg_btn = gameObjectUIbuttons.transform.FindChild("gg").GetComponent<UIButton>();
            gg_btn.transform.localScale = Vector3.zero;
            GiveButtonFunction(gg_btn, on_gg);
            printed_logs = create_game_object(client.loader.mod_simple_ngui_text, Vector3.zero, Quaternion.identity, client.ui_back_ground_2d).GetComponent<UILabel>();
            LazyRoom = create_game_object(client.loader.mod_room_window, new Vector3(0, 1000, 0), Quaternion.identity, client.ui_main_2d);
            LazyRoom_ui = LazyRoom.GetComponent<lazy_room>();
            temp = LazyRoom_ui.btn_deck_edit.gameObject.AddComponent<Delegater>();
            temp.f = on_edit_deck;
            LazyRoom_ui.btn_deck_edit.onClick.Add(new EventDelegate(temp, "function"));
            LazyRoom_ui.list_deck.Clear();
            DirectoryInfo TheFolder = new DirectoryInfo("deck");
            FileInfo[] fileInfo = TheFolder.GetFiles();
            foreach (FileInfo NextFile in fileInfo)
            {
                string name = NextFile.Name;
                if (name.Length > 4)
                {
                    if (name.Substring(name.Length - 4, 4) == ".ydk")
                    {
                        LazyRoom_ui.list_deck.AddItem(name.Substring(0, name.Length - 4));
                        LazyRoom_ui.list_deck.value = name.Substring(0, name.Length - 4);
                    }
                }
            }
            LazyRoom_ui.btn_start.enabled = false;
            LazyRoom_ui.label_start.gradientTop = Color.gray;
            LazyRoom_ui.btn_start.name = "0";
            LazyRoom_ui.player_A.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_A.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_A.transform.FindChild("Sprite").GetComponent<UIButton>(), delegate_player_A);
            LazyRoom_ui.player_A_kick.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_A_kick.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_A_kick, delegate_player_A_kick);
            LazyRoom_ui.player_A_prep.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_A_prep.name = "0";
            LazyRoom_ui.player_B.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_B.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_B.transform.FindChild("Sprite").GetComponent<UIButton>(), delegate_player_B);
            LazyRoom_ui.player_B_kick.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_B_kick.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_B_kick, delegate_player_B_kick);
            LazyRoom_ui.player_B_prep.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_B_prep.name = "0";
            LazyRoom_ui.player_C.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_C.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_C.transform.FindChild("Sprite").GetComponent<UIButton>(), delegate_player_C);
            LazyRoom_ui.player_C_kick.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_C_kick.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_C_kick, delegate_player_C_kick);
            LazyRoom_ui.player_C_prep.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_C_prep.name = "0";
            LazyRoom_ui.player_D.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_D.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_D.transform.FindChild("Sprite").GetComponent<UIButton>(), delegate_player_D);
            LazyRoom_ui.player_D_kick.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_D_kick.name = "0";
            GiveButtonFunction(LazyRoom_ui.player_D_kick, delegate_player_D_kick);
            LazyRoom_ui.player_D_prep.transform.localScale = Vector3.zero;
            LazyRoom_ui.player_D_prep.name = "0";
            GiveButtonFunction(LazyRoom_ui.btn_start, delegate_start);
            GiveButtonFunction(LazyRoom_ui.btn_change, delegate_change);
            GiveButtonFunction(LazyRoom_ui.btn_exit, delegate_exit);
            GiveButtonFunction(LazyRoom_ui.btn_prep, delegate_prep);
        }
        LazyRoom_ui.label_prep.text = "准备完毕";
        temp_delegate_prep = false;
        CtosMessage_HsNotReady();
        LazyRoom_ui.list_deck.enabled = true;
        LazyRoom_ui.btn_start.enabled = false;
        LazyRoom_ui.label_start.gradientTop = Color.gray;
        LazyRoom_ui.btn_start.name = "0";
        LazyRoom_ui.player_A.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_A.name = "0";
        LazyRoom_ui.player_A_kick.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_A_kick.name = "0";
        LazyRoom_ui.player_A_prep.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_A_prep.name = "0";
        LazyRoom_ui.player_B.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_B.name = "0";
        LazyRoom_ui.player_B_kick.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_B_kick.name = "0";
        LazyRoom_ui.player_B_prep.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_B_prep.name = "0";
        LazyRoom_ui.player_C.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_C.name = "0";
        LazyRoom_ui.player_C_kick.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_C_kick.name = "0";
        LazyRoom_ui.player_C_prep.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_C_prep.name = "0";
        LazyRoom_ui.player_D.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_D.name = "0";
        LazyRoom_ui.player_D_kick.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_D_kick.name = "0";
        LazyRoom_ui.player_D_prep.transform.localScale = Vector3.zero;
        LazyRoom_ui.player_D_prep.name = "0";
    }

    private void on_gg()
    {
        if(OCGCORE!=null)
        {
            OCGCORE.clear_all_cookies();
        }
        CtosMessage_Surrender();
    }

    private static Delegater GiveButtonFunction(UIButton btn, CLIENT_SERVANT_OCGCORE.refresh_function function)
    {
        Delegater temp;
        temp = btn.gameObject.AddComponent<Delegater>();
        temp.f = function;
        btn.onClick.Add(new EventDelegate(temp, "function"));
        return temp;
    }

    GameObject get_player(int i)
    {
        GameObject return_value = null;
        if (tag_mod)
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_A;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_B;
            }
            if (i == 2)
            {
                return_value = LazyRoom_ui.player_C;
            }
            if (i == 3)
            {
                return_value = LazyRoom_ui.player_D;
            }
        }
        else
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_B;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_C;
            }
        }
        return return_value;
    }

    UIButton get_player_kick(int i)
    {
        UIButton return_value = null;
        if (tag_mod)
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_A_kick;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_B_kick;
            }
            if (i == 2)
            {
                return_value = LazyRoom_ui.player_C_kick;
            }
            if (i == 3)
            {
                return_value = LazyRoom_ui.player_D_kick;
            }
        }
        else
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_B_kick;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_C_kick;
            }
        }
        return return_value;
    }

    UITexture get_player_face(int i)
    {
        UITexture return_value = null;
        if (tag_mod)
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_A_face;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_B_face;
            }
            if (i == 2)
            {
                return_value = LazyRoom_ui.player_C_face;
            }
            if (i == 3)
            {
                return_value = LazyRoom_ui.player_D_face;
            }
        }
        else
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_B_face;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_C_face;
            }
        }
        return return_value;
    }

    GameObject get_player_prep(int i)
    {
        GameObject return_value = null;
        if (tag_mod)
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_A_prep;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_B_prep;
            }
            if (i == 2)
            {
                return_value = LazyRoom_ui.player_C_prep;
            }
            if (i == 3)
            {
                return_value = LazyRoom_ui.player_D_prep;
            }
        }
        else
        {
            if (i == 0)
            {
                return_value = LazyRoom_ui.player_B_prep;
            }
            if (i == 1)
            {
                return_value = LazyRoom_ui.player_C_prep;
            }
        }
        return return_value;
    }

    void delegate_player_A()
    {
        if(i_am_the_host)
        {
            shift_kick_btn(LazyRoom_ui.player_A_kick);
        }
    }

    private static void shift_kick_btn(UIButton target)
    {
        if (target.name == "0")
        {
            target.name = "1";
            iTween.ScaleTo(target.gameObject, new Vector3(0.48f, 0.48f, 0.48f), 0.6f);
        }
        else
        {
            target.name = "0";
            iTween.ScaleTo(target.gameObject, new Vector3(0, 0, 0), 0.6f);
        }
    }

    void delegate_player_B()
    {
        if (i_am_the_host)
        {
            shift_kick_btn(LazyRoom_ui.player_B_kick);
        }
    }

    void delegate_player_C()
    {
        if (i_am_the_host)
        {
            shift_kick_btn(LazyRoom_ui.player_C_kick);
        }
    }

    void delegate_player_D()
    {
        if (i_am_the_host)
        {
            shift_kick_btn(LazyRoom_ui.player_D_kick);
        }
    }

    void delegate_player_A_kick()
    {
        if(tag_mod)
        {
            CtosMessage_HsKick(0);
        }
        else
        {
        }
    }

    void delegate_player_B_kick()
    {
        if (tag_mod)
        {
            CtosMessage_HsKick(1);
        }
        else
        {
            CtosMessage_HsKick(0);
        }
    }

    void delegate_player_C_kick()
    {
        if (tag_mod)
        {
            CtosMessage_HsKick(2);
        }
        else
        {
            CtosMessage_HsKick(1);
        }
    }

    void delegate_player_D_kick()
    {
        if (tag_mod)
        {
            CtosMessage_HsKick(3);
        }
        else
        {
        }
    }

    void delegate_start()
    {
        CtosMessage_HsStart();
    }

    void delegate_change()
    {
        CtosMessage_HsToDuelist();
    }

    void delegate_exit()
    {
        CtosMessage_LeaveGame();
    }

    bool temp_delegate_prep = false;

    void delegate_prep()
    {
        if (temp_delegate_prep)
        {
            LazyRoom_ui.label_prep.text = "准备完毕";
            temp_delegate_prep = false;
            CtosMessage_HsNotReady();
            LazyRoom_ui.list_deck.enabled = true;
        }
        else
        {
            LazyRoom_ui.label_prep.text = "取消准备";
            temp_delegate_prep = true;
            CtosMessage_UpdateDeck( from_ydk_to_deck("deck\\" + LazyRoom_ui.list_deck.value + ".ydk"));
            CtosMessage_HsReady();
            LazyRoom_ui.list_deck.enabled = false;
        }
        
    }

    void kill_oneself()
    {
        refresh_functions.Clear();
        for (int i = 0; i < all_objects.Count; i++)
        {
            MonoBehaviour.Destroy(all_objects[i]);
        }
    }

    public void update()
    {
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
        if (OCGCORE != null)
        {
            OCGCORE.update();
        }
    }

    public void fit_screen()
    {
        switch (room_window_condition)  
        {
            case ROOM_WINDOW_CONDITION.all_showed:
                vector_for_UIButton_setting = client.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width - 50, 50, 0));
                vector_for_window = new Vector3(0, 0, 0);
                break;
            case ROOM_WINDOW_CONDITION.only_window_hided:
                vector_for_UIButton_setting = client.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width - 50, 50, 0));
                vector_for_window = client.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height*2, 0));
                break;
            case ROOM_WINDOW_CONDITION.all_hide:
                vector_for_UIButton_setting = client.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width - 50, -100, 0));
                vector_for_window = client.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height*2, 0));
                break;
            default:
                break;
        }
        if(OCGCORE!=null){
            OCGCORE.fit_screen();
        }
    }

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

    public void kill_game_object(GameObject game_object, float time = 0)
    {
        MonoBehaviour.Destroy(game_object, time);
        all_objects.Remove(game_object);
        game_object = null;
    }

    private void on_setting_clicked()
    {
        client.setting.show(true);
    }

    private void on_chat()
    {
        CtosMessage_Chat(chat_input.value);
        chat_input.value = "";
    }

    private void on_edit_deck()
    {
        if (i_can_edit_deck)
        {
            if(client.DeckMaster==null)
            {
                client.DeckMaster = new CLIENT_SERVANT_DECKMANAGER(client,false);
                client.DeckMaster.set_return_function(on_edit_end);
                client.DeckMaster.fit_screen();
                client.DeckMaster.from_deck_to_field(client.DeckMaster.from_ydk_to_deck("deck\\"+LazyRoom_ui.list_deck.value+".ydk"),true);
            }
            hide_all();
        }
    }

    private void on_edit_end()
    {
        client.DeckMaster.from_deck_to_ydk(client.DeckMaster.from_field_to_deck(), "deck\\" + LazyRoom_ui.list_deck.value + ".ydk");
        client.DeckMaster.kill_oneself();
        client.DeckMaster = null;
        show_all();
    }

    public void Link()
    {
        string name = System.IO.File.ReadAllText("name.txt");
        CtosMessage_PlayerInfo(name);
        CtosMessage_JoinGame();
    }

    public enum ROOM_WINDOW_CONDITION
    {
        all_showed=1,
        only_window_hided=2,
        all_hide=3,
    }

    private ROOM_WINDOW_CONDITION room_window_condition = ROOM_WINDOW_CONDITION.all_hide;

    public void show_all()
    {
        room_window_condition = ROOM_WINDOW_CONDITION.all_showed;
        fit_screen();
    }

    public void hide_all()
    {
        room_window_condition = ROOM_WINDOW_CONDITION.all_hide;
        fit_screen();
    }

    public void hide_only_window()
    {
        room_window_condition = ROOM_WINDOW_CONDITION.only_window_hided;
        fit_screen();
    }

    void refresh()
    {
        gameObjectUIbuttons.transform.position += (vector_for_UIButton_setting - gameObjectUIbuttons.transform.position) * 0.3f;
        LazyRoom.transform.position += (vector_for_window - LazyRoom.transform.position) * 0.1f;
        delog();
    }

    private void CtosMessage_Surrender()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.Surrender;
        client.sendMessgae(message);
    }

    private void CtosMessage_HsToDuelist()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsToDuelist;
        client.sendMessgae(message);
    }

    private void CtosMessage_HsToObserver()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsToObserver;
        client.sendMessgae(message);
    }

    private void CtosMessage_LeaveGame()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.LeaveGame;
        client.sendMessgae(message);
    }

    private void CtosMessage_HsReady()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsReady;
        client.sendMessgae(message);
    }

    private void CtosMessage_HsStart()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsStart;
        client.sendMessgae(message);
    }

    private void CtosMessage_HsNotReady()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsNotReady;
        client.sendMessgae(message);
    }

    private void CtosMessage_Response(byte[] response)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.Response;
        message.Params.writer.Write(response);
        client.sendMessgae(message);
    }

    DECK_MANAGER_DECK from_ydk_to_deck(string path)
    {
        DECK_MANAGER_DECK return_value = new DECK_MANAGER_DECK();
        List<CardData> right_ini = new List<CardData>();
        try
        {
            string text = System.IO.File.ReadAllText(path);
            string st = text.Replace("\r", "");
            string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int flag = -1;
            foreach (string line in lines)
            {
                if (line == "#main")
                {
                    flag = 1;
                }
                else if (line == "#extra")
                {
                    flag = 2;
                }
                else if (line == "!side")
                {
                    flag = 3;
                }
                else
                {
                    int code = 0;
                    try
                    {
                        code = Int32.Parse(line);
                    }
                    catch (Exception)
                    {

                    }
                    if (code > 100)
                    {
                        switch (flag)
                        {
                            case 1:
                                {
                                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                                    card.code = code;
                                    return_value.main.Add(card);
                                }
                                break;
                            case 2:
                                {
                                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                                    card.code = code;
                                    return_value.extra.Add(card);
                                }
                                break;
                            case 3:
                                {
                                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                                    card.code = code;
                                    return_value.side.Add(card);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            debugger.Log(e);
        }
        return return_value;
    }

    private void CtosMessage_UpdateDeck(DECK_MANAGER_DECK deck)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.UpdateDeck;
        message.Params.writer.Write((int)deck.main.Count + deck.extra.Count);
        message.Params.writer.Write((int)deck.side.Count);
        for (int i = 0; i < deck.main.Count; i++)
        {
            message.Params.writer.Write((int)deck.main[i].code);
        }
        for (int i = 0; i < deck.extra.Count; i++)
        {
            message.Params.writer.Write((int)deck.extra[i].code);
        }
        for (int i = 0; i < deck.side.Count; i++)
        {
            message.Params.writer.Write((int)deck.side[i].code);
        }
        client.sendMessgae(message);
    }

    private void CtosMessage_TpResult(bool tp)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.TpResult;
        if (tp)
        {
            message.Params.writer.Write((byte)1);
        }
        else
        {
            message.Params.writer.Write((byte)0);
        }
        client.sendMessgae(message);
    }

    private void CtosMessage_HandResult(int res)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HandResult;
        message.Params.writer.Write((byte)res);
        client.sendMessgae(message);
    }

    private void CtosMessage_HsKick(int pos)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.HsKick;
        message.Params.writer.Write((byte)pos);
        client.sendMessgae(message);
    }

    private void CtosMessage_Chat(string str)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.Chat;
        message.Params.writer.WriteUnicode(str, str.Length+1);
        client.sendMessgae(message);
    }

    private void CtosMessage_JoinGame()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.JoinGame;
        client.sendMessgae(message);
    }

    private void CtosMessage_PlayerInfo(string name)
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Fuction = (int)CtosMessage.PlayerInfo;
        message.Params.writer.WriteUnicode(name, 20);
        client.sendMessgae(message);
    }

    public void get_message(HASH_MESSAGE message)
    {
        StocMessage function_index = (StocMessage)message.Fuction;
        BinaryReader r = message.Params.reader;
        switch (function_index)
        {
            case StocMessage.GameMsg:
                StocMessage_GameMsg(message);
                break;
            case StocMessage.ErrorMsg:
                StocMessage_ErrorMsg(r);
                break;
            case StocMessage.SelectHand:
                StocMessage_SelectHand();
                break;
            case StocMessage.SelectTp:
                StocMessage_SelectTp();
                break;
            case StocMessage.HandResult:
                StocMessage_HandResult(r);
                break;
            case StocMessage.TpResult:
                break;
            case StocMessage.ChangeSide:
                StocMessage_ChangeSide(r);
                break;
            case StocMessage.WaitingSide:
                StocMessage_WaitingSide(r);
                break;
            case StocMessage.CreateGame:
                break;
            case StocMessage.JoinGame:
                StocMessage_JoinGame(r);
                break;
            case StocMessage.TypeChange:
                StocMessage_TypeChange(r);
                break;
            case StocMessage.LeaveGame:
                break;
            case StocMessage.DuelStart:
                StocMessage_DuelStart();
                break;
            case StocMessage.DuelEnd:
                StocMessage_DuelEnd();
                break;
            case StocMessage.Replay:
                StocMessage_Replay(message);
                break;
            case StocMessage.TimeLimit:
                StocMessage_TimeLimit(r);
                break;
            case StocMessage.Chat:
                StocMessage_Chat(message, r);
                break;
            case StocMessage.HsPlayerEnter:
                StocMessage_HsPlayerEnter(r);
                break;
            case StocMessage.HsPlayerChange:
                StocMessage_HsPlayerChange(r);
                break;
            case StocMessage.HsWatchChange:
                StocMessage_HsWatchChange(r);
                break;
            case StocMessage.jinglai:
                Link();
                show_all();
                break;
            case StocMessage.chuqu:
                hide_all();

                if(OCGCORE!=null)
                {
                    OCGCORE.kill_oneself();
                    OCGCORE = null;
                }

                if (client.DeckMaster != null)
                {
                    client.DeckMaster.kill_oneself();
                    client.DeckMaster = null;
                    show_all();
                    hide_only_window();
                }

                client.Dating.fit_screen();
                break;
            default:
                break;
        }
    }

    int count_watch = 0;

    private void StocMessage_HsWatchChange(BinaryReader r)
    {
        count_watch = r.ReadUInt16();
        LazyRoom_ui.room_rule_description.text = description + "\r\n观战者：" + count_watch + "人";
    }

    private void StocMessage_HsPlayerChange(BinaryReader r)
    {
        byte data = r.ReadByte();
        int player = data >> 4;
        int desc = data & 0xf;
        if (desc<8)
        {
            GameObject player_btn = get_player(player);
            string name = player_btn.transform.FindChild("Label").GetComponent<UILabel>().text;
            iTween.ScaleTo(get_player_kick(player).gameObject, new Vector3(0, 0, 0), 0.6f);
            iTween.ScaleTo(player_btn, new Vector3(0,0,0), 1f);
            player_btn.name = "0";

            player_btn = get_player(desc);
            player_btn.transform.FindChild("Label").GetComponent<UILabel>().text = name;
            get_player_face(desc).mainTexture = ui_helper.get_rand_face();
            iTween.ScaleTo(player_btn, new Vector3(0.8f, 0.8f, 0.8f), 1f);
            player_btn.name = "1";
        }
        else
        {
            PlayerChange condition = (PlayerChange)(desc);
            GameObject player_btn;
            GameObject player_prep;
            string name;
            switch (condition)
            {
                case PlayerChange.Observe:
                    count_watch++;
                    LazyRoom_ui.room_rule_description.text = description + "\r\n观战者：" + count_watch + "人";
                    player_btn = get_player(player);
                    name = player_btn.transform.FindChild("Label").GetComponent<UILabel>().text;
                    iTween.ScaleTo(get_player_kick(player).gameObject, new Vector3(0, 0, 0), 0.6f);
                    iTween.ScaleTo(player_btn, new Vector3(0, 0, 0), 1f);
                    player_btn.name = "0";
                    break;
                case PlayerChange.Ready:
                    player_prep = get_player_prep(player);
                    iTween.ScaleTo(player_prep,new Vector3(1,1,1),0.6f);
                    player_prep.name = "1";
                    break;
                case PlayerChange.NotReady:
                     player_prep = get_player_prep(player);
                    iTween.ScaleTo(player_prep,new Vector3(0,0,0),0.6f);
                    player_prep.name = "0";
                    break;
                case PlayerChange.Leave:
                     player_btn = get_player(player);
                    name = player_btn.transform.FindChild("Label").GetComponent<UILabel>().text;
                    iTween.ScaleTo(get_player_kick(player).gameObject, new Vector3(0, 0, 0), 0.6f);
                    iTween.ScaleTo(player_btn, new Vector3(0, 0, 0), 1f);
                    player_btn.name = "0";
                    break;
                default:
                    break;
            }
        }
        
    }

    private void StocMessage_HsPlayerEnter(BinaryReader r)
    {
        string name = r.ReadUnicode(20);
        int index = r.ReadByte();
        GameObject player_btn = get_player(index);
        player_btn.transform.FindChild("Label").GetComponent<UILabel>().text = name;
        iTween.ScaleTo(player_btn,new Vector3(0.8f,0.8f,0.8f),1f);
        get_player_face(index).mainTexture = ui_helper.get_rand_face();
        player_btn.name = "1";
    }

    private void StocMessage_Chat(HASH_MESSAGE message, BinaryReader r)
    {
        int player = r.ReadInt16();
        int length = message.Params.getLength();
        string str = r.ReadUnicode(length);
        log(str);
    }

    int last_log_time = 0;

    List<string> logs = new List<string>();

    public void log(string str)
    {
        last_log_time = client.time;
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
        printed_logs.text = att;
        printed_logs.width = Screen.width - 700;
        Vector3 vector = new Vector3(0,0,0);
        vector.y = printed_logs.height / 2;
        vector.x = printed_logs.width / 2 + 230;
        printed_logs.transform.position = client.camera_back_ground_2d.ScreenToWorldPoint(vector);
    }

    void delog()
    {
        if (client.time - last_log_time > 4000)
        {
            last_log_time = client.time;
            if (logs.Count > 0)
            {
                logs.RemoveAt(0);
            }
            string att = "";
            foreach (string ss in logs)
            {
                att += ss + "\n";
            }
            printed_logs.text = att;
            printed_logs.width = Screen.width - 700;
            Vector3 vector = new Vector3(0, 0, 0);
            vector.y = printed_logs.height / 2;
            vector.x = printed_logs.width / 2 + 230;
            printed_logs.transform.position = client.camera_back_ground_2d.ScreenToWorldPoint(vector);
        }
    }

    private void StocMessage_TimeLimit(BinaryReader r)
    {
        int player = r.ReadByte();
        r.ReadByte();
        int time_limit = r.ReadInt16();
    }

    private void StocMessage_Replay(HASH_MESSAGE message)
    {
        byte[] data = message.Params.get();
    }

    private void StocMessage_DuelEnd()
    {
    }

    private void StocMessage_DuelStart()
    {
        hide_only_window();
        iTween.ScaleTo(gg_btn.gameObject,new Vector3(0.5f,0.5f,1),1f);
        if (client.DeckMaster != null)
        {
            client.DeckMaster.kill_oneself();
            client.DeckMaster = null;
            show_all();
            hide_only_window();
        }
    }

    public bool i_am_the_host = false;

    private void StocMessage_TypeChange(BinaryReader r)
    {
        int data = r.ReadByte();
        if (data >= 16)
        {
            i_am_the_host = true;
            data -= 16;
        }
        else
        {
            i_am_the_host = false;
        }
        if (i_am_the_host)
        {
            i_am_the_host = true;
            LazyRoom_ui.btn_start.name = "1";
            LazyRoom_ui.btn_start.enabled = true;
            LazyRoom_ui.label_start.gradientTop = Color.white;
            LazyRoom_ui.label_start.text = "开始游戏";
        }
        else
        {
            i_am_the_host = false;
            LazyRoom_ui.btn_start.name = "0";
            LazyRoom_ui.btn_start.enabled = false;
            LazyRoom_ui.label_start.gradientTop = Color.gray;
            LazyRoom_ui.label_start.text = "正在等待";
        }
    }

    public bool tag_mod = false;

    string description = "";

    private void StocMessage_JoinGame(BinaryReader r)
    {
        startScript();
        string Banlist = r.ReadUnicode(20);
        int rule = r.ReadByte();
        int Mode = r.ReadByte();
        bool EnablePriority = r.ReadBoolean();
        bool NoCheckDeck = r.ReadBoolean();
        bool NoShuffleDeck = r.ReadBoolean();
        r.ReadByte();
        r.ReadByte();
        r.ReadByte();
        int StartLp = r.ReadInt32();
        int StartHand = r.ReadByte();
        int DrawCount = r.ReadByte();
        int Time_limit = r.ReadInt16();
        description = "";
        description += "禁限卡表:" + Banlist + "\r\n";
        if (rule == 0)
        {
            description += "OCG卡池\r\n";
        }
        if (rule == 1)
        {
            description += "TCG卡池\r\n";
        }
        if (rule == 2)
        {
            description += "混合卡池\r\n";
        }
        if (Mode == 0)
        {
            description += "单局模式\r\n";
            LazyRoom_ui.tag_kuang.SetActive(false);
            tag_mod = false;
        }
        if (Mode == 1)
        {
            description += "比赛模式\r\n";
            LazyRoom_ui.tag_kuang.SetActive(false);
            tag_mod = false;
        }
        if (Mode == 2)
        {
            description += "双打模式\r\n";
            tag_mod = true;
        }
        if (EnablePriority)
        {
            description += "允许优先权\r\n";
        }
        if (NoCheckDeck)
        {
            description += "不检查卡组\r\n";
        }
        if (NoShuffleDeck)
        {
            description += "不洗牌\r\n";
        }
        description += "生命值:" + StartLp + "\r\n";
        description += "手牌数:" + StartHand + "\r\n";
        description += "抽卡数:" + DrawCount + "\r\n";
        description += "时间限制:" + Time_limit + "秒";
        LazyRoom_ui.room_rule_description.text = description;
    }

    private void StocMessage_WaitingSide(BinaryReader r)
    {

    }

    bool need_side = false;
    private void StocMessage_ChangeSide(BinaryReader r)
    {
        need_side = true;
    }

    void ChangeSide_response()
    {
        var deck=client.DeckMaster.from_field_to_deck();
        CtosMessage_UpdateDeck(deck);

    }

    private void StocMessage_HandResult(BinaryReader r)
    {
        int hand_result_0 = r.ReadByte();
        int hand_result_1 = r.ReadByte();
        if (hand_result_0 != hand_result_1)
        {
            if ((hand_result_0 == 1 && hand_result_1 == 2) ||
                   (hand_result_0 == 2 && hand_result_1 == 3) ||
                   (hand_result_0 == 3 && hand_result_1 == 1))
            {
                GameObject tempobj = create_game_object(client.loader.mod_ocgcore_coin, Vector3.zero, Quaternion.identity);
                tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                tempobj.GetComponent<coiner>().coin_app();
                tempobj.GetComponent<coiner>().tocoin(false);
                kill_game_object(tempobj, 7);
            }
            else
            {
                GameObject tempobj = create_game_object(client.loader.mod_ocgcore_coin, Vector3.zero, Quaternion.identity);
                tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                tempobj.GetComponent<coiner>().coin_app();
                tempobj.GetComponent<coiner>().tocoin(true);
                kill_game_object(tempobj, 7);
            }
        }
    }

    private void StocMessage_SelectTp()
    {
        window_tp = new ROOM_SELECT_TP(this);
    }

    private void StocMessage_SelectHand()
    {
        UnityEngine.Random.seed = (int)Time.time;
        CtosMessage_HandResult(UnityEngine.Random.Range(1,4));
    }

    private void StocMessage_ErrorMsg(BinaryReader r)
    {
        int ErrorMsg = r.ReadByte();
        r.ReadByte();
        r.ReadByte();
        r.ReadByte();
        int result = r.ReadInt32();
        if (result>1000)
        {
            log("卡片错误，无法准备：" + client.card_data_manager.GetById(result).Name+"。");
        }
        else
        {
            log("请检查你的卡组再准备。");
        }
        LazyRoom_ui.label_prep.text = "准备完毕";
        temp_delegate_prep = false;
        CtosMessage_HsNotReady();
        LazyRoom_ui.list_deck.enabled = true;
    }

    CLIENT_SERVANT_OCGCORE OCGCORE=null;

    private void StocMessage_GameMsg(HASH_MESSAGE message)
    {
        byte[] data = message.Params.get();
        if (OCGCORE==null)
        {
            OCGCORE = new CLIENT_SERVANT_OCGCORE(client);
            OCGCORE.set_end(duel_end);
        }
        if(data.Length>0){
            MemoryStream s = new MemoryStream(data);
            BinaryReader r = new BinaryReader(s);
            while (s.Position<s.Length)
            {
                HASH_MESSAGE mes_g = new HASH_MESSAGE();
                int length = r.ReadUInt16();
                mes_g.Fuction = r.ReadByte();
                if (length>1)
                {
                    mes_g.Params = new BYTE_HELPER(r.ReadBytes(length-1));
                }
                OCGCORE.get_message(mes_g);
            }
        }
    }

    void duel_end()
    {
        OCGCORE.kill_oneself();
        OCGCORE = null;
        if(need_side){
            client.DeckMaster = new CLIENT_SERVANT_DECKMANAGER(client, true);
            client.DeckMaster.set_return_function(ChangeSide_response);
            client.DeckMaster.fit_screen();
            client.DeckMaster.from_deck_to_field(client.DeckMaster.from_ydk_to_deck("deck\\" + LazyRoom_ui.list_deck.value + ".ydk"), true);
            hide_all();
        }
        need_side = false;
    }

    ROOM_SELECT_TP window_tp = null;

    internal void tp_windows_selected(int p)
    {
        CtosMessage_TpResult(p==0);
        window_tp.kill_oneself();
        window_tp = null;
    }
}

