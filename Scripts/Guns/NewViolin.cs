using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Dungeonator;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class NewViolin : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Violince", "Violin");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:violince", "ski:violince");
            gun.gameObject.AddComponent<NewViolin>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("A Bet Well Won");
            gun.SetLongDescription("Jams and Unjams enemies. Extra Damage when Unjamming an enemy. Reload to charm Jammed Enemies\n\n" +
                "" +
                "A Fiddle won from the lord of the jammed himself in a contest.\n\n" +
                "This fiddle holds great holy power and will quell the hatred of the jammed."+
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Violin_idle_001", 8);

            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);

            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(8) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 3f;

            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 5;
            gun.SetBaseMaxAmmo(300);
           
            gun.quality = PickupObject.ItemQuality.B;
            
            gun.muzzleFlashEffects = null;
            gun.PreventNormalFireAudio = true;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.chargeProjectiles[0].Projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 40f;
            projectile.baseData.range = 4000f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            projectile.SetProjectileSpriteRight("Goldtip", 19, 5, false, tk2dBaseSprite.Anchor.LowerLeft, 19,5);
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Goldtip", "Knives/Resources/Violin_clipfull", "Knives/Resources/Violin_clipempty");
            
            gun.barrelOffset.localPosition = new Vector3(1.1875f, 0.8125f, 0f);
            tk2dSpriteAnimationClip animationclip = gun.sprite.spriteAnimator.GetClipByName(gun.idleAnimation);
            float universalX = 10;
            float[] offsetsX = new float[] { 0.0625f * universalX };
            float[] offsetsY = new float[] { 0.0000f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < animationclip.frames.Length; i++) { int id = animationclip.frames[i].spriteId; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i]; }

            tk2dSpriteAnimationClip animationclipch = gun.sprite.spriteAnimator.GetClipByName(gun.chargeAnimation);
            float[] offsetsXch = new float[] { 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX };
            float[] offsetsYch = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f };
            for (int i = 0; i < offsetsXch.Length && i < offsetsYch.Length && i < animationclipch.frames.Length; i++) { int id = animationclipch.frames[i].spriteId; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsXch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsYch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsXch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsYch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsXch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsYch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsXch[i]; animationclipch.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsYch[i]; }


            tk2dSpriteAnimationClip animationclipfr = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            float[] offsetsXfr = new float[] { 0.0625f * universalX, 0.0625f * universalX };
            float[] offsetsYfr = new float[] { 0.0000f, 0.0000f };
            for (int i = 0; i < offsetsXfr.Length && i < offsetsYfr.Length && i < animationclipfr.frames.Length; i++) { int id = animationclipfr.frames[i].spriteId; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsXfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsYfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsXfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsYfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsXfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsYfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsXfr[i]; animationclipfr.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsYfr[i]; }
            projectile.transform.parent = gun.barrelOffset;

            tk2dSpriteAnimationClip animationclipre = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
            float[] offsetsXre = new float[] { 0.0625f * universalX, 0.0625f * 18, 0.0625f * 18, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * 18, 0.0625f * 18, 0.0625f * universalX, 0.0625f * universalX, 0.0625f * universalX };
            float[] offsetsYre = new float[] { 0.0000f, 0.0625f * 8, 0.0625f * 8, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0000f, 0.0625f * 8, 0.0625f * 8, 0.0000f, 0.0000f, 0.0000f };
            for (int i = 0; i < offsetsXre.Length && i < offsetsYre.Length && i < animationclipre.frames.Length; i++) { int id = animationclipre.frames[i].spriteId; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsXre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsYre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsXre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsYre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsXre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsYre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsXre[i]; animationclipre.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsYre[i]; }

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .5f,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
               
            };


            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }



        public override void OnPostFired(PlayerController player, Gun gun)
        {
            this.gun.PreventNormalFireAudio = true;
        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            int sfx = rng.Next(0, 2);
            if (sfx == 0)
            {
                AkSoundEngine.PostEvent("Play_Vio_fire_001", base.gameObject);
            }
            if (sfx == 1)
            {
                AkSoundEngine.PostEvent("Play_Vio_fire_002", base.gameObject);

            }
            if (sfx == 2)
            {
                AkSoundEngine.PostEvent("Play_Vio_fire_003", base.gameObject);
            }

            
            projectile.OnHitEnemy += this.OnHitEnemy;

            base.PostProcessProjectile(projectile);
        }


        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody body, bool yes)
        {
            if (body)
            {
                if (body.aiActor)
                {
                    if (body.aiActor.healthHaver)
                    {
                        if(yes == false)
                        {
                            if (!body.aiActor.IsBlackPhantom)
                            {
                                body.aiActor.BecomeBlackPhantom();
                            }
                            else
                            {
                                body.aiActor.UnbecomeBlackPhantom();
                                body.aiActor.healthHaver.ApplyDamage(30, Vector2.zero, "banishment");
                                if (body.aiActor.healthHaver.IsBoss)
                                {
                                    body.aiActor.healthHaver.ApplyDamage(30, Vector2.zero, "banishment");
                                }
                            }
                           
                        }
                    }
                }
            }
        }

        private bool HasReloaded;
        public bool toggle = true;

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
                }

            }




            base.Update();
        }
        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                
                int sfx = rng.Next(1, 3);
                if (sfx == 1)
                {
                    AkSoundEngine.PostEvent("Play_Vio_reload_001", base.gameObject);
                }
                if (sfx == 2)
                {
                    AkSoundEngine.PostEvent("Play_Vio_reload_002", base.gameObject);

                }
                if (sfx == 3)
                {
                    AkSoundEngine.PostEvent("Play_Vio_reload_003", base.gameObject);
                }
                if (sfx == 4)
                {
                    AkSoundEngine.PostEvent("Play_Vio_reload_004", base.gameObject);
                }
                //charm jammed enemies 
                RoomHandler room = player.CurrentRoom;
                if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
                foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (enemy.IsBlackPhantom)
                    {

                        enemy.ApplyEffect(PickupObjectDatabase.GetById(527).GetComponent<BulletStatusEffectItem>().CharmModifierEffect);
                        enemy.UnbecomeBlackPhantom();
                    }
                }

            }
        }


    }
}


