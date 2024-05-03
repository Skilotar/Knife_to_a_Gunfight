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



namespace Knives
{
    public class GlassBoi : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "Glass Shrine";
            string resourcePath = "Knives/Resources/Glass_shrine";
            GameObject gameObject = new GameObject();
            GlassBoi rock = gameObject.AddComponent<GlassBoi>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Wait What?";
            string longDesc = "An intact glass shrine that has yet to be attached to its pedistal. The production of these shrines in the gungeon is quite common as they shatter on use." +
                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.C;
            GlassBoi.BuildPrefab();

            rock.OrbitalPrefab = GlassBoi.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.CLEAR;


            ID = rock.PickupObjectId;
        }

        public static int ID;


        public static void BuildPrefab()
        {
            bool flag = GlassBoi.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Glass_shrine", null);
                gameObject.name = "glass";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(38, 35));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;

                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                GlassBoi.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                GlassBoi.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                GlassBoi.orbitalPrefab.orbitDegreesPerSecond = 90f;
                GlassBoi.orbitalPrefab.shouldRotate = false;
                GlassBoi.orbitalPrefab.orbitRadius = 2.8f;


                GlassBoi.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);


            }
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            if (this.m_extantOrbital != null)
            {
                if(this.Owner != null)
                {
                    Owner.healthHaver.OnDamaged += this.OnDamaged;
                }
            }


        }

        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            PlayerController player = (PlayerController)Owner;
            player.RemovePassiveItem(this.PickupObjectId);
            player.GiveItem("glass_guon_stone");
            player.GiveItem("glass_guon_stone");
            player.GiveItem("glass_guon_stone");
            player.GiveItem("glass_guon_stone");
            Owner.healthHaver.OnDamaged -= this.OnDamaged;
        }

        public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
        {
            orig(self, player);
        }
        private void FixGuon()
        {
           
        }
       

        public override void  Update()
        {




            base.Update();
        }

      
       
        public override DebrisObject Drop(PlayerController player)
        {
            Owner.healthHaver.OnDamaged -= this.OnDamaged;
            GlassBoi.speedUp = false;
            return base.Drop(player);
        }

        public override void  OnDestroy()
        {
            GlassBoi.speedUp = false;
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
