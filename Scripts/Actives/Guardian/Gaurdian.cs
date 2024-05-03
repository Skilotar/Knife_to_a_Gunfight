using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;
using System.Reflection;

namespace Knives
{
    class Guardian_Shield : PlayerItem
    {
        
        public static void Register()
        {

            string itemName = "Guardian Shield";

            string resourceName = "Knives/Resources/Guard/Guardian_idle";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Guardian_Shield>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hold -Use- To Shield";
            string longDesc = "A hulking mass of metal and sciencey magic that projects a projectile destroying barrier. \n" +
                "Can be used any time there is fuel in the active meter. Running out of fuel will require a full reset." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            // item.AddToSubShop(ItemBuilder.ShopType.Flynt, .01f);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 20);
            item.UsesNumberOfUsesBeforeCooldown = false;
            item.consumableHandlesOwnDuration = true;
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;
            Guardian_Shield.spriteIDs = new int[Guardian_Shield.spritePaths.Length];
            Guardian_Shield.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(Guardian_Shield.spritePaths[0], item.sprite.Collection);
            Guardian_Shield.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(Guardian_Shield.spritePaths[1], item.sprite.Collection);
            Guardian_Shield.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(Guardian_Shield.spritePaths[2], item.sprite.Collection);
            item.sprite.SetSprite(spriteIDs[0]);

        }
       
        public override void Pickup(PlayerController player)
        {
           
            m_player = player;
            base.Pickup(player);
        }
        
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if (this.IsCurrentlyActive)
                {
                    if (this.LastOwner.CurrentItem == this)
                    {


                        if (Key(GungeonActions.GungeonActionType.UseItem) && !locked)
                        {

                            this.sprite.SetSprite(spriteIDs[1]);
                            UseHeld = true;
                            locked = true;

                        }

                        if (!Key(GungeonActions.GungeonActionType.UseItem) && locked == true)
                        {

                            this.sprite.SetSprite(spriteIDs[0]);
                            UseHeld = false;
                            locked = false;

                        }

                        if (UseHeld == true)
                        {
                            if (GunSwapped == false)
                            {
                                //gun quickswitch to dummy
                                PreviousGun = LastOwner.CurrentGun;
                                DummyShield = LastOwner.inventory.AddGunToInventory(PickupObjectDatabase.GetById(Dummy_Shield.ID) as Gun, true);
                                DummyShield.CanBeDropped = false;
                                DummyShield.CanBeSold = false;
                                LastOwner.inventory.GunLocked.SetOverride("Guardian_Shielding", true, null);
                                this.CanBeDropped = false;
                                GunSwapped = true;
                            }
                            if (m_indicator == null)
                            {
                                //radius
                                HandleRadialIndicator();
                            }



                            if (internalCooldown >= 0)
                            {
                                internalCooldown -= Time.deltaTime;
                            }
                            else
                            {

                                GameObject gameObject = new GameObject("silencer");
                                SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                                SilencerInstance.DestroyBulletsInRange(LastOwner.CenterPosition, 4, true, false, LastOwner, false, 4, true, Docallback);
                                internalCooldown = .1f;
                            }

                            if (beepbeep && beeprunning == false)
                            {
                                StartCoroutine(doWarningBeeps());
                            }

                            
                        }
                        else
                        {
                            if (GunSwapped == true)
                            {
                                this.LastOwner.inventory.GunLocked.RemoveOverride("Guardian_Shielding");
                                this.LastOwner.inventory.DestroyGun(DummyShield);
                                this.LastOwner.ChangeToGunSlot(this.LastOwner.inventory.AllGuns.IndexOf(PreviousGun), true);
                                this.CanBeDropped = true;
                                GunSwapped = false;
                            }
                            if (m_indicator != null)
                            {
                                UnhandleRadialIndicator();
                            }
                            
                        }
                    }
                }
                else
                {
                    if (IsOnCooldown == true)
                    {
                        this.sprite.SetSprite(spriteIDs[2]);
                        if (cooldownlocked == false)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_Chest_Synergy_Lose_01", base.gameObject);
                            cooldownlocked = true;
                        }

                    }
                    else
                    {
                        this.cooldownlocked = false;
                        this.sprite.SetSprite(spriteIDs[0]);
                        
                    }

