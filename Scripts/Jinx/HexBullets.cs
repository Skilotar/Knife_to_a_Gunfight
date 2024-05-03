using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class HexingRounds : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Hexing Rounds";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Hex_bullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HexingRounds>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hex The World";
            string longDesc =

                "Chance to hex both the user and enemies. Creatures afflicted with [Hex] may be damaged upon attacking.\n\n" +
                "Each of these bullets were forged by a witch in training. The seal on the bullets is not very well made so the hex sometimes leaks onto the user." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            //Set the rarity of the item

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Chance to hex enemies and you.";
        }
        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        public bool hexImmune;
        private void PostProcessProjectile(Projectile source, float chance)
        {

            if(UnityEngine.Random.value < (chance * 0.15f))
            {
                source.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(source.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
                source.HasDefaultTint = true;
                source.DefaultTintColor = new Color(.84f, .0f, .33f);
                
            }
            
            if (UnityEngine.Random.value < (chance * 0.015f) && this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>().statused == false  && !hexImmune)
            { 
                HexStatusEffectController hexen = this.Owner.gameObject.GetOrAddComponent<HexStatusEffectController>();
                hexen.statused = true;

                AkSoundEngine.PostEvent("Play_Hex_laugh_001", base.gameObject);
                StartCoroutine(reHexImmuneity());
            }

        }

        private IEnumerator reHexImmuneity()
        {
            hexImmune = true;

            yield return new WaitForSeconds(7f);

            hexImmune = false;
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {

                    HexStatusEffectController hexen = aiActor.gameObject.GetOrAddComponent<HexStatusEffectController>();
                    hexen.statused = true;

                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);

        }
    }
}