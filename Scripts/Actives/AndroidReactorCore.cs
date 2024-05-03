using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class AndroidReactorCore : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Android Reactor Core";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/android_reactor_core";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<AndroidReactorCore>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I Have No Other Options";
            string longDesc = "Triggering the meltdown of this core will be enough to intantly kill anything in the immediate radius. However as a safety procaution the blast will never fully kill the user.\n\n\n" +
                "Take you to 1 hp and insta-kills the whole room including bosses." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 1200f);


            //Set the rarity of the item

            item.consumable = true;
            
            item.quality = PickupObject.ItemQuality.A;
        }

      

        public void DoVeryMuchUnsafeExplosion(Vector3 position)
        {
            Hurt(this.LastOwner);
            DoSafeExplosion(position);
           
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            GameObject Nuke = assetBundle.LoadAsset<GameObject>("assets/data/vfx prefabs/impact vfx/vfx_explosion_nuke.prefab");
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Nuke);
            
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(position, tk2dBaseSprite.Anchor.LowerCenter);
            gameObject2.transform.position = gameObject.transform.position.Quantize(0.0625f);
            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            {
                float FlashHoldtime = 0.1f;
                float FlashFadetime = 0.5f;
                Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashHoldtime);
                StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); 
            }

        }
        public void Hurt(PlayerController player)
        {
            float current = player.healthHaver.GetCurrentHealth();
            float armor = player.healthHaver.Armor;
            float altarmor = armor - 1;
            float altered = current - .5f;
            
            if (this.LastOwner.characterIdentity == PlayableCharacters.Robot)
            {
                RobotHurt(player, altarmor);
            }
            else
            {
                player.healthHaver.NextDamageIgnoresArmor = true;
                player.healthHaver.ApplyDamage(altered, Vector2.zero, "Raw level 3", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
            }
        }

        public float reduced = -1;
        public override void Update()
        {
            if (this.LastOwner)
            {
                RobotHurt(this.LastOwner, reduced);
            }
            
            base.Update();
        }
        public void RobotHurt(PlayerController player, float tokens)
        {
            if(tokens >= 0)
            {
                player.healthHaver.ApplyDamage(.5f, Vector2.zero, "Raw level 3", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                reduced = tokens - .5f;
                
            }
        }
        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 20,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 999999999f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 40f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true
        };

    }
}
