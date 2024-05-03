using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using System.Reflection;
using System.Collections;
using SaveAPI;

namespace Knives
{
    public class WitchWatch : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Witch Watch";

            string resourceName = "Knives/Resources/WitchWatch";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<WitchWatch>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Which Watch?";
            string longDesc =

                "Why This Watch of Course!\n\n" +
                "Morgun la Laye's precious timepiece gifted to you for assisting her research of Hex.  \n______________ \n" +
                "Use once to set a spot and again to return to it." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.B;

            BuildPrefab();
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.HEXLINDED, true);
        }

       

        public bool toggle = false;

        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Timespot/timespot_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Timespot/timespot_001", spriteObject); //add 1
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
            for (int i = 1; i < 15; i++) 
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                if(i <= 9)
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Timespot/timespot_00{i}", spriteForObject);
                }
                else
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Timespot/timespot_0{i}", spriteForObject);
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
            TimeSpot = gameObject;
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public bool looping;
        public override void  DoEffect(PlayerController player)
        {

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(TimeSpot, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            m_spot = gameObject;
            StartCoroutine(ItemBuilder.HandleDuration(this, 10, player, EndEffect));
            zoomy =  player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            zoomy.dashColor = new Color(0, 0, 0);
            zoomy.spawnShadows = true;
            zoomy.shadowTimeDelay = .5f;
            zoomy.shadowLifetime = 2.5f;
            zoomy.minTranslation = 0.1f;
            looping = true;
            StartCoroutine(SoundLoop());
            
        }

        private IEnumerator SoundLoop()
        {
            if (looping)
            {
                AkSoundEngine.PostEvent("Play_Tick_001", base.gameObject);
                yield return new WaitForSeconds(1);
            }
            if(looping)
            {
                AkSoundEngine.PostEvent("Play_Tock_001", base.gameObject);
                yield return new WaitForSeconds(1);
            }
            if (looping)
            {
                AkSoundEngine.PostEvent("Play_Tick_001", base.gameObject);
                yield return new WaitForSeconds(1);
            }
            if (looping)
            {
                AkSoundEngine.PostEvent("Play_Tick_002", base.gameObject);
                yield return new WaitForSeconds(1);
                this.LastOwner.StartCoroutine(SoundLoop());
            }
           
        }

        public override void  DoActiveEffect(PlayerController user)
        {
            
            EndEffect(user);
            base.DoActiveEffect(user);
        }
        protected void EndEffect(PlayerController player)
        {
            if(m_spot != null)
            {
                looping = false;
                TeleportPlayerToCursorPosition.StartTeleport(player, m_spot.transform.position + new Vector3(.5f,.5f,0));
                UnityEngine.GameObject.DestroyObject(m_spot);
                zoomy.spawnShadows = false;
            }
        }

        public override void  OnPreDrop(PlayerController user)
        {
            if(m_spot != null)
            {
                UnityEngine.GameObject.DestroyObject(m_spot);
               
                FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
                remainingTimeCooldown.SetValue(this, 0);
                looping = false;
                zoomy.spawnShadows = false;
            }

            base.OnPreDrop(user);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.IsInCombat)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public override void Update()
        {
            if(this.LastOwner != null)
            {
                CanBeUsed(this.LastOwner);

                if (this.LastOwner.IsInCombat)
                {
                    FallingEdgeCombatTrigger = true;
                }
                else
                {
                    if (FallingEdgeCombatTrigger) // combat ended or Emergency leave room safety 
                    {
                        
                        FallingEdgeCombatTrigger = false;
                        if (m_spot != null)
                        {
                            UnityEngine.GameObject.DestroyObject(m_spot);
                        }
                        
                        FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
                        remainingTimeCooldown.SetValue(this, 0);
                        zoomy.spawnShadows = false;
                        looping = false;
                    }
                }
            }


            base.Update();
        }

        public bool FallingEdgeCombatTrigger;
        public static GameObject TimeSpot;
        public static GameObject m_spot;
        private ImprovedAfterImage zoomy; //per tradition
    }
}
