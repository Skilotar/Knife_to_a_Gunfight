using System;
using ItemAPI;
using Dungeonator;
using Gungeon;
using SaveAPI;

using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;

namespace Knives
{
    class FirstImpression : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("First Impression", "First");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:first_impression", "ski:first_impression");
            gun.gameObject.AddComponent<FirstImpression>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Make It Count");
            gun.SetLongDescription("A blade that knows how to make an entrance! Gains massive strength if it is the first attack you do in a room." + 
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "First_idle_001", 8);

            
            gun.SetAnimationFPS(gun.shootAnimation, 17);
            gun.SetAnimationFPS(gun.reloadAnimation, 17);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 12);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.4f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.DefaultModule.cooldownTime = .5f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, 2f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 15f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            gun.muzzleFlashEffects = null;
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
           
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            
            projectile2.baseData.damage = 40f;
            projectile2.baseData.speed *= 4f;
            projectile2.baseData.range = 15f;
            projectile2.baseData.force = 5;
            projectile2.SetProjectileSpriteRight("Firstimpress", 28, 39, false, tk2dBaseSprite.Anchor.MiddleCenter, 20, 36);
            gun.muzzleFlashEffects = null; // dont set this to use the other no VFX thing. It breaks it, but only sometimes.
            specialsyn = projectile2;
            gun.carryPixelOffset = new IntVector2(0, 2);
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1.5f, StatModifier.ModifyMethod.ADDITIVE);
            gun.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 4f);
            mat.SetFloat("_EmissivePower", 3);

            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                ETGModConsole.Log("nope");
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.FIRST_IMPRESS, true);
            MiscToolMethods.TrimAllGunSprites(gun);
            FirstImpression.StandardID = gun.PickupObjectId;

        }
        public static Projectile specialsyn;

        public static int StandardID;

        public System.Random rng = new System.Random();

       
       
        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;

        }
        public IEnumerator delaydown(PlayerController player)
        {

            gun.DefaultModule.cooldownTime = 1.3f;
            player.inventory.GunLocked.SetOverride("Impress_Safe", true, null);
            gun.CanBeDropped = false;
            yield return new WaitForSeconds(1.2f);
            hasAttackedThisCombat = true;
            gun.CanBeDropped = false;
            player.inventory.GunLocked.RemoveOverride("Impress_Safe");
            gun.DefaultModule.cooldownTime = .5f;
        }
        

        public override void PostProcessProjectile(Projectile projectile)
        {
           
            if (hasAttackedThisCombat != true)
            {
                StartCoroutine(delaydown(this.gun.CurrentOwner as PlayerController));
            }

            if (FirstImpression.hasAttackedThisCombat == true)
            {
                projectile.SuppressHitEffects = true;
                projectile.sprite.enabled = false;
                projectile.hitEffects.suppressMidairDeathVfx = true;
                projectile.DieInAir();
                StartCoroutine(slightslashdelay());
                int sound = rng.Next(1, 4);
                if (sound == 1)
                {
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
                }
                if(sound == 2)
                {
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
                }
                if(sound == 3)
                {
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
                    AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
                }
               
            }
            else
            {

                tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("First_critical_fire");
                gun.spriteAnimator.Play(clip, 0, 10, true);

                projectile.baseData.damage = 50;
                ProjectileSlashingBehaviour slashy = projectile.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                
                slashy.SlashRange = 6.5f;
                slashy.SlashDimensions = 75;
                slashy.DoSound = false;
                slashy.SlashVFX.type = VFXPoolType.None;
                int sound = rng.Next(1, 3);
                if(sound == 1)
                {
                    AkSoundEngine.PostEvent("Play_impress_fire_Special_001", base.gameObject);
                    AkSoundEngine.PostEvent("Play_impress_fire_Special_001", base.gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_impress_fire_Special_002", base.gameObject);
                    AkSoundEngine.PostEvent("Play_impress_fire_Special_002", base.gameObject);
                }
                if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Strike First"))
                {

                    
                    GameObject gameObject = SpawnManager.SpawnProjectile(specialsyn.gameObject, (this.Owner as PlayerController).specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, ((this.Owner as PlayerController).CurrentGun == null) ? 0f : (this.Owner as PlayerController).CurrentGun.CurrentAngle));
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.Owner = (this.Owner as PlayerController);
                  

                }
            }           

            base.PostProcessProjectile(projectile);
        }
      
        public IEnumerator slightslashdelay()
        {
            yield return new WaitForSeconds(.15f);
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f, .5f, 0), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.AdditionalScaleMultiplier = .001f;
            component.Owner = this.gun.CurrentOwner;
            component.baseData.damage = 15;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 5f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 60;
            slashy.SlashVFX.type = VFXPoolType.None;
        }

        private bool HasReloaded;
       
        public override void  Update()
        {
           
            if (gun.CurrentOwner)
            {
               
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                //if (this.gun.CurrentOwner.healthHaver.GetCurrentHealthPercentage() == 0)
                {
                    //UnityEngine.GameObject.Destroy(this);
                }

                if ((this.gun.CurrentOwner as PlayerController).IsInCombat)
                {
                    if(knownInCombatState == false) // new encounter started moved from known false to known true.
                    {
                        hasAttackedThisCombat = false;
                        knownInCombatState = true;
                        //ETGModConsole.Log(hasAttackedThisCombat + "Enter Combat?");
                    }
                }
                else
                {
                    if(knownInCombatState == true)
                    {
                        knownInCombatState = false;
                    }
                }

               
            }
        }

        public bool knownInCombatState = false;
        public static bool hasAttackedThisCombat = false;
       

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {


                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
               
                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }

      
    }
}