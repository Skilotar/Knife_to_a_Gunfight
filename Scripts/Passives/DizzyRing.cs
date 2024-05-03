using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using System.Collections;
using Alexandria;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Knives
{
    public class Dizzyring :PassiveItem
    { 
        public static void Register()
        {
            string itemName = "Dragun Pearl";

            string resourceName = "Knives/Resources/DragunPearl";

            GameObject obj = new GameObject(itemName);
            
            var item = obj.AddComponent<Dizzyring>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Charged Rolls";
            string longDesc = "Hold to charge your roll into a more Offensive Stance. \n\n" +
                "" +
                "A pearl given as a death-bed gift by the dragun of the west. This pearl contains strong magic that allow his kind to fly to the heavens, but for a beast of your kind you can only tap some of its power." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            var comp = item.gameObject.AddComponent<ChargeRoll>();


            item.quality = PickupObject.ItemQuality.SPECIAL;
            

        }


        public static bool InChargeState;

        private ChargeRoll dodgeRoller;

        public override void Pickup(PlayerController player)
        {
            IncrementFlag(player, typeof(LiveAmmoItem));
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            player.OnNewFloorLoaded += this.OnNewFloorLoad;
            dodgeRoller = this.gameObject.GetComponent<ChargeRoll>();
            dodgeRoller.owner = player;
            if(CustomDodgeRoll.customDodgeRollHook == null)
            {
                CustomDodgeRoll.InitCustomDodgeRollHooks();
            }

            base.Pickup(player);

        }
        public float ExplosionDamage = 5;
        private void OnNewFloorLoad(PlayerController obj)
        {
            ExplosionDamage = 5 * GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier;
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            if(dodgeRoller.RollActuallyStarted && dodgeRoller.stataugmentActive)
            {
                dodgeRoller.rollDamageModifier.amount *= .5f;
                this.Owner.ownerlessStatModifiers.Remove(dodgeRoller.rollDamageModifier);
                this.Owner.ownerlessStatModifiers.Add(dodgeRoller.rollDamageModifier);
                Owner.stats.RecalculateStats(Owner, true);
            }
        }

        public bool blinkOverride = false;
        public float coyoteTime = .3f;
        
        public override void Update()
        {
            if (this.Owner != null)
            {
                
                if(dodgeRoller.RollActuallyStarted && !Owner.IsDodgeRolling)
                {
                    
                    if (dodgeRoller.stataugmentActive)
                    {
                        this.Owner.ownerlessStatModifiers.Remove(dodgeRoller.rollDamageModifier);
                        this.Owner.ownerlessStatModifiers.Remove(dodgeRoller.speedModifier);
                        this.Owner.ownerlessStatModifiers.Remove(dodgeRoller.rollDistanceModifier);
                        dodgeRoller.stataugmentActive = false;
                        dodgeRoller.zoomy.spawnShadows = false;
                        Owner.stats.RecalculateStats(Owner, true);
                    }
                    Owner.knockbackDoer.ClearContinuousKnockbacks(); // Stop the Stupid fricking table softlock. I am in pain.
                    dodgeRoller.RollActuallyStarted = false;
                }

                

                if (dodgeRoller.RollActuallyStarted && Owner.CurrentRollState == PlayerController.DodgeRollState.OnGround && dodgeRoller.Boom == true && !Owner.IsSlidingOverSurface && Owner.healthHaver.IsAlive)
                {
                    // why...

                    dodgeRoller.Boom = false;
                    GameObject dusty = Owner.PlayEffectOnActor(EasyVFXDatabase.RollBoom, new Vector3(0f, -.4f), false);
                    dusty.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");

                    tk2dBaseSprite DustyDust = dusty.GetComponent<tk2dBaseSprite>();
                    DustyDust.HeightOffGround = -1f;
                    DustyDust.UpdateZDepth();

                    
                    this.DoSafeExplosion(this.Owner.specRigidbody.UnitCenter);


                }

                if (this.Owner.IsOverPitAtAll)
                {
                    if(coyoteTime > 0)
                    {
                        coyoteTime -= Time.deltaTime;
                        this.Owner.FallingProhibited = true;
                    }
                    else
                    {
                        this.Owner.FallingProhibited = false;
                    }
                }
                else
                {
                    coyoteTime = .25f;
                    this.Owner.FallingProhibited = false;
                }

            }
            base.Update();
        }

        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = null;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            this.smallPlayerSafeExplosion.damage = ExplosionDamage;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }

        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 3f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 10f,
            doDestroyProjectiles = false,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = true,
            explosionDelay = 0.0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
            forcePreventSecretWallDamage = true,
            secretWallsRadius = 3,
            force = 20,
            

        };
        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(LiveAmmoItem));
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            player.OnNewFloorLoaded -= this.OnNewFloorLoad;
            return base.Drop(player);
        }

        
    }
   
    public class ChargeRoll : CustomDodgeRoll
    {
        public float MinChargeTime = .9f;
        public float elapsed = 0;
        public bool charged = false;
        
        public StatModifier speedModifier = null;
        public StatModifier rollDamageModifier = null;
        public StatModifier rollDistanceModifier = null;
        
        public float HealthMult = GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier;
        public override void BeginDodgeRoll()
        {
            if(GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
            {
                if (GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier != 0)
                {
                    HealthMult = GameManager.Instance.GetLastLoadedLevelDefinition().enemyHealthMultiplier;
                }
                
            }
            if (owner.Velocity.magnitude > 0)
            {
                StartCoroutine(ShortInvuln());
                SilencerInstance.DestroyBulletsInRange(owner.CenterPosition, 1.1f, true, false);
            }
                
            elapsed = 0;
            charged = false;
            base.BeginDodgeRoll();
            
        }

        private IEnumerator ShortInvuln()
        {
            owner.healthHaver.IsVulnerable = false;
            yield return new WaitForSeconds(.25f);
            yield return new WaitForEndOfFrame();
            owner.healthHaver.IsVulnerable = true;
        }

        public bool stataugmentActive = false;
        public Vector2 RollDirection;
        public bool RollActuallyStarted = false;
        public bool Boom = false;
        public bool fallen = false;
        public override IEnumerator ContinueDodgeRoll()
        {
            if(owner.DodgeRollIsBlink == false && owner.WasPausedThisFrame == false && !owner.IsSlidingOverSurface)
            {
                RollDirection = Vector2.zero;
                AkSoundEngine.PostEvent("Play_Dizzy_RollCharge", base.gameObject);
                while (DodgeButtonHeld)
                {
                    
                    if (elapsed <= MinChargeTime)
                    {
                        elapsed += Time.deltaTime;
                    }
                    if (elapsed > MinChargeTime && charged == false)
                    {
                        charged = true;
                        
                        GameObject dusty = owner.PlayEffectOnActor(EasyVFXDatabase.RollCharge, new Vector3(0f, -.7f), true);
                        dusty.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                        
                        tk2dBaseSprite DustyDust = dusty.GetComponent<tk2dBaseSprite>();
                        DustyDust.HeightOffGround = -1f;
                        DustyDust.UpdateZDepth();
                        
                        AkSoundEngine.PostEvent("Play_CHR_weapon_charged_01", base.gameObject);

                    }
                    if (charged)
                    {
                        if (owner.IsFalling)
                        {
                            fallen = true;
                        }
                    }
                    yield return null;
                }
                
                AkSoundEngine.PostEvent("Stop_Dizzy_RollCharge", base.gameObject);
                if (fallen)
                {
                    charged = false;
                    fallen = false;
                    yield break;
                }
               
                if (!charged) // Standard roll
                {
                    
                    owner.ForceStartDodgeRoll();
                    RollActuallyStarted = true;
                    RollDirection = owner.NonZeroLastCommandedDirection;
                    owner.DidUnstealthyAction();

                }
                else // Augmented Charged roll
                {
                    owner.DidUnstealthyAction();
                    
                    AkSoundEngine.PostEvent("Play_Dizzy_RollFire", base.gameObject);
                    if (stataugmentActive == false)
                    {
                        this.rollDamageModifier = new StatModifier();
                        this.rollDamageModifier.statToBoost = PlayerStats.StatType.DodgeRollDamage;
                        this.rollDamageModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
                        this.rollDamageModifier.amount = (HealthMult) * 4 + 8f;
                        this.owner.ownerlessStatModifiers.Add(rollDamageModifier);

                        this.speedModifier = new StatModifier();
                        this.speedModifier.statToBoost = PlayerStats.StatType.DodgeRollSpeedMultiplier;
                        this.speedModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
                        this.speedModifier.amount = .12f;
                        this.owner.ownerlessStatModifiers.Add(speedModifier);

                        this.rollDistanceModifier = new StatModifier();
                        this.rollDistanceModifier.statToBoost = PlayerStats.StatType.DodgeRollDistanceMultiplier;
                        this.rollDistanceModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
                        this.rollDistanceModifier.amount = .7f;
                        this.owner.ownerlessStatModifiers.Add(rollDistanceModifier);
                        stataugmentActive = true;

                        owner.stats.RecalculateStats(owner, true);
                    }
                    zoomy = owner.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                    zoomy.dashColor = UnityEngine.Color.white;
                    zoomy.spawnShadows = true;
                    zoomy.shadowTimeDelay = .05f;
                    zoomy.shadowLifetime = .3f;
                    zoomy.minTranslation = 0.1f;
                    
                    owner.ForceStartDodgeRoll();
                    RollActuallyStarted = true;
                    Boom = true;
                    RollDirection = owner.NonZeroLastCommandedDirection;

                    GlobalSparksDoer.DoRandomParticleBurst(40, owner.sprite.WorldBottomLeft, owner.sprite.WorldBottomRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                    
                }

                yield return new WaitForSeconds(.1f);
                if (owner.IsSlidingOverSurface && RollDirection != Vector2.zero)
                {
                    owner.knockbackDoer.ApplyContinuousKnockback(AdjustInputVector(RollDirection, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal), 10f);
                }

            }

            yield break;
        }

        public ImprovedAfterImage zoomy; //per tradition


        public override void FinishDodgeRoll()
        {
            // cleanup

            
            base.FinishDodgeRoll();
        }

        protected Vector2 AdjustInputVector(Vector2 rawInput, float cardinalMagnetAngle, float ordinalMagnetAngle)
        {
            float num = BraveMathCollege.ClampAngle360(BraveMathCollege.Atan2Degrees(rawInput));
            float num2 = num % 90f;
            float num3 = (num + 45f) % 90f;
            float num4 = 0f;
            if (cardinalMagnetAngle > 0f)
            {
                if (num2 < cardinalMagnetAngle)
                {
                    num4 = -num2;
                }
                else if (num2 > 90f - cardinalMagnetAngle)
                {
                    num4 = 90f - num2;
                }
            }
            if (ordinalMagnetAngle > 0f)
            {
                if (num3 < ordinalMagnetAngle)
                {
                    num4 = -num3;
                }
                else if (num3 > 90f - ordinalMagnetAngle)
                {
                    num4 = 90f - num3;
                }
            }
            num += num4;
            return (Quaternion.Euler(0f, 0f, num) * Vector3.right).XY() * rawInput.magnitude;
        }
    }

}

        
