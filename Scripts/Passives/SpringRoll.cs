using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;


namespace Knives
{
    class Spring_roll :PassiveItem
    {


        public static void Register()
        {
            //The name of the item
            string itemName = "Spring Roll";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Spring_roll";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Spring_roll>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Awful";
            string longDesc = "This roll contains a literal spring. A coil of metal wraps around the seaweed wrap squeazing the roll tighter. \n\n" +
            "The crunchy taste is, unsuprisingly, an awful experience. At least you now have a spring in your step." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollSpeedMultiplier, .2f, StatModifier.ModifyMethod.ADDITIVE);
            

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;


        }
        public override void Pickup(PlayerController player)
        {
            PassiveItem.IncrementFlag(player, typeof(LiveAmmoItem));
            PassiveItem.IncrementFlag(player, typeof(PegasusBootsItem));
            player.AdditionalCanDodgeRollWhileFlying.AddOverride("Spring");
            player.OnIsRolling += Player_OnIsRolling;
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            base.Pickup(player);
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            Vector2 vector = arg1.sprite.WorldCenter - arg2.sprite.WorldCenter;
            arg1.knockbackDoer.ApplyKnockback(-vector, 40,true);
        }

        public bool rollFly;
        private void Player_OnIsRolling(PlayerController obj)
        {
            obj.SetIsFlying(true,"spring");
            rollFly = true;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.AdditionalCanDodgeRollWhileFlying.RemoveOverride("Spring");
            this.Owner.SetIsFlying(false, "spring"); ;
            player.OnIsRolling -= Player_OnIsRolling;
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            PassiveItem.DecrementFlag(player, typeof(PegasusBootsItem));
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            return base.Drop(player);
        }
        public override void  Update()
        {
            if (this.Owner)
            {
                if (rollFly)
                {
                    rollFly = false;
                    this.Owner.SetIsFlying(false, "spring");
                }
            }
            base.Update();
        }

    }






}
