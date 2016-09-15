using UnityEngine;
using System;
public class DECK_MANAGER_CARD
{
    public CLIENT_SERVANT_DECKMANAGER father;
    public GameObject game_object;
    public CardData data;
    public long code;
    public Rigidbody rid ;
    public DECK_MANAGER_CARD(CLIENT_SERVANT_DECKMANAGER f,long c)
    {
        code = c;
        father = f;
        data = father.father.card_data_manager.GetById(code);
        debugger.Log("一张卡被创建");
        game_object = father.create_game_object(father.father.loader.mod_deck_manager_card,get_good_position(),Quaternion.identity);
        game_object.transform.eulerAngles = new Vector3(80,0,0);
        game_object.AddComponent<card_container>().card = this;
        rid = game_object.AddComponent<Rigidbody>();
        game_object.layer = 16;
        set_phsical(false);
        game_object.transform.localScale = Vector3.zero;
        iTween.ScaleTo(game_object,new Vector3(3,4,1),0.6f);
        
        father.refresh_functions.Add(handler);
    }
    public void kill_oneself()
    {
        iTween.ScaleTo(game_object, Vector3.zero, 0.6f);
        father.kill_game_object(game_object, 0.6f);
        father.refresh_functions.Remove(handler);
    }
    public Vector3 get_good_position()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        float height = 4;
        Vector3 to_ltemp = father.father.camera_game_main.ScreenToWorldPoint(new Vector3(x, y, 1));
        Vector3 dv = to_ltemp - father.father.camera_game_main.transform.position;
        if (dv.y == 0) dv.y = 0.01f;
        to_ltemp.x = ((height - father.father.camera_game_main.transform.position.y) 
            * (dv.x) / dv.y + father.father.camera_game_main.transform.position.x);
        to_ltemp.y = ((height - father.father.camera_game_main.transform.position.y)
            * (dv.y) / dv.y + father.father.camera_game_main.transform.position.y);
        to_ltemp.z = ((height - father.father.camera_game_main.transform.position.y) 
            * (dv.z) / dv.y + father.father.camera_game_main.transform.position.z);
        return to_ltemp;
    }
    public Vector3 get_hashed_mouse_position(float height)
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        Vector3 to_ltemp = father.father.camera_game_main.ScreenToWorldPoint(new Vector3(x, y, 1));
        Vector3 dv = to_ltemp - father.father.camera_game_main.transform.position;
        if (dv.y == 0) dv.y = 0.01f;
        to_ltemp.x = ((height - father.father.camera_game_main.transform.position.y)
            * (dv.x) / dv.y + father.father.camera_game_main.transform.position.x);
        to_ltemp.y = ((height - father.father.camera_game_main.transform.position.y)
            * (dv.y) / dv.y + father.father.camera_game_main.transform.position.y);
        to_ltemp.z = ((height - father.father.camera_game_main.transform.position.y)
            * (dv.z) / dv.y + father.father.camera_game_main.transform.position.z);
        return to_ltemp;
    }
    void set_phsical(bool physical)
    {
        if (physical)
        {
            rid.useGravity = true;
           // if(game_object.GetComponent<Rigidbody>()==null)
            //{
           //     game_object.AddComponent<Rigidbody>().useGravity = true;
            //}
           //// game_object.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            rid.useGravity = false;
           // Rigidbody rid = game_object.GetComponent<Rigidbody>();
            //if (rid!=null)
           // {
           //     MonoBehaviour.Destroy(rid);
           // }
            //game_object.GetComponent<BoxCollider>().enabled = false;
        }
    }
    bool is_dragging = false;
    public void begin_drag()
    {
        is_dragging = true;
        debugger.Log("一张卡被开始拖拽");
        set_phsical(false);
    }
    public void end_drag()
    {
        is_dragging = false;
        debugger.Log("一张卡被结束拖拽");
        set_phsical(true);
        //Vector3 form_position = get_hashed_mouse_position(4);
        //form_position.y = 0;
        //Vector3 to_position = get_hashed_mouse_position(0);
        //Vector3 delta_position = to_position - form_position;
        //float length = Vector3.Distance(delta_position,Vector3.zero);
        //Vector3 out_v = (delta_position * length * Vector3.Distance(Physics.gravity, Vector3.zero) / (16 * Time.deltaTime));
        //out_v *= 0.7f;
        //rid.AddForce(out_v);



        Vector3 form_position = get_hashed_mouse_position(4);
        Vector3 to_position = get_hashed_mouse_position(0);
        Vector3 delta_position = to_position - form_position;
        rid.AddForce(delta_position*200);
    }
    bool pic_loaded=false;
    void handler()
    {
        if (father.father.pointed_game_object == game_object && father.father.left_mouse_button_is_down == true )
        {
            if (father.drag_card == null)
            {
                father.drag_card = this;
                begin_drag();
                iTween.RotateTo(game_object, new Vector3(90, 0, 0), 0.6f);
            }
        }
        else
        {
            //if (father.drag_card == this && father.father.left_mouse_button_is_down == false)
            //{
            //    father.drag_card = null;
            //    end_drag();
            //}
        }
        if (Vector3.Distance(Vector3.zero, game_object.transform.position) > 50 && this != father.drag_card)
        {
            if (father.is_siding)
            {
                game_object.transform.position = new Vector3(0,5,0);
                rid.Sleep();
            }
            else
            {
                father.kill_card(this);
            }
        }
        if (code != 0)
        {
            if (pic_loaded == false)
            {
                Texture2D pic = father.father.picture_loader.get(code, ocgcore_picture_type.card_picture);
                if (pic != null)
                {
                    game_object.transform.FindChild("face").GetComponent<Renderer>().material.mainTexture = pic;
                    pic_loaded = true;
                }
            }
        }
        if (is_dragging)
        {
            game_object.transform.position = get_good_position();
        }
    }

    public void tween_to_vector_then_fall(Vector3 position, Vector3 rotation,float delay,float time)
    {
        set_phsical(false);
        game_object.GetComponent<BoxCollider>().enabled = false;
        iTween.MoveTo(game_object, iTween.Hash(
                          "delay", delay,
                          "x", position.x,
                          "y", position.y,
                          "z", position.z,
                          "time", time,
                          "oncomplete",(Action)tween_to_vector_then_fall_handler
                          ));
        iTween.RotateTo(game_object, iTween.Hash(
                          "delay", delay,
                          "x", rotation.x,
                          "y", rotation.y,
                          "z", rotation.z,
                          "time", time
                          ));
    }
    void tween_to_vector_then_fall_handler()
    {
        game_object.GetComponent<BoxCollider>().enabled = true;
        set_phsical(true);
    }
}
