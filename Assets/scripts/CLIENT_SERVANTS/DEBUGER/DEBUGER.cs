using System;
using System.Collections.Generic;
using UnityEngine;
public class DEBUGER
{
    CLIENT father;
    GameObject game_object_windows;
    UITextList UITextList_output;
    UIInput UIInput_input;
    public DEBUGER(CLIENT f)
    {
        father = f;
        game_object_windows = father.create_game_object(father.loader.mod_ui_debbger, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        UITextList_output = game_object_windows.transform.FindChild("Chat Area").GetComponent<UITextList>();


        UIInput_input = game_object_windows.transform.FindChild("Chat Input").GetComponent<UIInput>();
        Delegater temp = UIInput_input.gameObject.AddComponent<Delegater>();
        temp.f = on_submit;
        UIInput_input.onSubmit.Add(new EventDelegate(temp, "function"));

    }

    void on_submit()
    {
        //add_line(UIInput_input.value);
        handle(UIInput_input.value);
        UIInput_input.value = "";
    }

    void handle(string str)
    {
        if (str=="a")
        {
            string user_name = "miao";
            string user_pw = "123456";
            father.send_buffer_for_ygo_3d(HELPER_TCP.GetLoginBuffer(user_name, user_pw));
        }
        if (str == "b")
        {
            var room_option = protos.Public.Types.RoomOption.CreateBuilder();
            room_option.Name = "miaowu";
            room_option.Password = "";
            room_option.Lp = 8000;
            room_option.HandCount = 5;
            room_option.DrawCount = 1;
            room_option.LFList = 0;
            room_option.Mode = protos.Public.Types.RoomMode.Single;
            room_option.Rule = protos.Public.Types.RoomRule.Ocg;

            var CreateRoom = protos.Public.Types.Cts.Types.Hall.Types.CreateRoom.CreateBuilder();
            CreateRoom.Option = room_option.Build();

            string type = "protos.Public.Cts.Hall.CreateRoom";
            byte[] data = CreateRoom.Build().ToByteArray();
            father.send_buffer_for_ygo_3d(HELPER_TCP.WrappeBuffer(type, data));
        }
    }


    public void add_line(string ph)
    {
        UITextList_output.Add(ph);
    }

    public List<byte[]> buffers = new List<byte[]>();

    public void update()
    {
        if (buffers.Count > 0)
        {
            lock (buffers)
            {
                foreach (byte[] message in buffers)
                {
                    debug_buffer(message);
                }
                buffers.Clear();
            }
        }
    }


    void debug_buffer(byte[] buffer)
    {
        var gilgamesh = protos.Gilgamesh.ParseFrom(buffer);
        if (gilgamesh.Type == "protos.Public.Stc.LoginResponse")
        {
            var message=protos.Public.Types.Stc.Types.LoginResponse.ParseFrom(gilgamesh.Data);
            var state = message.State;
            var reason = message.Reason;
            add_line("ok");
        }
        if (gilgamesh.Type == "protos.Public.Stc.Resource.GetLFListResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Resource.GetLFListDataResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Resource.UploadAvatarResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Resource.GetAvatarResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Player")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Player.NeedCreatePlayer")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Player.QueryResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Player.CreateResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Player.ModifyResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Videotape")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Videotape.VideoTapeList")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Videotape.VideoTapeData")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Deck")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Deck.UploadResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Deck.DownloadResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Deck.RemoveResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Deck.QueryListResponse")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.RoomList")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.RoomCreated")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.RoomDestoried")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.RoomStateChanged")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.YouEnterHall")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.YouLeaveHall")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.Room.YouBeKick")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.Room.RoomStateChanged")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.Room.YouEnterRoom")
        {

        }
        if (gilgamesh.Type == "protos.Public.Stc.Hall.Room.YouLeaveRoom")
        {

        }
        
    }
}

