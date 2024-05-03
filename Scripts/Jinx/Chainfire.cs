using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class ChainFire_Reagent : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "ChainFire Reagent";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/ChainFire_reagent";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<ChainFire_Reagent>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Wild Fire";
            string longDesc =

                "Enemies burst into bullets upon death, bursts are dangerous to Enemies and Players.\n\n" +
                "A supersaturated solution of gunpowder and nitroglycerin that soaks into the very being of the gundead. This causes their bodies to melt into pure bullet energy upon death." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            //Set the rarity of the item

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, .5f);
            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Enemies may burst into bullets upon death, bursts deal damage to anything.";

            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0].projectile);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            projectile3.baseData.damage = 28f * (1 + GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier);
            projectile3.baseData.speed = 5f;
            projectile3.baseData.range = 10f;
            projectile3.SuppressHitEffects = true;
            projectile3.hitEffects.suppressMidairDeathVfx = true;
            projectile3.objectImpactEventName = null;
            PierceProjModifier stab = projectile3.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 3;
            projectile3.SetProjectileSpriteRight("ChainFire_bullet", 11, 11, false, tk2dBaseSprite.Anchor.MiddleCenter, 9, 9);

            specialproj = projectile3;
        }


        public static Projectile specialproj;


        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        
        private void PostProcessProjectile(Projectile source, float chance)
        {

            if (source != null)
            {
                ChainFireModifier chain = source.gameObject.GetOrAddComponent<ChainFireModifier>();
            }
            
        }

        

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);

        }
       
    }
}