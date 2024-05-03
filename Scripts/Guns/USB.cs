using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class USB : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("USB", "USB");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:usb", "ski:usb");
            gun.gameObject.AddComponent<USB>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Universal.Space.Blaster.");
            gun.SetLongDescription("Reload to scan enemies and copy their traits until a new scan is done and they are overridden\n\n" +
                "A highly moddable space blaster with a port in the handle for a memory drive to be inserted. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "USB_idle_001", 2);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(345) as Gun, true, false);
            Gun gun2 = PickupObjectDatabase.GetById(345) as Gun;

            gun.muzzleFlashEffects.type = VFXPoolType.None;
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.6f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 1f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //Setting static values for a custom gun's projectile stats prevents them from scaling with player stats and bullet modifiers (damage, shotspeed, knockback)
            //You have to multiply the value of the original projectile you're using instead so they scale accordingly. For example if the projectile you're using as a base has 10 damage and you want it to be 6 you use this
            //In our case, our projectile has a base damage of 5.5, so we multiply it by 1.1 so it does 10% more damage from the ak-47.
            projectile.baseData.damage = 5f;
            projectile.baseData.speed *= 1.2f;
            projectile.AdditionalScaleMultiplier = .6f;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;


        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);

        }
        public enum EnemyType
        {
            NONE,
            BASIC,
            SHOTGUN,
            SNIPER,
            MAGIC,
            RADIAL,
            MELEE,
            EXPLOSIVE,
            BOSS,
            FULLAUTO,
            SPLIT,
            DEVO,
            SPOOKY,
            LASER
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            switch (gundownloadedState)
            {
                case 0: //None
                    break;

                case 1: //basic - duplicate
                    StartCoroutine(USBbasic(projectile,player));
                    break;

                case 2: //shotgun
                    StartCoroutine(USBshotty(projectile, player));
                    break;

                case 3: //sniper
                    StartCoroutine(USBsnipe(projectile, player));
                    break;

                case 4: //magic
                    StartCoroutine(USBmagic(projectile, player));
                    break;

                case 5: //radial
                    StartCoroutine(USBpulse(projectile, player));
                    break;

                case 6: //melee
                    StartCoroutine(USBmelee(projectile, player));
                    break;

                case 7: //explosive
                    StartCoroutine(USBExplosive(projectile, player));
                    break;

                case 8: //boss
                    StartCoroutine(USBHome(projectile, player));
                    break;

                case 9: // full auto
                    StartCoroutine(USBAuto(projectile, player));
                    break;

                case 10: //split
                    StartCoroutine(USBsplit(projectile, player));
                    break;

                case 11: //Devo
                    StartCoroutine(USBDevolve(projectile, player));
                    break;

                case 12: //spooky
                    StartCoroutine(USBSpooky(projectile, player));
                    break;

                case 13: //laser
                    StartCoroutine(USBLaser(projectile, player));
                    break;

            }
            base.PostProcessProjectile(projectile);
        }
        private IEnumerator USBAuto(Projectile proj, PlayerController player)
        {
            
            yield return new WaitForSeconds(.05f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle +2), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

            yield return new WaitForSeconds(.05f);
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle -2), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag22 = component2 != null;
            if (flag22)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
        }

        private IEnumerator USBsplit(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.001f);

            if(projectile != null)
            {
                ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
                split.distanceBasedSplit = true;
                split.amtToSplitTo = 2;
                split.distanceTillSplit = .001f;
                split.dmgMultAfterSplit = 1f;
                split.numberofsplits = 1;
                split.removeComponentAfterUse = true;
                split.splitAngles = 45f;

               
            }

        }
        private IEnumerator USBmagic(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.1f);
            
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[145]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.ChanceToTransmogrify = .3f;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBsnipe(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.1f);
            
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[25]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_m1rifle_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }
        private IEnumerator USBDevolve(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.001f);
            if (projectile != null)
            {
                projectile.OnHitEnemy += this.OnHitEnemy;
                projectile.sprite.color = UnityEngine.Color.black;
            }

        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (UnityEngine.Random.Range(1, 4) == 1)
                {
                    arg2.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid("05891b158cd542b1a5f3df30fb67a7ff"), BlinkpoofVfx);
                }
            }
            
        }

        private IEnumerator USBSpooky(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.1f);

            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[29]).DefaultModule.projectiles[0];
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }

        }

        private IEnumerator USBLaser(Projectile projectile, PlayerController player)
        {
            yield return new WaitForSeconds(.01f);
            if(projectile != null)
            {
                projectile.baseData.speed *= .5f;
                projectile.UpdateSpeed();
                BeamBulletsBehaviour beam = projectile.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
                beam.beamToFire = ((Gun)PickupObjectDatabase.GetById(87)).DefaultModule.projectiles[0].projectile;
                beam.firetype = BeamBulletsBehaviour.FireType.CROSS;
            }
           
        }

        private IEnumerator USBHome(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.2f);
            if(projectile != null)
            {
                projectile.SendInDirection(OMITBMathsAndLogicExtensions.GetVectorToNearestEnemy(projectile.LastPosition), true);
                projectile.baseData.damage *= 2f;
            }
            

        }

        private IEnumerator USBExplosive(Projectile projectile, PlayerController player)
        {

            yield return new WaitForSeconds(.001f);
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.blast));

        }

        private void blast(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (!arg3)
                {
                    BlastBlightedStatusController blast = arg2.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                    blast.statused = true;
                }
            }
        }

        public int startingRotation = 0;
        private IEnumerator USBpulse(Projectile proj, PlayerController player)
        {
            Projectile projectile = proj;
            yield return new WaitForSeconds(.001f);
            
            for (int i = 0; i < 6; i++)
            {

                GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 60) + startingRotation), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component);
                    }

                }
            }
            if (startingRotation != 180)
            {
                startingRotation += 20;
            }
            
        }
        public System.Random rng = new System.Random();
        private void doslashy(PlayerController player)
        {
            Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.transform.position + new Vector3(.5f, .5f, .0f), Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.AdditionalScaleMultiplier = .001f;
            component.Owner = this.gun.CurrentOwner;
            component.baseData.damage = 10;
            ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashRange = 3.7f;
            slashy.DoSound = false;
            slashy.SlashDimensions = 50;
            slashy.SlashVFX = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;
            slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
        }
        private IEnumerator USBmelee(Projectile proj, PlayerController player)
        {
            Projectile projectile = proj;
            yield return new WaitForSeconds(.001f);
            doslashy(player);
            int sound = rng.Next(1, 4);
            if (sound == 1)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_001", base.gameObject);
            }
            if (sound == 2)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_002", base.gameObject);
            }
            if (sound == 3)
            {
                AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
                AkSoundEngine.PostEvent("Play_impress_fire_normal_003", base.gameObject);
            }

        }

        private IEnumerator USBshotty(Projectile proj, PlayerController player)
        {
            Projectile projectile = proj;
            yield return new WaitForSeconds(.001f);
           
            GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 5), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }

            }
            GameObject gameObject2 = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 5), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag3 = component2 != null;
            if (flag3)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                if (UnityEngine.Random.Range(1, 11) == 1)
                {
                    player.DoPostProcessProjectile(component);
                }


            }
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
            
        }
        private IEnumerator USBbasic(Projectile proj,PlayerController player)
        {
            Projectile projectile = proj;
            yield return new WaitForSeconds(.1f);
            
            
                AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);
                GameObject gameObject = SpawnManager.SpawnProjectile(this.gun.DefaultModule.projectiles[0].projectile.gameObject.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component);
                    }

                }
            
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void  Update()
        {
            if (gun.CurrentOwner != null)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                    AkSoundEngine.PostEvent("Play_USB_reload_002", base.gameObject);

                }


            }

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_USB_reload_001", base.gameObject);

                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));

                component.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/GlitchUnlit");
                component.sprite.usesOverrideMaterial = true;
                component.sprite.renderer.material.SetFloat("_GlitchInterval", 0.1f);
                component.sprite.renderer.material.SetFloat("_DispProbability", 0.03f);
                component.sprite.renderer.material.SetFloat("_DispIntensity", 0.03f);
                component.sprite.renderer.material.SetFloat("_ColorProbability", 0.4f);
                component.sprite.renderer.material.SetFloat("_ColorIntensity", 0.04f);

                component.pierceMinorBreakables = true;
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 0f;
                   
                }
                
            }

        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            GetEnemyType(arg2.aiActor);
            StartCoroutine(playchime());
        }

        private IEnumerator playchime()
        {
            
            AkSoundEngine.PostEvent("Play_USB_fire_002", base.gameObject);
            yield return new WaitForSeconds(.5f);
            AkSoundEngine.PostEvent("Play_USB_fire_001", base.gameObject);
            
        }

        private void GetEnemyType(AIActor hitenemy)
        {
            foreach(string id in Basic)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BASIC;
                   
                    break;
                }

            }
            foreach (string id in Auto)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.FULLAUTO;

                    break;
                }

            }
            foreach (string id in split)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SPLIT;

                    break;
                }

            }
            foreach (string id in devo)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.DEVO;

                    break;
                }

            }
            foreach (string id in laser)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.LASER;

                    break;
                }

            }
            foreach (string id in Spooky)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SPOOKY;

                    break;
                }

            }
            foreach (string id in Shotguns)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SHOTGUN;
                    
                    break;
                }

            }
            foreach (string id in Sniper)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.SNIPER;
                    
                    break;
                }

            }
            foreach (string id in Magic)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MAGIC;
                    
                    break;
                }

            }
            foreach (string id in Radial)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.RADIAL;
                    
                    break;
                }

            }
            foreach (string id in Melee)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.MELEE;
                    
                    break;
                }

            }
            foreach (string id in Explosive)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.EXPLOSIVE;
                    
                    break;
                }

            }
            foreach (string id in Boss)
            {
                if (id == hitenemy.EnemyGuid)
                {
                    gundownloadedState = (int)EnemyType.BOSS;
                    
                    break;
                }

            }
           
        }
        public int gundownloadedState;
        

        public List<string> Basic = new List<string> // Extra bullets
        {
            "01972dee89fc4404a5c408d50007dad5", // bulletkin
            "70216cae6c1346309d86d4a0b4603045", // veterin 
            "3cadf10c489b461f9fb8814abc1a09c1", // minelet
            "8bb5578fba374e8aae8e10b754e61d62", // cardinal
            "1a78cfb776f54641b832e92c44021cf2", // ashen bk
            "d4a9836f8ab14f3fadd0f597438b1f1f", // mutant 
            "5f3abc2d561b4b9c9e72b879c6f10c7e", // fallen
            "844657ad68894a4facb1b8e1aef1abf9", // hooded 
            "906d71ccc1934c02a6f4ff2e9c07c9ec", // office kin man
            "9eba44a0ea6c4ea386ff02286dd0e6bd", // office kin woman
            "05cb719e0178478685dc610f8b3e8bfc", // brolett
            "5861e5a077244905a8c25c2b7b4d6ebb", // west
            "6f818f482a5c47fd8f38cce101f6566c", // pirate
            "39e6f47a16ab4c86bec4b12984aece4c", // armor
            "699cd24270af4cd183d671090d8323a1", // key
            "a446c626b56d4166915a4e29869737fd", // chance
            "2feb50a6a40f4f50982e89fd276f6f15", // bullat
            "7ec3e8146f634c559a7d58b19191cd43", // spiralatt
            "2ebf8ef6728648089babb507dec4edb7", // br_mimic
            "d8d651e3484f471ba8a2daa4bf535ce6", // blue
            "abfb454340294a0992f4173d6e5898a8", // green
            "57255ed50ee24794b7aac1ac3cfb8a95", // gun cultist
            "9b4fb8a2a60a457f90dcf285d34143ac", // gat 
            "d4f4405e0ff34ab483966fd177f2ece3", // grey cylinder
            "534f1159e7cf4f6aa00aeea92459065e", // red cylinder
            "2b6854c0849b4b8fb98eb15519d7db1c", // bullet mech
            "9d50684ce2c044e880878e86dbada919", // coaler
            "72d2f44431da43b8a3bae7d8a114a46d", // bullet shark
            "c182a5cb704d460d9d099a47af49c913", // pot fairy
            "226fd90be3a64958a5b13cb0a4f43e97", // musket
            "3b0bd258b4c9432db3339665cc61c356", // western cactus
            "37de0df92697431baa47894a075ba7e9", // candle
            "ed37fa13e0fa4fcf8239643957c51293", //
            "45192ff6d6cb43ed8f1a874ab6bef316",
            "98ca70157c364750a60f5e0084f9d3e2",
            "6e972cd3b11e4b429b888b488e308551",
            "8a9e9bedac014a829a48735da6daf3da",
            "80ab6cd15bfc46668a8844b2975c6c26",
            "4b21a913e8c54056bc05cafecf9da880",
            "f020570a42164e2699dcf57cac8a495c",
            "06f5623a351c4f28bc8c6cda56004b80",
            "143be8c9bbb84e3fb3ab98bcd4cf5e5b",
            "475c20c1fd474dfbad54954e7cba29c1",
            "3f6d6b0c4a7c4690807435c7b37c35a5",
            "22fc2c2c45fb47cf9fb5f7b043a70122",
            "7b0b1b6d9ce7405b86b75ce648025dd6",
            "cf27dd464a504a428d87a8b2560ad40a",
            "f38686671d524feda75261e469f30e0b",
            "47bdfec22e8e4568a619130a267eab5b",
            "78a8ee40dff2477e9c2134f6990ef297",
            "566ecca5f3b04945ac6ce1f26dedbf4f",
            "eeb33c3a5a8e4eaaaaf39a743e8767bc",
            "b5e699a0abb94666bda567ab23bd91c4",
            "d4dd2b2bbda64cc9bcec534b4e920518",
            "02a14dec58ab45fb8aacde7aacd25b01",
            "4538456236f64ea79f483784370bc62f",
            "be0683affb0e41bbb699cb7125fdded6",
            "78eca975263d4482a4bfa4c07b32e252",
            "2e6223e42e574775b56c6349921f42cb",
            "a9cc6a4e9b3d46ea871e70a03c9f77d4",
            "556e9f2a10f9411cb9dbfd61e0e0f1e1",
            "12a054b8a6e549dcac58a82b89e319e5",
        };

        public List<string> Auto = new List<string> //*
        {
          "df7fb62405dc4697b7721862c7b6b3cd", // tread kin -- auto
             "db35531e66ce41cbb81d507a34366dfe", // ak kin -- auto
           "88b6b6a93d4b4234a67844ef4728382c", // bandana kin red -- auto
        };
        public List<string> split = new List<string> //*
        {
           "e5cffcfabfae489da61062ea20539887", // shroomer -- special
        };
        public List<string> devo = new List<string> //*
        {
            "05891b158cd542b1a5f3df30fb67a7ff", // arrowhead - devolver rounds
        };
        public List<string> Spooky = new List<string> //*
        {
            "4db03291a12144d69fe940d5a01de376", // hollow -- ghost
            "56f5a0f2c1fc4bc78875aea617ee31ac", // spectre
            "336190e29e8a4f75ab7486595b700d4a", // skullet
            "95ec774b5a75467a9ab05fa230c0c143", // skullmet
            "5288e86d20184fa69c91ceb642d31474", // gunmy
            "e21ac9492110493baef6df02a2682a0d", // gunmey_spent 
            "af84951206324e349e1f13f9b7b60c1a", // skusket
            "1cec0cdf383e42b19920787798353e46", // black skusk
            "c2f902b7cbe745efb3db4399927eab34", // skusk head 
            "21dd14e5ca2a4a388adab5b11b69a1e1", // shelleton
            "d5a7b95774cd41f080e517bea07bf495", // revol
            "88f037c3f93b4362a040a87b30770407", // gunreap -- ghost
        };
        public List<string> laser = new List<string> //*
        {
            "ac986dabc5a24adab11d48a4bccf4cb1", //Det -- laser bullet mod
            "48d74b9c65f44b888a94f9e093554977", // X det
            "c5a0fd2774b64287bf11127ca59dd8b4", // diag x det
            "b67ffe82c66742d1985e5888fd8e6a03", // vert det
            "d9632631a18849539333a92332895ebd", // diag det
            "1898f6fe1ee0408e886aaf05c23cc216", // hor x det
            "abd816b0bcbf4035b95837ca931169df", // vert x det
            "07d06d2b23cc48fe9f95454c839cb361", // hor x det -- laser bullet mod
        };
        public List<string> Shotguns = new List<string> // shotgun of bullets
        {
            "128db2f0781141bcb505d8f00f9e4d47",
            "b54d89f9e802455cbb2b8a96a31e8259",
            "2752019b770f473193b08b4005dc781f",
            "7f665bd7151347e298e4d366f8818284",
            "b1770e0f1c744d9d887cc16122882b4f",
            "1bd8e49f93614e76b140077ff2e33f2b",
            "044a9f39712f456597b9762893fbc19c",
            "37340393f97f41b2822bc02d14654172",
            "ddf12a4881eb43cfba04f36dd6377abb", 
            "86dfc13486ee4f559189de53cfb84107",
            "e861e59012954ab2b9b6977da85cb83c",

            "2d4f8b5404614e7d8b235006acde427a",
        };
        public List<string> Sniper = new List<string> // sniper bullets
        {
            "31a3ea0c54a745e182e22ea54844a82d",
            "c5b11bfc065d417b9c4d03a5e385fe2c",
        };
        public List<string> Magic = new List<string> // Witch Bullets
        {
            "206405acad4d4c33aac6717d184dc8d4",
            "c4fba8def15e47b297865b18e36cbef8",
            "9b2cf2949a894599917d4d391a0b7394",
            "56fb939a434140308b8f257f0f447829",
            "c0ff3744760c4a2eb0bb52ac162056e6",
            "6f22935656c54ccfb89fca30ad663a64",
            "a400523e535f41ac80a43ff6b06dc0bf",
            "216fd3dfb9da439d9bd7ba53e1c76462", 
            "78e0951b097b46d89356f004dda27c42",
            "cf2b7021eac44e3f95af07db9a7c442c",
            "c50a862d19fc4d30baeba54795e8cb93",
            "b1540990a4f1480bbcb3bea70d67f60d",
            "8b4a938cdbc64e64822e841e482ba3d2",
            "ba657723b2904aa79f9e51bce7d23872",
            "43426a2e39584871b287ac31df04b544",
            "ffdc8680bdaa487f8f31995539f74265",
            "d8a445ea4d944cc1b55a40f22821ae69",
            "43426a2e39584871b287ac31df04b544",
        };
        public List<string> Radial = new List<string> //Ring of bullets
        {
            "022d7c822bc146b58fe3b0287568aaa2",
            "ccf6d241dad64d989cbcaca2a8477f01",
            "062b9b64371e46e195de17b6f10e47c8",
            "116d09c26e624bca8cca09fc69c714b3",
            "864ea5a6a9324efc95a0dd2407f42810",
            "0b547ac6b6fc4d68876a241a88f5ca6a",
            "1bc2a07ef87741be90c37096910843ab",
            "1a4872dafdb34fd29fe8ac90bd2cea67",
            "981d358ffc69419bac918ca1bdf0c7f7",
            "d8fd592b184b4ac9a3be217bc70912a2",
            "6450d20137994881aff0ddd13e3d40c8",
            "ac9d345575444c9a8d11b799e8719be0",
            "796a7ed4ad804984859088fc91672c7f",
            "cd4a4b7f612a4ba9a720b9f97c52f38c",
            "98ea2fe181ab4323ab6e9981955a9bca",
            "f905765488874846b7ff257ff81d6d0c",
            "ff4f54ce606e4604bf8d467c1279be3e",
        };
        public List<string> Melee = new List<string> // swipe
        {
            "0239c0680f9f467dbe5c4aab7dd1eca6",
            "042edb1dfb614dc385d5ad1b010f2ee3", 
            "42be66373a3d4d89b91a35c9ff8adfec",
            "e61cab252cfb435db9172adc96ded75f",
            "fe3fe59d867347839824d5d9ae87f244", 
            "b8103805af174924b578c98e95313074",
            "f155fd2759764f4a9217db29dd21b7eb",
            "33b212b856b74ff09252bf4f2e8b8c57",
            "3f2026dc3712490289c4658a2ba4a24b",
            "ec8ea75b557d4e7b8ceeaacdf6f8238c",
            "463d16121f884984abe759de38418e48",
            "383175a55879441d90933b5c4e60cf6f",
            "6b7ef9e5d05b4f96b04f05ef4a0d1b18",
            "98fdf153a4dd4d51bf0bafe43f3c77ff",
            "249db525a9464e5282d02162c88e0357", // spent
        };
        public List<string> Explosive = new List<string> // boom
        {
            "b4666cb6ef4f4b038ba8924fd8adf38f",
            "4d37ce3d666b4ddda8039929225b7ede",
            "c0260c286c8d4538a697c5bf24976ccf",
            "5f15093e6f684f4fb09d3e7e697216b4",
        };
        public List<string> Boss = new List<string> // Auto Aim bullets
        {
            "bb73eeeb9e584fbeaf35877ec176de28",
            "edc61b105ddd4ce18302b82efdc47178",
            "39de9bd6a863451a97906d949c103538",
            "db97e486ef02425280129e1e27c33118",
            "ea40fcc863d34b0088f490f4e57f8913",
            "c00390483f394a849c36143eb878998f",
            "ec6b674e0acd4553b47ee94493d66422",
            "ffca09398635467da3b1f4a54bcfda80", 
            "1b5810fafbec445d89921a4efb4e42b7", 
            "4b992de5b4274168a8878ef9bf7ea36b", 
            "c367f00240a64d5d9f3c26484dc35833", 
            "da797878d215453abba824ff902e21b4", 
            "5729c8b5ffa7415bb3d01205663a33ef", 
            "fa76c8cfdf1c4a88b55173666b4bc7fb",
            "8b0dd96e2fe74ec7bebc1bc689c0008a", 
            "5e0af7f7d9de4755a68d2fd3bbc15df4", 
            "9189f46c47564ed588b9108965f975c9", 
            "6868795625bd46f3ae3e4377adce288b",
            "4d164ba3f62648809a4a82c90fc22cae",
            "6c43fddfd401456c916089fdd1c99b1c",
            "3f11bbbc439c4086a180eb0fb9990cb4",
            "f3b04a067a65492f8b279130323b41f0",
            "41ee1c8538e8474a82a74c4aff99c712",
            "465da2bb086a4a88a803f79fe3a27677",
            "05b8afe0b6cc4fffa9dc6036fa24c8ec", 
            "cd88c3ce60c442e9aa5b3904d31652bc",
            "68a238ed6a82467ea85474c595c49c6e", 
            "7c5d5f09911e49b78ae644d2b50ff3bf", 
        };
        public static BlinkPassiveItem m_BlinkPassive = PickupObjectDatabase.GetById(436).GetComponent<BlinkPassiveItem>();
        public GameObject BlinkpoofVfx = m_BlinkPassive.BlinkpoofVfx;
    }
}

