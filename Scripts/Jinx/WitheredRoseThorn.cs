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
    public class Thornstone : IounStoneOrbitalItem
    {
        public static void Register()
        {
            string name = "Thornstone";
            string resourcePath = "Knives/Resources/Thorn_Guon";
            GameObject gameObject = new GameObject();
            Thornstone rock = gameObject.AddComponent<Thornstone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Every Rose";
            string longDesc = "Has Torns" +
                "\n\n\n - Knife_to_a_Gunfight";
            rock.SetupItem(shortDesc, longDesc, "ski");
            rock.quality = PickupObject.ItemQuality.EXCLUDED;
            Thornstone.BuildPrefab();

            rock.OrbitalPrefab = Thornstone.orbitalPrefab;

            rock.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.CLEAR;

            ID = rock.PickupObjectId;
        }
        public static GameObject Thorn;
        public static int ID;
        

        public static void BuildPrefab()
        {
            bool flag = Thornstone.orbitalPrefab != null;
            if (!flag)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Thorn_Guon", null);
                gameObject.name = "Thorn";

                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 13));
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.Projectile;
                Thornstone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                Thornstone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
                
                Thornstone.orbitalPrefab.orbitDegreesPerSecond = 10f;
                Thornstone.orbitalPrefab.shouldRotate = true;
                Thornstone.orbitalPrefab.orbitRadius = 2f;
                Thornstone.orbitalPrefab.SetOrbitalTier(10);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);



                Thorn = gameObject;
            }
        }

    
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);



        }


        public override void Update()
        {




            base.Update();
        }

        public override void OnDestroy()
        {
            Thornstone.speedUp = false;
            base.OnDestroy();
        }

        public static bool speedUp = false;
        public static PlayerOrbital orbitalPrefab;
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();

    }

}