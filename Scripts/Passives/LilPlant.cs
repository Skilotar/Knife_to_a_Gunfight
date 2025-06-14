using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;

namespace Knives
{
    class StellarOfficePlant : OnDamagedPassiveItem
    {

        public static void Register()
        {

            string itemName = "Stellar Office Plant";


            string resourceName = "Knives/Resources/littlePlant";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<StellarOfficePlant>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Happy Little Plant";
            string longDesc = "Chance on kill to make a infinite ammo inducing house plant.\n\n" +
                "Tracking Number: 1250***-****\n" +
                "Space quadrant 1 Auckland NZ. \nto \n" +
                "Space quadrant 4 Gunyemede gundrominian Region 18231.\n\n" +
                "This modified office plant exhibits unique traits when planted inside soil rich with carcasses." +
                " \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.A;
            BuildPrefab();
            ID = item.PickupObjectId;
        }
        public static int ID;

        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/lilPlant/littlePlant_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 6,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/lilPlant/littlePlant_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++)
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/lilPlant/littlePlant_00{i}", spriteForObject);
                
                
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            plant = gameObject;
           
        }
        public static GameObject plant;
        public GameObject pottedPlant;
        public override void Pickup(PlayerController player)
        {
            player.OnKilledEnemyContext += Player_OnKilledEnemyContext;
            
            base.Pickup(player);
        }

        private void Player_OnKilledEnemyContext(PlayerController arg1, HealthHaver arg2)
        {
            if(arg2.aiActor != null)
            {
                if (pottedPlant != null) UnityEngine.GameObject.Destroy(pottedPlant);
                if(UnityEngine.Random.Range(0, 5f) < 1 || this.Owner.PlayerHasActiveSynergy("Farmer Prowess"))
                {
                    pottedPlant = UnityEngine.Object.Instantiate<GameObject>(plant, arg2.aiActor.CenterPosition.ToVector3ZisY(0f), Quaternion.identity, null);
                    MagazineRack rack = pottedPlant.GetOrAddComponent<MagazineRack>();
                    rack.Duration = 5f;
                    rack.Radius = 4f;
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnKilledEnemyContext -= Player_OnKilledEnemyContext;
            return base.Drop(player);
        }
       
       
        public override void Update()
        {
            if (this.Owner)
            {
               
            }
            base.Update();
        }
        public GameObject MagazineRackPrefab;
    }
}