using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class CLIENT
{
    #region net

    public TcpClient tcpClient = null;

    public NetworkStream networkStream = null;

    private void networkInitialize()
    {
        try
        {
            string ip = System.IO.File.ReadAllText("ip.txt");
            string port = System.IO.File.ReadAllText("port.txt");
            tcpClient = new TcpClient();
            tcpClient.Connect(ip,int.Parse(port));
            networkStream = tcpClient.GetStream();
            Thread thread = new Thread(new ThreadStart(net_thread));
            thread.IsBackground = true;
            thread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void net_thread()
    {
        while(is_running){
            byte[] hdr = networkStreamReadFull(networkStream, 2);
            int plen = BitConverter.ToUInt16(hdr, 0);
            byte[] head = networkStreamReadFull(networkStream, 1);
            byte[] buf = new byte[0];
            if (plen>1)
            {
                buf = networkStreamReadFull(networkStream, plen - 1);
            }
            HASH_MESSAGE message = new HASH_MESSAGE();
            message.Fuction = head[0];
            message.Params = new BYTE_HELPER(buf);
            lock (message_to_be_handled)
            {
                message_to_be_handled.Add(message);
            }
        }
    }

    byte[] networkStreamReadFull(NetworkStream stream, int length)
    {
        var buf = new byte[length];
        int rlen = 0;
        while (rlen < buf.Length && is_running)
        {
            rlen += stream.Read(buf, rlen, buf.Length - rlen);
        }
        return buf;
    }

    public void sendGameMessgae(HASH_MESSAGE message)
    {
        try
        {
            //byte[] params_byte = message.Params.get();
            //networkStream.Write(BitConverter.GetBytes((UInt16)params_byte.Length + 1), 0, 2);
            //networkStream.Write(BitConverter.GetBytes((byte)CtosMessage.Response), 0, 1);
            //networkStream.Write(params_byte, 0, params_byte.Length);
            MemoryStream m = new MemoryStream();
            byte[] params_byte = message.Params.get();
            //m.Write(BitConverter.GetBytes((UInt16)params_byte.Length + 1), 0, 2);
            m.Write(BitConverter.GetBytes((byte)CtosMessage.Response), 0, 1);
            m.Write(params_byte, 0, params_byte.Length);
            send_duel_buffer_for_ygo_3d(m.ToArray());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void sendMessgae(HASH_MESSAGE message)
    {
        try
        {
            //byte[] params_byte = message.Params.get();
            //networkStream.Write(BitConverter.GetBytes((UInt16)params_byte.Length + 1), 0, 2);
            //networkStream.Write(BitConverter.GetBytes((byte)message.Fuction), 0, 1);
            //networkStream.Write(params_byte, 0, params_byte.Length);

            MemoryStream m = new MemoryStream();
            byte[] params_byte = message.Params.get();
           // m.Write(BitConverter.GetBytes((UInt16)params_byte.Length + 1), 0, 2);
            m.Write(BitConverter.GetBytes((byte)message.Fuction), 0, 1);
            m.Write(params_byte, 0, params_byte.Length);
            send_duel_buffer_for_ygo_3d(m.ToArray());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    #region ygo_3d_net

    public string ip = "121.42.62.14";

    public void networkInitialize_for_ygo_3d()
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ip, 11001);
            networkStream = tcpClient.GetStream();
            bool result=HELPER_TCP.Handshaking(networkStream, 12345678987654321);
            Debug.Log("Handshaking:"+result);
            if (result)
            {
                debuger_show_bool=false;
            }
            Thread thread = new Thread(new ThreadStart(net_thread_for_ygo_3d));
            thread.IsBackground = true;
            thread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public List<byte[]> buffers = new List<byte[]>();

    private void net_thread_for_ygo_3d()
    {
        while (is_running && tcpClient.Connected)
        {
            var sad = HELPER_TCP.ReadPacket(networkStream);
            lock (buffers)
            {
                buffers.Add(sad);
            }
        }
    }

    public void send_buffer_for_ygo_3d(byte[] buffer)
    {
        try
        {
            HELPER_TCP.WritePacket(networkStream, buffer);

        }
        catch (Exception)
        {
            
        }
    }

    public void send_duel_buffer_for_ygo_3d(byte[] buffer)
    {
        var p = protos.Public.Types.Cts.Types.Duel.CreateBuilder();
        p.Data = Google.ProtocolBuffers.ByteString.CopyFrom(buffer);
        send_buffer_for_ygo_3d(HELPER_TCP.Warper(p.Build()));

        //HELPER_TCP.WritePacket(networkStream, buffer);
    }
    #endregion

    //public CLIENT_SERVANT_OCGCORE servant_ocgcore_debug;
    public CLIENT_SERVANT_DECKMANAGER DeckMaster;
    public CLIENT_SERVANT_ROOM Room;
    public CLIENT_SERVANT_LOGIN Login;
    public CLIENT_SERVANT_DATING Dating;

    public SHANGCHENG shangcheng;


    private DEBUGER debuger;
    public loader loader;
    public bool is_running = true;
    public int time = Environment.TickCount;
    public GameObject pointed_game_object=null;
    public GameObject preview_pointed_game_object = null;
    public bool left_mouse_button_is_down = false;
    public bool preview_left_mouse_button_is_down = false;
    public float mouse_wheel_change_value = 0f;
    private Vector3 pre_mouse_position = Vector3.zero;

    public CardDataManager card_data_manager;
    public string_reader string_data_manager;
    public PictureLoader picture_loader;
    public Lflist_manager lflist_manager;
    public SETTING setting;
    int fliter=0;
    bool debuger_show_bool = false;

    bool is_in_sharp = false;

    public string name = "";

    public CLIENT(loader l)
    {
        loader = l;

        if (is_in_sharp)
        {
            networkInitialize();
        }
        else
        {
            networkInitialize_for_ygo_3d();
        }
      
        ini_cameras();
        card_data_manager = new CardDataManager();
        string_data_manager = new string_reader("strings.txt");
        picture_loader = new PictureLoader(this);
        lflist_manager = new Lflist_manager();
        //servant_ocgcore_debug = new CLIENT_SERVANT_OCGCORE(this);
        //DeckMaster = new CLIENT_SERVANT_DECKMANAGER(this);
        Room = new CLIENT_SERVANT_ROOM(this);
        setting = new SETTING(this);
        //Dating = new CLIENT_SERVANT_DATING(this);
       // shangcheng = new SHANGCHENG(this);
        if(debuger_show_bool)
        {
            debuger = new DEBUGER(this);
        }

        if (is_in_sharp == false)
        {
            Login = new CLIENT_SERVANT_LOGIN(this);
        }

        fit_screen();
        for (int i = 0; i < 32;i++ )
        {
            if(i==15){
                continue;
            }
            fliter |= (int)Math.Pow(2, i);
        }
       debug_script();
    }

    private void debug_script()
    {
        if(is_in_sharp)
        {
            Room.Link();
            Room.show_all();
        }
    }


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

    public void ini_cameras()
    {
        camera_game_main = loader.main_camera;

        ui_back_ground_2d = create_game_object(loader.mod_ui_2d, Vector3.zero, Quaternion.identity);
        camera_back_ground_2d = ui_back_ground_2d.transform.FindChild("Camera").GetComponent<Camera>();
        camera_back_ground_2d.depth = -2;
        ui_back_ground_2d.layer = 8;
        ui_back_ground_2d.transform.FindChild("Camera").gameObject.layer = 8;
        camera_back_ground_2d.cullingMask = (int)Math.Pow(2, 8);


        ui_container_3d = create_game_object(loader.mod_ui_3d, Vector3.zero, Quaternion.identity);
        camera_container_3d = ui_container_3d.transform.FindChild("Camera").GetComponent<Camera>();
        camera_container_3d.depth = -1;
        ui_container_3d.layer = 9;
        ui_container_3d.transform.FindChild("Camera").gameObject.layer = 9;
        camera_container_3d.cullingMask = (int)Math.Pow(2, 9);
        camera_container_3d.fieldOfView = camera_game_main.fieldOfView;
        camera_container_3d.rect = camera_game_main.rect;
        camera_container_3d.transform.localPosition = camera_game_main.transform.position;

        camera_game_main = loader.main_camera;
        camera_game_main.depth = 0;
        camera_game_main.gameObject.layer = 0;

        ui_main_2d = create_game_object(loader.mod_ui_2d, Vector3.zero, Quaternion.identity);
        camera_main_2d = ui_main_2d.transform.FindChild("Camera").GetComponent<Camera>();
        camera_main_2d.depth = 1;
        ui_main_2d.layer = 10;
        ui_main_2d.transform.FindChild("Camera").gameObject.layer = 10;
        camera_main_2d.cullingMask = (int)Math.Pow(2, 10);

        ui_main_3d = create_game_object(loader.mod_ui_3d, Vector3.zero, Quaternion.identity);
        camera_main_3d = ui_main_3d.transform.FindChild("Camera").GetComponent<Camera>();
        camera_main_3d.depth = 2;
        ui_main_3d.layer = 11;
        ui_main_3d.transform.FindChild("Camera").gameObject.layer = 11;
        camera_main_3d.cullingMask = (int)Math.Pow(2, 11);
        camera_main_3d.fieldOfView = camera_game_main.fieldOfView;
        camera_main_3d.rect = camera_game_main.rect;
        camera_main_3d.transform.localPosition = camera_game_main.transform.position;

        back_ground_texture = create_game_object(loader.mod_simple_ngui_background_texture, Vector3.zero, Quaternion.identity, ui_back_ground_2d);
        FileStream file = new FileStream("desk.jpg", FileMode.Open, FileAccess.Read);
        file.Seek(0, SeekOrigin.Begin);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        file.Close();
        file.Dispose();
        file = null;

        Texture2D pic = new Texture2D(1024, 600);
        pic.LoadImage(data);
        back_ground_texture.GetComponent<UITexture>().mainTexture = pic;
        back_ground_texture.GetComponent<UITexture>().depth = -100;

    }

    public GameObject create_game_object(GameObject mod, Vector3 position, Quaternion quaternion, GameObject ui = null)
    {
        GameObject ob = (GameObject)MonoBehaviour.Instantiate(mod, position, quaternion);
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

    public int screen_fixed_time = 0;

    public void fit_screen()
    {
        try
        {
            UIRoot root = ui_back_ground_2d.GetComponent<UIRoot>();
            float s = root.activeHeight / Screen.height;
            var tex = back_ground_texture.GetComponent<UITexture>().mainTexture;
            float ss = (float)tex.height / (float)tex.width;
            int width = (int)(Screen.width * s);
            int height = (int)(width * ss);
            if (height < Screen.height)
            {
                height = (int)(Screen.height * s);
                width = (int)(height / ss);
            }
            back_ground_texture.GetComponent<UITexture>().height = height;
            back_ground_texture.GetComponent<UITexture>().width = width;
        }
        catch (Exception e)
        {
            debugger.Log(e);
        }
        if(DeckMaster!=null)
        {
            DeckMaster.fit_screen();
        }
        //if (servant_ocgcore_debug!=null)
        //{
        //    servant_ocgcore_debug.fit_screen();
        //}
        if (setting != null)
        {
            setting.fit_screen();
        }
        if (Room != null)
        {
            Room.fit_screen();
        }
        if (Dating != null)
        {
            Dating.fit_screen();
        }
    }

    List<HASH_MESSAGE> message_to_be_handled=new List<HASH_MESSAGE>();

    public void update()
    {
        lock (message_to_be_handled)
        {
            for (int i = 0; i < message_to_be_handled.Count; i++)
            {
                get_message(message_to_be_handled[i]);
            }
            message_to_be_handled.Clear();
        }
        if (buffers.Count>0)
        {
            lock (buffers)
            {
                for (int i = 0; i < buffers.Count; i++)
                {
                    get_buffer(buffers[i]);
                }
                buffers.Clear();
            }
        }
        if (screen_fixed_time<10)
        {
            fit_screen();
            screen_fixed_time++;
        }
        time = Environment.TickCount;
        /////mouse_pointed_object
        pointed_game_object = null;
        Ray line = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(line, out hit, (float)1000, fliter))
        {
            pointed_game_object = hit.collider.gameObject;
        }
        GameObject hoverobject = UICamera.Raycast(Input.mousePosition) ? UICamera.lastHit.collider.gameObject : null;
        if (hoverobject != null)
        {
            //debugger.Log("ui event");
            pointed_game_object = hoverobject;
        }
        /////mouse_position
        left_mouse_button_is_down=Input.GetMouseButton(0);
        ///servant_update
        //if (servant_ocgcore_debug!=null) servant_ocgcore_debug.update();
        if (DeckMaster != null) DeckMaster.update();
        if (setting != null) setting.update();
        if (debuger != null) debuger.update();
        if (Room != null) Room.update();
        if (Dating != null) Dating.update();
        //pre
        preview_pointed_game_object = pointed_game_object;
        preview_left_mouse_button_is_down = left_mouse_button_is_down;
        ///
        
        mouse_wheel_change_value = Input.GetAxis("Mouse ScrollWheel") * 50;

        if (mouse_wheel_change_value < -10 || mouse_wheel_change_value>10)
        {
            mouse_wheel_change_value = 0;
        }
        pre_mouse_position = Input.mousePosition;
    }

    private void get_buffer(byte[] p)
    {

        var gilgamesh = protos.Gilgamesh.ParseFrom(p);

        string type = gilgamesh.Type;

        Debug.Log(type);

        if (type == "protos.Public.Stc.Duel")
        {
            Dating.hide();
            var message = protos.Public.Types.Stc.Types.Duel.ParseFrom(gilgamesh.Data);
            var data = message.Data.ToByteArray();
            byte[] hashed_data = new byte[data.Length - 1];
            for (int i = 0; i < hashed_data.Length; i++)
            {
                hashed_data[i] = data[i + 1];
            }
            HASH_MESSAGE hashed_message = new HASH_MESSAGE();
            hashed_message.Fuction = data[0];
            hashed_message.Params = new BYTE_HELPER(hashed_data);
            lock (message_to_be_handled)
            {
                message_to_be_handled.Add(hashed_message);
            }
           
        }


        if (type == "protos.Public.Stc.LoginResponse")
        {
            var message = protos.Public.Types.Stc.Types.LoginResponse.ParseFrom(gilgamesh.Data);
            var state = message.State;
            var reason = message.Reason;
        }

        if (type == "protos.Public.Chat")
        {
            var message = protos.Public.Types.Chat.ParseFrom(gilgamesh.Data);
            Dating.liaotian.GetComponent<lazy_chat>().chat_list.Add(message.From + ":" + message.Msg);
        }

        if (type == "protos.Public.Stc.Player.QueryResponse")
        {
            //var message = protos.Public.Types.Stc.Types.Player.Types.QueryResponse.ParseFrom(gilgamesh.Data);
        }

        if (type == "protos.Public.Stc.Hall.YouEnterHall")
        {
            var message = protos.Public.Types.Stc.Types.Hall.Types.YouEnterHall.ParseFrom(gilgamesh.Data);
            Login.kill_oneself();
            Login = null;
            Dating = new CLIENT_SERVANT_DATING(this);
            Dating.fit_screen();
        }

        if (type == "protos.Public.Stc.Hall.RoomList")
        {
            var message = protos.Public.Types.Stc.Types.Hall.Types.RoomList.ParseFrom(gilgamesh.Data);
            for (int i = 0; i < message.RoomList_List.Count;i++ )
            {
                to_room(message.RoomList_List[i]);
            }
        }
        if (type == "protos.Public.Stc.Hall.RoomDestoried")
        {
            var message = protos.Public.Types.Stc.Types.Hall.Types.RoomDestoried.ParseFrom(gilgamesh.Data);
            Dating.del_one_room((int)message.Id);
        }
        if (type == "protos.Public.Stc.Hall.RoomStateChanged")
        {
            var message = protos.Public.Types.Stc.Types.Hall.Types.RoomStateChanged.ParseFrom(gilgamesh.Data);
            to_room(message.Room);
        }

        if (type == "protos.Public.Stc.Hall.RoomCreated")
        {
            var message = protos.Public.Types.Stc.Types.Hall.Types.RoomCreated.ParseFrom(gilgamesh.Data);
           
            to_room(message.Room);
        }
        

    }

    private void to_room(protos.Public.Types.Room room)
    {
        string name = "";
        name += room.Option.Name;
        if (room.State == protos.Public.Types.RoomState.Duel)
        {
            name += " 决斗中";
        }
        else
        {
            name += " 正在等待";
        }
        name += "\r\n";
        for (int i = 0; i < room.PlayersCount; i++)
        {
            name += room.PlayersList[i] + " ";
        }
        Dating.to_a_room((int)room.Id, name);
        Debug.Log("Dating.to_a_room((int)message.Room.Id, name);");
    }




    public void get_message(HASH_MESSAGE message)
    {
        try
        {
            if (Room != null)
            {
                Room.get_message(message);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
}
