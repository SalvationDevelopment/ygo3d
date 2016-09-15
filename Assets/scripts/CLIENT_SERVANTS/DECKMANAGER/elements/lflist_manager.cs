using System;
using System.Collections.Generic;
using UnityEngine;
public struct lflist_line
{
    public long code;
    public int max;
}
public class Lflist
{
    public List<lflist_line> lines = new List<lflist_line>();
    public string name="";
}
public class Lflist_manager
{
    public List<Lflist> Lflists = new List<Lflist>();
    public Lflist_manager()
    {
        string text = System.IO.File.ReadAllText("lflist.txt");
        string st = text.Replace("\r", "");
        string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        Lflist current_writing_Lflist = new Lflist();
        current_writing_Lflist.name = "无";
        for (int index_1 = 0; index_1 < lines.Length; index_1++)
        {
            try
            {
                string line = lines[index_1];
                if (line.Length > 0)
                {
                    if (line[0] == '!')
                    {
                        Lflists.Add(clone_Lflist(current_writing_Lflist));
                        current_writing_Lflist = new Lflist();
                        current_writing_Lflist.name = line.Substring(1, line.Length - 1);
                    }
                    else
                    {
                        string[] mats = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (mats.Length >= 2)
                        {
                            lflist_line lflist_line_a = new lflist_line();
                            lflist_line_a.code = long.Parse(mats[0]);
                            lflist_line_a.max = int.Parse(mats[1]);
                            current_writing_Lflist.lines.Add(lflist_line_a);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                debugger.Log(e);
            }
           
        }
        Lflists.Add(clone_Lflist(current_writing_Lflist));
    }
    Lflist clone_Lflist(Lflist in_)
    {
        Lflist out_ = new Lflist();
        out_.name=in_.name;
        foreach(lflist_line l in in_.lines){
            out_.lines.Add(l);
        }
        return out_;
    }
    public Lflist get_list(string name)
    {
        Lflist re = new Lflist();
        for (int i = 0; i < Lflists.Count; i++)
        {
            if (Lflists[i].name == name)
            {
                re = Lflists[i];
            }
        }
        return re;
    }
}
