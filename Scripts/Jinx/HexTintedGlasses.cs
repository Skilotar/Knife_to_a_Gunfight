using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class HexGlasses : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Hex Tinted Glasses";


            string resourceName = "Knives/Resources/HexGlasses";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<HexGlasses>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Gamble Your Life";
            string longDesc = "" +
                "Greatly Increases damage upon being Hexed. Reload at full clip to hex yourself. Creatures afflicted with [Hex] may be damaged upon attacking.\n\n" +
                "A pair of rosey shades worn by morgun herself. She was skilled at poker but never knew when to walk away. Let the hexes boil your blood into a frenzy." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item




            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "+1 damage while afflicted by Hex, Reload at a Full clip to hex yourself";

        }

        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            m_player = player;
            
            player.CurrentGun.OnReloadPressed = (Action<PlayerController, Gun,bool>)Delegate.Combine(player.CurrentGun.OnReloadPressed, new Action<PlayerController, Gun,bool>(this.OnReloadPressed));
            player.inventory.OnGunChanged += Inventory_OnGunChanged;
            base.Pickup(player);
        }


        private void OnReloadPressed(PlayerController arg1, Gun arg2,bool idrkwtd)
        {
            if (arg2.ClipShotsRemaining >= arg2.ClipCapacity && arg1.gameObject.GetOrAddComponent<HexStatusEffectController>().statused == false)
            {
                HexStatusEffectController hexen = this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>();
                hexen.statused = true;

                AkSoundEngine.PostEvent("Play_Hex_laugh_001", base.gameObject);
            }
        }

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            m_player.CurrentGun.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Remove(m_player.CurrentGun.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.OnReloadPressed));
            m_player.CurrentGun.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(m_player.CurrentGun.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.OnReloadPressed));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.CurrentGun.OnReloadPressed = (Action<PlayerController, Gun,bool>)Delegate.Remove(player.CurrentGun.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.OnReloadPressed));
            player.inventory.OnGunChanged -= Inventory_OnGunChanged;
            
            return base.Drop(player);
        }
        public bool Once = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>())
            {
                if (this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>().statused && Once == false )
                {

                    
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 1); 
                    Once = true;

                    this.Owner.stats.RecalculateStats(Owner, true);
                }
                if (!this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>().statused && Once == true)
                {

                   
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 0f);
                    Once = false;

                    this.Owner.stats.RecalculateStats(Owner, true);
                }
            }

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

        public PlayerController m_player;
    }
}
