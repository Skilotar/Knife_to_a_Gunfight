using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Core_otfg : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Core of the Forbidden Gun";

            string resourceName = "Knives/Resources/Explodia/Core_otfg_2";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Core_otfg>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "The Imprisoned";
            string longDesc = "The core of a forbidden gun. Its internals striped from it and its parts scattered; gaurded by kaliber's chosen few. \n\n" +
                "The gun yearns to be whole. Free Him..." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .1f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = PickupObject.ItemQuality.B;
            item.RespawnsIfPitfall = true;

            ID = item.PickupObjectId;

            BuildPrefab();
        }

        public static int ID;
        public static GameObject combineVFX;
        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Explodia/Combine_VFX/ExplodiaCombine_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 8,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Explodia/Combine_VFX/ExplodiaCombine_001", spriteObject); //add 1
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
            for (int i = 1; i < 18; i++)
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                if (i <= 9)
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Explodia/Combine_VFX/ExplodiaCombine_00{i}", spriteForObject);
                }
                else
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Explodia/Combine_VFX/ExplodiaCombine_0{i}", spriteForObject);
                }

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
            combineVFX = gameObject;
        }

        public override void Pickup(PlayerController player)
        {
            player.gameObject.GetOrAddComponent<Explodia_part_holder_Controller>();

            base.Pickup(player);
        }

        public override void Update()
        {
            if(this.Owner != null)
            {
                if(this.Owner.HasPassiveItem(Core_otfg.ID) && this.Owner.HasPassiveItem(Eye_otfg.ID) && this.Owner.HasPassiveItem(Stock_otfg.ID) && this.Owner.HasPassiveItem(Drum_otfg.ID) && this.Owner.HasPassiveItem(Barrel_otfg.ID))
                {
                    this.Owner.StartCoroutine(CombineExplodia(Owner));
                }
            }

            base.Update();
        }

        private IEnumerator CombineExplodia(PlayerController  owner)
        {
            
            GameObject Parry = owner.PlayEffectOnActor(combineVFX, new Vector3(-2f, .0f), true);
            Parry.GetComponent<tk2dBaseSprite>().scale *= 1f;
            Parry.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.Owner.transform.position + new Vector3(-2f, .0f), tk2dBaseSprite.Anchor.LowerLeft);
            Parry.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            AkSoundEngine.PostEvent("Play_NPC_magic_blessing_01",base.gameObject);
            
            Explodia_part_holder_Controller part = owner.gameObject.GetOrAddComponent<Explodia_part_holder_Controller>();
            part.obtainedExplodia = true;

            owner.RemovePassiveItem(Core_otfg.ID);
            owner.RemovePassiveItem(Eye_otfg.ID);
            owner.RemovePassiveItem(Stock_otfg.ID);
            owner.RemovePassiveItem(Drum_otfg.ID);
            owner.RemovePassiveItem(Barrel_otfg.ID);
            yield return new WaitForSeconds(2f);
            float FlashHoldtime = 0.1f;
            float FlashFadetime = 0.3f;
            Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashHoldtime);
           
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(Explodia.ID).gameObject, owner.sprite.WorldTopLeft, new Vector3(0, 0, 1), 7f, false, false, false);
            AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01", owner.gameObject);

          
        }
    }
}
