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
    public class HellFire : PlayerItem
    {
        public static void Register()
        {
            string itemName = "HellFire Swarm";

            string resourceName = "Knives/Resources/SwarmRocketpack";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HellFire>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "The Skies Belong To Me";
            string longDesc =
                "Nowhere to run.. Nowhere to hide.\n\n" +
                "A rocketpack with a targetable missile swarm." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.B;

            PoofSetup();

        }

        public static GameObject AirPillar;

        public static void PoofSetup()
        {
            GameObject Air = ItemBuilder.AddSpriteToObject("Air_VFX", "Knives/Resources/AirPillar/AirPillar_001", null);
            FakePrefab.MarkAsFakePrefab(Air);
            UnityEngine.Object.DontDestroyOnLoad(Air);
            tk2dSpriteAnimator animator = Air.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Air.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Blockcollection = SpriteBuilder.ConstructCollection(Air, ("Air_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 8 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 3; i++)
            {
                tk2dSpriteCollectionData collection = Blockcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/AirPillar/AirPillar_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animator.playAutomatically = true;



            AirPillar = Air;

        }


        public override void Pickup(PlayerController player)
        {
            reticleQuad = (PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).reticleQuad;
            Staty =StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 999);
            base.Pickup(player);
        }
        public bool looping;
        public override void DoEffect(PlayerController player)
        {
            StartCoroutine(ItemBuilder.HandleDuration(this, 7, player, EndEffect));
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.reticleQuad);
            reticle = gameObject;
            this.m_extantReticleQuad = gameObject.GetComponent<tk2dBaseSprite>();
            this.m_currentAngle = BraveMathCollege.Atan2Degrees(player.unadjustedAimPoint.XY() - player.CenterPosition);
            this.m_currentDistance = 7f;
            this.UpdateReticlePosition();
            PlayerGoUp uppie = player.gameObject.GetOrAddComponent<PlayerGoUp>();
            uppie.Height = 5;
            uppie.Up = true;
            

            looping = true;
           
        }
        public bool Uppies = false;
        public Vector3 targetposi;

        public override void DoActiveEffect(PlayerController user)
        {
            if (this.m_extantReticleQuad) //If the cursor is there, do ROCKETS!!!
            {
                try
                {
                    var cm = UnityEngine.Object.Instantiate<GameObject>((GameObject)BraveResources.Load("Global Prefabs/_ChallengeManager", ".prefab"));
                    this.Rocket = (cm.GetComponent<ChallengeManager>().PossibleChallenges.Where(c => c.challenge is SkyRocketChallengeModifier).First().challenge as SkyRocketChallengeModifier).Rocket;
                    UnityEngine.Object.Destroy(cm);

                    Transform main = m_extantReticleQuad.sprite.transform;
                    for (int i = 0; i <= offsets.Count -1; i++)
                    {
                        Vector2 pointinspace = main.TransformPoint(offsets[i]);
                        calc.Add(pointinspace + new Vector2(1.5f, 1.5f));
                    }
                    StartCoroutine(Rocketvolley(Rocket, calc, user));
                    if (m_extantReticleQuad != null) UnityEngine.GameObject.Destroy(m_extantReticleQuad.gameObject);

                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                }

            }
            else
            {
                EndEffect(user);
            }
            
            base.DoActiveEffect(user);
        }
        public List<Vector2> calc = new List<Vector2>
        {
        };

        private IEnumerator Rocketvolley(GameObject rocket, List<Vector2> point,PlayerController user)
        {
            for (int i = 0; i <= point.Count - 1; i++)
            {
                SkyRocket component = SpawnManager.SpawnProjectile(this.Rocket, Vector3.zero, Quaternion.identity, true).GetComponent<SkyRocket>();
                component.TargetVector2 = point[i];
                component.AscentTime = 0.5f;
                component.DescentTime = 1f;
                component.Variance = 0f;
                component.ExplosionData.damageToPlayer = 0;
                component.ExplosionData.damage = 20;
                component.ExplosionData.damageRadius = 3;
                component.ExplosionData.usesComprehensiveDelay = false;
                component.IgnoreExplosionQueues = true;
                //component.ExplosionData.effect = EasyVFXDatabase.RedFireBlastVFX;
                component.transform.position = user.sprite.transform.position;
                tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
                component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
                yield return new WaitForSeconds(.1f);

                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Missile_01", base.gameObject);
            }
            calc.Clear();
            EndEffect(user);
        }

        protected void EndEffect(PlayerController player)
        {
            if (m_extantReticleQuad != null)
            {
                DoActiveEffect(player);
            }
            
            player.CurrentStoneGunTimer = 0;
            PlayerGoUp uppie = player.gameObject.GetOrAddComponent<PlayerGoUp>();
            uppie.Up = false;
            
            SetPrivateType<PlayerItem>(this,"m_activeDuration", 0f);
            
        }

        private void UpdateReticlePosition()
        {
            PlayerController user = (PlayerController)this.LastOwner;
            if (user)
            {

                if (BraveInput.GetInstanceForPlayer(user.PlayerIDX).IsKeyboardAndMouse(false))
                {
                    Vector2 vector = user.unadjustedAimPoint.XY();
                    Vector2 vector2 = vector - this.m_extantReticleQuad.GetBounds().extents.XY();
                    this.m_extantReticleQuad.transform.position = vector2;
                }
                else
                {
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(user.PlayerIDX);
                    Vector2 vector3 = user.CenterPosition + (Quaternion.Euler(0f, 0f, user.CurrentGun.CurrentAngle) * Vector2.right).XY() * this.m_currentDistance;
                    vector3 += instanceForPlayer.ActiveActions.Aim.Vector * 12f * BraveTime.DeltaTime;
                    this.m_currentAngle = BraveMathCollege.Atan2Degrees(vector3 - user.CenterPosition);
                    this.m_currentDistance = Vector2.Distance(vector3, user.CenterPosition);
                    this.m_currentDistance = Mathf.Min(this.m_currentDistance, 100);
                    vector3 = user.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
                    Vector2 vector4 = vector3 - this.m_extantReticleQuad.GetBounds().extents.XY();
                    this.m_extantReticleQuad.transform.position = vector4;
                }


            }
            else
            {
                if (m_extantReticleQuad != null)
                {
                    UnityEngine.GameObject.Destroy(this.m_extantReticleQuad.gameObject);
                }

            }


        }

        public override void OnPreDrop(PlayerController player)
        {
            if (base.IsCurrentlyActive)
            {
                if (m_extantReticleQuad != null) UnityEngine.GameObject.Destroy(m_extantReticleQuad.gameObject);
                player.CurrentStoneGunTimer = 0;

                PlayerGoUp uppie = player.gameObject.GetOrAddComponent<PlayerGoUp>(); // EMERGENCY SLAM DOWN PLAYER
                uppie.Up = false;
                uppie.t = 0;


                SetPrivateType<PlayerItem>(this, "m_activeDuration", 0f);

            }
            else
            {
                
            }
            
        }

        

        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if (m_extantReticleQuad != null)
                {
                    UpdateReticlePosition();
                }

                if (this.LastOwner.gameObject.GetComponent<PlayerGoUp>() != null)
                {
                    this.CanBeDropped = false;
                    this.LastOwner.ownerlessStatModifiers.Add(Staty);
                    this.LastOwner.stats.RecalculateStats(this.LastOwner, true);
                }
                else
                {
                    this.CanBeDropped = true;
                    this.LastOwner.ownerlessStatModifiers.Remove(Staty);
                    this.LastOwner.stats.RecalculateStats(this.LastOwner, true);
                }

                CanBeUsed(this.LastOwner);

            }

            base.Update();
        }
        public StatModifier Staty;

        public override bool CanBeUsed(PlayerController user)
        {
            if (this.LastOwner.gameObject.GetComponent<PlayerGoUp>() != null)
            {
                if (this.LastOwner.gameObject.GetComponent<PlayerGoUp>().Up == false) // Must Complete Grounding Sequence In Order To Use Again.
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            
        }


        public float t;
         
        public override void OnItemSwitched(PlayerController user)
        {
            if (base.IsCurrentlyActive)
            {
                this.DoActiveEffect(user);
            }
        }
        

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        public static Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>
        {
            { 0,new Vector2(0,0)},
            { 1,new Vector2(0f,3)},
            { 2,new Vector2(2.5f,1.25f)},
            { 3,new Vector2(1.5f,-2f)},
            { 4,new Vector2(-1.5f,-2f)},
            { 5,new Vector2(-2.5f,1.25f)},
        };

        public static GameObject reticle;
        public GameObject reticleQuad;
        public tk2dBaseSprite m_extantReticleQuad;
        private float m_currentAngle;
        private float m_currentDistance = 5f;
        public GameObject Rocket;
        
    }
}
