using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Cacophony : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Cacophony Horn";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cacaphony_horn";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Cacophony>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hellish Sounds";
            string longDesc = "Hexes All who hear its wretched note. Creatures afflicted with [Hex] may be damaged upon attacking." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            //Adds the actual passive effect to the item

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Hexes anything that hears it, including you.";
        }
        public static int itemID;
        public override void  DoEffect(PlayerController user)
        {
            Exploder.DoDistortionWave(user.sprite.WorldCenter, .25f, 0.25f, 6, 1f);
            HexStatusEffectController hex = user.gameObject.GetOrAddComponent<HexStatusEffectController>();
            AkSoundEngine.PostEvent("Play_Hell_horn", base.gameObject);
            AkSoundEngine.PostEvent("Play_Hex_laugh_001", base.gameObject);
            hex.statused = true;
            bool flag2 = user.CurrentRoom != null;
            if (flag2)
            {
                user.CurrentRoom.ApplyActionToNearbyEnemies(user.CenterPosition, 8f, new Action<AIActor, float>(this.ProcessEnemy));

            }
        }

        private void ProcessEnemy(AIActor arg1, float arg2)
        {
            HexStatusEffectController hex = arg1.gameObject.GetOrAddComponent<HexStatusEffectController>();
            hex.statused = true;
        }

        protected void EndEffect(PlayerController user)
        {

        }
    }
}