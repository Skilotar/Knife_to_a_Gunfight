using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using System.Runtime.CompilerServices;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;

namespace Knives
{
    class noclip : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Devtool_Noclip";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/no clip";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<noclip>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "noclip";
            string longDesc = "That sign wont stop me because I can't read! Dont Pickup or drop Guns while small";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .25f);


            //Set the rarity of the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 4, StatModifier.ModifyMethod.ADDITIVE);
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }

        bool toggle = true;
        public float OrigX;
        public float OrigY = 0;

       
        public override void  DoEffect(PlayerController user)
        {
            user.secondaryHand.sprite.renderer.enabled = false;
            var X = user.transform.localScale.x;
            var Y = user.transform.localScale.y;

            if (OrigY == 0)
            {
                OrigX = X;
                OrigY = Y;
            }
            var deminishX = (user.transform.localScale.x * .10f);
            var deminishY = (user.transform.localScale.y * .10f);
           
            //l
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(user.CenterPosition, VectorConversions.Round));
            CellData cellaim = room.GetNearestCellToPosition(user.CenterPosition);

            // Oh hey there,
            // I see you looking through my code...
            // its okay you can stay
            // we're all friends here.
            // take whatever you need
            // alright have a good day,
            // see you later   
            //- Skilotar_

            if (toggle)
            {
                if (cellaim.isNextToWall == false)
                {
                    user.transform.localScale = new Vector3(deminishX, deminishY, user.transform.localScale.z);


                   
                    user.specRigidbody.UpdateCollidersOnScale = true;
                    user.specRigidbody.UpdateColliderPositions();
                    toggle = false;
                    
                   
                }

            }
            else
            {
                if (cellaim.isNextToWall == false)
                {
                    user.transform.localScale = new Vector3(OrigX, OrigY, user.transform.localScale.z);
                   

                    user.specRigidbody.UpdateColliderPositions();

                    toggle = true;
                   
                }
            }

        }
        
        public override void Update()
        {
           
           

            RemoveStat(PlayerStats.StatType.AdditionalItemCapacity);
            AddStat(PlayerStats.StatType.AdditionalItemCapacity, this.LastOwner.activeItems.Count + 1);
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
           
            base.Update();
        }
      

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }

    }
}