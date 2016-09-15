using System;
using System.Collections.Generic;
using UnityEngine;

public class DECK_MANAGER_DECK
{
    public class CARD_IN_DECK
    {
        public long code;
        public GameObject game_object;
    }
    public List<CARD_IN_DECK> main = new List<CARD_IN_DECK>();
    public List<CARD_IN_DECK> extra = new List<CARD_IN_DECK>();
    public List<CARD_IN_DECK> side = new List<CARD_IN_DECK>();
    public List<CARD_IN_DECK> lancer = new List<CARD_IN_DECK>();
}
