
using NpcApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LootTableAPI;
using UnityEngine;
using GungeonAPI;

using System.Reflection;


namespace Knives
{
    public static class Morgun
    {
        public static GenericLootTable JinxLootTable;
        public static void Init()
        {
            try
            {
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "I am Morgun la Faye, apprentice witch and professional mischief maker.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "I am Morgun la Faye, apprentice witch and professional mischief maker.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "Most of this is just practice for my Occult Sciences Degree.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "Have you seen my cat Salem? I hope everyone else is alright..");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "Why do I have a gun? Well, incase I misspell of course. haha- miss. spell. Get it?");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "The case of the gundead is my major reseach topic for my necromancy 231 class.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "Can I borrow some blood?");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "I'm not a very good witch yet, most of what I sell has side effects.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "My mama and her sisters are around here somewhere.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "I've got a few tricks in my sleeves. By tricks I mean guns.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "This place could use some spicing up.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_GENERIC_TALK", "They put me on trial a long time ago, Luckily I'm heavier than a duck.");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOPPER_TALK", "Busy on the next one.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOPPER_TALK", "Gotta figure what I'm making next.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOPPER_TALK", "Can't talk gotta focus.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOPPER_TALK", "Hexes don't make themselves!");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_PURCHASE_TALK", "Stay Safe!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_PURCHASE_TALK", "Remeber to be careful with that.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_PURCHASE_TALK", "Watchout for the side effects!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_PURCHASE_TALK", "Enjoy! -no refunds-");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_PURCHASE_TALK", "Practice safe Hex. Haha -no but, really don't kill yourself with that.");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_NOSALE_TALK", "College Debts don't pay themselves.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_NOSALE_TALK", "Can't just give it away.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_NOSALE_TALK", "Sorry dude. It ain't free.");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "Hail to you!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "I've got hexes, vexes, and oddities galore!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "You wanna see something wierd?");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "Hey, its you! You're still alive!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "Bet you've never seen one of these before.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_INTRO_TALK", "Hey, hows it goin?");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_ATTACKED_TALK", "Careful!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_ATTACKED_TALK", "I'll have you turned into a newt for that.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_ATTACKED_TALK", "Watch it!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_ATTACKED_TALK", "Dude...   Uncool.");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_ATTACKED_TALK", "Don't tempt me.");

                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOLEN_TALK", "Saw that!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOLEN_TALK", "Not today!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOLEN_TALK", "We got a WISE GUY!");
                ETGMod.Databases.Strings.Core.AddComplex("#MORGUN_STOLEN_TALK", "Cheap tricks? I've got some too!");

                List<int> LootTable = new List<int>()
                {

                   Book_of_misspelled_spells.itemID,
                   Cacophony.itemID,
                   Fallen_armor.itemID,
                   MiniGlockodileHeart.itemID,
                   HexingRounds.itemID,
                   HexGlasses.itemID,
                   LifeInsure.itemID,
                   loan.itemID,
                   Sundail.itemID,
                   StonePotion.itemID,
                   WickerHeart.itemID,
                   WitheringRose.itemID,
                   FingerTrap.ID,
                   //nightmare_mode.itemID,
                   clean_soul.itemID,
                   Cheshire_purfume.itemID,
                   DiogenesCoinpurse.itemID,
                   HellBound.itemID,
                   Greedy.ID,
                   Ammochemy_belt.ID,
                   ChainFire_Reagent.itemID,
                   BuiltDifferent.itemID,
                   PlasmaBuckler.ID,

                };

                JinxLootTable = LootTableTools.CreateLootTable();
                foreach (int i in LootTable)
                {
                    JinxLootTable.AddItemToPool(i);
                    //ETGModConsole.Log(i.ToString());
                }


                GameObject MorgunObj = ItsDaFuckinShopApi.SetUpShop(
                            "Morgun_La_Faye",//name
                            "ski",//prefix
                            new List<string>()//idle
                            {
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_idle_001",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_idle_002",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_idle_003",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_idle_004",

                            },
                            7,//idle fps
                            new List<string>()// talk
                            {
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_talk_001",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_talk_002",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_talk_003",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_talk_004",
                            },
                            4,//talk fps
                            JinxLootTable,//table
                            CustomShopItemController.ShopCurrencyType.COINS,//cost
                                                                            //talk stuff
                            "#MORGUN_GENERIC_TALK",
                            "#MORGUN_STOPPER_TALK",
                            "#MORGUN_PURCHASE_TALK",
                            "#MORGUN_NOSALE_TALK",
                            "#MORGUN_INTRO_TALK",
                            "#MORGUN_ATTACKED_TALK",
                            "#MORGUN_STOLEN_TALK",
                            
                            //talk stuff
                            new Vector3(.6f, 3.3f, 0),//talk point

                            ItsDaFuckinShopApi.defaultItemPositions,
                            
                            .40f,//costmod
                            false, // stats?
                            null, // stats?
                            null, // can buy?
                            null, // remove currency
                            null, // custom price
                            null, // on buy

                            null, // don't rob morgun   >:(
                            null, // currecny
                            null, // currency
                            true, // can be robbed
                            true, // has carpet
                            "Knives/Resources/JinxShop/Jinx_Shop_carpet",
                            true,
                            "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_MapIcon",
                            false,
                            .1f,
                            null,
                            new List<string>()// hex
                            {
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_001",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_002",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_003",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_004",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_005",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_006",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_007",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_008",
                                "Knives/Resources/JinxShop/MorgunLaFaye/Morgun_hex_009",
                            },
                            10

                            ); ;

                PrototypeDungeonRoom Mod_Shop_Room = RoomFactory.BuildFromResource("Knives/Resources/JinxShop/JinxShop_bigger.room").room;
                ItsDaFuckinShopApi.RegisterShopRoom(MorgunObj, Mod_Shop_Room, new UnityEngine.Vector2(8f, 5.5f),3.1f); // 3.6
                MorgunGameobject = MorgunObj;

            }
            catch(Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }
           
        }

      
        public static GameObject MorgunGameobject;
    }
}