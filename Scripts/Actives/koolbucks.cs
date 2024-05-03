using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System.Collections;
using System.Reflection;
using SGUI;
using SaveAPI;

namespace Knives
{ 
    class koolbucks :PlayerItem
    { 
        public static void Register()
        {
            string itemName = "Kool Kat Bucks";

            string resourceName = "Knives/Resources/koolkatbucks";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<koolbucks>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Way Past Koool!!";
            string longDesc = "Spend coolness to gain prizes." +
                "\n\n" +
                "Despite their name these 'dollars' are the least cool form of currency in the known universe. They were originally tokens for a pizza arcade. Just having to touch them makes you feel less cool.\n\n" +
                "The tokens still hold some of their orignal power. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .5f);
            SetupCollection();
            item.quality = PickupObject.ItemQuality.SPECIAL;

            
            ID = item.PickupObjectId;
        }

        public static int ID;

        public PlayerController KnownLastOwner;
        public bool shown = false;
        public int Current_Select = 3;
        public bool HasUIInitialized = false;
        public override void Pickup(PlayerController player)
        {
            KnownLastOwner = player;
            if (HasUIInitialized)
            {
                SGUIRoot.Main.Children.Add(KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>().container);
            }
            
            
            base.Pickup(player);
        }

       

        public override void OnPreDrop(PlayerController user)
        {
            Current_Select = 3;
            shown = false;

            if (KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>() != null)
            {
                if (KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>().container != null)
                {
                    SGUIRoot.Main.Children.Remove(KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>().container);
                }

            }
            base.OnPreDrop(user);
        }

        public override void OnDestroy()
        {
            if (KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>() != null)
            {
                if(KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>().container != null)
                {
                    SGUIRoot.Main.Children.Remove(KnownLastOwner.gameObject.GetOrAddComponent<KoolnessUIComponent>().container);
                }

            }

            base.OnDestroy();
        }

        public override void OnItemSwitched(PlayerController user)
        {
            if(shown == true)
            {

                Current_Select = 3;
                shown = false;
                
            }
            
           

            base.OnItemSwitched(user);
        }
        public override void  DoEffect(PlayerController user)// startup
        {
            if(user.HasPassiveItem(433))
            
            Current_Select = 3;
            shown = true;
            StartCoroutine(HandleUse(this, .5f, user, EndEffect));
            float coolness = PlayerStats.GetTotalCoolness();
           
        }

        public List<int> jackpotItemRepo = new List<int>()
        {
            433, //stuffed_star B
            493, //briefcase_of_cash A
            451, //pig A
            431, //liquid_valkyrie A
            495, //unity B
            572, //chicken_flute
            110, //magic_sweet A

            134, //ammo_belt
            290, //sunglasses
            118, //eyepatch
            435, //mustache
            426, //shotga_cola
            645, //turtle_problem
            321, //gold_ammolet
            425, //heart_purse

            CharmBauble.ID,
            CharmBauble.ID,

            566, // RAD GUN!!!
            514, // D-pad
            149, // FaceMelter

            HeavyImpact.ID,
            HoverBoard.ID,
            JetSetterRadio.ID,

            TaggAr.ID,
        };

        private int SelectItem()
        {
            int ID = 425; // heart purse in a real run this failsafe should not be possible to trigger since it takes over 150 coolness to buy out the shop, But Like Modded gungeon is wacky so I've gotta prep
            if (jackpotItemRepo.Count != 0)
            {
                
                int rng = UnityEngine.Random.Range(0, jackpotItemRepo.Count - 1);
                ID = jackpotItemRepo[rng];
                CheckUnlockID(ID);
                jackpotItemRepo.RemoveAt(rng);
            }


            return ID;
        }

