using System;
using System.IO;
using UnityEngine;

public class SETTING
{
    CLIENT father=null;
    GameObject window;
    public SETTING(CLIENT f)
    {
        father = f;
        window = father.create_game_object(father.loader.mod_setting, new Vector3(1000, 1000, 0), Quaternion.identity, father.ui_main_2d);
        window_vector = new Vector3(Screen.width / 2, Screen.height + 400);
        start_script();
    }
    Vector3 window_vector;
    public void update()
    {
        if (father.left_mouse_button_is_down == true && father.pointed_game_object == null)
        {
            show(false);
        }
        window.transform.position += (father.camera_main_2d.ScreenToWorldPoint(window_vector) - window.transform.position) * 0.3f;
    }
    public UIPopupList UIPopupList_music = null;
    public UIPopupList UIPopupList_screen = null;
    public audio_helper audio = null;
    public UIToggle UIToggle_change_bgm_in_duel=null;
    public UISlider UISlider_main=null;
    public UISlider UISlider_eff=null;
    void start_script()
    {
        UIPopupList_music = window.transform.FindChild("list_music").GetComponent<UIPopupList>();
        Delegater temp = UIPopupList_music.gameObject.AddComponent<Delegater>();
        temp.f = on_music_selected;
        UIPopupList_music.onChange.Add(new EventDelegate(temp, "function"));


        UIPopupList_screen = window.transform.FindChild("list_screen").GetComponent<UIPopupList>();
        temp = UIPopupList_screen.gameObject.AddComponent<Delegater>();
        temp.f = on_screen_selected;
        UIPopupList_screen.onChange.Add(new EventDelegate(temp, "function"));
        if(Screen.fullScreen)
        {
            UIPopupList_screen.value = Screen.width.ToString() + "*" + Screen.height.ToString() + ",全屏";
        }
        else
        {
            UIPopupList_screen.value = Screen.width.ToString() + "*" + Screen.height.ToString() + ",窗口";
        }



        UIToggle_change_bgm_in_duel = window.transform.FindChild("music_change_by_system").GetComponent<UIToggle>();


        UISlider_main = window.transform.FindChild("val_bgm").GetComponent<UISlider>();
        temp = UISlider_main.gameObject.AddComponent<Delegater>();
        temp.f = on_UISlider_main_changed;
        UISlider_main.onChange.Add(new EventDelegate(temp, "function"));


        UISlider_eff = window.transform.FindChild("val_eff").GetComponent<UISlider>();
        



        audio = window.transform.FindChild("backmusic").GetComponent<audio_helper>();
        DirectoryInfo TheFolder = new DirectoryInfo("bgm/main");
        FileInfo[] fileInfo = TheFolder.GetFiles();
        bool first = true;
        foreach (FileInfo NextFile in fileInfo)
        {
            UIPopupList_music.AddItem(NextFile.Name);
            if (first)
            {
                first = false;
                UIPopupList_music.value = NextFile.Name;
                on_music_selected();
            }
        }
       
    }

    public void fit_screen()
    {
        if (window_vector.y < Screen.height)
        {
            window_vector = new Vector3(Screen.width / 2, Screen.height/2);
        }
    }

    void on_UISlider_main_changed()
    {
        audio.gameObject.GetComponent<AudioSource>().volume = UISlider_main.value;
    }

    void on_music_selected()
    {
        //audio.play();
        string path = "bgm/main/" + UIPopupList_music.value;
        if (File.Exists(path) == false)
        {
            audio.close_bgm();
            return;
        }
        path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
        path = "file:///" + path;
        audio.change_bgm(path);
    }

    void on_screen_selected()
    {
        string[] lines = UIPopupList_screen.value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length==2)
        {
            string[] lines_s = lines[0].Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines_s.Length==2)
            {
                if (lines[1]=="窗口")
                {
                    Screen.SetResolution(int.Parse(lines_s[0]), int.Parse(lines_s[1]), false);
                    father.screen_fixed_time = 0;
                }
                else
                {
                    Screen.SetResolution(int.Parse(lines_s[0]), int.Parse(lines_s[1]), true);
                    father.screen_fixed_time = 0;
                }
            }
        }
    }

    void random_in_fold(string path)
    {
        DirectoryInfo TheFolder = new DirectoryInfo(path);
        FileInfo[] fileInfo = TheFolder.GetFiles();
        FileInfo NextFile ;
        if(fileInfo.Length>0){
            NextFile = fileInfo[UnityEngine.Random.Range(0,fileInfo.Length)];
            path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path + "/" + NextFile.Name;
            path = "file:///" + path;
            audio.change_bgm(path);
        }
    }

    public void i_am(bool good)
    {
        if (UIToggle_change_bgm_in_duel.value)
        {
            if (good)
            {
                random_in_fold("bgm/advantage");
            }
            else
            {
                random_in_fold("bgm/disadvantage");
            }
        }
    }

    public void show(bool show)
    {
        if (show)
        {
            window_vector = new Vector3(Screen.width / 2, Screen.height / 2);
        }
        else
        {
            window_vector = new Vector3(Screen.width / 2, Screen.height + 400);
        }
    }
}

