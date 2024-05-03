using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using GungeonAPI;

namespace Knives
{
    class SideCar : PlayerItem
    {
        private static readonly string[] spritePaths = new string[]
        {

            "Knives/Resources/SideCar/SideCarSmg_fire_001", // 0
            "Knives/Resources/SideCar/SideCarSmg_fire_002", // 1
            "Knives/Resources/SideCar/SideCarSmg_fire_003", // 2

            "Knives/Resources/SideCar/SideCarSmg_reload_001", // 3
            "Knives/Resources/SideCar/SideCarSmg_reload_002", // 4

            "Knives/Resources/SideCar/SideCarSmg_screw_001", // 5
          
            "Knives/Resources/SideCar/SideCarSmg_screw_002", // 6

             "Knives/Resources/SideCarSmg_idle_001" // 7


        };
        public static void Register()
        {
            //The name of the item
            string itemName = "SideCar SMG";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/SideCarSmg_idle_001";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<SideCar>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Tag-Along";
            string longDesc = "Use to attach or detach to the current gun \n\n" +
                "" +
                "A curious smg with no handle. It has mounting points located on all sides." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed,.5f);


            SideCar.spriteIDs = new int[SideCar.spritePaths.Length];
            SideCar.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[0], item.sprite.Collection); //fire
            SideCar.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[1], item.sprite.Collection);
            SideCar.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[2], item.sprite.Collection);

            SideCar.spriteIDs[3] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[3], item.sprite.Collection); //reload
            SideCar.spriteIDs[4] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[4], item.sprite.Collection);

            SideCar.spriteIDs[5] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[5], item.sprite.Collection); //spin
            SideCar.spriteIDs[6] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[6], item.sprite.Collection);

            SideCar.spriteIDs[7] = SpriteBuilder.AddSpriteToCollection(SideCar.spritePaths[7], item.sprite.Collection);

            item.sprite.SetSprite(spriteIDs[7]);



            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }
        public static int ID;
        private static int[] spriteIDs;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessThrownGun += Player_PostProcessThrownGun;
            base.Pickup(player);
        }

        private void Player_PostProcessThrownGun(Projectile obj)
        {
            if(obj.GetComponentInChildren<Gun>() == Stored)
            {
                
                deconnect(this.LastOwner);
            }
        }

        public Gun CarSMG = PickupObjectDatabase.GetById(43) as Gun;
        public Gun Stored;
        public int orgiAmmo;

        public ProjectileVolleyData storedprojectileVolleyData = new ProjectileVolleyData();
        public override void DoEffect(PlayerController user)
        {
            if (user.CurrentGun != null)
            {
                if(Stored == null)
                {
                    Gun Target = user.CurrentGun;
                    StoreCombine(Target);


                    ProjectileVolleyData projectileVolleyData = CombineVolleys(Target, CarSMG);
                    ReconfigureVolley(projectileVolleyData);
                    Target.RawSourceVolley = projectileVolleyData;
                   

                    if (Target.DuctTapeMergedGunIDs == null)
                    {
                        Target.DuctTapeMergedGunIDs = new List<int>();
                    }
                    if (CarSMG.DuctTapeMergedGunIDs != null)
                    {
                        Target.DuctTapeMergedGunIDs.AddRange(CarSMG.DuctTapeMergedGunIDs);
                    }
                    Target.DuctTapeMergedGunIDs.Add(CarSMG.PickupObjectId);
                    user.stats.RecalculateStats(user, false, false);
                    user.CurrentGun.OnDropped += TargetDrop;
                    user.CurrentGun.PostProcessProjectile += postprocess;

                    this.sprite.SetSprite(spriteIDs[6]);

                }
                else
                {
                    deconnect(user);
                }

                StartCoroutine(ItemBuilder.HandleDuration(this, 1.5f, this.LastOwner, EndEffect));
            }


        }
        protected void EndEffect(PlayerController user)
        {
            // Nothing here 
        }
        private void postprocess(Projectile obj)
        {
            if(obj.gameObject.GetComponent<projectileStates>() != null)
            {
                if(obj.gameObject.GetComponent<projectileStates>().CarSMG == true)
                {
                    if(Stored.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                    {
                        if (UnityEngine.Random.Range(0, 4) > 1)
                        {
                            Stored.GainAmmo(1);
                        }
                    }
                    if(Stored.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic || Stored.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic || Stored.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Beam || Stored.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Burst)
                    {
                        if (UnityEngine.Random.Range(0, 7) > 1)
                        {
                            Stored.GainAmmo(1);
                        }
                    }
                    
                    
                }
            }
        }

        private void TargetDrop()
        {

            if (Stored != null)
            {
                deconnect(this.LastOwner);
                Stored.OnDropped -= TargetDrop;
            }
            
        }

        public void deconnect(PlayerController user)
        {
            Stored.PostProcessProjectile -= postprocess;

            ReconfigureVolley(storedprojectileVolleyData);
            Stored.RawSourceVolley = storedprojectileVolleyData;
            


            Stored.DuctTapeMergedGunIDs.Remove(CarSMG.PickupObjectId);
            user.stats.RecalculateStats(user, false, false);
            this.sprite.SetSprite(spriteIDs[5]);


            Stored = null;
            orgiAmmo = 0;
            storedprojectileVolleyData = null;

        }

        
        public override void OnPreDrop(PlayerController user)
        {
            if (Stored != null)
            {
                deconnect(this.LastOwner);
            }
            user.PostProcessThrownGun += Player_PostProcessThrownGun;
            base.OnPreDrop(user);
        }

        private void StoreCombine(Gun Target)
        {
            Stored = Target;
            orgiAmmo = Target.GetBaseMaxAmmo();

            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            if (Target.RawSourceVolley != null)
            {
                projectileVolleyData.InitializeFrom(Target.RawSourceVolley);
            }
            else
            {
                projectileVolleyData.projectiles = new List<ProjectileModule>();
                projectileVolleyData.projectiles.Add(ProjectileModule.CreateClone(Target.singleModule, true, -1));
                projectileVolleyData.BeamRotationDegreesPerSecond = float.MaxValue;
            }
            storedprojectileVolleyData = projectileVolleyData;
        }

        public override void Update()
        {
            if(this.LastOwner != null)
            {
                CanBeUsed(this.LastOwner);
            }
            
            base.Update();
        }
        public override bool CanBeUsed(PlayerController user)
        {
            if(user.CurrentGun != null)
            {

                return base.CanBeUsed(user) && user.CurrentGun.InfiniteAmmo != true;
            }
            else
            {
                return false;
            }

        }


        protected static ProjectileVolleyData CombineVolleys(Gun sourceGun, Gun mergeGun)
        {
            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            if (sourceGun.RawSourceVolley != null)
            {
                projectileVolleyData.InitializeFrom(sourceGun.RawSourceVolley);
            }
            else
            {
                projectileVolleyData.projectiles = new List<ProjectileModule>();
                projectileVolleyData.projectiles.Add(ProjectileModule.CreateClone(sourceGun.singleModule, true, -1));
                projectileVolleyData.BeamRotationDegreesPerSecond = float.MaxValue;
            }
            if (mergeGun.RawSourceVolley != null)
            {
                for (int i = 0; i < mergeGun.RawSourceVolley.projectiles.Count; i++)
                {
                    ProjectileModule projectileModule = ProjectileModule.CreateClone(mergeGun.RawSourceVolley.projectiles[i], true, -1);
                    projectileModule.IsDuctTapeModule = true;
                    projectileModule.ignoredForReloadPurposes = (projectileModule.ammoCost <= 0 || projectileModule.numberOfShotsInClip <= 0);
                    projectileStates State = projectileModule.projectiles[0].gameObject.GetOrAddComponent<projectileStates>();
                    State.CarSMG = true;
                    projectileVolleyData.projectiles.Add(projectileModule);
                    if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup) && i == 0)
                    {
                        projectileModule.runtimeGuid = ((projectileModule.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule.runtimeGuid);
                        sourceGun.AdditionalShootSoundsByModule.Add(projectileModule.runtimeGuid, mergeGun.gunSwitchGroup);
                    }
                    if (mergeGun.RawSourceVolley.projectiles[i].runtimeGuid != null && mergeGun.AdditionalShootSoundsByModule.ContainsKey(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid))
                    {
                        sourceGun.AdditionalShootSoundsByModule.Add(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid, mergeGun.AdditionalShootSoundsByModule[mergeGun.RawSourceVolley.projectiles[i].runtimeGuid]);
                    }
                }
            }
            else
            {
                ProjectileModule projectileModule2 = ProjectileModule.CreateClone(mergeGun.singleModule, true, -1);
                projectileModule2.IsDuctTapeModule = true;
                projectileModule2.ignoredForReloadPurposes = (projectileModule2.ammoCost <= 0 || projectileModule2.numberOfShotsInClip <= 0);
                projectileStates State = projectileModule2.projectiles[0].gameObject.GetOrAddComponent<projectileStates>();
                State.CarSMG = true;
                projectileVolleyData.projectiles.Add(projectileModule2);
                if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup))
                {
                    projectileModule2.runtimeGuid = ((projectileModule2.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule2.runtimeGuid);
                    sourceGun.AdditionalShootSoundsByModule.Add(projectileModule2.runtimeGuid, mergeGun.gunSwitchGroup);
                }
            }
            return projectileVolleyData;
        }

        public void ReconfigureVolley(ProjectileVolleyData newVolley)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int num = 0;
            for (int i = 0; i < newVolley.projectiles.Count; i++)
            {
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Automatic)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Beam)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Burst)
                {
                    flag4 = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Charged)
                {
                    flag3 = true;
                    num++;
                }
            }
            if (!flag && !flag2 && !flag3 && !flag4)
            {
                return;
            }
            if (!flag && !flag2 && !flag3 && flag4)
            {
                return;
            }
            int num2 = 0;
            for (int j = 0; j < newVolley.projectiles.Count; j++)
            {
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
                {
                    newVolley.projectiles[j].shootStyle = ProjectileModule.ShootStyle.Automatic;
                }
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.Charged && num > 1)
                {
                    num2++;
                    if (num > 1)
                    {
                    }
                }
            }
        }


      


    }
}