                    if (GunSwapped == true)
                    {
                        this.LastOwner.inventory.GunLocked.RemoveOverride("Guardian_Shielding");
                        this.LastOwner.inventory.DestroyGun(DummyShield);
                        this.LastOwner.ChangeToGunSlot(this.LastOwner.inventory.AllGuns.IndexOf(PreviousGun), true);
                        this.CanBeDropped = true;
                        GunSwapped = false;
                    }
                    if (m_indicator != null)
                    {
                        UnhandleRadialIndicator();
                    }
                }

            }

            base.Update();
        }

       

        private IEnumerator doWarningBeeps()
        {
            beeprunning = true;
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
            yield return new WaitForSeconds(.1f);
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
            beeprunning = false;
        }

        private void Docallback(Projectile obj)
        {
            AkSoundEngine.PostEvent("Play_WPN_energy_impact_01", base.gameObject);
            float elapsed = GetPrivateType<PlayerItem, float>(this, "m_activeElapsed");
            elapsed += .25f;
            KnownElaps = elapsed;
            SetPrivateType<PlayerItem>(this, "m_activeElapsed", elapsed);
        }
        
        private void HandleRadialIndicator()
        {

            bool flag = !this.m_indicator;
            if (flag)
            {
                Exploder.DoDistortionWave(LastOwner.CenterPosition, .5f, 0.04f, 4, .4f);
                AkSoundEngine.PostEvent("Play_BOSS_energy_shield_01",base.gameObject);
                GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Guard/BubbleShield_001", null);
                gameObject.SetActive(false);
                
                ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                this.m_indicator = UnityEngine.Object.Instantiate(gameObject, LastOwner.CenterPosition - new Vector2(32/16f,32/16f), Quaternion.identity, LastOwner.transform);
                tk2dSprite sprite = m_indicator.GetOrAddComponent<tk2dSprite>();
                sprite.SetColor(opaque_blue);
            }
        }

        private void UnhandleRadialIndicator()
        {
            bool flag = this.m_indicator;
            if (flag)
            {
                
                AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_fade_01", base.gameObject);
                UnityEngine.GameObject.Destroy(m_indicator);
                this.m_indicator = null;
            }
        }
        
        public override void DoEffect(PlayerController user)
        {
            StartCoroutine(HandleMeterUse(this, 7f, user, EndEffect));
            this.sprite.SetSprite(spriteIDs[1]);
        }

        
        public static IEnumerator HandleMeterUse(PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
        {
            if (item.IsCurrentlyActive)
            {
                yield break;
            }
            UseHeld = true;
            beeprunning = false;
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", true);
            SetPrivateType<PlayerItem>(item, "m_activeElapsed", 0f);
            SetPrivateType<PlayerItem>(item, "m_activeDuration", duration);
            item.OnActivationStatusChanged?.Invoke(item);

            float elapsed = GetPrivateType<PlayerItem, float>(item, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(item, "m_activeDuration");
            float scale = GetPrivateType<PlayerItem, float>(item, "m_adjustedTimeScale");
            
            while (GetPrivateType<PlayerItem, float>(item, "m_activeElapsed") < GetPrivateType<PlayerItem, float>(item, "m_activeDuration") && item.IsCurrentlyActive)
            {
                if (UseHeld == true)// decay normaly
                {

                    elapsed += BraveTime.DeltaTime * scale;
                    KnownElaps = elapsed;

                    if ((KnownElaps / dur) >= .85) // 85% used up or above
                    {
                        beepbeep = true;
                    }
                    else
                    {
                        beepbeep = false;
                    }

                    if (user.CurrentStoneGunTimer <= 1)
                    {
                        user.CurrentStoneGunTimer = .5f;
                    }
                    
                }
                if (UseHeld == false) 
                {
                    if(KnownElaps > 0) 
                    {
                        elapsed = KnownElaps;
                        KnownElaps -= BraveTime.DeltaTime / 7 * scale;

                        
                    }
                    else
                    {
                        
                        SetPrivateType<PlayerItem>(item, "m_activeElapsed", dur);
                        SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
                        item.CurrentTimeCooldown = 0;
                        item.DidDamage(item.LastOwner, 1);
                        item.LastOwner.stats.RecalculateStats(item.LastOwner, true);
                        UseHeld = false;
                    }
                   
                }
                SetPrivateType<PlayerItem>(item, "m_activeElapsed", elapsed);

                yield return null;
            }
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
            UseHeld = false;
            
            item.OnActivationStatusChanged?.Invoke(item);
            OnFinish?.Invoke(user);
            
            yield break;
        }

        private void EndEffect(PlayerController obj)
        {

        }
        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
        public PlayerController m_player;

        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }

        public static float KnownElaps = 0;
        public static bool beepbeep = false;
        public static float internalCooldown = .1f;
        private static bool beeprunning;
        public GameObject m_indicator;
        public Gun DummyShield;
        public Gun PreviousGun;
        public static Color opaque_blue = new Color(.36f, .86f, .95f, .3f);
        public bool GunSwapped = false;
        public static bool locked = false;
        public static bool UseHeld = false;
        public bool cooldownlocked = false;
        public Projectile projectile1 = null;
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/Guard/Guardian_idle",
            "Knives/Resources/Guard/Guardian_inuse",
            "Knives/Resources/Guard/Guardian_depleted",
        };
    }




}