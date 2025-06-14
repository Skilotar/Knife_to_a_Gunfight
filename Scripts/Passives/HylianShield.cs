using ItemAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knives
{
    class HylianShield : BannerPassive
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Hylian Shield";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/HylianShield/HylianShieldItem";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HylianShield>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Has Your Back";
            string longDesc = "Shields attacks from behind.\n" +
                "A shield passed down through the Hyrulean royal family, along with the legend of the hero who wielded it. \nIts defensive capabilities and durability outshine all others." +
                "\n\n\n - Knife_to_a_Gunfight";

           
            item.quality = PickupObject.ItemQuality.S;

            // Call setup sprites first then setup banner sprites.
            item.SetSpriteNames("HylianShield_up", "HylianShield_down", "HylianShield_left", "HylianShield_right", "HylianShield_up_left", "HylianShield_up_right");
            
            item.SetupBannerSprites("HylianShield",
                "Knives/Resources/HylianShield/HylianShield_up",
                "Knives/Resources/HylianShield/HylianShield_down",
                "Knives/Resources/HylianShield/HylianShield_left",
                "Knives/Resources/HylianShield/HylianShield_right",
                "Knives/Resources/HylianShield/HylianShield_up_left",
                "Knives/Resources/HylianShield/HylianShield_up_right"
                );

            item.downOffset = new Vector2(4 / 16f, 6 / 16f);
            
            item.leftOffset = new Vector2(10 / 16f, 2 / 16f);
            item.rightOffset = new Vector2(-2 / 16f, 2 / 16f);

            item.uprightOffset = new Vector2(1 / 16f, 1 / 16f);
            item.upLeftOffset = new Vector2(8 / 16f, 1 / 16f);

            item.upOffset = new Vector2(4 / 16f, 0 / 16f);
            item.visibleDuringRoll = false;

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.specRigidbody.OnPreRigidbodyCollision += this.OnPreHit;
            player.healthHaver.ModifyDamage += this.ModDamage;

            PassiveItem.IncrementFlag((PlayerController)player, typeof(HelmetItem));
            base.Pickup(player);
        }

        private void ModDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if(BlockThisOne == true)
            {
                arg2.ModifiedDamage = 0;
                BlockThisOne = false;

                if(this.Owner.PlayerHasActiveSynergy("Counter Flurry"))
                {
                    FlurryTimer = 1.25f;
                }

                if(this.Owner.PlayerHasActiveSynergy("Resonance Alloy"))
                {
                    this.Owner.ForceBlank(3.5f, 0.5f, false, true, null, true, 0f);
                }
            }
        }

        public bool BlockThisOne = false;
        private void OnPreHit(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.GetComponent<Projectile>() != null)
            {
                
                Projectile proj = otherRigidbody.GetComponent<Projectile>();
                if (proj.collidesWithPlayer)
                {
                    PlayerController player = myRigidbody.gameObject.GetComponent<PlayerController>();
                    proj.shouldRotate = true;
                    proj.SendInDirection((Vector2)player.CenterPosition - (Vector2)proj.LastPosition, true, true); // This has to be done because SOMEONE at dodgeroll made bulletscripts not update rotation.

                    float DotProd = Vector2.Dot(proj.Direction, player.CurrentGun.CurrentAngle.DegreeToVector2());
                    //ETGModConsole.Log(DotProd);
                    if (DotProd >= .25)
                    {
                        BlockThisOne = true;
                        AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", base.gameObject);
                        player.knockbackDoer.ApplyKnockback(proj.Direction, 5);

                    }
                }
                
            }
            
            if (otherRigidbody.aiActor != null)
            {
                AIActor actor = otherRigidbody.GetComponent<AIActor>();
                PlayerController player = myRigidbody.gameObject.GetComponent<PlayerController>();
                float DotProd = Vector2.Dot(actor.Velocity, player.CurrentGun.CurrentAngle.DegreeToVector2());
                if (DotProd >= .35f) 
                {
                    BlockThisOne = true;
                    AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", base.gameObject);
                    actor.knockbackDoer.ApplyKnockback(-actor.Velocity, 10);
                }
            }
        }

        public override void Update()
        {
            if(this.Owner != null)
            {
                if(backpackInstance != null)
                {
                    if(backpackInstance.gameObject.GetComponent<SpeculativeRigidbody>() == null)
                    {
                        SpeculativeRigidbody body = backpackInstance.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
                        PixelCollider pixelCollider = new PixelCollider();
                        pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
                        pixelCollider.CollisionLayer = CollisionLayer.BeamBlocker;
                        pixelCollider.ManualWidth = 14;
                        pixelCollider.ManualHeight = 14;
                        pixelCollider.ManualOffsetX = 0;
                        pixelCollider.ManualOffsetY = 0;
                        body.PixelColliders = new List<PixelCollider>()
                        {
                            pixelCollider
                        };
                    }
                    else
                    {
                        SpeculativeRigidbody body = backpackInstance.gameObject.GetComponent<SpeculativeRigidbody>();
                        body.UpdateColliderPositions();
                        body.Reinitialize();
                    }

                    if(FlurryTimer > 0)
                    {
                        FlurryTimer -= BraveTime.DeltaTime;
                        if(Upgrade == false)
                        {
                            RemoveStat(PlayerStats.StatType.MovementSpeed);
                            AddStat(PlayerStats.StatType.MovementSpeed, 2);
                            RemoveStat(PlayerStats.StatType.RateOfFire);
                            AddStat(PlayerStats.StatType.RateOfFire, .5f);
                            this.Owner.stats.RecalculateStats(this.Owner, true);
                            Upgrade = true;
                        }
                    }
                    else
                    {
                        if (Upgrade == true)
                        {
                            RemoveStat(PlayerStats.StatType.MovementSpeed);
                            AddStat(PlayerStats.StatType.MovementSpeed, 0);
                            RemoveStat(PlayerStats.StatType.RateOfFire);
                            AddStat(PlayerStats.StatType.RateOfFire, 0);
                            this.Owner.stats.RecalculateStats(this.Owner, true);
                            Upgrade = false;
                        }
                    }
                }

            }
            base.Update();
        }

        public bool Upgrade = false;
        public float FlurryTimer = 0;
        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }


        public override DebrisObject Drop(PlayerController player)
        {
            player.specRigidbody.OnPreRigidbodyCollision -= this.OnPreHit;
            PassiveItem.DecrementFlag((PlayerController)player, typeof(HelmetItem));
            return base.Drop(player);

        }
    }
}
