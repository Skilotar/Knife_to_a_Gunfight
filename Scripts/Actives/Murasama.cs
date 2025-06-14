using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Alexandria.BreakableAPI;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Murasama : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Murasama";
            string resourceName = "Knives/Resources/Murasama";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Murasama>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "There Will Be Bloodshed!";
            string longDesc = "A blinding fast strike that disarms opponents.\n\n" +
                "A blade once wielded by a cocky swordsman. A unique balistic mechanism in the scabbard can launch the blade to the owners hand faster than the eye.\n" +
                "\n\n\n - Knife_to_a_Gunfight";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 100);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);


            item.quality = ItemQuality.C;

            pool = new VFXPool();
            pool.type = VFXPoolType.SequentialGroups;

            VFXComplex muzzle1 = MiscToolMethods.CreateVFXComplex("Muzzleflash",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/Murasama/Murasama_slash_001",
                    "Knives/Resources/Muzzleflashes/Murasama/Murasama_slash_002",
                    "Knives/Resources/Muzzleflashes/Murasama/Murasama_slash_003",
                    "Knives/Resources/Muzzleflashes/Murasama/Murasama_slash_004",
                    "Knives/Resources/Muzzleflashes/Murasama/Murasama_slash_005",
                },
                18, //FPS
                new IntVector2(74, 76), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

            pool.effects = new VFXComplex[]
            {
                muzzle1,
            };

        }
        public static VFXPool pool;
        public override void DoEffect(PlayerController user)
        {
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, user.CenterPosition, user, user.CurrentGun);
            ProjectileSlashingBehaviour ArmCutter = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            ArmCutter.SlashRange = 4.5f;
            ArmCutter.SlashDamage = 14 * GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier;
            ArmCutter.SlashDamageUsesBaseProjectileDamage = false;
            ArmCutter.soundToPlay = "Play_obj_katana_slash_01";
            ArmCutter.OnSlashHitEnemy += this.ProcessSlash;
            ArmCutter.SlashVFX = pool;
            ArmCutter.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            AkSoundEngine.PostEvent("Play_WPN_deserteagle_shot_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_obj_katana_slash_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_obj_katana_slash_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_obj_katana_slash_01", base.gameObject);
        }

        private void ProcessSlash(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if (hitEnemy.aiShooter != null && hitEnemy.healthHaver.IsBoss != true)
            {
                if(hitEnemy.gameObject.GetComponent<DisarmedStatus>() == null)
                {
                    Vector2 dir = hitEnemy.specRigidbody.UnitCenter - player.specRigidbody.UnitCenter;
                    if(hitEnemy.aiShooter.handObject != null)
                    {
                        hitEnemy.aiShooter.ToggleHandRenderers(false, "Disarmed");
                        //hitEnemy.aiShooter.handObject.transform.SetParent(null, true);
                    }

                    if (hitEnemy.aiShooter.CurrentGun != null)
                    {
                        hitEnemy.aiShooter.CurrentGun.sprite.BecomeDebris(dir * 4, .5f);
                        hitEnemy.aiShooter.CurrentGun.transform.SetParent(null, true);
                    }
                }

                hitEnemy.aiShooter.customShootCooldownPeriod = 999999;
                hitEnemy.CanTargetPlayers = false;
                hitEnemy.CanTargetEnemies = false;
                DisarmedStatus status = hitEnemy.gameObject.GetOrAddComponent<DisarmedStatus>();
                status.player = player;
                UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.TeleporterPrototypeTelefragVFX, hitEnemy.CenterPosition, Quaternion.identity);

                
               
            }

        }
    }
}