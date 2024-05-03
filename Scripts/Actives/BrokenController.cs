using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;

namespace Knives
{
    class Broken_controller : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Broken Controller";

            string resourceName = "Knives/Resources/Broken_Controller";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Broken_controller>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Perfectly Wrong";
            string longDesc =
                "Use while dodgerolling to cancel dodges into a wave landing. \n\n" +

                "A controller with a specific defect that allows it to do the Koopa-backdash wave-side hover-walk moon-landing a whole 2 frames faster than other controllers! This will help anyone win a fighting game tournament.\n\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .5f);
            item.usableDuringDodgeRoll = true;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public static int ID;
        
        public override void DoEffect(PlayerController player)
        {
            if (player != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Dodge_01", base.gameObject);
                player.ForceStopDodgeRoll();
                player.ToggleGunRenderers(true);
                player.ToggleHandRenderers(true);
                player.StartCoroutine(HandleDash(player));
               
            }
        }
        public float LoopTimer;
        public IEnumerator HandleDash(PlayerController user)
        {

            float duration = .12f;
            float adjSpeed = 25;
            float elapsed = -BraveTime.DeltaTime;
            float angle = user.NonZeroLastCommandedDirection.ToAngle();
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.LastOwner.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle).normalized * adjSpeed;
                yield return null;
            }

        }

        protected void EndEffect(PlayerController user)
        {

        }
        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.IsDodgeRolling && !user.IsSlidingOverSurface;
        }


        public override void Update()
        {

            if(this.LastOwner != null)
            {

                this.CanBeUsed(this.LastOwner);
                
               
                
            }
            base.Update();
        }


    }
}