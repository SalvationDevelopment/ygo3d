using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class CLIENT_SERVANT_DECKMANAGER
{
    #region ini

    public CLIENT father;

    public bool is_siding = false;

    List<GameObject> all_objects = new List<GameObject>();

    public delegate void refresh_function();

    public List<refresh_function> refresh_functions = new List<refresh_function>();

    public CLIENT_SERVANT_DECKMANAGER(CLIENT c,bool side=false)
    {
        is_siding = side;
        father = c;
        initialize();
    }

    void initialize()
    {
        start_script();
        if (is_debuging)
        {
            debug_script();
        }
    }

    public void fit_screen()
    {
        screen_size_changed();
    }

    public void kill_oneself()
    {
        father.camera_game_main.rect = new Rect(0, 0, 1, 1);
        father.camera_main_3d.rect = new Rect(0, 0, 1, 1);
        father.camera_container_3d.rect = new Rect(0, 0, 1, 1);
        refresh_functions.Clear();
        iTween.MoveTo(object_card_hint, father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(-500,Screen.height/2,0)), 1);
        iTween.MoveTo(object_search_bed, father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width+500, Screen.height / 2, 0)), 1);
        MonoBehaviour.Destroy(object_search_bed, 1f);
        object_search_bed = null;
        MonoBehaviour.Destroy(object_card_hint, 1f);
        object_card_hint = null;
        iTween.MoveTo(father.camera_game_main.gameObject,new Vector3(0,230,-230),5f);
        for (int i = 0; i < all_objects.Count; i++)
        {
            try
            {
                //if (all_objects[i]!=null)
                //{
                //    iTween[] ts = all_objects[i].GetComponents<iTween>();
                //    for (int p = 0; p < ts.Length;p++ )
                //    {
                //        MonoBehaviour.Destroy(ts[p]);
                //    }
                //    iTween.ScaleTo(all_objects[i], Vector3.zero, 1);
                //}
                MonoBehaviour.Destroy(all_objects[i], 2f);
            }
            catch (Exception)
            {

            }
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


    #endregion

    #region UI

    GameObject desk = null;

    GameObject object_card_hint;

    UITexture UITexture_card_pic;

    GameObject object_search_bed;

    UISprite UISprite_search_bed;

    GameObject object_search_max_bed;

    Vector3 camera_position = new Vector3(0, 23f, -17.5f);

    float camera_length = Vector3.Distance(new Vector3(0, 23f, -17.5f),Vector3.zero);

    float camera_angle = Mathf.Atan(23/17.5f);

    Vector3 destinate_card_hint_vector = Vector3.zero;

    Vector3 destinate_search_main_bed_vector = Vector3.zero;

    Vector3 destinate_search_max_bed_vector = Vector3.zero;

    //control left

    UILabel UILabel_card_name;

    UILabel UILabel_card_type;

    UITextList UITextList_card_description;

    UISprite UISprite_card_hint;

    //control right

    UILabel UILabel_count;

    UIInput UIInput_search;

    UIButton UIButton_normal_search;

    UIButton UIButton_max_search;

    UIButton UIButton_home;

    UIButton UIButton_sort;

    UIButton UIButton_rand;

    UIButton UIButton_clear;

    GameObject GameObject_result_container;

    UIScrollBar UIScrollBar_list;

    UIScrollView UIScrollView_list;

    GameObject GameObject_lflist;

    UIPopupList UIPopupList_lflist;

    //control max

    UIPopupList UIPopupList_main;

    UIPopupList UIPopupList_second;

    UIPopupList UIPopupList_race;

    UIPopupList UIPopupList_attribute;

    UIInput UIInput_stars;

    UIInput UIInput_atk;

    UIInput UIInput_def;

    UIToggle[] UIToggles = new UIToggle[32];

    List<GameObject> results = new List<GameObject>();

    class string_fliter
    {
        public string str;
        public UInt32 fliter;
    }

    List<string_fliter> fliters = new List<string_fliter>();

    string_fliter create_string_fliter(string str,UInt32 fliter)
    {
        string_fliter re = new string_fliter();
        re.str = str;
        re.fliter = fliter;
        fliters.Add(re);
        return re;
    }

    Lflist current_list;

    void start_script()
    {
        Delegater temp;
        refresh_functions.Add(refresh);
        iTween[] ts = father.camera_game_main.GetComponents<iTween>();
        for (int p = 0; p < ts.Length; p++)
        {
            MonoBehaviour.Destroy(ts[p]);
        }
        father.camera_game_main.transform.position = new Vector3(0, 230, -230);
        father.camera_main_3d.transform.position = new Vector3(0, 230, -230);
        father.camera_container_3d.transform.position = new Vector3(0, 230, -230);
        desk = create_game_object(father.loader.mod_simple_quad,Vector3.zero,Quaternion.identity);
        desk.layer = 16;
        desk.transform.position = Vector3.zero;
        desk.transform.eulerAngles = new Vector3(90,0,0);
        desk.transform.localScale = new Vector3(30,28,1);
        FileStream file = new FileStream("deck.png", FileMode.Open, FileAccess.Read);
        file.Seek(0, SeekOrigin.Begin);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        file.Close();
        file.Dispose();
        file = null;
        Texture2D pic = new Texture2D(1024, 600);
        pic.LoadImage(data);
        desk.GetComponent<Renderer>().material.mainTexture = pic;
        Rigidbody rbd = desk.AddComponent<Rigidbody>();
        rbd.useGravity = false;
        rbd.isKinematic = true;
        BoxCollider desk_collider = desk.AddComponent<BoxCollider>();
        desk_collider.size = new Vector3(1,1,1f);
        if(is_siding)
        {
            desk_collider.size = new Vector3(100, 100, 1f);
        }
        //left
       
        object_card_hint = create_game_object(father.loader.mod_ocgcore_ui_card_hint, Vector3.zero, Quaternion.identity, father.ui_back_ground_2d);
       
        UILabel_card_name = object_card_hint.transform.FindChild("card_name").GetComponent<UILabel>();
        UILabel_card_type = object_card_hint.transform.FindChild("card_type").GetComponent<UILabel>();
        UITextList_card_description = object_card_hint.transform.FindChild("card_description").GetComponent<UITextList>();
        UITexture_card_pic = object_card_hint.transform.FindChild("card_pic").GetComponent<UITexture>();
        UISprite_card_hint = object_card_hint.transform.FindChild("under_pic").GetComponent<UISprite>();
        object_card_hint.transform.position = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(-400, Screen.height / 2));
        //right
        object_search_bed = create_game_object(father.loader.mod_deck_manager_main_bed, Vector3.zero, Quaternion.identity, father.ui_back_ground_2d);
        UILabel_count = object_search_bed.transform.FindChild("Label").GetComponent<UILabel>();
        UISprite_search_bed = object_search_bed.transform.FindChild("under_pic").GetComponent<UISprite>();
        object_search_bed.transform.position = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width+400, Screen.height / 2));
        //max
        object_search_max_bed = create_game_object(father.loader.mod_deck_manager_max_bed, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        object_search_max_bed.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        object_search_max_bed.transform.position = father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 490, Screen.height + 700));

        ////control right

        UIInput_search = object_search_bed.transform.FindChild("name_input").GetComponent<UIInput>();
        temp = UIInput_search.gameObject.AddComponent<Delegater>();
        temp.f = on_text_change;
        UIInput_search.onChange.Add(new EventDelegate(temp, "function"));
        
        UIButton_normal_search = object_search_bed.transform.FindChild("button_normal_search").GetComponent<UIButton>();
        temp = UIButton_normal_search.gameObject.AddComponent<Delegater>();
        temp.f = on_search;
        UIButton_normal_search.onClick.Add(new EventDelegate(temp, "function"));

        UIButton_max_search = object_search_bed.transform.FindChild("button_max_search").GetComponent<UIButton>();
        temp = UIButton_max_search.gameObject.AddComponent<Delegater>();
        temp.f = button_advanced_clicked;
        UIButton_max_search.onClick.Add(new EventDelegate(temp, "function"));

        UIButton_home = object_search_bed.transform.FindChild("home").GetComponent<UIButton>();

        UIButton_sort = object_search_bed.transform.FindChild("sort").GetComponent<UIButton>();
        temp = UIButton_sort.gameObject.AddComponent<Delegater>();
        temp.f = on_sort;
        UIButton_sort.onClick.Add(new EventDelegate(temp, "function"));

        UIButton_rand = object_search_bed.transform.FindChild("rand").GetComponent<UIButton>();
        temp = UIButton_rand.gameObject.AddComponent<Delegater>();
        temp.f = on_rand;
        UIButton_rand.onClick.Add(new EventDelegate(temp, "function"));

        UIButton_clear = object_search_bed.transform.FindChild("clear").GetComponent<UIButton>();
        temp = UIButton_clear.gameObject.AddComponent<Delegater>();
        temp.f = on_clear;
        UIButton_clear.onClick.Add(new EventDelegate(temp, "function"));

        GameObject_result_container = object_search_bed.transform.FindChild("container").FindChild("Right Scroll View").FindChild("Grid").gameObject;

        UIScrollView_list = object_search_bed.transform.FindChild("container").FindChild("Right Scroll View").GetComponent<UIScrollView>();

        UIScrollBar_list = object_search_bed.transform.FindChild("bar").GetComponent<UIScrollBar>();
        temp = UIScrollBar_list.gameObject.AddComponent<Delegater>();
        temp.f = on_bar_change;
        UIScrollBar_list.onChange.Add(new EventDelegate(temp, "function"));

        //lflist

        GameObject_lflist = create_game_object(father.loader.mod_deck_manager_lflist, Vector3.zero, Quaternion.identity, father.ui_main_2d);
        UIPopupList_lflist = GameObject_lflist.GetComponent<UIPopupList>();
        GameObject_lflist.transform.position = father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 660, -100, 0));
        UIPopupList_lflist.Clear();
        for (int i = 0; i < father.lflist_manager.Lflists.Count;i++ )
        {
            UIPopupList_lflist.AddItem(father.lflist_manager.Lflists[i].name);
        }
        if (UIPopupList_lflist.items.Count>1)
        {
            UIPopupList_lflist.value = UIPopupList_lflist.items[1];
        }
        temp = UIPopupList_lflist.gameObject.AddComponent<Delegater>();
        temp.f = on_lflist_change;
        UIPopupList_lflist.onChange.Add(new EventDelegate(temp, "function"));
        on_lflist_change();
        ////control max

        UIPopupList_main = object_search_max_bed.transform.FindChild("type").GetComponent<UIPopupList>();
        UIPopupList_main.Clear();
        UIPopupList_main.AddItem("无种类");
        UIPopupList_main.AddItem(father.string_data_manager.get("system", 1312));
        UIPopupList_main.AddItem(father.string_data_manager.get("system", 1313));
        UIPopupList_main.AddItem(father.string_data_manager.get("system", 1314));
        temp = UIPopupList_main.gameObject.AddComponent<Delegater>();
        temp.f = list_mian_selected;
        UIPopupList_main.onChange.Add(new EventDelegate(temp, "function"));


        UIPopupList_second = object_search_max_bed.transform.FindChild("type_second").GetComponent<UIPopupList>();
        UIPopupList_second.Clear();
        UIPopupList_second.AddItem("无种类");

        UIPopupList_race = object_search_max_bed.transform.FindChild("type_race").GetComponent<UIPopupList>();
        UIPopupList_race.Clear();
        UIPopupList_race.AddItem("无种类");

        UIPopupList_attribute = object_search_max_bed.transform.FindChild("type_attribute").GetComponent<UIPopupList>();
        UIPopupList_attribute.Clear();
        UIPopupList_attribute.AddItem("无种类");

        UIInput_stars = object_search_max_bed.transform.FindChild("input_stars").GetComponent<UIInput>();

        UIInput_atk = object_search_max_bed.transform.FindChild("input_atk").GetComponent<UIInput>();

        UIInput_def = object_search_max_bed.transform.FindChild("input_def").GetComponent<UIInput>();

        for (int i = 0; i < 32;i++ )
        {
            UIToggles[i] = object_search_max_bed.transform.FindChild("select ("+(i+1)+")").GetComponent<UIToggle>();
            UIToggles[i].transform.FindChild("Label").GetComponent<UILabel>().text = father.string_data_manager.get("system",1100+i);
            UIToggles[i].value = false;
        }
        load_fliters();
        debugger.Log("all_finished");
        good_list _good_list = GameObject_result_container.AddComponent<good_list>();
        _good_list.cam = father.camera_back_ground_2d;
        _good_list.onchange = on_list_change;
        _good_list.pic_loader = father.picture_loader;
        _good_list.father = this;

        UIScrollBar_list.barSize = 0;
        UIScrollBar_list.value = 0;
        UIScrollView_list.can_be_draged = false;//pc
    }

    public void set_return_function(CLIENT_SERVANT_OCGCORE.refresh_function f)
    {
        Delegater temp = UIButton_home.gameObject.AddComponent<Delegater>();
        temp.f = f;
        UIButton_home.onClick.Add(new EventDelegate(temp, "function"));
    }

    void load_fliters()
    {
        create_string_fliter("无种类", 0);
        for (int i = 0; i < 25; i++)
        {
            create_string_fliter(father.string_data_manager.get("system", 1050 + i) + " ", (UInt32)Math.Pow(2, i));
        }
        for (int i = 0; i < 32; i++)
        {
            create_string_fliter(father.string_data_manager.get("system", 1100 + i), (UInt32)Math.Pow(2, i));
        }
        for (int i = 0; i < 24; i++)
        {
            create_string_fliter(father.string_data_manager.get("system", 1020 + i) + "族", (UInt32)Math.Pow(2, i));
        }
        for (int i = 0; i < 7; i++)
        {
            create_string_fliter(father.string_data_manager.get("system", 1010 + i) + " ", (UInt32)Math.Pow(2, i));
        }
    }

    void screen_size_changed()
    {

        father.camera_game_main.rect = new Rect(-0.05f, 0, 1, 1);
        father.camera_main_3d.rect = new Rect(-0.05f, 0, 1, 1);
        father.camera_container_3d.rect = new Rect(-0.05f, 0, 1, 1);
        UISprite_card_hint.height = (int)(Screen.height / 0.85) - 16;
        destinate_card_hint_vector = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(UISprite_card_hint.width / 2 + 190, Screen.height / 2 + 106));
        UISprite_search_bed.height = (int)(Screen.height / 1.1f) - 16;
        destinate_search_main_bed_vector = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width- UISprite_search_bed.width / 2 - 8f, Screen.height / 2));
        destinate_search_max_bed_vector = father.camera_main_2d.ScreenToWorldPoint( new Vector3(Screen.width - 480, Screen.height + 200));

    }

    float destinate_angel_1 = -233;

    float destinate_angel_2 = -233;

    void refresh()
    {
        if (destinate_angel_1 != -233)
        {
            camera_angle += (destinate_angel_1 - camera_angle) * Time.deltaTime * 2.5f;
            if (Math.Abs(camera_angle - destinate_angel_1)<0.03)
            {
                camera_angle = destinate_angel_1;
                destinate_angel_1 = -233;
            }
        }
        else if (destinate_angel_2 != -233)
        {
            camera_angle += (destinate_angel_2 - camera_angle) * Time.deltaTime * 3.5f;
            if (Math.Abs(camera_angle - destinate_angel_2) < 0.03)
            {
                camera_angle = destinate_angel_2;
                destinate_angel_2 = -233;
            }
        }
        else if(Input.mousePosition.x<Screen.width-280){
            if (Input.mousePosition.x >250)
            {
                camera_angle += father.mouse_wheel_change_value / 60;
                if (camera_angle < 20f / 180f * 3.1415926f)
                {
                    camera_angle = 20f / 180f * 3.1415926f;
                }
                if (camera_angle > 87f / 180f * 3.1415926f)
                {
                    camera_angle = 87f / 180f * 3.1415926f;
                }
            }
        }
        camera_length = 29 - (camera_angle - 3.1415926f / 180f * 60f) * 13f;
        camera_position = new Vector3(0, camera_length * Mathf.Sin(camera_angle), -camera_length * Mathf.Cos(camera_angle));
        father.camera_game_main.gameObject.transform.eulerAngles += (new Vector3(camera_angle / 3.1415926f * 180f, 0, 0) - father.camera_game_main.gameObject.transform.eulerAngles) * 0.1f;
        father.camera_game_main.transform.position += (camera_position - father.camera_game_main.transform.position) * 0.1f;
        father.camera_game_main.transform.position = father.camera_game_main.transform.position;
        father.camera_main_3d.transform.localPosition = father.camera_game_main.transform.position;
        father.camera_container_3d.transform.localPosition = father.camera_game_main.transform.position;
        father.camera_main_3d.transform.localEulerAngles = father.camera_game_main.gameObject.transform.eulerAngles;
        father.camera_container_3d.transform.localEulerAngles = father.camera_game_main.gameObject.transform.eulerAngles;
        //left
        object_card_hint.transform.position += (destinate_card_hint_vector - object_card_hint.transform.position) * 0.2f;
        //right
        object_search_bed.transform.position += (destinate_search_main_bed_vector - object_search_bed.transform.position) * 0.2f;
        //max
        object_search_max_bed.transform.position += (destinate_search_max_bed_vector - object_search_max_bed.transform.position) * 0.2f;
        //lflist
        GameObject_lflist.transform.position += (father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 660, 30, 0)) - GameObject_lflist.transform.position) * 0.2f;


        //update list

        if (Input.GetKeyDown(KeyCode.Return) && UIInput_search.value != "")
        {
            on_search();
        }
        if (last_chane_time!=0)
        {
            if (father.time - last_chane_time>500)
            {
                last_chane_time = 0;
                if (UIInput_search.value!="") search();
            }
        }

        //print
        print_handler();

        //

        if (Input.GetMouseButton(0) == false && preb0 == true)
        {
            //if (is_draging) 
                end_drag();
            is_draging = false;
        }
        preb0 = Input.GetMouseButton(0);
        if (is_draging) drag_handler();
    }

    void button_advanced_clicked()
    {
        UILabel lable = UIButton_max_search.transform.FindChild("Label").GetComponent<UILabel>();
        if (lable.text == "高级搜索")
        {
            lable.text = "清空";
            destinate_search_max_bed_vector = father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 490, Screen.height -165));
        }
        else
        {
            for (int i = 0; i < 32;i++ )
            {
                UIToggles[i].value = false;
            }
            UIInput_atk.value = "";
            UIInput_def.value = "";
            UIInput_stars.value = "";
            UIPopupList_main.value = "无种类";
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem("无种类");
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem("无种类");
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem("无种类");
            UIPopupList_second.value = "无种类";
            UIPopupList_attribute.value = "无种类";
            UIPopupList_race.value = "无种类";
            lable.text = "高级搜索";
            destinate_search_max_bed_vector = father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 490, Screen.height + 200));
        }
    }

    void list_mian_selected()
    {
        debugger.Log(UIPopupList_main.value);
        if (UIPopupList_main.value=="无种类")
        {
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem("无种类");
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem("无种类");
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem("无种类");
            UIPopupList_second.value = "无种类";
            UIPopupList_attribute.value = "无种类";
            UIPopupList_race.value = "无种类";
        }
        if (UIPopupList_main.value == "怪兽")
        {
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem("无种类");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1054) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1055) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1056) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1057) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1063) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1073) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1062) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1061) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1060) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1059) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1071) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1072) + " ");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1074) + " ");
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem("无种类");
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem("无种类");
            for (int i = 0; i < 24; i++)
            {
                UIPopupList_race.AddItem(father.string_data_manager.get("system", 1020 + i)+"族");
            }
            for (int i = 0; i < 7; i++)
            {
                UIPopupList_attribute.AddItem(father.string_data_manager.get("system", 1010 + i) + " ");
            }
            UIPopupList_second.value = "无种类";
            UIPopupList_attribute.value = "无种类";
            UIPopupList_race.value = "无种类";
        }
        if (UIPopupList_main.value == "魔法")
        {
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem("无种类");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1066));
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1067));
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1057));
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1068));
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1069));
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem("无种类");
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem("无种类");
            UIPopupList_second.value = "无种类";
            UIPopupList_attribute.value = "无种类";
            UIPopupList_race.value = "无种类";
        }
        if (UIPopupList_main.value == "陷阱")
        {
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem("无种类");
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1067));
            UIPopupList_second.AddItem(father.string_data_manager.get("system", 1070));
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem("无种类");
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem("无种类");
            UIPopupList_second.value = "无种类";
            UIPopupList_attribute.value = "无种类";
            UIPopupList_race.value = "无种类";
        }
    }

    #endregion

    bool is_debuging = false;

    void debug_script()
    {
        debugger.Log("debug_script");
        var d = from_ydk_to_deck("deck//1.ydk");
        List<CardData> t = new List<CardData>();
        foreach(DECK_MANAGER_DECK.CARD_IN_DECK one in d.main){
            t.Add(father.card_data_manager.GetById(one.code));
        }
        if (t.Count > 0) change_data(t[0]);
        print(t);
        from_deck_to_field(d,true);
    }

    #region 2d

    float k_list=1;

    void print(List<CardData> code_list)
    {
        code_list_to_print = code_list;
        for (int i = 0; i < results.Count; i++)
        {
            kill_game_object(results[i]);
        }
        results.Clear();
        Vector3 world_position_1 = new Vector3(1, 1, 1);
        Vector3 world_position_2 = new Vector3(1, 1, 1);
        print_one_with_trace(code_list, ref world_position_1, ref world_position_2, 0);
        print_one_with_trace(code_list, ref world_position_1, ref world_position_2, (int)(5f / 600f * (float)Screen.height));
        print_one_with_trace(code_list, ref world_position_1, ref world_position_2, code_list.Count - 1);
        k_list = world_position_1.y - world_position_2.y;
        if(k_list==0){
            k_list = 0.0001f;
        }
        float size = (float)1 / (float)code_list.Count * 6f;
        if (size > 1) size = 1;
        UIScrollBar_list.barSize = size;
        if (UIScrollBar_list.barSize<0.2f)
        {
            UIScrollBar_list.barSize = 0.2f;
        }
        UIScrollView_list.mScroll = 0;
        UIScrollView_list.Scroll(0.01f);
        UIScrollView_list.UpdatePosition();
        
        now_index = 0;
    }

    private void print_one_with_trace(List<CardData> code_list, ref Vector3 world_position_1, ref Vector3 world_position_2, int i)
    {
        if (i < code_list.Count && i >= 0)
        {
            GameObject obj;
            obj = create_game_object(father.loader.mod_deck_manager_card_on_list, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(GameObject_result_container.transform, false);
            obj.transform.localPosition = new Vector3(0, 135f / 768f * (float)Screen.height - i * 65, 0);
            obj.name = code_list[i].code.ToString();
            obj.transform.FindChild("Label").GetComponent<UILabel>().text = "" + code_list[i].Name + "\n"
                + card_string_helper.get_string(code_list[i], false);
            obj.transform.FindChild("ban").GetComponent<ban_icon>().show(check_lflist(code_list[i].code));
            var t = obj.AddComponent<data_container>();
            t.data = code_list[i];
            t.father = this;
            results.Add(obj);
            if (i == 0)
            {
                world_position_1 = obj.transform.position;
                world_position_2 = obj.transform.position;
            }
            if (i == (int)(5f / 600f * (float)Screen.height))
            {
                world_position_1 = obj.transform.position;
            }
            if (i == code_list.Count - 1)
            {
                world_position_2 = obj.transform.position;
            }
        }
    }

    public List<CardData> code_list_to_print = new List<CardData>();

    public int now_index = 0;

    void print_handler()
    {
        if (now_index == 0 || now_index >= code_list_to_print.Count)
        {
            now_index++;
            return;
        }
        if (now_index == (int)(5f / 600f * (float)Screen.height))
        {
            now_index++;
            return;
        }
        if (now_index == code_list_to_print.Count - 1)
        {
            now_index++;
            return;
        }
        Vector3 up = Vector3.zero;
        Vector3 down = Vector3.zero; ;
        if (father.camera_back_ground_2d != null)
        {
            up = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(0, Screen.height + 200, 0));
            down = father.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(0, -200, 0));
        }
        for (int d = 0; d < 100;d++ )
        {
            if (now_index < code_list_to_print.Count)
            {
                GameObject obj;
                obj = create_game_object(father.loader.mod_deck_manager_card_on_list, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(GameObject_result_container.transform, false);
                obj.transform.localPosition = new Vector3(0, 135f / 768f * (float)Screen.height - now_index * 65, 0);
                obj.name = code_list_to_print[now_index].code.ToString();
                obj.transform.FindChild("Label").GetComponent<UILabel>().text = "" + code_list_to_print[now_index].Name + "\n"
                    + card_string_helper.get_string(code_list_to_print[now_index], false);
                obj.transform.FindChild("ban").GetComponent<ban_icon>().show(check_lflist(code_list_to_print[now_index].code));
                var t = obj.AddComponent<data_container>();
                t.data = code_list_to_print[now_index];
                t.father = this;
                results.Add(obj);
                now_index++;
                if (obj.transform.position.y > down.y && obj.transform.position.y < up.y)
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
        return;
    }

    void on_clear()
    {
        if(is_siding==false)
        {
            for (int i = 0; i < 3; i++)
            {
                iTween.ShakePosition(desk, iTween.Hash(
                       "delay", (i) * 0.4f,
                       "x", 1,
                       "y", 1,
                       "z", 1,
                       "time", 0.2f
                       ));
                iTween.RotateTo(desk, iTween.Hash(
                       "delay", (i) * 0.4f,
                       "x", 0,
                       "y", 0,
                       "z", 0,
                       "time", 0.2f
                       ));
                iTween.RotateTo(desk, iTween.Hash(
                              "delay", 0.2f + (i) * 0.4f,
                              "x", 90,
                              "y", 0,
                              "z", 0,
                              "time", 0.2f
                              ));

            }
        }
    }

    void on_sort()
    {
        DECK_MANAGER_DECK deck = from_field_to_deck();
        deck = sort_deck(deck);
        from_deck_to_field(deck);
    }

    void on_rand()
    {
        DECK_MANAGER_DECK deck = from_field_to_deck();
        deck = rand_deck(deck);
        from_deck_to_field(deck);
    }

    public DECK_MANAGER_DECK from_ydk_to_deck(string path)
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
                                    right_ini.Add(father.card_data_manager.GetById(code));
                                }
                                break;
                            case 2:
                                {
                                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                                    card.code = code;
                                    return_value.extra.Add(card);
                                    right_ini.Add(father.card_data_manager.GetById(code));
                                }
                                break;
                            case 3:
                                {
                                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                                    card.code = code;
                                    return_value.side.Add(card);
                                    right_ini.Add(father.card_data_manager.GetById(code));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (right_ini.Count > 0) change_data(right_ini[0]);
            print(right_ini);
        }
        catch (Exception e) 
        {
            debugger.Log(e);
        }
        return return_value;
    }

    public DECK_MANAGER_DECK from_field_to_deck()
    {
        DECK_MANAGER_DECK deck = new DECK_MANAGER_DECK();
        for (int i = 0; i < cards.Count;i++ )
        {
            long code = cards[i].code;
            GameObject game_object = cards[i].game_object;
            Vector3 v = game_object.transform.position;
            if (v.y < -0.5)
            {
                if (game_object.GetComponent<Rigidbody>().useGravity)
                {
                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                    card.code = code;
                    card.game_object = game_object;
                    deck.lancer.Add(card);
                    continue;
                }
            }

            if (v.z > -8)
            {
                if (
                (cards[i].data.Type & (long)game_type.TYPE_FUSION) > 0
                ||
                (cards[i].data.Type & (long)game_type.TYPE_SYNCHRO) > 0
                ||
                (cards[i].data.Type & (long)game_type.TYPE_XYZ) > 0
                )
                {
                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                    card.code = code;
                    card.game_object = game_object;
                    deck.extra.Add(card);
                }
                else
                {
                    DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                    card.code = code;
                    card.game_object = game_object;
                    deck.main.Add(card);
                }

            }
            else
            {
                DECK_MANAGER_DECK.CARD_IN_DECK card = new DECK_MANAGER_DECK.CARD_IN_DECK();
                card.code = code;
                card.game_object = game_object;
                deck.side.Add(card);
            }
        }
        for (int x = 0; x < deck.lancer.Count; x++)
        {
            try
            {
                kill_card(deck.lancer[x].game_object.GetComponent<card_container>().card);
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
        return deck;
    }

    public void from_deck_to_ydk(DECK_MANAGER_DECK deck_, string path)
    {
        string value = "#created by ygofroge\r\n#main\r\n";
        DECK_MANAGER_DECK deck = deck_;
        for (int i = 0; i < deck.main.Count; i++)
        {
            value += deck.main[i].code.ToString() + "\r\n";
        }
        value += "#extra\r\n";
        for (int i = 0; i < deck.extra.Count; i++)
        {
            value += deck.extra[i].code.ToString() + "\r\n";
        }
        value += "!side\r\n";
        for (int i = 0; i < deck.side.Count; i++)
        {
            value += deck.side[i].code.ToString() + "\r\n";
        }
        System.IO.File.WriteAllText(path,value,System.Text.Encoding.UTF8);
    }

    GameObject effect_of_deck_read = null;

    void effect_of_deck_read_handler()
    {
        if (effect_of_deck_read==null)
        {
            refresh_functions.Remove(effect_of_deck_read_handler);
        }
        else
        {
            Vector3 scr = father.camera_game_main.WorldToScreenPoint(new Vector3(0,5,0));
            scr.z = 10;
            effect_of_deck_read.transform.position = father.camera_game_main.ScreenToWorldPoint(scr);
        }
    }

    public void from_deck_to_field(DECK_MANAGER_DECK deck_, bool effect = false)
    {
        if (effect)
        {
            effect_of_deck_read = create_game_object(father.loader.mod_deck_manager_effect, Vector3.zero, Quaternion.identity);
            refresh_functions.Add(effect_of_deck_read_handler);
            MonoBehaviour.Destroy(effect_of_deck_read, 3);
        }
        destinate_angel_1 = 30f / 180f * 3.1415926f;
        destinate_angel_2 = 87f / 180f * 3.1415926f;
        DECK_MANAGER_DECK deck = deck_;
        int hangshu = get_lieshu(deck.main.Count);
        for (int i = 0; i < deck.main.Count; i++)
        {
            if (deck.main[i].game_object == null)
            {
                deck.main[i].game_object = create_card(deck.main[i].code).game_object;
                //if (Vector3.Distance(deck.main[i].game_object.transform.position, Vector3.zero) > 50)
                {
                    deck.main[i].game_object.transform.position =
                         new Vector3(0,5,0);
                }
            }
            try
            {
                Vector2 v = ui_helper.get_hang_lie(i,hangshu);
                deck.main[i].game_object.GetComponent<card_container>().card.
                   tween_to_vector_then_fall(
                    new Vector3(get_left_right_index(-12.5f, 12.5f, (int)v.y, hangshu), 1 + v.y / 2f, get_left_right_index(11, 0.3f, (int)v.x, 4)),
                    new Vector3(90, 0, 0),
                    (float)i / 20f,
                    0.6f);
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
        for (int i = 0; i < deck.extra.Count; i++)
        {
            if (deck.extra[i].game_object == null)
            {
                deck.extra[i].game_object = create_card(deck.extra[i].code).game_object;
                //if (Vector3.Distance(deck.extra[i].game_object.transform.position, Vector3.zero) > 50)
                {
                    deck.extra[i].game_object.transform.position
                        = new Vector3(0, 5, 0);
                }
            }
            try
            {
                deck.extra[i].game_object.GetComponent<card_container>().card.
                    tween_to_vector_then_fall(
                    new Vector3(get_left_right_index(-12.5f, 12.5f, i, deck.extra.Count), 1 + (float)i / 2f, -5.5f),
                    new Vector3(90, 0, 0),
                    (float)i / 5f,
                    0.6f);
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
        for (int i = 0; i < deck.side.Count; i++)
        {
            if (deck.side[i].game_object == null)
            {
                deck.side[i].game_object = create_card(deck.side[i].code).game_object;
                //if (Vector3.Distance(deck.side[i].game_object.transform.position, Vector3.zero) > 50)
                {
                    deck.side[i].game_object.transform.position =
                          new Vector3(0, 5, 0);
                }
            }
            try
            {
                deck.side[i].game_object.GetComponent<card_container>().card.
                    tween_to_vector_then_fall(
                    new Vector3(get_left_right_index(-12.5f, 12.5f, i, deck.side.Count), 1 + (float)i / 2f, -11.3f),
                    new Vector3(90, 0, 0),
                    (float)i / 5f,
                    0.6f);
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
        }
    }

    float get_left_right_index(float left,float right,int i,int count)
    {
        float return_value=0;
        if (count==1)
        {
            return_value = left + right;
            return_value /= 2;
        }
        else
        {
            return_value = left + (right - left) * (float)i / ((float)(count - 1));
        }
        return return_value; 
    }

    int get_lieshu(int zongshu)
    {
        int return_value = 10;
        for (int i = 0; i < 100;i++ )
        {
            if ((zongshu + i)%4==0)
            {
                return_value = (zongshu + i) / 4;
                break;
            }
        }
        return return_value;
    }

    DECK_MANAGER_DECK sort_deck(DECK_MANAGER_DECK deck)
    {
        DECK_MANAGER_DECK return_value = deck;
        sort_deck(return_value.main);
        sort_deck(return_value.extra);
        sort_deck(return_value.side);
        return return_value;
    }

    DECK_MANAGER_DECK rand_deck(DECK_MANAGER_DECK deck)
    {
        DECK_MANAGER_DECK return_value = deck;
        System.Random rand = new System.Random();
        var main = return_value.main;
        rand_deck(rand, return_value.main);
        rand_deck(rand, return_value.extra);
        rand_deck(rand, return_value.side);
        return return_value;
    }

    private static void rand_deck(System.Random rand, List<DECK_MANAGER_DECK.CARD_IN_DECK> main)
    {
        for (int i = 0; i < main.Count; i++)
        {
            int random_index = rand.Next() % main.Count;
            var t = main[i];
            main[i] = main[random_index];
            main[random_index] = t;
        }
    }

    void sort_deck(List<DECK_MANAGER_DECK.CARD_IN_DECK> main)
    {
        main.Sort((l, r) =>
        {
            if (l.game_object==null)
            {
                return 1;
            }
            if (r.game_object == null)
            {
                return 1;
            }
            CardData left = l.game_object.GetComponent<card_container>().card.data;
            CardData right = r.game_object.GetComponent<card_container>().card.data;
            int a = 1;
            if ((left.Type & 7) < (right.Type & 7))
            {
                a = -1;
            }
            else if ((left.Type & 7) > (right.Type & 7))
            {
                a = 1;
            }
            else
            {
                if (left.Level > right.Level)
                {
                    a = -1;
                }
                else if (left.Level < right.Level)
                {
                    a = 1;
                }
                else
                {
                    if (left.Attack > right.Attack)
                    {
                        a = -1;
                    }
                    else if (left.Attack < right.Attack)
                    {
                        a = 1;
                    }
                    else
                    {
                        if ((left.Type >> 3) > (right.Type >> 3))
                        {
                            a = 1;
                        }
                        else if ((left.Type >> 3) < (right.Type >> 3))
                        {
                            a = -1;
                        }
                        else
                        {
                            if (left.Attribute > right.Attribute)
                            {
                                a = 1;
                            }
                            else if (left.Attribute < right.Attribute)
                            {
                                a = -1;
                            }
                            else
                            {
                                if (left.Race > right.Race)
                                {
                                    a = 1;
                                }
                                else if (left.Race < right.Race)
                                {
                                    a = -1;
                                }
                                else
                                {
                                    if (left.Category > right.Category)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Category < right.Category)
                                    {
                                        a = -1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return a;
        });
    }

    long code_now = 0;

    public void change_data(CardData data)
    {
        if (code_now != data.code)
        {
            code_now = data.code;
            UILabel_card_name.text = card_string_helper.get_name_string(data);
            UILabel_card_type.text = card_string_helper.get_type_string((int)data.Type);
            UITextList_card_description.Clear();
            UITextList_card_description.Add(card_string_helper.get_string(data));
            refresh_functions.Add(load_pic);
        }
    }

    void load_pic()
    {
        Texture2D pic = father.picture_loader.get(code_now,ocgcore_picture_type.card_picture);
        if (pic!=null)
        {
            refresh_functions.Remove(load_pic);
            UITexture_card_pic.mainTexture = pic;
        }
    }

    void on_list_change()
    {
        UIScrollBar_list.value = GameObject_result_container.transform.position.y / k_list; 
    }

    void on_bar_change()
    {
        Vector3 v = GameObject_result_container.transform.position;
        v.y = UIScrollBar_list.value * k_list;
        GameObject_result_container.transform.position = v;
    }

    void on_search()
    {
        debugger.Log(UIInput_search.value);
        search();
        UIInput_search.value = "";
    }

    void on_lflist_change()
    {
            current_list = father.lflist_manager.get_list(UIPopupList_lflist.value);
            for (int i = 0; i < results.Count; i++)
            {
                try
                {
                    results[i].transform.FindChild("ban").GetComponent<ban_icon>().show(check_lflist(code_list_to_print[i].code));
                }
                catch (Exception)
                {
                    
                }
                
            }
    }

    public int check_lflist(long code)
    {
        int re = 3;
        foreach(lflist_line l in current_list.lines){
            if(l.code==code){
                re = l.max;
                break;
            }
        }
        return re;
    }

    void search()
    {
        List<CardData> re = father.card_data_manager.search_advanced(
            get_fliter_type(),
            get_fliter_race(),
            get_fliter_attribute(),
            get_fliter_catagory(),
            UIInput_search.value,
            get_atk(),
            get_def(),
            get_star()
            );
        print(re);
        UILabel_count.text="数量:"+re.Count.ToString();
    }

    UInt32 get_fliter_type()
    {
        UInt32 re = 0;
        if (UIPopupList_main.value == "怪兽")
        {
            re |= 1;
        }
        if (UIPopupList_main.value == "魔法")
        {
            re |= 2;
        }
        if (UIPopupList_main.value == "陷阱")
        {
            re |= 4;
        }
        string type = UIPopupList_second.value;
        foreach(string_fliter t in fliters){
            if (type==t.str)
            {
                re |= t.fliter;
            }
        }
        return re;
    }

    UInt32 get_fliter_race()
    {
        UInt32 re = 0;
        string race = UIPopupList_race.value;
        foreach (string_fliter t in fliters)
        {
            if (race == t.str)
            {
                re |= t.fliter;
            }
        }
        return re;
    }

    UInt32 get_fliter_attribute()
    {
        UInt32 re = 0;
        string attribute = UIPopupList_attribute.value;
        foreach (string_fliter t in fliters)
        {
            if (attribute == t.str)
            {
                re |= t.fliter;
            }
        }
        return re;
    }

    UInt32 get_fliter_catagory()
    {
        UInt32 re = 0;
        for (int i = 0; i < 32;i++ )
        {
            if (UIToggles[i].value==true)
            {
                string catagory = UIToggles[i].transform.FindChild("Label").GetComponent<UILabel>().text;
                foreach (string_fliter t in fliters)
                {
                    if (catagory == t.str)
                    {
                        re |= t.fliter;
                    }
                }
            }
            
        }
        return re;
    }

    int get_atk()
    {
        int re = 0;
        try
        {
            re = int.Parse(UIInput_atk.value);
        }
        catch(Exception e)
        {
            re = -2;
        }
        if (UIInput_atk.value=="")
        {
            re = -233;
        }
        return re;
    }

    int get_def()
    {
        int re = 0;
        try
        {
            re = int.Parse(UIInput_def.value);
            
        }
        catch (Exception e)
        {
            re = -2;
        }
        if (UIInput_def.value == "")
        {
            re = -233;
        }
        return re;
    }

    int get_star()
    {
        int re = 0;
        try
        {
            re = int.Parse(UIInput_stars.value);
        }
        catch (Exception e)
        {
            re = -233;
        }
        if (UIInput_stars.value == "")
        {
            re = -233;
        }
        return re;
    }

    int last_chane_time = 0;

    void on_text_change()
    {
        last_chane_time = father.time;
    }

    #endregion

    #region 3d

    public bool is_draging = false;

    public void begin_drag(long code, DECK_MANAGER_CARD card = null)
    {
        debugger.Log("开始drag"+code.ToString());
    }

    public void rush_to_main(CardData data)
    {
        if(check_code_bool(data.code)){
            var game_object = create_card(data.code).game_object;
            if (
                        (data.Type & (long)game_type.TYPE_FUSION) > 0
                        ||
                        (data.Type & (long)game_type.TYPE_SYNCHRO) > 0
                        ||
                        (data.Type & (long)game_type.TYPE_XYZ) > 0
                        )
            {

                game_object.GetComponent<card_container>().card.
                        tween_to_vector_then_fall(
                        new Vector3(
                            get_left_right_index(-12.5f, 12.5f, UnityEngine.Random.Range(0, 15), 15),
                            5,
                            -5.5f
                            ),
                        new Vector3(90, 0, 0),
                        0,
                        0.6f);
            }
            else
            {
                game_object.GetComponent<card_container>().card.
                       tween_to_vector_then_fall(
                        new Vector3(
                            get_left_right_index(-12.5f, 12.5f, UnityEngine.Random.Range(0, 10), 10),
                            5,
                            get_left_right_index(11, 0.3f, UnityEngine.Random.Range(0, 4), 4)
                            ),
                        new Vector3(90, 0, 0),
                        0,
                        0.6f);
            }
        }
        
    }

    public void rush_to_side(CardData data)
    {
        if (check_code_bool(data.code))
        {
            var game_object = create_card(data.code).game_object;
            game_object.GetComponent<card_container>().card.
                        tween_to_vector_then_fall(
                        new Vector3(
                            get_left_right_index(-12.5f, 12.5f, UnityEngine.Random.Range(0, 15), 15),
                            5,
                            -11.3f
                            ),
                        new Vector3(90, 0, 0),
                        0,
                        0.6f);
        }
    }

    bool check_code_bool(long code)
    {
        bool return_value = true;
        int can_put = check_lflist(code);
        int has = 0;
        for (int i = 0; i < cards.Count;i++ )
        {
            if (cards[i].data.code == code)
            {
                has++;
            }
        }
        if (has >= can_put)
        {
            return_value = false;
        }
        return return_value;
    }

    public void end_drag()
    {
        debugger.Log("结束drag");
        if (drag_card!=null)
        {
            drag_card.end_drag();
            drag_card = null;
        }
        drag_card = null;
    }

    public DECK_MANAGER_CARD drag_card = null;

    void drag_handler()
    {
        ////android
        //if (Input.mousePosition.x < Screen.width - 280)
        //{
        //    if (UIScrollView_list.can_be_draged == true)
        //    {
        //        UIScrollView_list.can_be_draged = false;
        //        debugger.Log("逃逸");
        //        if (drag_card == null)
        //        {
        //            drag_card = create_card(code_now);
        //            drag_card.begin_drag();
        //        }
        //    }
        //}
        //else
        //{
        //    if (UIScrollView_list.can_be_draged == false)
        //    {
        //        UIScrollView_list.can_be_draged = true;
        //    }
        //}
        if (Input.mousePosition.x < Screen.width - 280)
        {
            if (UIScrollView_listcan_be_draged == true)
            {
                UIScrollView_listcan_be_draged = false;
                debugger.Log("逃逸");
                if (drag_card == null)
                {
                    drag_card = create_card(code_now);
                    drag_card.begin_drag();
                }
            }
        }
        else
        {
            if (UIScrollView_listcan_be_draged == false)
            {
                UIScrollView_listcan_be_draged = true;
            }
        }
    }

    List<DECK_MANAGER_CARD> cards = new List<DECK_MANAGER_CARD>();

    DECK_MANAGER_CARD create_card(long code)
    {
        DECK_MANAGER_CARD card = new DECK_MANAGER_CARD(this, code);
        cards.Add(card);
        return card;
    }

    public void kill_card(DECK_MANAGER_CARD c)
    {
        c.kill_oneself();
        cards.Remove(c);
        c = null;
    }

    #endregion

    public bool UIScrollView_listcan_be_draged { get; set; }

    public bool preb0 { get; set; }
}
