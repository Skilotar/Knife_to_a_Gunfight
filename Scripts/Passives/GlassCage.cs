using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    class GlassCage : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Glass Cage";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/GlassCage";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<GlassCage>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "BulletProof Bastille";
            string longDesc = "Glass Guons Trap Enemies\n\n" +
                "A delicate cage representing the idea of imprisonment more than actually being practical for it.\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");



            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }


        public static int ID;



        public override void Pickup(PlayerController player)
        {
            if (startup == false)
            {
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");
                


                for (int i = player.orbitals.Count - 1; i >= 0; i--)
                {
                    if (player.orbitals[i] != null)
                    {
                        PlayerOrbital wee = (PlayerOrbital)player.orbitals[i];
                        if (wee.SourceItem.PickupObjectId == 565)
                        {
                            orgrad = wee.orbitRadius;
                        }
                    }
                }

                startup = true;
            }
            player.OnNewFloorLoaded += OnLoadedFloor;
            base.Pickup(player);


        }
        public bool startup = false;
        public bool attached = false;
        public AIActor target = null;
        public float orgrad;
        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                int countGuon = 0;
                for (int i = player.orbitals.Count - 1; i >= 0; i--)
                {
                    if (player.orbitals[i] != null)
                    {
                        PlayerOrbital wee = (PlayerOrbital)player.orbitals[i];
                        if (wee.SourceItem.PickupObjectId == 565)
                        {
                            countGuon++;
                        }
                    }
                }

                if(countGuon < 3)
                {
                    for(int i = countGuon; i < 3; i++)
                    {
                        player.GiveItem("glass_guon_stone");
                    }
                }
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }
        public bool hassyn = false;
        public override void Update()
        {
            if (this.Owner != null)
            {
                PlayerController player = this.Owner;
                if (player.PlayerHasActiveSynergy("Glassier Cage"))
                {
                    hassyn = true;
                }
                else
                {
                    hassyn = false;
                }
                    
                if (player.IsInCombat)
                {
                    if (player.orbitals != null)
                    {
                        if (target == null)
                        {
                            if (attached == true)
                            {   
                                unattach(this.Owner);
                            }
                            else
                            {
                                AcquireTargetNOW();
                            }
                        }
                        else
                        {

                            if(attached == false)
                            {
                                for (int i = player.orbitals.Count - 1; i >= 0; i--)
                                {
                                    if (player.orbitals[i] != null)
                                    {
                                        PlayerOrbital wee = (PlayerOrbital)player.orbitals[i];
                                        if (wee.SourceItem.PickupObjectId == 565)
                                        {

                                            wee.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_TARGET;

                                            wee.orbitRadius = Math.Max(target.specRigidbody.HitboxPixelCollider.Dimensions.x / 16, target.specRigidbody.HitboxPixelCollider.Dimensions.y / 16) + 1f;
                                            wee.orbitDegreesPerSecond = 180;
                                            if (hassyn) wee.orbitDegreesPerSecond = 270;

                                            SetPrivateType<PlayerOrbital>(wee, "m_currentTarget", target);
                                            target.ApplyEffect(StaticStatusEffects.tripleCrossbowSlowEffect);
                                            if (hassyn) target.knockbackDoer.SetImmobile(true, "Bound");
                                        }
                                    }
                                }

                                attached = true;
                            }
                            
                            if(Vector2.Distance(target.sprite.WorldCenter,this.Owner.sprite.WorldCenter) >= 16)
                            {
                                unattach(this.Owner);
                            }

                        }
                      
                    }
                   
                }
                else
                {
                    target = null;
                    unattach(this.Owner);
                }

            }
            base.Update();
        }

        private static void SetPrivateType<T>(T obj, string field, AIActor value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private void AcquireTargetNOW()
        {
            List<AIActor> activeEnemies = this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null && activeEnemies.Count > 0)
            {
                AIActor aiactor = null;
                float num = -1f;
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor2 = activeEnemies[i];
                    if (aiactor2 && aiactor2.HasBeenEngaged && aiactor2.IsWorthShootingAt && !aiactor2.healthHaver.IsBoss)
                    {
                        float num2 = Vector2.Distance(base.transform.position.XY(), aiactor2.specRigidbody.UnitCenter);
                        if (aiactor == null || num2 < num)
                        {
                            aiactor = aiactor2;
                            num = num2;
                        }
                    }
                }
                if (aiactor)
                {
                    target = aiactor;
                }
            }
        }

        public void unattach(PlayerController player)
        {
            for (int i = player.orbitals.Count - 1; i >= 0; i--)
            {
                if (player.orbitals[i] != null)
                {
                    PlayerOrbital wee = (PlayerOrbital)player.orbitals[i];
                    if (wee.SourceItem.PickupObjectId == 565)
                    {
                        wee.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_TARGET;
                        wee.orbitRadius = orgrad;
                        SetPrivateType<PlayerOrbital>(wee, "m_currentTarget", null);
                    }
                }
            }
            
            if(target != null)target.RemoveEffect(StaticStatusEffects.tripleCrossbowSlowEffect);
            
            target = null;
            attached = false;

        }


        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            unattach(player);
            player.OnNewFloorLoaded += OnLoadedFloor;
            return debrisObject;
        }
        private List<AIActor> roomEnemies = new List<AIActor>();

        

    }
}