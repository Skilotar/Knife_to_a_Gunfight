using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.ItemAPI;
using SaveAPI;
using UnityEngine;
using System.Collections;

namespace Knives
{
    class DoubleWHAMMY : PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension class
        public static void Register()
        {
            //The name of the item
            string itemName = "Double W.H.A.M.M.Y.";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it.
            string resourceName = "Knives/Resources/DoubleWhammy";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a ActiveItem component to the object
            var item = obj.AddComponent<DoubleWHAMMY>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Keep Away";
            string longDesc = "A pair of missiles designed for reflection rather than destruction. Hitting enemy projectiles will scatter them away from the blast." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"; 

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //"kts" here is the item pool. In the console you'd type kts:sweating_bullets
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Set the cooldown type and duration of the cooldown
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 13);

            //Set some other fields
            item.consumable = false;
            item.quality = ItemQuality.C;
            item.numberOfUses = 2;
            item.UsesNumberOfUsesBeforeCooldown = true;


            Gun gun = (Gun)ETGMod.Databases.Items[15];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.shouldRotate = true;
            projectile2.baseData.damage = 10f;
            projectile2.baseData.speed = 5f;
            projectile2.baseData.range = 25f;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.pierceMinorBreakables = true;
            
            

            WhammyCollision smack = projectile2.gameObject.GetOrAddComponent<WhammyCollision>();

            ImprovedAfterImage trail = projectile2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .1f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = new Color(.45f, 1f, .14f);
            trail.spawnShadows = true;

            projectile2.SetProjectileSpriteRight("WhammyProj", 19, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 19, 9);

            proj = projectile2;


            ID = item.PickupObjectId;
        }
        public static int ID;
        public static Projectile proj;
        
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            
        }
       
        
        public override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
           
        }
        public override void DoEffect(PlayerController user)
        {
            user.DidUnstealthyAction();
            SpawnMissile();
        }
        private void SpawnMissile()
        {
            PlayerController user = LastOwner;
            
            AkSoundEngine.PostEvent("Play_WPN_comm4nd0_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, user.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            if (component != null)
            {
                component.Owner = user;
                component.Shooter = user.specRigidbody;
                component.pierceMinorBreakables = true;
                component.specRigidbody.CollideWithTileMap = false;
                component.UpdateCollisionMask();
                StartCoroutine(delaySpeed(component));
               
            }
        }

        private IEnumerator delaySpeed(Projectile missile)
        {
            yield return new WaitForSeconds(.18f);
            if(missile != null)
            {
                missile.baseData.speed *= 7f;
                missile.specRigidbody.CollideWithTileMap = true;
                missile.UpdateCollisionMask();
                missile.UpdateSpeed();
            }
            
        }
        
       
      
       
       
    }
}