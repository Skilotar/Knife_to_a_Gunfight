using System;
using System.Collections;

using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;


namespace Knives
{
    public class Guardian_Heart : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Guardian Heart";

            string resourceName = "Knives/Resources/Gaurded_Heart_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Guardian_Heart>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hold Me Close.";
            string longDesc = "Gain 1 super armor between floors. This armor does not prevent mastery if broken by a boss.\n\n" +
                "A cold exterior keeps this heart protected from outside elements. It has been able to continue beating outside of its host." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.ArmorToGainOnInitialPickup = 1;
            

            item.quality = PickupObject.ItemQuality.D;
            ID = item.PickupObjectId;

        }

        public static int ID;
        public int DoSaveFlaw = 1;

        public override void Pickup(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += this.GainArmorOnLevelLoad;
            m_player = player;
            player.healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(player.healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            base.Pickup(player);

            
        }

        private void ModifyIncomingDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if (DoSaveFlaw > 0) StartCoroutine(SaveFlawless());
            
        }

        PlayerController m_player;
        public void GainArmorOnLevelLoad()
        {
            this.m_player.healthHaver.Armor += 1f;

            DoSaveFlaw++;
        }

        private IEnumerator SaveFlawless()
        {
            yield return new WaitForSeconds(0.1f);
            PlayerController player = base.Owner;
            bool flag = player.CurrentRoom != null;
            if (flag && DoSaveFlaw > 0)
            {
                player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
                DoSaveFlaw--;
            }
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.GainArmorOnLevelLoad;
            player.healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(player.healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            return base.Drop(player);
        }

    }
}

