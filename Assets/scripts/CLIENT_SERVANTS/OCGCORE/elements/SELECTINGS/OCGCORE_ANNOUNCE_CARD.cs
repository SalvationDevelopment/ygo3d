using System;
using System.Collections.Generic;
using UnityEngine;
public class OCGCORE_ANNOUNCE_CARD
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject game_object;
    public UIInput input;
    public UIButton button;
    public OCGCORE_ANNOUNCE_CARD(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        game_object = father.create_game_object(father.client.loader.mod_ocgcore_search_cards, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        game_object.transform.localScale = new Vector3(0, 0, 1);
        father.refresh_functions.Add(handler);
        input = game_object.transform.FindChild("input").GetComponent<UIInput>();
        button = game_object.transform.FindChild("text_button").GetComponent<UIButton>();

        Delegater temp = button.gameObject.AddComponent<Delegater>();
        temp.f = clicked;
        button.onClick.Add(new EventDelegate(temp, "function"));
    }
    public void clicked()
    {
        father.search_button_clicked();
    }
    public void kill_oneself()
    {
        iTween.ScaleTo(game_object, Vector3.zero, 0.6f);
        father.kill_game_object(game_object, 0.6f);
        father.refresh_functions.Remove(handler);
    }
    void handler()
    {
        game_object.transform.localScale += (new Vector3(1, 1, 1) - game_object.transform.localScale) * 0.3f;
    }
}

