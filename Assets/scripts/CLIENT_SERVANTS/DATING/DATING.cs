using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CLIENT_SERVANT_DATING
{
    private CLIENT client;
   public GameObject liaotian;
    GameObject dating;



    List<GameObject> all_objects = new List<GameObject>();

    public delegate void refresh_function();

    public List<refresh_function> refresh_functions = new List<refresh_function>();


    public CLIENT_SERVANT_DATING(CLIENT client)
    {
        this.client = client;

        initialize();
        debug();
    }


    void on_chat()
    {
        string send_str = liaotian.GetComponent<lazy_chat>().input.value;
        var p = protos.Public.Types.Chat.CreateBuilder();
        p.From = client.name;
        p.Msg = send_str;
        client.send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));
        liaotian.GetComponent<lazy_chat>().input.value = "";
    }


    private void initialize()
    {
        dating = create_game_object(client.loader.mod_dating_window,Vector3.zero,Quaternion.identity,client.ui_main_2d);
        
        dating.transform.localScale = Vector3.zero;
        iTween.ScaleTo(dating,new Vector3(1,1,1),1f);

        liaotian = create_game_object(client.loader.mod_dating_chat, new Vector3(1000, 0, 0), Quaternion.identity, client.ui_main_2d);
        refresh_functions.Add(refresh);


        liaotian.GetComponent<lazy_chat>().face.mainTexture = ui_helper.get_rand_face();
        liaotian.GetComponent<lazy_chat>().jianjie.text = "用户名："+client.name+"\r\n等级：1\r\nDB:1";

        GiveButtonFunction(liaotian.GetComponent<lazy_chat>().send, on_chat);
        Delegater temp = liaotian.GetComponent<lazy_chat>().input.gameObject.AddComponent<Delegater>();
        temp.f = on_chat;
        liaotian.GetComponent<lazy_chat>().input.onSubmit.Add(new EventDelegate(temp, "function"));


        //dating.GetComponent<lazy_room_ui>().deck_edit
        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().deck_edit, on_click_edit);
        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().pipei, on_click_create_single_room);
        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().new_room_singel,on_click_create_single_room);
        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().new_room_match, on_click_create_match_room);
        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().new_room_tag, on_click_create_tag_room);

        GiveButtonFunction(dating.GetComponent<lazy_room_ui>().shop, on_click_shangcheng);
        dating.GetComponent<lazy_room_ui>().deck_list.Clear();
        DirectoryInfo TheFolder = new DirectoryInfo("deck");
        FileInfo[] fileInfo = TheFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)
        {
            string name = NextFile.Name;
            if (name.Length > 4)
            {
                if (name.Substring(name.Length - 4, 4) == ".ydk")
                {
                    dating.GetComponent<lazy_room_ui>().deck_list.AddItem(name.Substring(0, name.Length - 4));
                    dating.GetComponent<lazy_room_ui>().deck_list.value = name.Substring(0, name.Length - 4);
                }
            }
        }
    }

    void on_click_shangcheng()
    {
        client.shangcheng = new SHANGCHENG(client);
    }

    void on_click_create_single_room()
    {
        var p = protos.Public.Types.Cts.Types.Hall.Types.CreateRoom.CreateBuilder();
        var option = protos.Public.Types.RoomOption.CreateBuilder();
        option.Name = "单局房间";
        option.Password = "";
        option.Lp =8000;
        option.LFList = 0;
        option.HandCount = 5;
        option.DrawCount = 1;
        option.Mode = protos.Public.Types.RoomMode.Single;
        option.Rule = protos.Public.Types.RoomRule.OcgTcg;
        p.Option = option.Build();
        client.send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));
    }
    void on_click_create_match_room()
    {
        var p = protos.Public.Types.Cts.Types.Hall.Types.CreateRoom.CreateBuilder();
        var option = protos.Public.Types.RoomOption.CreateBuilder();
        option.Name = "比赛房间";
        option.Password = "";
        option.Lp = 8000;
        option.LFList = 0;
        option.HandCount = 5;
        option.DrawCount = 1;
        option.Mode = protos.Public.Types.RoomMode.Match;
        option.Rule = protos.Public.Types.RoomRule.OcgTcg;
        p.Option = option.Build();
        client.send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));
    }
    void on_click_create_tag_room()
    {
        var p = protos.Public.Types.Cts.Types.Hall.Types.CreateRoom.CreateBuilder();
        var option = protos.Public.Types.RoomOption.CreateBuilder();
        option.Name = "tag房间";
        option.Password = "";
        option.Lp = 16000;
        option.LFList = 0;
        option.HandCount = 5;
        option.DrawCount = 1;
        option.Mode = protos.Public.Types.RoomMode.Tag;
        option.Rule = protos.Public.Types.RoomRule.OcgTcg;
        p.Option = option.Build();
        client.send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));
    }



    private static Delegater GiveButtonFunction(UIButton btn, CLIENT_SERVANT_OCGCORE.refresh_function function)
    {
        Delegater temp;
        temp = btn.gameObject.AddComponent<Delegater>();
        temp.f = function;
        btn.onClick.Add(new EventDelegate(temp, "function"));
        return temp;
    }

    void on_click_edit()
    {
        if (client.DeckMaster == null)
        {
            client.DeckMaster = new CLIENT_SERVANT_DECKMANAGER(client, false);
            client.DeckMaster.set_return_function(on_edit_return);
            client.DeckMaster.fit_screen();
            client.DeckMaster.from_deck_to_field(client.DeckMaster.from_ydk_to_deck("deck\\" + dating.GetComponent<lazy_room_ui>().deck_list.value + ".ydk"), true);
        }
        hide();
    }

    void on_edit_return()
    {
        client.DeckMaster.from_deck_to_ydk(client.DeckMaster.from_field_to_deck(), "deck\\" + dating.GetComponent<lazy_room_ui>().deck_list.value + ".ydk");
        client.DeckMaster.kill_oneself();
        client.DeckMaster = null;
        fit_screen();
    }

    private void refresh()
    {

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
    }

    public void fit_screen()
    {
        iTween.MoveTo(dating,client.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2 - 175, Screen.height / 2+10, 0)),1f);
        liaotian.GetComponent<lazy_chat>().pic.width = 360;
        liaotian.GetComponent<lazy_chat>().pic.height = Screen.height-10;
        iTween.MoveTo(liaotian,client.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width-170,Screen.height/2,0)),1f);
        dating.GetComponent<lazy_room_ui>().under_pic.width = Screen.width - 450;
        dating.GetComponent<lazy_room_ui>().under_pic.height = Screen.height - 120;
    }

    public void hide()
    {
        iTween.MoveTo(dating, client.camera_main_2d.ScreenToWorldPoint(new Vector3(-Screen.width -10000, Screen.height / 2 + 10, 0)), 1f);
        iTween.MoveTo(liaotian, client.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width +1000, Screen.height / 2, 0)), 1f);
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



    public class hashed_room_on_list
    {
        public string name = "";
        public int ptr;
        public GameObject object_;
    }

    List<hashed_room_on_list> rooms = new List<hashed_room_on_list>();

    public void add_one_room(hashed_room_on_list room)
    {
        rooms.Add(room);
    }

    public void del_one_room(int id)
    {
        hashed_room_on_list r = null;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].ptr == id)
            {
                r = rooms[i];
            }
        }
        kill_game_object(r.object_);
        rooms.Remove(r);
    }

    public void to_a_room(int ptr, string name)
    {
        hashed_room_on_list a_room = null;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].ptr == ptr)
            {
                a_room = rooms[i];
                a_room.name = name;
            }
        }
        if (a_room == null)
        {
            hashed_room_on_list one_room = new hashed_room_on_list();
            one_room.name = name;
            one_room.ptr = ptr;
            add_one_room(one_room);
        }
        refresh_all_room();
    }

    public void refresh_all_room()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].object_==null)
            {
                rooms[i].object_ = create_game_object(client.loader.mod_room_in_dating_window, Vector3.zero, Quaternion.identity,client.ui_main_2d);
                rooms[i].object_.transform.SetParent(dating.GetComponent<lazy_room_ui>().gird.transform,false);
                rooms[i].object_.name = rooms[i].ptr.ToString();
                var aadsa=rooms[i].object_.AddComponent<nengdiadefangjian>();
                aadsa.client = client;
                aadsa.room = rooms[i];
                aadsa.dating = this;
                aadsa.button = rooms[i].object_.transform.FindChild("under_button");
            }
        }
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].object_.transform.FindChild("under_button").GetComponent<UISprite>().width = Screen.width-450;
            rooms[i].object_.transform.localPosition = new Vector3((Screen.width - 450) / 2, 175f / 600f * (float)Screen.height - i * 50, 0);
            rooms[i].object_.transform.FindChild("Label").GetComponent<UILabel>().text = rooms[i].name;
        }
        //dating.GetComponent<lazy_room_ui>().sv.mScroll = 0;
        //dating.GetComponent<lazy_room_ui>().sv.Scroll(0.01f);
        //dating.GetComponent<lazy_room_ui>().sv.UpdatePosition();
    }

    public void room_clicked(int ptr)
    {
        for (int i = 0; i < rooms.Count;i++ )
        {
            if (rooms[i].object_!=null)
            {
                if (rooms[i].object_.name == ptr.ToString())
                {
                    var p = protos.Public.Types.Cts.Types.Hall.Types.EnterRoom.CreateBuilder();
                    p.Id = (ulong)rooms[i].ptr;
                    p.Password = "";
                    client.send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));
                }
            }
        }
    }

    void debug()
    {
        //hashed_room_on_list one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //refresh_all_room();
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //one_room = new hashed_room_on_list();
        //one_room.name = "babababbabababbababaababaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        //one_room.ptr = 10010;
        //add_one_room(one_room);
        //refresh_all_room();
    }
}

