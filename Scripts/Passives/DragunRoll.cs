using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;



    namespace Knives
{
    class dragun_roll :PassiveItem
    {

        public static void Register()
        {
            string itemName = "Dragun Roll";

            string resourceName = "Knives/Resources/Dragun_roll";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<dragun_roll>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Spicy";
            string longDesc = "This tightly packed roll contains the meat of a dragun. As you might assume obtaining this meat is quite hard so a roll of this type is a delicacy.\n\n"+
            "The meat of the dragun causes all the other items inside this roll to taste spicy even though no spices are used. You now understand how to cause pain with a roll." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
                
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollDamage, 5f, StatModifier.ModifyMethod.ADDITIVE);
            
                
            item.quality = PickupObject.ItemQuality.C;


        }

        public override void Pickup(PlayerController player)
        {
            PassiveItem.IncrementFlag(player, typeof(LiveAmmoItem));
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            player.OnIsRolling += Player_OnIsRolling;
            base.Pickup(player);
        }

        private void Player_OnIsRolling(PlayerController obj)
        {
            GlobalSparksDoer.DoRandomParticleBurst(1, obj.sprite.WorldCenter + new Vector2(0,-.5f), obj.sprite.WorldCenter + new Vector2(0, -.5f), new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            arg2.ApplyEffect(StaticStatusEffects.hotLeadEffect);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnIsRolling -= Player_OnIsRolling;
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            return base.Drop(player);
        }

    }
}

