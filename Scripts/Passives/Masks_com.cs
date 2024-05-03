using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using SaveAPI;


namespace Knives
{
    public class Masks_Com : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "Com-Eddie";
            string resourcePath = "Knives/Resources/play/guons/Mask_com";
            GameObject gameObject = new GameObject();
            Masks_Com rock = gameObject.AddComponent<Masks_Com>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "The Oldest";
            string longDesc = "Hailing from a distant star system, Eddie and Eddy are a pair of twins searching the universe for the grandest events. " +
                "They seem to fully believe that Everything is a play and the gungeon and its tales of action and dread are their favorites so far. They feel like joining in to show their acting prowess to a new stage! " +
                "Eddie thinks you're hillarious!" +
                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.A;
            Masks_Com.BuildPrefab();

            rock.OrbitalPrefab = Masks_Com.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;
            rock.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_MANIC, true);
            ID = rock.PickupObjectId;
        }

        public static int ID;


        public static void BuildPrefab()
        {
            bool flag = Masks_Com.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/play/guons/Mask_com", null);
                gameObject.name = "Com";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;

                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                Masks_Com.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                Masks_Com.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                Masks_Com.orbitalPrefab.orbitDegreesPerSecond = 90f;
                Masks_Com.orbitalPrefab.shouldRotate = false;
                Masks_Com.orbitalPrefab.orbitRadius = 2.5f;


                Masks_Com.orbitalPrefab.SetOrbitalTier(20);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);


            }
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            Masks_Com.guonHook = new Hook(typeof(PlayerOrbital).GetMethod("Initialize"), typeof(Masks_Com).GetMethod("GuonInit"));
            bool flag = player.gameObject.GetComponent<Masks_Com.BaBoom>() != null;
            bool flag2 = flag;
            bool flag3 = flag2;
            bool flag4 = flag3;
            if (flag4)
            {
                player.gameObject.GetComponent<Masks_Com.BaBoom>().Destroy();
            }
            player.gameObject.AddComponent<Masks_Com.BaBoom>();
            GameManager.Instance.OnNewLevelFullyLoaded += this.FixGuon;
            bool flag5 = this.m_extantOrbital != null;
            bool flag6 = flag5;
            bool flag7 = flag6;
            if (flag7)
            {
                SpeculativeRigidbody specRigidbody = this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            }
            PlayerSpecialStates freebe = player.gameObject.GetOrAddComponent<PlayerSpecialStates>();

            if (!this.Owner.HasPassiveItem(Masks_trag.ID) && freebe.theatrefreebegotten == false)
            {
                this.Owner.GiveItem("ski:trag-eddy");
                freebe.theatrefreebegotten = true;
            }

        }
        public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
        {
            orig(self, player);
        }
        private void FixGuon()
        {
            bool flag = base.Owner && base.Owner.GetComponent<Masks_Com.BaBoom>() != null;
            bool flag2 = flag;
            bool flag3 = flag2;
            bool flag4 = flag3;
            if (flag4)
            {
                base.Owner.GetComponent<Masks_Com.BaBoom>().Destroy();
            }
            bool flag5 = this.m_extantOrbital != null;
            bool flag6 = flag5;
            bool flag7 = flag6;
            if (flag7)
            {
                SpeculativeRigidbody specRigidbody = this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            }
            PlayerController owner = base.Owner;
            owner.gameObject.AddComponent<Masks_Com.BaBoom>();
        }
        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            if(other.projectile != null)
            {   
                PassiveReflectItem.ReflectBullet(other.projectile, true, this.Owner, 15f, 1f, 1f, 5f);
                other.projectile.baseData.damage = 9;
                other.projectile.OnHitEnemy += this.OnHitEnemy;
                
                PhysicsEngine.SkipCollision = true;
            }

        }
        public void OnHitEnemy(Projectile projectile,SpeculativeRigidbody body, bool Lethal)
        {
            if(body.aiActor != null)
            {
                if (Lethal)
                {
                    AkSoundEngine.PostEvent("Play_Mask_laugh", base.gameObject);
                }

            }

        }

        public override void  Update()
        {




            base.Update();
        }

       
        public override DebrisObject Drop(PlayerController player)
        {
            Masks_Com.speedUp = false;
            return base.Drop(player);
        }

        public override void  OnDestroy()
        {
            Masks_Com.speedUp = false;
            base.OnDestroy();
        }

        public static bool speedUp = false;
        public static PlayerOrbital orbitalPrefab;
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();
        public static Hook guonHook;


        private class BaBoom : BraveBehaviour
        {
            // Token: 0x06000B0A RID: 2826 RVA: 0x0005EB58 File Offset: 0x0005CD58
            private void Start()
            {
                this.owner = base.GetComponent<PlayerController>();
            }

            // Token: 0x06000B0B RID: 2827 RVA: 0x0005EB67 File Offset: 0x0005CD67
            public void Destroy()
            {
                UnityEngine.Object.Destroy(this);
            }

            // Token: 0x040005BC RID: 1468
            private PlayerController owner;
        }
    }

}