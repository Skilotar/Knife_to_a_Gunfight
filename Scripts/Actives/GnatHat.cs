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

namespace Knives
{
    class GnatHat :PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Diminished Cap";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/deminish_cap";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<GnatHat>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "This Hat Is Way Too Small";
            string longDesc = "Wait, no its not....  Wait! Yes It IS!! \nWait, its not.\n" + 
            "_________________________\n" +
            "This hat will shrink the user and cause them to take double damage. Due to the caps strange size morphing abilities many many items can be fit inside.\n\n" +
            "" +
            "You can not toggle next to the wall." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .25f);


            //Set the rarity of the item

            
            item.quality = PickupObject.ItemQuality.B;
            
        }
       
        bool toggle = true;
        public float BaseSize = 1;
        public float deminishXmain = 0;
        public float deminishYmain = 0;

        public override void Pickup(PlayerController player)
        {
            if(deminishXmain == 0 && deminishYmain == 0)
            {
                float deminishX = (player.transform.localScale.x * .45f);
                float deminishY = (player.transform.localScale.y * .45f);
                deminishXmain = deminishX;
                deminishYmain = deminishY;
            }

            player.GunChanged += LastOwner_GunChanged;

            base.Pickup(player);
        }


        private void LastOwner_GunChanged(Gun arg1, Gun arg2, bool arg3)
        {
            if (toggle == false)
            {
                
                arg2.transform.localScale = new Vector3(1, 1, this.LastOwner.transform.localScale.z);
                arg2.PreventOutlines = true;
                float GUNdeminishX = (arg1.transform.localScale.x * .45f);
                float GUNdeminishY = (arg1.transform.localScale.y * .45f);
                arg1.transform.localScale = new Vector3(GUNdeminishX, GUNdeminishY, this.LastOwner.transform.localScale.z);
                arg1.PreventOutlines = true;

            }
            else
            {
                arg2.PreventOutlines = false;
                arg1.PreventOutlines = false;
            }
           

        }



        public override void  OnPreDrop(PlayerController user)
        {
            if (toggle == false && CanBeUsed(user))
            {
                this.DoEffect(user);
                user.GunChanged -= LastOwner_GunChanged;
            }


            base.OnPreDrop(user);
        }


        public override void  DoEffect(PlayerController user)
        {
            user.secondaryHand.sprite.renderer.enabled = false;
            

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
                if (CanBeUsed(user))
                {
                    //is vry smol
                    user.transform.localScale = new Vector3(deminishXmain, deminishYmain, user.transform.localScale.z);
                    user.specRigidbody.UpdateCollidersOnScale = true;
                    user.specRigidbody.UpdateColliderPositions();
                    toggle = false;
                    this.LastOwner.CurrentGun.transform.localScale = new Vector3(deminishXmain, deminishYmain, this.LastOwner.transform.localScale.z);
                    this.LastOwner.CurrentGun.PreventOutlines = true;


                }

            }
            else
            {
                if (CanBeUsed(user))
                {
                    //is not vry smol
                    user.transform.localScale = new Vector3(BaseSize, BaseSize, user.transform.localScale.z);
                    user.specRigidbody.UpdateColliderPositions();
                    toggle = true;
                    this.LastOwner.CurrentGun.transform.localScale = new Vector3(1, 1, this.LastOwner.transform.localScale.z);
                    this.LastOwner.CurrentGun.PreventOutlines = false;
                }
            }

        }
        public override bool CanBeUsed(PlayerController user)
        {

            //if not in or near wall

            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(user.CenterPosition, VectorConversions.Round));
            CellData cellaim = room.GetNearestCellToPosition(user.CenterPosition);
            CellData cellaimmunis = room.GetNearestCellToPosition(user.CenterPosition - new Vector2(0, 1f));
            if (cellaim.HasWallNeighbor(true, true) == false && cellaimmunis.HasWallNeighbor(true, true) == false)
            {
                return base.CanBeUsed(user) && true;
            }
            else
            {
                return base.CanBeUsed(user) && false;
            }

        }

        public override void Update()
        {

            if (this.LastOwner != null)
            {

                if (CanBeUsed(this.LastOwner))
                {
                    this.CanBeDropped = true;
                }
                else
                {
                    this.CanBeDropped = false;
                }
                if (toggle == false)
                {

                    foreach (var projectile in GetBullets())
                    {
                        projectile.BecomeBlackBullet();
                    }
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 1);
                    this.LastOwner.stats.RecalculateStats(LastOwner, true);

                }
                else
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0);
                    this.LastOwner.stats.RecalculateStats(LastOwner, true);
                }

                RemoveStat(PlayerStats.StatType.AdditionalItemCapacity);
                AddStat(PlayerStats.StatType.AdditionalItemCapacity, this.LastOwner.activeItems.Count + 1);
                this.LastOwner.stats.RecalculateStats(LastOwner, true);


                if (toggle == false)
                {
                    this.LastOwner.CurrentGun.transform.localScale = new Vector3(deminishXmain, deminishYmain, this.LastOwner.transform.localScale.z);
                    

                }
                else
                {
                    this.LastOwner.CurrentGun.transform.localScale =  new Vector3(1, 1, this.LastOwner.transform.localScale.z);
                   
                }

                if (this.LastOwner.HasPassiveItem(BuiltDifferent.itemID))
                {
                    BaseSize = BuiltDifferent.Scaler;
                }
                else
                {
                    BaseSize = 1;
                }

            }
           
                
            base.Update();

        }

      
        private List<Projectile> GetBullets()
        {
           
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable)
                        {
                           
                            list.Add(projectile);
                           
                        }
                    }
                }
            }
            return list;

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
