/*
 * BinaryReader r = Raw.Params.reader;
        r.BaseStream.Seek(0,0);
        BYTE_HELPER rr = Raw.Params;
        int interval_time = 0;
        int count = 0;
        buttons.Clear();
        screen.button_clear();
        switch ((game_function)Raw.Fuction)
        {
            case game_function.MSG_CUSTOM_MSG:
                {
                    screen.add_log("MSG_CUSTOM_MSG " + Raw.Params.ToString());
                    Player = r.ReadByte();
                    Operator = r.ReadByte();
                    screen.add_log("我是玩家" + Player.ToString() + "的第" + Operator.ToString()+"个操作者");
                }
                break;
            case game_function.MSG_START:
                {
                    int[] lp = new int[2];
                    int[] main_count = new int[2];
                    int[] extra_count = new int[2];
                    lp[0] = r.ReadUInt16();
                    main_count[0] = r.ReadByte();
                    extra_count[0] = r.ReadByte();
                    lp[1] = r.ReadUInt16();
                    main_count[1] = r.ReadByte();
                    extra_count[1] = r.ReadByte();
                    screen.set_lp(if_me(0), lp[0]);
                    screen.set_lp(if_me(1), lp[1]);
                    for (int i = 0; i < main_count[0];i++)
                    {
                        create_card(creat_point(if_me(0), game_location.LOCATION_DECK, get_count(if_me(0), game_location.LOCATION_DECK)));
                    }
                    for (int i = 0; i < main_count[1]; i++)
                    {
                        create_card(creat_point(if_me(1), game_location.LOCATION_DECK, get_count(if_me(1), game_location.LOCATION_DECK)));
                    }
                    for (int i = 0; i < extra_count[0]; i++)
                    {
                        create_card(creat_point(if_me(0), game_location.LOCATION_EXTRA, get_count(if_me(0), game_location.LOCATION_EXTRA)));
                    }
                    for (int i = 0; i < extra_count[1]; i++)
                    {
                        create_card(creat_point(if_me(1), game_location.LOCATION_EXTRA, get_count(if_me(1), game_location.LOCATION_EXTRA)));
                    }
                }
                break;
            case game_function.MSG_QUERY_CODE:
                {
                    long code = r.ReadUInt32();
                    point loc = read_point(r);
                    card target = get_card(loc);
                    if (target == null)
                    {
                        target = create_card(loc);
                    }
                    card_set_code(target,code);
                }
                break;
            case game_function.MSG_MOVE:
                {
                    point p1 = read_point(r);
                    point p2 = read_point(r);
                    screen.add_log(game_info_reader.get_point_string(p1) + ">>>" + game_info_reader.get_point_string(p2));
                    move_card(p1,p2);
                }
                break;
            case game_function.MSG_SHUFFLE_DECK:
                {
                    int player= r.ReadByte();
                    for (int i = 0; i < cards.Count;i++ )
                    {
                        if (cards[i].p.me == if_me(player) && cards[i].p.location==game_location.LOCATION_DECK)
                        {
                            card_erase(cards[i]);
                        }
                    }
                    screen.animation_shuffle_deck(if_me(player));
                }
                break;
            case game_function.MSG_SHUFFLE_HAND:
                {
                    int player = r.ReadByte();
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i].p.me == if_me(player) && cards[i].p.location == game_location.LOCATION_HAND)
                        {
                            card_erase(cards[i]);
                        }
                    }
                    screen.animation_shuffle_deck(if_me(player));
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        card_set_code(get_card(creat_point(if_me(player),game_location.LOCATION_HAND,i)),r.ReadUInt32());
                    }
                }
                break;
            case game_function.MSG_SHUFFLE_SET_CARD:
                {
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        card c = get_card(p);
                        card_erase(c);
                        screen.animation_shuffle_field_card(c.ptr);
                    }
                }
                break;
            case game_function.MSG_WIN:
                {
                    int player = r.ReadByte();
                    int reason = r.ReadByte();
                    screen.add_log("决斗结束！");
                    screen.add_log("胜利玩家：" + player.ToString());
                    screen.add_log("胜利原因：" + reason.ToString());
                }
                break;
            case game_function.MSG_MATCH_KILL:
                {
                    int player = r.ReadByte();
                    screen.add_log("Match Kill！");
                    screen.add_log("胜利玩家：" + player.ToString());
                }
                break;
            case game_function.MSG_ANNOUNCE_RACE:
                {
                    UInt32 available = r.ReadUInt32();
                    count = r.ReadByte();//这里要是出新卡了要注意
                    screen.set_hint("请宣言一个种族！");
                    now = game_function.MSG_SELECT_IDLECMD;
                    if (game_info_reader.differ(available, game_race.RACE_WARRIOR))
                        button_create(null, (int)game_race.RACE_WARRIOR, game_button.select, "战士");
                    if (game_info_reader.differ(available, game_race.RACE_SPELLCASTER))
                        button_create(null, (int)game_race.RACE_SPELLCASTER, game_button.select, "魔法师");
                    if (game_info_reader.differ(available, game_race.RACE_FAIRY))
                        button_create(null, (int)game_race.RACE_FAIRY, game_button.select, "天使");
                    if (game_info_reader.differ(available, game_race.RACE_FIEND))
                        button_create(null, (int)game_race.RACE_FIEND, game_button.select, "恶魔");
                    if (game_info_reader.differ(available, game_race.RACE_ZOMBIE))
                        button_create(null, (int)game_race.RACE_ZOMBIE, game_button.select, "不死");
                    if (game_info_reader.differ(available, game_race.RACE_MACHINE))
                        button_create(null, (int)game_race.RACE_MACHINE, game_button.select, "机械");
                    if (game_info_reader.differ(available, game_race.RACE_AQUA))
                        button_create(null, (int)game_race.RACE_AQUA, game_button.select, "水");
                    if (game_info_reader.differ(available, game_race.RACE_PYRO))
                        button_create(null, (int)game_race.RACE_PYRO, game_button.select, "炎");
                    if (game_info_reader.differ(available, game_race.RACE_ROCK))
                        button_create(null, (int)game_race.RACE_ROCK, game_button.select, "岩石");
                    if (game_info_reader.differ(available, game_race.RACE_WINDBEAST))
                        button_create(null, (int)game_race.RACE_WINDBEAST, game_button.select, "鸟兽");
                    if (game_info_reader.differ(available, game_race.RACE_PLANT))
                        button_create(null, (int)game_race.RACE_PLANT, game_button.select, "植物");
                    if (game_info_reader.differ(available, game_race.RACE_INSECT))
                        button_create(null, (int)game_race.RACE_INSECT, game_button.select, "昆虫");
                    if (game_info_reader.differ(available, game_race.RACE_THUNDER))
                        button_create(null, (int)game_race.RACE_THUNDER, game_button.select, "雷");
                    if (game_info_reader.differ(available, game_race.RACE_DRAGON))
                        button_create(null, (int)game_race.RACE_DRAGON, game_button.select, "龙");
                    if (game_info_reader.differ(available, game_race.RACE_BEAST))
                        button_create(null, (int)game_race.RACE_BEAST, game_button.select, "兽");
                    if (game_info_reader.differ(available, game_race.RACE_BEASTWARRIOR))
                        button_create(null, (int)game_race.RACE_BEASTWARRIOR, game_button.select, "兽战士");
                    if (game_info_reader.differ(available, game_race.RACE_DINOSAUR))
                        button_create(null, (int)game_race.RACE_DINOSAUR, game_button.select, "恐龙");
                    if (game_info_reader.differ(available, game_race.RACE_FISH))
                        button_create(null, (int)game_race.RACE_FISH, game_button.select, "鱼");
                    if (game_info_reader.differ(available, game_race.RACE_SEASERPENT))
                        button_create(null, (int)game_race.RACE_SEASERPENT, game_button.select, "海龙");
                    if (game_info_reader.differ(available, game_race.RACE_REPTILE))
                        button_create(null, (int)game_race.RACE_REPTILE, game_button.select, "爬虫");
                    if (game_info_reader.differ(available, game_race.RACE_PSYCHO))
                        button_create(null, (int)game_race.RACE_PSYCHO, game_button.select, "念动力");
                    if (game_info_reader.differ(available, game_race.RACE_DEVINE))
                        button_create(null, (int)game_race.RACE_DEVINE, game_button.select, "幻神兽");
                    if (game_info_reader.differ(available, game_race.RACE_CREATORGOD))
                        button_create(null, (int)game_race.RACE_CREATORGOD, game_button.select, "创造神");
                    if (game_info_reader.differ(available, game_race.RACE_PHANTOMDRAGON))
                        button_create(null, (int)game_race.RACE_PHANTOMDRAGON, game_button.select, "幻龙");
                }
                break;
            case game_function.MSG_ANNOUNCE_ATTRIB:
                {
                    UInt32 available = r.ReadUInt32();
                    count = r.ReadByte();//这里要是出新卡了要注意
                    screen.set_hint("请宣言一个属性！");
                    now = game_function.MSG_SELECT_IDLECMD;
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_EARTH))
                        button_create(null, (int)game_attributes.ATTRIBUTE_EARTH, game_button.select, "地");
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_WATER))
                        button_create(null, (int)game_attributes.ATTRIBUTE_WATER, game_button.select, "水");
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_FIRE))
                        button_create(null, (int)game_attributes.ATTRIBUTE_FIRE, game_button.select, "炎");
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_WIND))
                        button_create(null, (int)game_attributes.ATTRIBUTE_WIND, game_button.select, "风");
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_DARK))
                        button_create(null, (int)game_attributes.ATTRIBUTE_DARK, game_button.select, "暗");
                    if (game_info_reader.differ(available, game_attributes.ATTRIBUTE_DEVINE))
                        button_create(null, (int)game_attributes.ATTRIBUTE_DEVINE, game_button.select, "神");
                }
                break;
            case game_function.MSG_ANNOUNCE_CARD:
                {
                    UInt32 available = r.ReadUInt32();
                    //count = r.ReadByte();//这里要是出新卡了要注意
                    screen.set_hint("请宣言一个卡片！");
                    now = game_function.MSG_ANNOUNCE_CARD;
                    screen.query_announce_code();
                }
                break;
            case game_function.MSG_ANNOUNCE_NUMBER:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    count = r.ReadByte();
                    screen.set_hint("请宣言一个数字！");
                    for (int i = 0; i < count; i++)
                    {
                        int number = r.ReadInt32();
                        button_create(null, i, game_button.select, number.ToString());
                    }
                }
                break;
            case game_function.MSG_SELECT_IDLECMD:
                {
                    screen.set_hint("现在是主要阶段，请选择操作。");
                    now = game_function.MSG_SELECT_IDLECMD;
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 0),game_button.summon,"通常召唤");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 1), game_button.spsummon, "特殊召唤");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 2), game_button.change, "改变表示形式");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 3), game_button.set, "放置");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 4), game_button.set, "放置");
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        button_create(get_card(p), ((i << 16) + 5), game_button.active, "发动 " + get_string(description));
                    }
                    byte bp = r.ReadByte();
                    byte ep = r.ReadByte();
                    byte shuffle = r.ReadByte();
                    if (bp == 1) button_create(null,6,game_button.battle_phrase,"战斗阶段");
                    if (ep == 1) button_create(null, 7, game_button.end_phrase, "结束阶段");
                    if (shuffle == 1) button_create(null, 8, game_button.shuffle, "洗牌");
                }
                break;
            case game_function.MSG_SELECT_BATTLECMD:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    screen.set_hint("现在是战斗阶段，请选择操作。");
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        button_create(get_card(p), ((i << 16) + 0), game_button.active, "发动 " + get_string(description));
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), ((i << 16) + 1), game_button.attack, "攻击");
                    }
                    byte m2 = r.ReadByte();
                    byte ep = r.ReadByte();
                    if (m2 == 1) button_create(null, 2, game_button.battle_phrase, "主要阶段2");
                    if (ep == 1) button_create(null, 3, game_button.battle_phrase, "结束阶段");
                }
                break;
            case game_function.MSG_SELECT_EFFECTYN:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    screen.set_hint("是否连锁！");
                    point p = read_point(r);
                    button_create(get_card(p), 1, game_button.chain, "发动");
                    button_create(null, 0, game_button.no, "取消");
                }
                break;
            case game_function.MSG_SELECT_YESNO:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    UInt32 description = r.ReadUInt32();
                    screen.set_hint("是否发动 " + get_string(description));
                    point p = read_point(r);
                    button_create(null, 1, game_button.chain, "发动");
                    button_create(null, 0, game_button.no, "取消");
                }
                break;
            case game_function.MSG_SELECT_OPTION:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    screen.set_hint("选择一个效果！");
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        UInt32 description = r.ReadUInt32();
                        button_create(null, i, game_button.select, get_string(description));
                    }
                }
                break;
            case game_function.MSG_SELECT_CHAIN:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    screen.set_hint("请选择连锁！");
                    int spe_count = r.ReadByte();
                    int forced = r.ReadByte();
                    count = r.ReadByte();
                    if (spe_count > 0)
                    {
                        if (screen.ignore_some_chains)
                        {
                            button_clicked(-1);
                        }
                        else
                        {
                            button_create(null, -1, game_button.no, "取消");
                        }
                    }
                    else
                    {
                        if (forced > 0)
                        {
                        }
                        else
                        {
                            button_create(null, -1, game_button.no, "取消");
                        }
                    }

                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        UInt32 description = r.ReadUInt32();
                        button_create(get_card(p), (i), game_button.chain, "发动 " + get_string(description));
                    }
                }
                break;
            case game_function.MSG_SELECT_PLACE:
                {
                    //无视
                }
                break;
            case game_function.MSG_SELECT_POSITION:
                {
                    now = game_function.MSG_SELECT_IDLECMD;
                    UInt32 code = r.ReadUInt32();
                    UInt32 positions = r.ReadUInt32();
                    screen.set_hint("请宣言一个表示形式！");
                    if ((positions & 0x1) > 0)
                    {
                        button_create(null, 1, game_button.POS_FACEUP_ATTACK, code.ToString());
                    }
                    if ((positions & 0x2) > 0)
                    {
                        button_create(null, 2, game_button.POS_FACEDOWN_ATTACK, code.ToString());
                    }
                    if ((positions & 0x4) > 0)
                    {
                        button_create(null, 4, game_button.POS_FACEUP_DEFENSE, code.ToString());
                    }
                    if ((positions & 0x8) > 0)
                    {
                        button_create(null, 8, game_button.POS_FACEDOWN_DEFENSE, code.ToString());
                    }
                }
                break;
            case game_function.MSG_SELECT_COUNTER:
                {
                    now = game_function.MSG_SELECT_COUNTER;
                    screen.set_hint("选择指示物！");
                    UInt32 countertype = r.ReadUInt32();
                    int all = r.ReadByte();
                    count = r.ReadByte();
                    //write_line("指示物代码：" + countertype.ToString());
                   // write_line("总共需要选择" + all.ToString() + "个指示物");
                    //write_line("总共有" + all.ToString() + "张卡");
                    for (int i = 0; i < count; i++)
                    {
                       // long code = r.ReadUInt32();
                       // locator loc = reader_get_locator(r);
                        //int size = r.ReadByte();
                        //write_line(i.ToString() + ":逐个输入/u8/:" + get_name_string(code) + get_locator_string(loc) + "，这张卡有" + size.ToString() + "个指示物");
                    }
                }
                break;
            case game_function.MSG_SELECT_CARD:
                {
                    MSG_SELECT_CARD_cards.Clear();
                    now = game_function.MSG_SELECT_CARD;
                    screen.set_hint("请选择卡片！");
                    MSG_SELECT_CARD_min = r.ReadByte();
                    MSG_SELECT_CARD_max = r.ReadByte();
                    MSG_SELECT_CARD_level = r.ReadByte();
                    if (MSG_SELECT_CARD_level == 0) MSG_SELECT_CARD_level = 99999;
                    bool stars = false;
                    if (MSG_SELECT_CARD_level < 100) stars = true;
                    int cancelable = r.ReadByte();
                    if (cancelable == 1)
                    {
                        button_create(null, -1, game_button.no, "取消");
                    }
                    button_create(null, 9999, game_button.no, "完成");
                    MSG_SELECT_CARD_must = r.ReadByte();
                    for (int i = 0; i < MSG_SELECT_CARD_must; i++)
                    {
                        point p = read_point(r);
                        screen.card_selected(get_card(p).ptr,stars);
                    }
                    count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        point p = read_point(r);
                        if (get_card(p) != null)
                        {
                            if (stars)
                            {
                                button_create(get_card(p), (i), game_button.select, get_card(p).data.Level.ToString());
                                get_card(p).cookie_2 = i;
                            }
                            else
                            {
                                button_create(get_card(p), (i), game_button.select, "");
                                get_card(p).cookie_2 = i;
                            }
                        }
                        
                    }
                }
                break;
            case game_function.MSG_SORT_CARD:
                {
                    MSG_SORT_CARD_cards.Clear();
                    now = game_function.MSG_SORT_CARD;
                    screen.set_hint("请给卡片排序！");
                    MSG_SORT_CARD_count = r.ReadByte();
                    button_create(null, 9999, game_button.no, "取消");
                    for (int i = 0; i < MSG_SORT_CARD_count; i++)
                    {
                        point p = read_point(r);
                        button_create(get_card(p), (i), game_button.select, "");
                        get_card(p).cookie_2 = i;
                    }
                }
                break;
            case game_function.MSG_CONFIRM_CARDS:
                {
                    long code = r.ReadUInt32();
                    point loc = read_point(r);
                    card target = get_card(loc);
                    if (target == null)
                    {
                        target = create_card(loc);
                    }
                    card_set_code(target, code);
                    screen.confirm_card(target.ptr);
                }
                break;
            case game_function.MSG_SWAP_GRAVE_DECK:
                {
                    int player = r.ReadByte();
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i].p.me == if_me(player))
                        {
                            if (cards[i].p.location == game_location.LOCATION_DECK)
                            {
                                cards[i].p.location = game_location.LOCATION_UNKNOWN;
                            }
                            if (cards[i].p.location == game_location.LOCATION_GRAVE)
                            {
                                cards[i].p.location = game_location.LOCATION_DECK;
                            }
                            if (cards[i].p.location == game_location.LOCATION_UNKNOWN)
                            {
                                cards[i].p.location = game_location.LOCATION_DECK;
                            }
                        }
                    }
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (
                            cards[i].prep.me != cards[i].p.me
                            ||
                            cards[i].prep.index != cards[i].p.index
                            ||
                            cards[i].prep.location != cards[i].p.location
                            ||
                            cards[i].prep.position != cards[i].p.position
                            ||
                            cards[i].prep.overlay_index != cards[i].p.overlay_index
                            )
                        {

                            screen.card_set_point(cards[i].ptr, cards[i].p);
                            screen.add_log(game_info_reader.get_point_string(cards[i].prep) + "->" + game_info_reader.get_point_string(cards[i].p));
                            cards[i].prep = cards[i].p;
                        }

                    }
                }
                break;
            case game_function.MSG_REVERSE_DECK:
                {
                    for (int i = 0; i < cards.Count; i++)
                    {
                            if (cards[i].p.location == game_location.LOCATION_DECK)
                            {
                                if (cards[i].p.position == game_position.POS_FACEUP_ATTACK)
                                    cards[i].p.position = game_position.POS_FACEDOWN_ATTACK;
                                if (cards[i].p.position == game_position.POS_FACEDOWN_ATTACK)
                                    cards[i].p.position = game_position.POS_FACEUP_ATTACK;
                                if (cards[i].p.position == game_position.POS_FACEUP_DEFENSE)
                                    cards[i].p.position = game_position.POS_FACEDOWN_DEFENSE;
                                if (cards[i].p.position == game_position.POS_FACEUP_DEFENSE)
                                    cards[i].p.position = game_position.POS_FACEDOWN_DEFENSE;
                            }
                            
                    }
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (
                            cards[i].prep.me != cards[i].p.me
                            ||
                            cards[i].prep.index != cards[i].p.index
                            ||
                            cards[i].prep.location != cards[i].p.location
                            ||
                            cards[i].prep.position != cards[i].p.position
                            ||
                            cards[i].prep.overlay_index != cards[i].p.overlay_index
                            )
                        {

                            screen.card_set_point(cards[i].ptr, cards[i].p);
                            screen.add_log(game_info_reader.get_point_string(cards[i].prep) + "->" + game_info_reader.get_point_string(cards[i].p));
                            cards[i].prep = cards[i].p;
                        }

                    }
                }
                break;
            case game_function.MSG_NEW_TURN:
                {
                    int player = r.ReadByte();
                    screen.new_turn(if_me(player));
                }
                break;
            case game_function.MSG_NEW_PHASE:
                {
                    int available = r.ReadUInt16();
                    if (game_info_reader.differ(available, game_phrases.PHASE_BATTLE))
                        screen.set_phrase("战斗阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_BATTLE_START))
                        screen.set_phrase("战斗阶段开始");
                    if (game_info_reader.differ(available, game_phrases.PHASE_BATTLE_STEP))
                        screen.set_phrase("伤害步骤阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_DAMAGE))
                        screen.set_phrase("伤害计算阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_DAMAGE_CAL))
                        screen.set_phrase("伤害判定阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_DRAW))
                        screen.set_phrase("抽卡阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_END))
                        screen.set_phrase("结束阶段");
                    if (game_info_reader.differ(available, game_phrases.PHASE_MAIN1))
                        screen.set_phrase("主要阶段1");
                    if (game_info_reader.differ(available, game_phrases.PHASE_MAIN2))
                        screen.set_phrase("主要阶段2");
                    if (game_info_reader.differ(available, game_phrases.PHASE_STANDBY))
                        screen.set_phrase("准备阶段");
                }
                break;
            case game_function.MSG_FIELD_DISABLED:
                {
                    UInt32 available = r.ReadUInt32();
                    UInt32 fliter = 1;
                    for (int i = 0; i < 32; i++)
                    {
                            if (i >= 0 && i < 5)
                            {
                                if ((available & fliter) > 0)
                                {
                                    screen.refresh_field(creat_point(if_me(0),game_location.LOCATION_MZONE,i-0),true);
                                }
                                else
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_MZONE, i - 0), false);
                                }
                            }
                            if (i >= 8 && i < 16)
                            {
                                if ((available & fliter) > 0)
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_SZONE, i - 8), true);
                                }
                                else
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_SZONE, i - 8), false);
                                }
                            }
                            if (i >= 16 && i < 21)
                            {
                                if ((available & fliter) > 0)
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_MZONE, i - 16), true);
                                }
                                else
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_MZONE, i - 16), false);
                                }
                            }
                            if (i >= 24 && i < 32)
                            {
                                if ((available & fliter) > 0)
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_SZONE, i - 24), true);
                                }
                                else
                                {
                                    screen.refresh_field(creat_point(if_me(0), game_location.LOCATION_SZONE, i - 24), false);
                                }
                            }
                       
                        fliter = fliter << 1;
                    }
                }
                break;
            case game_function.MSG_SUMMONING:
                {
                    point loc = read_point(r);
                    screen.animation_summon(loc);
                }
                break;
            case game_function.MSG_SPSUMMONING:
                {
                    point loc = read_point(r);
                    screen.animation_spsummon(loc);
                }
                break;
            case game_function.MSG_FLIPSUMMONING:
                {
                    point loc = read_point(r);
                    screen.animation_flipsummon(loc);
                }
                break;
            case game_function.MSG_CHAINING:
                {
                    point loc = read_point(r);
                    card c = get_card(loc);
                    screen.animation_card_chaining(c.ptr);
                }
                break;
            case game_function.MSG_CHAIN_SOLVED:
                {
                    int index = r.ReadByte();
                    screen.animation_card_chaining_clear_one(index);
                }
                break;
            case game_function.MSG_CHAIN_END:
                {
                    screen.animation_card_chaining_clear_all();
                }
                break;
        }
        return interval_time;
 */
