
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LootTableAPI;
using UnityEngine;
using GungeonAPI;
using Alexandria.NPCAPI;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    public static class ArmsDealer
    {
        public static GenericLootTable ArmLootTable;
        public static void Init()
        {
            
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_GENERIC_TALK", "I Handy. Arms Dealer. Deal Arms.  :)");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_GENERIC_TALK", "Can't Hear Well.");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_GENERIC_TALK", "Huh?");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_STOPPER_TALK", "Trade?");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_STOPPER_TALK", "Parts for parts?");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_STOPPER_TALK", "Parts for parts?");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_STOPPER_TALK", "Trade Parts?");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_PURCHASE_TALK", "Yes! Yes! Good Trade");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_PURCHASE_TALK", "Happy!");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_PURCHASE_TALK", "Enjoy! :D");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_NOSALE_TALK", "No Trade.");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_NOSALE_TALK", "More Health.");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_NOSALE_TALK", "Not Safe.");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_INTRO_TALK", "Friend?");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_INTRO_TALK", "Haii :3");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_INTRO_TALK", "Hellow");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_INTRO_TALK", "Welcom");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_ATTACKED_TALK", "AUGChK!");
                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_ATTACKED_TALK", "Bad!");

                ETGMod.Databases.Strings.Core.AddComplex("#ARMSDEALER_STOLEN_TALK", "???");

            List<int> LootTable = new List<int>()
                {
                    // all items sold based off Body parts/ prostetics handy has found.
                    114, //bionic leg
                    213, //trigger finger
                    190, //rolling eye
                    60, //DemonHead
                    196, //fossil
                    435, // stache
                    186, // machine fist
                    333, // mutation
                    36, // megahand
                    41, // heroine
                    543, // predator
                    576, // robots left
                    33, // jerker

                    //guns
                    Chainsaw.StandardID,//Ash's Chainsaw hand
                    Mares_Leg.ID,//Mare's Leg   
                    Arcane_Hand.ID,//Hand Cannon
                    

                    //items
                    CheatersDrawArm.ID,//Cheater's Draw arm
                    Deft_Hands.ID,//Deft Hands
                    Eye_of_the_tiger.ID,//tiger eye
                    KylesGoatHoof.ID,//Kyle's Goat Hoof
                    BearArms.ID,//Bare arms
                    Guardian_Heart.ID, //Gaurdian Heart
                    //Revolations coil
                    IronStomach.ID,//Iron Stomach
                    MutantLiver.itemID,//Mutant Liver
                    Second_Brain.ID,//Second Brain


                };

                ArmLootTable = LootTableTools.CreateLootTable();
                foreach (int i in LootTable)
                {
                    ArmLootTable.AddItemToPool(i);
                    //ETGModConsole.Log(i.ToString());
                }


            GameObject HandyObj = Alexandria.NPCAPI.ShopAPI.SetUpShop(
                        "Arms_Dealer",//name
                        "ski",//prefix
                        new List<string>()//idle
                        {
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_idle_001",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_idle_002",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_idle_003",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_idle_004",
                        },
                        6,//idle fps
                        new List<string>()// talk
                        {
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_001",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_002",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_003",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_004",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_005",
                                "Knives/Resources/ArmsDealerShop/Handy/Handy_talk_006",
                        },
                        5,//talk fps
                        ArmLootTable,//table
                        Alexandria.NPCAPI.CustomShopItemController.ShopCurrencyType.CUSTOM,//cost
                                                                                                    //talk stuff
                        "#ARMSDEALER_GENERIC_TALK",
                        "#ARMSDEALER_STOPPER_TALK",
                        "#ARMSDEALER_PURCHASE_TALK",
                        "#ARMSDEALER_NOSALE_TALK",
                        "#ARMSDEALER_INTRO_TALK",
                        "#ARMSDEALER_ATTACKED_TALK",
                        "#ARMSDEALER_STOLEN_TALK",

                        //talk stuff
                        new Vector3(.6f, 3.3f, 0),//talk point
                        new Vector3(1.5f,3,0), //NPC posi
                        Alexandria.NPCAPI.ShopAPI.VoiceBoxes.VAMPIRE,
                        Alexandria.NPCAPI.ShopAPI.defaultItemPositions,

                        1f,//costmod
                        false, // stats?
                        null, // stats?
                        HealthTraderCustomCanBuy, // can buy?
                        HealthTraderRemoveCurrency, // remove currency
                        HealthTraderCustomPrice, // custom price
                        null, // on buy
                        null, // Rob
                        "Knives/Resources/HeartCurrency.png", // currency icon
                        "HeartCurrency", // currency
                        true, // can be robbed
                        true, // has carpet
                        "Knives/Resources/ArmsDealerShop/Arms_Shop_carpet",
                        new Vector3(0, 0, 0),// Carpet Offset

                        true,
                        "Knives/Resources/ArmsDealerShop/Handy_Icon",
                        false,
                        .1f
                        ); 

                PrototypeDungeonRoom Mod_Shop_Room = RoomFactory.BuildFromResource("Knives/Resources/ArmsDealerShop/ArmsDealerShop_bigger.room").room;
                RegisterShopRoom(HandyObj, Mod_Shop_Room, new UnityEngine.Vector2(7.5f, 5f),1.1f); // 1.1
                
                HandyGameobject = HandyObj;

        }

        public static void RegisterShopRoom(GameObject shop, PrototypeDungeonRoom protoroom, Vector2 vector, float m_weight = 1)
        {
            protoroom.category = PrototypeDungeonRoom.RoomCategory.NORMAL;
            DungeonPrerequisite[] array = shop.GetComponent<CustomShopController>()?.prerequisites != null ? shop.GetComponent<CustomShopController>().prerequisites : new DungeonPrerequisite[0];
            //Vector2 vector = new Vector2((float)(protoroom.Width / 2) + offset.x, (float)(protoroom.Height / 2) + offset.y);
            protoroom.placedObjectPositions.Add(vector);
            protoroom.placedObjects.Add(new PrototypePlacedObjectData
            {
                contentsBasePosition = vector,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = array,
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = new DungeonPlaceable
                {
                    width = 2,
                    height = 2,
                    respectsEncounterableDifferentiator = true,
                    variantTiers = new List<DungeonPlaceableVariant>
                    {
                        new DungeonPlaceableVariant
                        {
                            percentChance = 1f,
                            nonDatabasePlaceable = shop,
                            prerequisites = array,
                            materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
                        }
                    }
                }
            });
            RoomFactory.RoomData roomData = new RoomFactory.RoomData
            {
                room = protoroom,
                isSpecialRoom = true,
                category = "SPECIAL",
                specialSubCategory = "WEIRD_SHOP",

            };
            roomData.weight = m_weight;
            RoomFactory.rooms.Add(shop.name, roomData);
            DungeonHandler.Register(roomData);

        }

        public static int HealthTraderCustomPrice(CustomShopController shop, CustomShopItemController itemCont, PickupObject item)
        {
            
            int price = 1;
            switch (item.quality)
            {
                case PickupObject.ItemQuality.S:
                    price = 4;
                    break;
                case PickupObject.ItemQuality.A:
                    price = 3;
                    break;
                case PickupObject.ItemQuality.B:
                    price = 2;
                    break;
                case PickupObject.ItemQuality.C:
                    price = 1;
                    break;
            }
            
            return price;
        }
        public static int HealthTraderRemoveCurrency(CustomShopController shop, PlayerController player, int cost)
        {
            float HealthAdjCost = cost / 2f; // gets cost to = the half heart system.

            player.healthHaver.ForceSetCurrentHealth(player.healthHaver.GetCurrentHealth() - HealthAdjCost);
            player.stats.RecalculateStats(player, true);
            return 1;
        }
        public static bool HealthTraderCustomCanBuy(CustomShopController shop, PlayerController player, int cost)
        {
            bool HasEnoughHealth = false;

            float HealthAdjCost = cost / 2f; // gets cost to = the half heart system.
            if (HealthAdjCost < player.healthHaver.GetCurrentHealth()) HasEnoughHealth = true;
           
            return HasEnoughHealth;
        }





        public static GameObject HandyGameobject;
    }
}