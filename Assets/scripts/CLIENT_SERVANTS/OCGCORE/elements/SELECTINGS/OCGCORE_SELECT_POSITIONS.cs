using UnityEngine;
public class OCGCORE_SELECT_POSITIONS
{
    CLIENT_SERVANT_OCGCORE father;
    GameObject game_object;
    public OCGCORE_SELECT_POSITIONS(CLIENT_SERVANT_OCGCORE f)
    {
        father = f;
        father.refresh_functions.Add(update);
        game_object = father.create_game_object(father.client.loader.mod_ocgcore_select_positions,Vector3.zero,Quaternion.identity,father.ui_main_2d);
        game_object.transform.localScale = new Vector3(0,0,1);


        Delegater temp = game_object.transform.FindChild("attack_button").gameObject.AddComponent<Delegater>();
        temp.f = left;
        game_object.transform.FindChild("attack_button").GetComponent<UIButton>().onClick.Add(new EventDelegate(temp, "function"));

        temp = game_object.transform.FindChild("defence_button").gameObject.AddComponent<Delegater>();
        temp.f = right;
        game_object.transform.FindChild("defence_button").GetComponent<UIButton>().onClick.Add(new EventDelegate(temp, "function"));
    }
    public void kill_oneself()
    {
        iTween.ScaleTo(game_object, Vector3.zero, 0.6f);
        father.kill_game_object(game_object,0.6f);
        father.refresh_functions.Remove(update);
    }
    long code = 0;
    bool loaded = false;
    public void set_code(long c)
    {
        code=c;
    }
    int l = 0, r = 0;
    public void set_left(int ptr)
    {
        l = ptr;
    }
    public void set_right(int ptr)
    {
        r = ptr;
    }
    public void left()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Params.writer.Write((int)l);
        father.send_message(message);
    }
    public void right()
    {
        HASH_MESSAGE message = new HASH_MESSAGE();
        message.Params.writer.Write((int)r);
        father.send_message(message);
    }
    void update()
    {
        game_object.transform.localScale += (new Vector3(1, 1, 1) - game_object.transform.localScale) * 0.3f;
        if(code!=0&&loaded==false){
            Texture2D texture = father.picture_loader.get(code,ocgcore_picture_type.card_picture);
            if (texture!=null)
            {
                loaded = true;
                game_object.transform.FindChild("card_attack_pic").GetComponent<UITexture>().mainTexture = texture;
                game_object.transform.FindChild("card_defence_pic").GetComponent<UITexture>().mainTexture = texture;
            }
        }
    }
}

