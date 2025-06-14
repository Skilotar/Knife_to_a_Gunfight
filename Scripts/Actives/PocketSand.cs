using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.ItemAPI;
using SaveAPI;
using UnityEngine;
using System.Collections;

namespace Knives
{
    class PocketSand : PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension class
        public static void Register()
        {
            //The name of the item
            string itemName = "Pocket Sand";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it.
            string resourceName = "Knives/Resources/pocketSand";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a ActiveItem component to the object
            var item = obj.AddComponent<PocketSand>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Accuracy Fell!";
            string longDesc = "Throw sand into the eyes of your enemies.\n\n" +
                "A fistful of sand mixed with lint coins and other pocket debris. \n" +
                "It's extra effective against a handful of enemies." +
                "\n\n\n - Knife_to_a_Gunfight" +
                "\n Sprite by Dallan";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //"kts" here is the item pool. In the console you'd type kts:sweating_bullets
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Set the cooldown type and duration of the cooldown
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 20);

            //Set some other fields
            item.consumable = false;
            item.quality = ItemQuality.C;
            

            ID = item.PickupObjectId;

            pool = new VFXPool();
            pool.type = VFXPoolType.SequentialGroups;
            VFXComplex muzzle1 = MiscToolMethods.CreateVFXComplex("SandyMuzzle",
                   new List<string>()
                   {
                    "Knives/Resources/Muzzleflashes/Sandy/Sandy_muzzleflash_001",
                    "Knives/Resources/Muzzleflashes/Sandy/Sandy_muzzleflash_002",
                    "Knives/Resources/Muzzleflashes/Sandy/Sandy_muzzleflash_003",
                    "Knives/Resources/Muzzleflashes/Sandy/Sandy_muzzleflash_004",
                    "Knives/Resources/Muzzleflashes/Sandy/Sandy_muzzleflash_005",
                   },
                   12, //FPS
                   new IntVector2(52, 35), //Dimensions
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

            pool2 = new VFXPool();
            pool2.type = VFXPoolType.SequentialGroups;
            VFXComplex muzzle2 = MiscToolMethods.CreateVFXComplex("SandSandyMuzzle",
                   new List<string>()
                   {
                    "Knives/Resources/Muzzleflashes/SleepySandy/Sleepy_Sandy_muzzleflash_001",
                    "Knives/Resources/Muzzleflashes/SleepySandy/Sleepy_Sandy_muzzleflash_002",
                    "Knives/Resources/Muzzleflashes/SleepySandy/Sleepy_Sandy_muzzleflash_003",
                    "Knives/Resources/Muzzleflashes/SleepySandy/Sleepy_Sandy_muzzleflash_004",
                    "Knives/Resources/Muzzleflashes/SleepySandy/Sleepy_Sandy_muzzleflash_005",
                   },
                   12, //FPS
                   new IntVector2(52, 35), //Dimensions
                   tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                   false, //Uses a Z height off the ground
                   0, //The Z height, if used
                   false,
                  VFXAlignment.Fixed
                  );

            pool2.effects = new VFXComplex[]
            {
                muzzle2,
            };

        }
        public static VFXPool pool;
        public static VFXPool pool2;
        public static int ID;
       

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }


        public override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);

        }
        public override void DoEffect(PlayerController user)
        {
            user.DidUnstealthyAction();
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, user.CenterPosition, user, user.CurrentGun);
            ProjectileSlashingBehaviour Sand = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            Sand.SlashRange = 4.5f;
            Sand.SlashDimensions = 70;
            Sand.SlashDamage = 5;
            Sand.SlashDamageUsesBaseProjectileDamage = false;
            Sand.soundToPlay = "";
            Sand.OnSlashHitEnemy += this.ProcessSlash;
            Sand.SlashVFX = pool;
            if (user.PlayerHasActiveSynergy("Mr. SandMan")) Sand.SlashVFX = pool2;
            Sand.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            for(int i = 0; i <= 13; i++)
            {
                Projectile proj2 = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, user.CenterPosition, user, user.CurrentGun, UnityEngine.Random.Range(-40f,40f));
                proj2.baseData.damage = 1f;
                proj2.baseData.range = 15 * (1 + UnityEngine.Random.Range(-.2f, .2f));
                proj2.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
                proj2.AdditionalScaleMultiplier *= .25f;
                proj2.hitEffects.suppressMidairDeathVfx = true;
                proj2.objectImpactEventName = null;
                proj2.enemyImpactEventName = null;

                SlowDownOverTimeModifier slowDown = proj2.gameObject.AddComponent<SlowDownOverTimeModifier>();
                slowDown.extendTimeByRangeStat = true;
                slowDown.activateDriftAfterstop = true;
                slowDown.doRandomTimeMultiplier = true;
                slowDown.killAfterCompleteStop = true;
                slowDown.timeTillKillAfterCompleteStop = 0f;
                slowDown.timeToSlowOver = 1f;
                slowDown.targetSpeed = proj2.baseData.speed * 0.1f;

                proj2.OnHitEnemy += hitEnemy;
            }

        }

        private void hitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if (this.LastOwner.PlayerHasActiveSynergy("Mr. SandMan"))
                {
                    applySleep(arg2.aiActor);
                }
                else
                {
                    applyBlind(arg2.aiActor);
                }

                if (arg2.aiActor.EnemyGuid == "7b0b1b6d9ce7405b86b75ce648025dd6" || arg2.aiActor.EnemyGuid == "4b992de5b4274168a8878ef9bf7ea36b" || arg2.aiActor.EnemyGuid == "barretina" || arg2.aiActor.EnemyGuid == "glockulus" || arg2.aiActor.EnemyGuid == "Ophanaim" || arg2.aiActor.EnemyGuid == "gazer" || arg2.aiActor.EnemyGuid == "observer")
                {
                    arg2.aiActor.healthHaver.ApplyDamage(300, Vector2.zero, "AUGH MY EYE!!!!!!!", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                    AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                    AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                }

            }
        }

        private void ProcessSlash(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if(hitEnemy.aiActor != null)
            {   
                if (hitEnemy.healthHaver.IsAlive)
                {
                    if (player.PlayerHasActiveSynergy("Mr. SandMan"))
                    {
                        applySleep(hitEnemy);
                    }
                    else
                    {
                        applyBlind(hitEnemy);
                    }
                        
                }
            }
        }

        private void applySleep(AIActor hitEnemy)
        {
            if(hitEnemy.aiActor != null)
            {
                SleepStatus status = hitEnemy.aiActor.gameObject.GetOrAddComponent<SleepStatus>();
                status.statused = true;
                status.duration = 6;
            }
        }

        public void applyBlind(AIActor victim)
        {
            if (victim.healthHaver.IsBoss)
            {
                victim.ApplyEffect(new GameActorBlindEffect()
                {
                    AffectsEnemies = true,
                    OverheadVFX = GameActorBlindEffect.BlindVFX,
                    AffectsPlayers = false,
                    AppliesTint = false,

                    AppliesDeathTint = false,
                    AppliesOutlineTint = false,

                    duration = 2f,
                    SpeedMultiplier = .75f,
                    effectIdentifier = "Blind",
                    resistanceType = EffectResistanceType.None,
                    //PlaysVFXOnActor = true,
                    stackMode = GameActorEffect.EffectStackingMode.Ignore,
                });
            }
            else
            {
                victim.ApplyEffect(new GameActorBlindEffect()
                {
                    AffectsEnemies = true,
                    OverheadVFX = GameActorBlindEffect.BlindVFX,
                    AffectsPlayers = false,
                    AppliesTint = false,

                    AppliesDeathTint = false,
                    AppliesOutlineTint = false,

                    duration = 7,
                    SpeedMultiplier = .25f,
                    effectIdentifier = "Blind",
                    resistanceType = EffectResistanceType.None,
                    //PlaysVFXOnActor = true,
                    stackMode = GameActorEffect.EffectStackingMode.Ignore,
                });
            }

        }

    }
}