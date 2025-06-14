using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;
using Alexandria.BreakableAPI;

namespace Knives
{
    class NonStickBandaids : PassiveItem
    {
        
        public static void Register()
        {
            //The name of the item
            string itemName = "NonStick Bandages";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/bandaids";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<NonStickBandaids>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Impractical Aid";
            string longDesc = "Heal half a heart if you go 3 rooms without, taking damage, dodgerolling, or stepping in goop. \n\n" +
                "A failed brand of nonstick bandaids. They were originally created to reduce the pain of removing a bandaid. Unfortunately, they tend to fall off in non optimal conditions." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            NonStickBandaids.one = SpriteBuilder.SpriteFromResource(NonStickBandaids.Aid_1, null);
            NonStickBandaids.one.name = NonStickBandaids.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(NonStickBandaids.one);
            FakePrefab.MarkAsFakePrefab(NonStickBandaids.one);
            NonStickBandaids.one.SetActive(false);

            NonStickBandaids.two = SpriteBuilder.SpriteFromResource(NonStickBandaids.Aid_2, null);
            NonStickBandaids.two.name = NonStickBandaids.vfxName2;
            UnityEngine.Object.DontDestroyOnLoad(NonStickBandaids.two);
            FakePrefab.MarkAsFakePrefab(NonStickBandaids.two);
            NonStickBandaids.two.SetActive(false);
            
            NonStickBandaids.three = SpriteBuilder.SpriteFromResource(NonStickBandaids.Aid_3, null);
            NonStickBandaids.three.name = NonStickBandaids.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(NonStickBandaids.three);
            FakePrefab.MarkAsFakePrefab(NonStickBandaids.three);
            NonStickBandaids.three.SetActive(false);
            
            NonStickBandaids.heart = SpriteBuilder.SpriteFromResource(NonStickBandaids.Aid_heart, null);
            NonStickBandaids.heart.name = NonStickBandaids.vfxName4;
            UnityEngine.Object.DontDestroyOnLoad(NonStickBandaids.heart);
            FakePrefab.MarkAsFakePrefab(NonStickBandaids.heart);
            NonStickBandaids.heart.SetActive(false);

            Falloff = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/Bandaid/bandaid_off", true, 7, 10, 100, 0, null, .2f, null, null, 0);


            item.quality = PickupObject.ItemQuality.C;

        }

        public static DebrisObject Falloff;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
            {
                return;
            }
            base.Pickup(player);
            player.OnEnteredCombat += this.onEnteredCombat;
            player.healthHaver.ModifyDamage += this.ModifyDamage;
            player.OnPreDodgeRoll += this.dodgerolled;
        }

        private void dodgerolled(PlayerController obj)
        {
            fail();
        }

        private void ModifyDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if(arg2.ModifiedDamage > 0)
            {
                fail();
            }
        }

        private void onEnteredCombat()
        {
            PlayerController user = this.Owner;

            bool FullHealth = false;
            if (user.healthHaver.GetMaxHealth() == user.healthHaver.GetCurrentHealth()) FullHealth = true;

            if (!FullHealth)
            {
                combatsCounter++;
                // Success sound
                AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                // room counter bamp
                if (combatsCounter == 1)
                {
                    GameObject original = NonStickBandaids.one;
                    tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                    this.Owner.BloopItemAboveHead(OhGeez, "");
                }
                if (combatsCounter == 2)
                {
                    GameObject original = NonStickBandaids.two;
                    tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                    this.Owner.BloopItemAboveHead(OhGeez, "");
                }
                if (combatsCounter == 3)
                {
                    GameObject original = NonStickBandaids.three;
                    tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                    this.Owner.BloopItemAboveHead(OhGeez, "");

                    FallingEdgeCombatCheck = true;
                }
               
            }
            else
            {
                combatsCounter = 0;
            }
            
        }
        public int combatsCounter = 0;
        public bool FallingEdgeCombatCheck = false;
      
        public override void Update()
        {
            if (this.Owner != null)
            {
                if (this.IsPointGoopy(this.Owner.CenterPosition) || this.IsPointfirey(this.Owner.CenterPosition))
                {
                    fail();
                }

                if(FallingEdgeCombatCheck && this.Owner.IsInCombat == false) // On Room 3 checks for combat ending and then calcuates if the required condition is met for heal.
                {
                    //Reward and reset
                    if (combatsCounter >= 3)
                    {

                        GameObject original = NonStickBandaids.heart;
                        tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
                        this.Owner.BloopItemAboveHead(OhGeez, "");

                        AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
                        this.Owner.healthHaver.ApplyHealing(.5f);
                        FallingEdgeCombatCheck = false;
                        combatsCounter = 0;
                    }
                    else
                    {
                        FallingEdgeCombatCheck = false;
                        combatsCounter = 0;
                    }
                }

            }
            base.Update();
        }

        public void fail()
        {
            if(combatsCounter > 0)
            {
                combatsCounter = 0;
                // Fail sound
                AkSoundEngine.PostEvent("Play_WPN_peashooter_shot_01", base.gameObject);

                // fall off sprite
                Vector2 dir = this.Owner.CurrentGun.barrelOffset.transform.position - this.Owner.CurrentGun.barrelOffset.transform.TransformPoint(5, 0, 0);
                GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(Falloff.gameObject, this.Owner.CenterPosition, Quaternion.identity);
                DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, this.Owner.CenterPosition, dir, 7, false, false, true, false);
                debrisObject.canRotate = true;
                debrisObject.angularVelocity = 50;

                FallingEdgeCombatCheck = false;
            }

        }

        

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= this.onEnteredCombat;
            player.healthHaver.ModifyDamage -= this.ModifyDamage;
            player.OnPreDodgeRoll -= this.dodgerolled;
            DebrisObject debrisObject = base.Drop(player);
            
            return debrisObject;
        }

        private bool IsPointGoopy(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionInGoop(testPos);
            }
            return false;
        }
        private bool IsPointfirey(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
            }
            return false;
        }

        private static string Aid_1 = "Knives/Resources/Bandaid/bandaid_1";
        private static GameObject one;
        private static string vfxName = "one";

        private static string Aid_2 = "Knives/Resources/Bandaid/bandaid_2";
        private static GameObject two;
        private static string vfxName2 = "two";

        private static string Aid_3 = "Knives/Resources/Bandaid/bandaid_3";
        private static GameObject three;
        private static string vfxName3 = "three";

        private static string Aid_heart = "Knives/Resources/Bandaid/Bandaid_heart";
        private static GameObject heart;
        private static string vfxName4 = "heart";
    }
}
