using System;
using System.Collections;
using System.IO;
using UnityEngine;
public class OCGCORE_SOUND_PLAYER
{
    CLIENT_SERVANT_OCGCORE father;
    public OCGCORE_SOUND_PLAYER(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
    }
    public void Play(string p,float val)
    {
        string path = "sound/" + p + ".mp3";
        if (File.Exists(path) == false)
        {
            path = "sound/" + p + ".wav";
        }
        if (File.Exists(path) == false)
        {
            path = "sound/" + p + ".ogg";
        }
        if (File.Exists(path) == false)
        {
            return;
        }
        path = Environment.CurrentDirectory.Replace("\\","/")+"/" + path;
        path = "file:///" + path;
        GameObject audio_helper = father.create_game_object(father.client.loader.mod_audio_effect,Vector3.zero,Quaternion.identity);
        audio_helper.GetComponent<audio_helper>().play(path, val*father.client.setting.UISlider_eff.value);
        father.kill_game_object(audio_helper,20f);
    }
}

