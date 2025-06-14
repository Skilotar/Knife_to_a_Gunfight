using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class TwitchingEye : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Twitching Eye";

            string resourceName = "Knives/Resources/Twitching_eye";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<TwitchingEye>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Myokymia";
            string longDesc = "Gain Homing and Damage, but become afflicted with High Stress.\n\n" +
                "The eye of a cursed gambler who saw it more fit to cut out his eye than to have a tell." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");


            item.CanBeDropped = false;
            

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .30f);

            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gain Homing and Damage, but become afflicted with High Stress";

        }
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            player.OnReceivedDamage += Player_OnReceivedDamage;
            player.PostProcessProjectile += Player_PostProcessProjectile;
            player.PostProcessBeamChanceTick += Player_PostProcessBeamChanceTick; 
            base.Pickup(player);
        }

        private void Player_PostProcessBeamChanceTick(BeamController obj)
        {
            obj.ChanceBasedHomingRadius = 10;
            obj.ChanceBasedHomingAngularVelocity = 540;
        }

        private void Player_PostProcessProjectile(Projectile obj, float arg2)
        {
            HomingModifier homingModifier = obj.gameObject.GetComponent<HomingModifier>();
            if(homingModifier == null)
            {
                homingModifier = obj.gameObject.AddComponent<HomingModifier>();
                homingModifier.HomingRadius = 0f;
                homingModifier.AngularVelocity = 0f;
            }
            homingModifier.HomingRadius = 8;
            homingModifier.AngularVelocity = 540;
        }

        private void Player_OnReceivedDamage(PlayerController player)
        {
            player.TriggerHighStress(2.5f);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            player.PostProcessBeamChanceTick -= Player_PostProcessBeamChanceTick;
            return base.Drop(player);
        }
        public override void Update()
        {
            if(this.Owner != null)
            {

            }
            base.Update();
            
        }
       
    }
}
