using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class NargaBlackFur : PassiveItem
    {
        //add red follow line (narga eyes)
        //change enter/exit stealth sounds
        //add custom icon and animate it for when player can stealth again
        //make it so can-stealth is true on entering combat
        //and proper sound for when player can stealth again
        public static void Register()
        {
            string itemName = "Nargacuga Black Fur";
            string resourceName = "Knives/Resources/bandaids";//placeholder cause i didnt have the original lol   go down to line 300 to see the Overhead setup

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<NargaBlackFur>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Pouncing Shadow";
            string longDesc = "You inherit Nargacuga's ability to stealthily dance around the battlefield." +
                "\nYou gain temporary stealth after a dodge roll during combat. Sneaking provides a 10% damage increase and a 50% movement increase. Stealth has a 15 second cooldown." +
                "\n\n- Monster Hunter Items";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "mhi");

            item.quality = PickupObject.ItemQuality.B;
            SetupCollection();
        }

        public override void Pickup(PlayerController player)
        {
            canStealth = false;
            prevStealth = false;
            stealthMethodLimiter = false;
            stealthCD = 0f;
            isNargaStealthing = false;
            this.nargaAfterImage = player.gameObject.AddComponent<AfterImageTrailController>();
            this.nargaAfterImage.spawnShadows = false;
            this.nargaAfterImage.shadowTimeDelay = 0.05f;
            this.nargaAfterImage.shadowLifetime = 0.2f;
            this.nargaAfterImage.minTranslation = 0.05f;
            this.nargaAfterImage.dashColor = Color.grey;

            //fix animation code
            //nargaStealthIcon.AddAnimation("nargaStealth", "MHItems/Resources/NargaStealthAnimation", 10, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.Single, DirectionalAnimation.FlipType.None).wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            player.OnEnteredCombat += EnterCombatStealth;
            canPlayVFX = false;
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            this.Owner.OnRollStarted -= StealthOnDodgeRoll;
            player.OnEnteredCombat += EnterCombatStealth;
            BreakStealth(this.Owner);
            if (this.nargaAfterImage)
            {
                UnityEngine.Object.Destroy(this.nargaAfterImage);
            }
            this.nargaAfterImage = null;
            
            return base.Drop(player);
        }
        public bool isdoingOffCooldownAnimation = false;
        public override void  Update()
        {
            if (this.Owner)
            {
                HandleNargaCooldown();
                if (canStealth && !stealthMethodLimiter && this.Owner.IsInCombat)
                {
                    stealthMethodLimiter = true;
                    this.Owner.OnRollStarted += StealthOnDodgeRoll;
                    //Show is off cooldown manager

                }
                if (!this.Owner.IsInCombat && isNargaStealthing)
                {
                    BreakStealth(this.Owner);
                    this.Owner.OnRollStarted -= StealthOnDodgeRoll;
                    stealthCD = 0f;
                    stealthMethodLimiter = false;
                }

               
               this.Owner.StartCoroutine(ShowChargeLevel(this.Owner, LastChargeLevel));
                

                if (doingWorldGrey)
                {
                    RenderSettings.ambientLight = new Color(0.14f, 0.19f, 0.19f); // Noir but chan be changed with percentage RGB
                }

            }

        }

        private IEnumerator OffCooldownAnimaion()
        {
            isdoingOffCooldownAnimation = true;

            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 1;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 2;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 3;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 4;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 5;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 6;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 7;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 8;
            yield return new WaitForSeconds(.15f);
            LastChargeLevel = 0;


            isdoingOffCooldownAnimation = false;
        }

        private void EnterCombatStealth()
        {
            stealthCD = 0f;
        }

        private void StealthOnDodgeRoll(PlayerController player, Vector2 v2)
        {
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
            isNargaStealthing = true;
            this.nargaAfterImage.spawnShadows = true;
            player.ChangeSpecialShaderFlag(1, 1f);
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, 0.1f);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, 0.5f);
            player.stats.RecalculateStats(Owner, true);
            player.OnRollStarted -= StealthOnDodgeRoll;
            stealthCD = 15f;
            stealthMethodLimiter = false;
            player.PlayEffectOnActor(this.poofVfx, Vector3.zero, false, true, false);
            player.OnDidUnstealthyAction += this.BreakStealth;
            player.OnItemStolen += this.BreakStealthOnSteal;
            player.healthHaver.OnDamaged += this.OnDamaged;
            player.SetIsStealthed(true, "NargacugaBlackFur");
            player.SetCapableOfStealing(true, "NargaBlackFur", null);
            doingWorldGrey = true;

            StartCoroutine(TempStealthHandler());
        }
        public bool doingWorldGrey = false; 
        private IEnumerator TempStealthHandler()
        {
            
            yield return new WaitForSeconds(5f);
            if (this.Owner.IsStealthed)
            {
                BreakStealth(this.Owner);
                
            }
        }

        private void BreakStealth(PlayerController player)
        {
            if (isNargaStealthing)
            {
                isNargaStealthing = false;
                doingWorldGrey = false;
                StartCoroutine(HandleAfterImageStop(player));
                player.ChangeSpecialShaderFlag(1, 0f);
                RemoveStat(PlayerStats.StatType.Damage);
                AddStat(PlayerStats.StatType.Damage, 0f);
                RemoveStat(PlayerStats.StatType.MovementSpeed);
                AddStat(PlayerStats.StatType.MovementSpeed, 0f);
                player.stats.RecalculateStats(Owner, true);
                player.PlayEffectOnActor(this.poofVfx, Vector3.zero, false, true, false);
                player.OnDidUnstealthyAction -= this.BreakStealth;
                player.OnItemStolen -= this.BreakStealthOnSteal;
                player.healthHaver.OnDamaged -= this.OnDamaged;
                player.SetIsStealthed(false, "NargacugaBlackFur");
                player.SetCapableOfStealing(false, "NargaBlackFur", null);
                AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
                
            }
        }
        private void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
        {
            this.BreakStealth(arg1);
        }
        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            PlayerController player = this.Owner;
            this.BreakStealth(player);
        }

        private IEnumerator HandleAfterImageStop(PlayerController player)
        {
            this.nargaAfterImage.spawnShadows = true;
            while (isNargaStealthing)
            {
                yield return null;
            }
            if (this.nargaAfterImage)
            {
                this.nargaAfterImage.spawnShadows = false;
            }
            yield break;
        }
        public bool canPlayVFX = false;
        private void HandleNargaCooldown()
        {
            if (stealthCD > 0)
            {
                canStealth = false;
                stealthCD -= Time.deltaTime;
                prevStealth = false;
                canPlayVFX = true;
            }
            else
            {
                canStealth = true;
                
                if (prevStealth != canStealth)
                {
                    if (canPlayVFX)
                    {
                        canPlayVFX = false;
                        this.Owner.StartCoroutine(OffCooldownAnimaion());
                        
                    }
                    
                }
                prevStealth = true;
            }
        }

        //adds a stat
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

        private bool canStealth = false;
        private bool prevStealth = false;
        private bool stealthMethodLimiter = false;
        private float stealthCD = 0f;
        private bool isNargaStealthing = false;
        

        private AfterImageTrailController nargaAfterImage;
        public GameObject poofVfx = (PickupObjectDatabase.GetById(462) as ConsumableStealthItem).poofVfx;


        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData GunVFXCollection;
        private static GameObject VFXScapegoat;

        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;
        private static int Meter5ID;
        private static int Meter6ID;
        private static int Meter7ID;
        private static int Meter8ID;
        private static int Meter9ID;


        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            NargaBlackFur.GunVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "NargaVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(NargaBlackFur.GunVFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_001", NargaBlackFur.GunVFXCollection);
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_002", NargaBlackFur.GunVFXCollection);
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_003", NargaBlackFur.GunVFXCollection);
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_004", NargaBlackFur.GunVFXCollection);
            Meter5ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_005", NargaBlackFur.GunVFXCollection);
            Meter6ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_006", NargaBlackFur.GunVFXCollection);
            Meter7ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_007", NargaBlackFur.GunVFXCollection);
            Meter8ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_008", NargaBlackFur.GunVFXCollection);
            Meter9ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Animations/narga_stealth_icon_empty", NargaBlackFur.GunVFXCollection);

        }

        private IEnumerator ShowChargeLevel(GameActor gunOwner, int chargeLevel)
        {
            if (extantSprites.Count > 0)
            {
                for (int i = extantSprites.Count - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(extantSprites[i].gameObject);
                }
                extantSprites.Clear();
            }
            GameObject newSprite = new GameObject("Level Popup", new Type[] { typeof(tk2dSprite) }) { layer = 0 };
            newSprite.transform.localScale *= 2f;

            newSprite.transform.position = (gunOwner.sprite.WorldCenter + new Vector2(-0.25f, 1));//center and offset
            
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {

                case 0:
                    spriteID = Meter9ID;
                    break;
                case 1:
                    spriteID = Meter1ID;
                    break;
                case 2:
                    spriteID = Meter2ID;
                    break;
                case 3:
                    spriteID = Meter3ID;
                    break;
                case 4:
                    spriteID = Meter4ID;
                    break;
                case 5:
                    spriteID = Meter5ID;
                    break;
                case 6:
                    spriteID = Meter6ID;
                    break;
                case 7:
                    spriteID = Meter7ID;
                    break;
                case 8:
                    spriteID = Meter8ID;
                    break;


            }
            m_ItemSprite.SetSprite(NargaBlackFur.GunVFXCollection, spriteID);
            m_ItemSprite.PlaceAtPositionByAnchor(newSprite.transform.position, tk2dBaseSprite.Anchor.LowerCenter);
            m_ItemSprite.transform.localPosition = m_ItemSprite.transform.localPosition.Quantize(0.0625f);
            newSprite.transform.parent = gunOwner.transform;
            if (m_ItemSprite)
            {
                //sprite.AttachRenderer(m_ItemSprite);
                m_ItemSprite.depthUsesTrimmedBounds = true;
                m_ItemSprite.UpdateZDepth();
            }
            //sprite.UpdateZDepth();
            yield return new WaitForSeconds(.12f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }



        public int LastChargeLevel = 0;
    }
}

