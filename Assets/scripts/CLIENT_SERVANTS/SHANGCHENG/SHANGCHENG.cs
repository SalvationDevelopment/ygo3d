using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SHANGCHENG
{
    CLIENT father;
    lazy_shangcheng shangcheng;
    public SHANGCHENG(CLIENT f)
    {
        father = f;
        shangcheng = father.create_game_object(father.loader.mod_shangcheng,Vector3.zero,Quaternion.identity,father.ui_main_2d).GetComponent<lazy_shangcheng>();
        shangcheng.client = father;
        shangcheng.shangceng = this;
        shangcheng.GetComponent<lazy_shangcheng>().ini();
        debug();

        GiveButtonFunction(shangcheng.GetComponent<lazy_shangcheng>().exit_btn, kill_oneself);
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
        MonoBehaviour.Destroy(shangcheng.gameObject);
    }


    public class shangping
    {
        public string name;
        public string tip;
        public int id;
        public int value;
        public Texture2D pic;
    }


    void debug()
    {
        List<shangping> goods = new List<shangping>();

        shangping shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);

        shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);
        shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);
        shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);
        shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);
        shangping = new shangping();
        shangping.name = "miao";
        shangping.tip = "miao";
        shangping.id = 1;
        shangping.value = 3000;

        goods.Add(shangping);


        shangcheng.shuabiao(goods, shangcheng.gird_1);
        shangcheng.shuabiao(goods, shangcheng.gird_2);
        shangcheng.shuabiao(goods, shangcheng.gird_3);
        shangcheng.shuabiao(goods, shangcheng.gird_4);

    }

    GameObject tishi;
    public void shangping_clicked(shangping good){
        if (tishi==null)
        {
            tishi = father.create_game_object(father.loader.mod_shangcheng_queren, Vector3.zero, Quaternion.identity, father.ui_main_2d);
            GiveButtonFunction(tishi.GetComponent<lazy_queren>().y, yes);
            GiveButtonFunction(tishi.GetComponent<lazy_queren>().n, no);
            tishi.GetComponent<lazy_queren>().tishi.text = "是否要购买" + good.name + "？";
            tishi.transform.position = father.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width/2,Screen.height/2-200,0));
        }
        
    }

    void yes()
    {
        MonoBehaviour.Destroy(tishi);
    }

    void no()
    {
        MonoBehaviour.Destroy(tishi);
    }
   



}
