using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.VisualAPI;
using Dungeonator;


namespace Knives
{
    class MackTheKnife : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Mack The Knife";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Mack_the_knife";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MackTheKnife>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Scarlet Billows";
            string longDesc = "A little knife friend who is happy to stab. \n\n" +
                "The Soul of Mack Heath haunts this blade. Once a notorious serial killer he circled the london area like a shark looking for a kill.\n" +
                "In a sick twist of fate his final victim was a mighty sorceress who cursed him to live in his own cold hungry steel for all eternity." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);

            Gun gun = (Gun)ETGMod.Databases.Items[15];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.shouldRotate = true;
            projectile2.baseData.damage = 10f;
            projectile2.baseData.speed = 12f;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.shouldFlipVertically = true;
            projectile2.shouldRotate = true;
            projectile2.pierceMinorBreakables = true;


            MackProjMod mack = projectile2.gameObject.GetOrAddComponent<MackProjMod>();
            ImprovedAfterImage trail = projectile2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .1f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = new Color(.29f,.65f, .79f);
            trail.spawnShadows = true;

            //projectile2.SetProjectileSpriteRight("Mack_the_knife", 23, 10, false, tk2dBaseSprite.Anchor.MiddleCenter, 19, 9);

            Mackie_proj = projectile2;


            MiscToolMethods.EasyAnimate(projectile2,new List<string>() {
                "Mack_the_knife_001",
                "Mack_the_knife_002",
                "Mack_the_knife_003",
                "Mack_the_knife_004",
            }, 4, new IntVector2(23, 10), 6, false, tk2dSpriteAnimationClip.WrapMode.Loop);


            item.quality = PickupObject.ItemQuality.C;
        }
       
        public override void Pickup(PlayerController player)
        {
            
            base.Pickup(player);
        }
        public static Projectile Mackie_proj;
        public static Projectile m_mackie;
        public bool onplayer = true;
        public GameObject target;
        public override void Update()
        {
            if(this.Owner == true && !GameManager.Instance.IsLoadingLevel)
            {
                if(m_mackie == null) // Spawn knife
                {
                    m_mackie = MiscToolMethods.SpawnProjAtPosi(Mackie_proj, this.Owner.CenterPosition, this.Owner);
                    m_mackie.baseData.range = int.MaxValue;
                    m_mackie.baseData.damage = 10f;
                   
                    onplayer = true;
                }
                else
                {
                    
                }

            }
            base.Update();
        }
       

        public override DebrisObject Drop(PlayerController player)
        {
            if(m_mackie != null)
            {
                m_mackie.DieInAir();
            }
            return base.Drop(player);

        }
       
    }
}
