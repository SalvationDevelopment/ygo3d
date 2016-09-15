using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class CLIENT_SERVANT_LOGIN
{
    lazy_login login_window;
    lazy_regist regist_window;
    CLIENT father;
    public CLIENT_SERVANT_LOGIN(CLIENT f)
    {
        father = f;
        login_window = father.create_game_object(father.loader.mod_login_window, Vector3.zero, Quaternion.identity, father.ui_main_2d).GetComponent<lazy_login>();
        
        Delegater temp;
        temp = login_window.login.gameObject.AddComponent<Delegater>();
        temp.f = on_click_login;
        login_window.login.onClick.Add(new EventDelegate(temp, "function"));

        temp = login_window.regist.gameObject.AddComponent<Delegater>();
        temp.f = on_click_regist;
        login_window.regist.onClick.Add(new EventDelegate(temp, "function"));

        temp = login_window.check_login.gameObject.AddComponent<Delegater>();
        temp.f = on_click_auto;
        login_window.check_login.onChange.Add(new EventDelegate(temp, "function"));

        temp = login_window.check_remember.gameObject.AddComponent<Delegater>();
        temp.f = on_click_remember;
        login_window.check_remember.onChange.Add(new EventDelegate(temp, "function"));



        regist_window = father.create_game_object(father.loader.mod_regist_window, Vector3.zero, Quaternion.identity, father.ui_main_2d).GetComponent<lazy_regist>();
        regist_window.transform.position = new Vector3(10000,10000,0);

        GiveButtonFunction(regist_window.end, regist_on_quxiao);
        GiveButtonFunction(regist_window.regist, regist_on_zhuce);

        on_start();
    }



    private static Delegater GiveButtonFunction(UIButton btn, CLIENT_SERVANT_OCGCORE.refresh_function function)
    {
        Delegater temp;
        temp = btn.gameObject.AddComponent<Delegater>();
        temp.f = function;
        btn.onClick.Add(new EventDelegate(temp, "function"));
        return temp;
    }



    public void kill_oneself()
    {
        iTween.MoveTo(login_window.gameObject,new Vector3(0,-1000,0),1f);
        MonoBehaviour.Destroy(login_window.gameObject,1f);
    }

    void on_click_login()
    {
        if (father.tcpClient.Connected == false)
        {
            father.networkInitialize_for_ygo_3d();
        }
        File.WriteAllText("config_name.txt", login_window.user_name.value);
        File.WriteAllText("config_psw.txt", login_window.user_psw.value);
        father.send_buffer_for_ygo_3d(HELPER_TCP.GetLoginBuffer(login_window.user_name.value, login_window.user_psw.value));

        father.name = login_window.user_name.value;
    }

    void on_click_regist()
    {
        login_window.transform.position = new Vector3(10000, 10000, 0);
        regist_window.transform.position = new Vector3(0, 0, 0);
    }

    void regist_on_quxiao()
    {
        login_window.transform.position =  new Vector3(0, 0, 0);
        regist_window.transform.position = new Vector3(10000, 10000, 0);
    }

    void regist_on_zhuce()
    {
        if (regist_window.psw1.value == regist_window.psw2.value)
        {
            if (father.tcpClient.Connected == false)
            {
                father.networkInitialize_for_ygo_3d();
            }
            string name = regist_window.name.value;
            string psw = regist_window.psw1.value;
            TcpClient client = new TcpClient();
            client.Connect(father.ip,11000);
            var bu=protos.Public.Types.Cts.Types.Register.CreateBuilder();
            bu.Account = name;
            bu.Password = Google.ProtocolBuffers.ByteString.CopyFrom(HELPER_TCP.HashString(psw));
            byte[] buffer = bu.Build().ToByteArray();
            NetworkStream stream = client.GetStream();
            if (HELPER_TCP.Handshaking(stream, 12345678987654321))
            {
                HELPER_TCP.WritePacket(stream, buffer);
                byte[] huiying = HELPER_TCP.ReadPacket(stream);
                var h = protos.Public.Types.Stc.Types.LoginResponse.ParseFrom(huiying);
                if (h.State == true)
                {
                    regist_on_quxiao();
                }
            }
           
        }
    }

    void on_click_remember()
    {
        if (login_window.check_remember.value)
        {
            File.WriteAllText("config_check_remember.txt", "1");
        }
        else
        {
            File.WriteAllText("config_check_remember.txt", "0");
        }
    }

    void on_click_auto()
    {
        if (login_window.check_login.value)
        {
            File.WriteAllText("config_check_auto_login.txt", "1");
        }
        else
        {
            File.WriteAllText("config_check_auto_login.txt", "0");
        }
    }

    void on_start()
    {
        if (File.Exists("config_name.txt"))
        {
            string value = File.ReadAllText("config_name.txt");
            login_window.user_name.value = value;
        }
        if (File.Exists("config_psw.txt"))
        {
            string value = File.ReadAllText("config_psw.txt");
            login_window.user_psw.value = value;
        }
        if (File.Exists("config_check_remember.txt"))
        {
            string value = File.ReadAllText("config_check_remember.txt");
            if (value[0]=='0')
            {
                login_window.check_remember.value = false;
            }
            else
            {
                login_window.check_remember.value = true;
            }
        }
        if (File.Exists("config_check_auto_login.txt"))
        {
            string value = File.ReadAllText("config_check_auto_login.txt");
            if (value[0] == '0')
            {
                login_window.check_login.value = false;
            }
            else
            {
                login_window.check_login.value = true;
            }
        }

        if (login_window.check_login.value)
        {
            father.send_buffer_for_ygo_3d(HELPER_TCP.GetLoginBuffer(login_window.user_name.value, login_window.user_psw.value));
            father.name = login_window.user_name.value;
        }


    }

}

