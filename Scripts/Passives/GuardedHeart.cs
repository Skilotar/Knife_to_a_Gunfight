using System;
using System.Collections;

using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Collections.Generic;

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
        public static int DoSaveFlaw = 1;
        public bool firstPickup = false;
        public override void Pickup(PlayerController player)
        {
            if (firstPickup == false)
            {
                firstPickup = true;
                DoSaveFlaw = 1;
            }
            GameManager.Instance.OnNewLevelFullyLoaded += this.GainArmorOnLevelLoad;
            m_player = player;
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            base.Pickup(player);
           
            
        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (DoSaveFlaw > 0) SaveFlawless();
        }


        PlayerController m_player;
        public void GainArmorOnLevelLoad()
        {
            if(m_player != null)
            {
                if(this.Owner != null)
                {
                    this.m_player.healthHaver.Armor += 1f;
                    
                    DoSaveFlaw++;
                }
            }
            
        }

        public static void UpdateHealthHook(Action<GameUIHeartController, HealthHaver> orig, GameUIHeartController self, HealthHaver hh)
        {
            orig(self, hh);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.healthHaver == hh)
                {
                    if (player.HasPickupID(Guardian_Heart.ID))
                    {
                        List<dfSprite> c = self.extantArmors;
                        if (c != null || c.Count > 0)
                        {
                            for (int i = 0; i < c.Count; i++)
                            {
                                if (i > (c.Count - 1) - DoSaveFlaw)
                                {
                                    c[i].Color = ExtendedColours.MikuBlue;
                                }
                                else
                                {
                                    c[i].Color = new Color32(255, 255, 255, 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SaveFlawless()
        {
            
            PlayerController player = base.Owner;
            bool flag = player.CurrentRoom != null;
            if (flag && DoSaveFlaw > 0)
            {
                player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
                DoSaveFlaw -= 1;
            }
            
        }

        public override DebrisObject Drop(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.GainArmorOnLevelLoad;
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            return base.Drop(player);
        }

    }
}

