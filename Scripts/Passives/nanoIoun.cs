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
    public class nanostone : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "nanostone";
            string resourcePath = "Knives/Resources/Nano_ioun_stone";
            GameObject gameObject = new GameObject();
            nanostone rock = gameObject.AddComponent<nanostone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Autoimmune Response";
            string longDesc = "Drones used to monitor people currently going through an overdose of nanite based cereal." +
                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.EXCLUDED;
            nanostone.BuildPrefab();

            rock.OrbitalPrefab = nanostone.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.CLEAR;

            ID = rock.PickupObjectId;
        }

        public static int ID;


        public static void BuildPrefab()
        {
            bool flag = nanostone.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Nano_ioun_stone", null);
                gameObject.name = "nano";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(14, 14));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;

                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                nanostone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                nanostone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                nanostone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                nanostone.orbitalPrefab.shouldRotate = false;
                nanostone.orbitalPrefab.orbitRadius = 2.8f;
                GlassBoi.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);


            }
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            

        }

      
        public override void  Update()
        {




            base.Update();
        }

        public override void  OnDestroy()
        {
            nanostone.speedUp = false;
            base.OnDestroy();
        }

        public static bool speedUp = false;
        public static PlayerOrbital orbitalPrefab;
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();
       
    }

}