        private void CheckUnlockID(int ID)
        {
            if(ID == HeavyImpact.ID)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SKATER_SLAP, true);

            }
            if (ID == JetSetterRadio.ID)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SKATER_JETSET, true);

            }
            if (ID == HoverBoard.ID)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SKATER_HOVER, true);

            }
            if (ID == TaggAr.ID)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SKATER_TAGGAR, true);

            }
        }

        public override void DoActiveEffect(PlayerController user) // Option Select
        {
            shown = false;
            
            switch (Current_Select)
            {

                case 0: // ammo
                    if(user.stats.GetStatValue(PlayerStats.StatType.Coolness) >= 1)
                    {
                        user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Coolness, StatModifier.ModifyMethod.ADDITIVE, -1));
                        user.stats.RecalculateStats(user, true);
                        int rng = UnityEngine.Random.Range(1, 10);
                        if(rng <= 5)// tiny
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(tinyAmmo.ID).gameObject, user.sprite.WorldCenter, new Vector3(0,0,1), 7f, false, false, false);
                        }
                        if (rng > 5 && rng < 8)// spread
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(600).gameObject, user.sprite.WorldCenter, new Vector3(0, 0, 1), 7f, false, false, false);
                        }
                        if(rng >= 8) // big
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, user.sprite.WorldCenter, new Vector3(0, 0, 1), 7f, false, false, false);
                        }
                        StartCoroutine(DoHappy(user));
                    }
                    else
                    {
                        AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", base.gameObject);
                    }


                    break;
                case 1: // Jackpot
                    if (user.stats.GetStatValue(PlayerStats.StatType.Coolness) >= (2 + JackPotPriceRamp))
                    {
                        user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Coolness, StatModifier.ModifyMethod.ADDITIVE, -2 - JackPotPriceRamp));
                        user.stats.RecalculateStats(user, true);
                        int itemID = SelectItem();
                       
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(itemID).gameObject, user.sprite.WorldCenter, new Vector3(0, 0, 1), 7f, false, false, false);
                        StartCoroutine(DoHappy(user));

                        JackPotPriceRamp += .25f;
                    }
                    else
                    {
                        AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", base.gameObject);
                    }


                    break;
                case 2: // consume
                    if (user.stats.GetStatValue(PlayerStats.StatType.Coolness) >= 1f)
                    {
                        user.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.Coolness, StatModifier.ModifyMethod.ADDITIVE, -1));
                        user.stats.RecalculateStats(user, true);
                        bool FullHealth = false;
                        if (user.healthHaver.GetMaxHealth() == user.healthHaver.GetCurrentHealth()) FullHealth = true;
                        int rng = UnityEngine.Random.Range(1, 4);
                        if (rng == 1)// .5heart
                        {
                            if (FullHealth)
                            {
                                user.healthHaver.Armor++;
                                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);

                            }
                            else 
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);

                                user.healthHaver.ApplyHealing(.5f);
                            }
                           
                        }
                        if (rng == 2) // key 
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(67).gameObject, user.specRigidbody.UnitCenter, Vector2.zero, 0f, false, false, false);
                        }
                        if (rng >= 3) // blank
                        {
                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(224).gameObject, user.specRigidbody.UnitCenter, Vector2.zero, 0f, false, false, false);
                        }
                        StartCoroutine(DoHappy(user));
                    }
                    else
                    {
                        AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", base.gameObject);
                    }

                    break;
                case 3: // exit
                    // :)
                    break;


            }

            
            base.DoActiveEffect(user);
        }

        private IEnumerator DoHappy(PlayerController player)
        {
            FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(player, true);

            player.CurrentInputState = PlayerInputState.NoInput;
            player.specRigidbody.Velocity = Vector2.zero;
            player.ToggleGunRenderers(false, "itemGet");
            player.ToggleHandRenderers(false, "itemGet");
            player.GetComponent<HealthHaver>().IsVulnerable = false;
            player.spriteAnimator.Play((!player.UseArmorlessAnim) ? "item_get" : "item_get_armorless");

            yield return new WaitForSeconds(2f);

            player.CurrentInputState = PlayerInputState.AllInput;
            player.GetComponent<HealthHaver>().IsVulnerable = true;
            player.ToggleGunRenderers(true, "itemGet");
            player.ToggleHandRenderers(true, "itemGet");

        }

       
        protected void EndEffect(PlayerController user) // cleanup
        {
            

        }
        public float showncost = -1;
        public Vector3 readoutOffset;
        public float JackPotPriceRamp = 0;
        public override void Update()
        {

            if(this.LastOwner != null)
            {
                PlayerController player = this.LastOwner as PlayerController;
                if (shown)
                {
                    RelativeLabelAttacher label = player.gameObject.GetOrAddComponent<RelativeLabelAttacher>();

                    if (player.CurrentGun.CurrentAngle <= 45 && player.CurrentGun.CurrentAngle >= -45)// right
                    {
                        Current_Select = 0;
                        showncost = 1;
                        readoutOffset = new Vector3(2.1f, .65f,0);
                    }
                    if (player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
                    {
                        Current_Select = 1;
                        showncost = 2 + JackPotPriceRamp;
                        readoutOffset = new Vector3(.45f, 2.2f,0);
                    }
                    if (player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
                    {
                        Current_Select = 2;
                        showncost = 1f;
                        readoutOffset = new Vector3(-1, .65f,0);
                    }
                    if (player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
                    {
                        Current_Select = 3;
                        showncost = 0;
                        readoutOffset = new Vector3(0, -100,0);
                    }


                    player.StartCoroutine(ShowChargeLevel(player, Current_Select));
                    if(doingCo == false) player.StartCoroutine(HandleLabel(label));

                }

                if(GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
                {
                    if (player.gameObject.GetComponent<KoolnessUIComponent>() == null)
                    {
                        player.gameObject.GetOrAddComponent<KoolnessUIComponent>();
                        HasUIInitialized = true;
                    }
                }
                /*
                if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SKATER_TAGGAR) == false)
                {

                    if (this.LastOwner.name == "Playerskater(clone)" || this.LastOwner.name == "PlayerSkater(clone)")
                    {
                        if (LastOwner.stats.GetStatValue(PlayerStats.StatType.Coolness) >= 10)
                        {
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SKATER_TAGGAR, true);
                        }
                    }
                }
                */

            }
            base.Update();
        }
        public bool doingCo = false;
        private IEnumerator HandleLabel(RelativeLabelAttacher label)
        {
            doingCo = true;
            label.labelValue = "-" + showncost.ToString();
            label.colour = new Color(1, .39f, 0);
            label.offset = readoutOffset;
            
            
            yield return new WaitForSeconds(.15f);
            UnityEngine.Object.Destroy(label);


            doingCo = false;
        }

        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData VFXCollection;
        private static GameObject VFXScapegoat;

        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;

        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            koolbucks.VFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "BucksVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(koolbucks.VFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/KoolRad/KoolRad_ammo", koolbucks.VFXCollection); //right
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/KoolRad/KoolRad_Jackpot", koolbucks.VFXCollection); // top
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/KoolRad/KoolRad_consumeables", koolbucks.VFXCollection); // left
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/KoolRad/KoolRad_exit", koolbucks.VFXCollection); // bottom

        }
        public PlayerController m_player;
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
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, -1.5f));
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {

                case 0:
                    spriteID = Meter1ID;
                    break;
                case 1:
                    spriteID = Meter2ID;
                    break;
                case 2:
                    spriteID = Meter3ID;
                    break;
                case 3:
                    spriteID = Meter4ID;
                    break;


            }
            m_ItemSprite.SetSprite(koolbucks.VFXCollection, spriteID);
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
            yield return new WaitForSeconds(.25f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }


        public IEnumerator HandleUse(PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
        {
            if (item.IsCurrentlyActive)
            {
                yield break;
            }

            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", true);
            SetPrivateType<PlayerItem>(item, "m_activeElapsed", 0f);
            SetPrivateType<PlayerItem>(item, "m_activeDuration", duration);
            item.OnActivationStatusChanged?.Invoke(item);

            float elapsed = GetPrivateType<PlayerItem, float>(item, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(item, "m_activeDuration");
            float scale = GetPrivateType<PlayerItem, float>(item, "m_adjustedTimeScale");

            while (GetPrivateType<PlayerItem, float>(item, "m_activeElapsed") < GetPrivateType<PlayerItem, float>(item, "m_activeDuration") && item.IsCurrentlyActive)
            {
                if (shown)// decay normaly
                {
                    elapsed = 0;


                }
                else
                {
                    elapsed = 1;

                }
                SetPrivateType<PlayerItem>(item, "m_activeElapsed", elapsed);

                yield return null;
            }
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
            item.OnActivationStatusChanged?.Invoke(item);
            OnFinish?.Invoke(user);
            
            yield break;
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
    }
}
