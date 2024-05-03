using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class RubberCharm : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "RubberDucky Charm";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Charm/rubber_charm";
            resource = resourceName;
            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<RubberCharm>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sqwaaaaaaak~";
            string longDesc = "Attaches to your current gun. Only one charm can be equipped to a gun.\n" +
                "Adds Bouncy and Increases damage on bounce.\n\n" +
                "" +
                "An ornamental gun charm made from a rubber ducky. The prized item of a very clean puppet." +

                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1);

            item.UsesCustomCost = true;
            item.CustomCost = 20;
            item.quality = PickupObject.ItemQuality.D;
            item.numberOfUses = 1;
            item.consumable = true;

            Gun gun = (Gun)ETGMod.Databases.Items[15];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.shouldRotate = true;
            projectile2.shouldFlipVertically = true;
            projectile2.baseData.speed *= .8f;
            projectile2.SetProjectileSpriteRight("Ducky", 15, 15, false, tk2dBaseSprite.Anchor.MiddleCenter, 15, 15);
            BounceProjModifier bounce = projectile2.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounce.numberOfBounces += 3;
            bounce.damageMultiplierOnBounce *= 1.2f;
            bounce.OnBounceContext += OnBounceDucky;
            PierceProjModifier stabby = projectile2.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration++;
            ducky = projectile2;


            ID = item.PickupObjectId;
            GlobalCharmList.Charmslist.Add(ID);
        }

        public static Projectile ducky;

        private static void OnBounceDucky(BounceProjModifier arg1, SpeculativeRigidbody arg2)
        {
            AkSoundEngine.PostEvent("Play_Ducky", arg2.gameObject);
        }

        public static string resource;
        public static int ID;

        //applies damage on last use
        public override void DoEffect(PlayerController user)
        {
            if (user.CurrentGun.GetComponent<MasterCharmComp>() == null)
            {

                MasterCharmComp charm = user.CurrentGun.gameObject.GetOrAddComponent<MasterCharmComp>();
                charm.baseitem = this;
                charm.attachedGun = user.CurrentGun;
                charm.player = user;
                charm.resource = resource;
                charm.type = MasterCharmComp.CharmType.Rubber;
                AkSoundEngine.PostEvent("Play_Ducky", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.gameObject);
            }

        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.CurrentGun.GetComponent<MasterCharmComp>() == null;
        }

        public override void Update()
        {

            if (this.LastOwner)
            {
                this.CanBeUsed(this.LastOwner);
            }


            base.Update();
        }

    }
}