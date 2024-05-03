using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class FractBullets : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Fractalis Bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/FractalBullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<FractBullets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Recursive";
            string longDesc =

                "A bullet type crammed with bullets that are crammed with bullets that are crammed with bullets that are crammed with bullets that are crammed with nothing..." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            
            

            item.quality = PickupObject.ItemQuality.B;
        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;

            base.Pickup(player);
        }
        public System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile SourceProjectile, float chance)
        {
            int doubles = rng.Next(1, 4);
            if(doubles == 1)
            {
                SourceProjectile.AdditionalScaleMultiplier = 1;
                ProjectileSplitController split = SourceProjectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
                split.distanceBasedSplit = true;
                split.distanceTillSplit = 2.5f;
                split.numberofsplits = 2;
                split.dmgMultAfterSplit = .9f;
                split.amtToSplitTo = 1;
                split.sizeMultAfterSplit = SourceProjectile.AdditionalScaleMultiplier * 1.02f;
                split.splitAngles = 55f;
                split.splitOnEnemy = false;

                split.removeComponentAfterUse = false;
            }
           
        }

       

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }

    }
}

