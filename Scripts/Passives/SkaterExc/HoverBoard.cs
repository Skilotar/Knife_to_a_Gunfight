using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;
using SaveAPI;

namespace Knives
{
    class HoverBoard : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Hover Board";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/HoverBoard";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HoverBoard>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Works on Water!";
            string longDesc = "Rolling over bullets allows an additional roll\n\n" +
                "New and improved! The HoverBoard Ultra has enough power to function at high speeds, Even over liquids!" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.C;

            ID = item.PickupObjectId;
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.SKATER_HOVER, true);


        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.OnDodgedProjectile += this.OnDodgedProjectile;
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;
           
            base.Pickup(player);
        }


        public bool CurrentlyRolling = false;
        public bool Onebullet = false;
        private void Player_OnPreDodgeRoll(PlayerController obj)
        {
           
            CurrentlyRolling = true;
            Onebullet = false;
        }

        private void OnDodgedProjectile(Projectile projectile)
        {
            if (this.Owner != null)
            {
                if (projectile.Owner != this.Owner)
                {
                    if (CurrentlyRolling == true && Onebullet == false)
                    {
                        FreeCancelState = true;
                    }
                }

            }

        }

       

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnDodgedProjectile -= this.OnDodgedProjectile;
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
           
            return base.Drop(player);
        }
        public bool FreeCancelState = false;
        public override void Update()
        {
            if (this.Owner != null && Time.timeScale != 0)
            {
                if (CurrentlyRolling == true)// roll was initiated
                {
                    if (FreeCancelState)
                    {
                        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX);
                        if (instanceForPlayer.ActiveActions.DodgeRollAction.WasPressed && !GameManager.Instance.IsPaused && Owner.LastCommandedDirection != null)
                        {
                            StartCoroutine(RollCancel());
                            FreeCancelState = false;
                        }
                    }
                   
                }

            }
            base.Update();
        }

        private IEnumerator RollCancel()
        {
            Owner.FallingProhibited = true;
            Owner.ForceStopDodgeRoll();
            Owner.ForceStartDodgeRoll();
            BraveTime.SetTimeScaleMultiplier(0, base.gameObject);
            yield return new WaitForSecondsRealtime(.15f);
            BraveTime.SetTimeScaleMultiplier(1, base.gameObject);
            Owner.FallingProhibited = false;
        }
    }
}