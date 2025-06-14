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
    class jojo_arrow :PlayerItem   
    {
        public static void Register()
        {
            string itemName = "Arrow of Destiny";

            string resourceName = "Knives/Resources/Jojo_Arrow";
            resource = resourceName;
            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<jojo_arrow>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Your Trial Awaits";
            string longDesc = "Begins the trial of the arrow. Temporarily lowers health and sets game to challenge mode.\n\n" +
                "The Arrow silently becons you. Strike its edge upon your flesh and let it's power sink into you.\n\n" +
                "If you survive.. \nYou will return stronger than ever. Such is the crucible of greatness." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1);
            item.quality = PickupObject.ItemQuality.A;
            item.consumable = true;
            
        }
        public static string resource;

        public override void  DoEffect(PlayerController user)
        {
            JoJoTrial jojo = user.gameObject.GetOrAddComponent<JoJoTrial>();

            user.healthHaver.TriggerInvulnerabilityPeriod();
            AkSoundEngine.PostEvent("Play_CHR_general_hurt_01", user.gameObject);
            float health = user.healthHaver.GetMaxHealth();
            if (health <= 2) // lower health reduction at low healths
            {
                if (health == 2)
                {
                    user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, 1));
                    jojo.healthTaken = 1;
                }
                if (health < 2 && health <= 1)
                {
                    user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, 1));
                    jojo.healthTaken = 1;
                }
                if (health < 1) jojo.healthTaken = 0;
                // dude is at one health anyways lets not bother making them suffer more
            }
            else // normal damage
            {
                user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Health, StatModifier.ModifyMethod.ADDITIVE, -2));
                jojo.healthTaken = 2;
            }
            user.stats.RecalculateStats(user, true);
        }
        protected void EndEffect(PlayerController user)
        {
            
        }

    }
}
