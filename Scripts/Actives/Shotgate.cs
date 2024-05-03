using ItemAPI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using GungeonAPI;
using Dungeonator;
using Gungeon;


namespace Knives
{
    class Shotgate : PlayerItem
    {
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/ShotGate/Eyecon/Eye_weight",
            "Knives/Resources/ShotGate/Eyecon/Eye_Dura",
            "Knives/Resources/ShotGate/Eyecon/Eye_Shatter",
            "Knives/Resources/ShotGate/Eyecon/Eye_loop"

        };
        public static void Init()
        {
            string itemName = "ShotCaster's Eye";

            string resourceName = "Knives/Resources/ShotGate/Eyecon/Eye_con_main";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Shotgate>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Bulk Enchantment";
            string longDesc = "Hold -USE- to select.\n\n This small glass eye was once a prop for a science fantasy rpg boardgame, Variance. The eye has since been upgraded to fit the originally imagined goal. \n\n" +
                "At the time enchanting gun muzzles had not been perfected as the firing would wear down the enchantment and enchanting each individual bullet was time consuming. This eye can project an enchantment ring to upgrade bullets on the fly.. Literally. \n\n" +
                "Red flower increases damage and weight.\n" +
                "Purple angel improves durability.\n" +
                "Yellow driver shatters bullets.\n" +
                "Green night Loops bullets on a successful hit." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 200);
           
            Shotgate.spriteIDs = new int[Shotgate.spritePaths.Length];
           
            SetupCollection();
            
            item.quality = PickupObject.ItemQuality.B;
            Shotgate.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(Shotgate.spritePaths[0], item.sprite.Collection);
            Shotgate.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(Shotgate.spritePaths[1], item.sprite.Collection);
            Shotgate.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(Shotgate.spritePaths[2], item.sprite.Collection);
            Shotgate.spriteIDs[3] = SpriteBuilder.AddSpriteToCollection(Shotgate.spritePaths[3], item.sprite.Collection);

            BuildPrefabWeight();
            BuildPrefabDura();
            BuildPrefabSplit();
            BuildPrefabLoop();
            
        }
        
        public static void BuildPrefabWeight()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/ShotGate/Weight/summon_ring_weight_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/ShotGate/Weight/summon_ring_weight_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/ShotGate/Weight/summon_ring_weight_00{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            Shotgate.Weight = gameObject;
        }

        public static void BuildPrefabDura()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/ShotGate/Durability/summon_ring_durability_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/ShotGate/Durability/summon_ring_durability_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/ShotGate/Durability/summon_ring_durability_00{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            Shotgate.Dura = gameObject;
        }

        public static void BuildPrefabSplit()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/ShotGate/Split/summon_ring_split_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/ShotGate/Split/summon_ring_split_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/ShotGate/Split/summon_ring_split_00{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            Shotgate.Split = gameObject;
        }

        public static void BuildPrefabLoop()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/ShotGate/Loop/summon_ring_loop_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 4,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/ShotGate/Loop/summon_ring_loop_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 4; i++) // add 2 and on
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/ShotGate/Loop/summon_ring_loop_00{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            Shotgate.Loop = gameObject;
        }

        public PlayerController m_player;
        public override void Pickup(PlayerController player)
        {
            m_player = player;
            base.Pickup(player);
        }
        public int usedSelect = 0;
        public override void  DoEffect(PlayerController player)
        {
            UnityEngine.GameObject.DestroyObject(m_extant);
            
            usedSelect = -10;
            switch (Current_Select)
            {

                case 0:
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Shotgate.Weight, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                    gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                    usedSelect = 0;
                    m_extant = gameObject;
                    break;
                case 1:

                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Shotgate.Dura, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                    gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                    usedSelect = 1;
                    m_extant = gameObject2;
                    break;
                case 2:

                    GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Shotgate.Split, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                    gameObject3.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                    usedSelect = 2;
                    m_extant = gameObject3;
                    break;
                case 3:

                    GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(Shotgate.Loop, player.transform.position + new Vector3(0.0f, 0f, 0f), Quaternion.identity);
                    gameObject4.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                    usedSelect = 3;
                    m_extant = gameObject4;
                    break;
            }
            float dura = 10f;
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, this.LastOwner, EndEffect));

            base.DoEffect(player);
        }

        private void EndEffect(PlayerController obj)
        {
            UnityEngine.GameObject.DestroyObject(m_extant);
            usedSelect = -10;
        }

        public bool locked = false;
        public bool shown = false;

        public override void Update()
        {
            if (this.LastOwner != null)
            {
                PlayerController player = this.LastOwner as PlayerController;
                //selector
                if (this.IsOnCooldown && player.CurrentItem == this)
                {




                    if (Key(GungeonActions.GungeonActionType.UseItem) && KeyTime(GungeonActions.GungeonActionType.UseItem) > .33f && !locked)
                    {
                        shown = true;
                        locked = true;
                    }

                    if (!Key(GungeonActions.GungeonActionType.UseItem) && locked == true)
                    {
                        locked = false;
                        shown = false;
                    }


                    if (shown)
                    {

                        if (player.CurrentGun.CurrentAngle <= 45 && player.CurrentGun.CurrentAngle >= -45)// right
                        {
                            Current_Select = 0;
                           
                            this.sprite.SetSprite(spriteIDs[0]);
                        }
                        if (player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
                        {
                            Current_Select = 1;
                            
                            this.sprite.SetSprite(spriteIDs[1]);
                        }
                        if (player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
                        {
                            Current_Select = 2;
                           
                            this.sprite.SetSprite(spriteIDs[2]);
                        }
                        if (player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
                        {
                            Current_Select = 3;
                           
                            this.sprite.SetSprite(spriteIDs[3]);
                        }


                        player.StartCoroutine(ShowChargeLevel(player, Current_Select));
                    }
                }

               
                //effects
                if (m_extant != null && LastOwner != null)
                {
                    if (usedSelect == 0) // weight
                    {

                        proximityWeight(this.LastOwner);
                    }
                    if (usedSelect == 1) // dura
                    {

                        proximityDura(this.LastOwner);
                    }
                    if (usedSelect == 2) // split
                    {
                        proximitySplit(this.LastOwner);
                    }
                    if (usedSelect == 3) //loop
                    {
                        proximityLoop(this.LastOwner);
                    }

                }
                else
                {
                    usedSelect = 10;

                    
                }

                

            }


            base.Update();
        }

        public void proximityWeight(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            if (m_extant != null)
            {
                center = m_extant.transform.position + new Vector3(1.2F, 1.2F, 0);
                foreach (var projectile in GetBullets())
                {

                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1.55f;
                    if (Vector2.Distance(bullet, center) < radius)
                    {
                        projectileStates buffed = projectile.gameObject.GetOrAddComponent<projectileStates>();
                        if (buffed.boostedbyshotgate == false)
                        {
                            projectile.baseData.force *= 10f;
                            projectile.baseData.damage *= 1.5f;  
                            AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                            projectile.AdjustPlayerProjectileTint(UnityEngine.Color.red, 1, 0);
                            buffed.boostedbyshotgate = true;
                        }

                    }

                }
            }

        }

        public void proximityDura(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            if (m_extant != null)
            {
                center = m_extant.transform.position + new Vector3(1.2F, 1.2F, 0);
                foreach (var projectile in GetBullets())
                {

                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1.55f;
                    if (Vector2.Distance(bullet, center) < radius)
                    {
                        projectileStates buffed = projectile.gameObject.GetOrAddComponent<projectileStates>();
                        if (buffed.boostedbyshotgate == false)
                        {
                            PierceProjModifier pierce = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                            pierce.penetration = 5;
                            BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                            bnc.numberOfBounces = 3;
                            projectile.baseData.range *= 4f;
                            projectile.ResetDistance();
                            AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                            projectile.AdjustPlayerProjectileTint(UnityEngine.Color.magenta, 1, 0);
                            buffed.boostedbyshotgate = true;
                        }

                    }

                }
            }

        }

        public void proximitySplit(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            if (m_extant != null)
            {
                center = m_extant.transform.position + new Vector3(1.2F, 1.2F, 0);
                foreach (var projectile in GetBullets())
                {

                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1.55f;
                    if (Vector2.Distance(bullet, center) < radius)
                    {
                        projectileStates buffed = projectile.gameObject.GetOrAddComponent<projectileStates>();
                        if (buffed.boostedbyshotgate == false)
                        {
                            ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
                            split.distanceBasedSplit = true;
                            split.distanceTillSplit = 4f;
                            split.dmgMultAfterSplit *= .7f;
                            split.splitAngles = 20;
                            split.amtToSplitTo = 4;
                            
                            AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                            projectile.AdjustPlayerProjectileTint(UnityEngine.Color.yellow, 1, 0);
                            buffed.boostedbyshotgate = true;
                        }

                    }

                }
            }

        }

        public void proximityLoop(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            if (m_extant != null)
            {
                center = m_extant.transform.position + new Vector3(1.2F, 1.2F, 0);
                foreach (var projectile in GetBullets())
                {

                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1.55f;
                    if (Vector2.Distance(bullet, center) < radius)
                    {
                        projectileStates buffed = projectile.gameObject.GetOrAddComponent<projectileStates>();
                        if (buffed.boostedbyshotgate == false)
                        {
                            ProjectileLoopModifier loopy = projectile.gameObject.GetOrAddComponent<ProjectileLoopModifier>();
                            loopy.islooper = true;
                            projectile.baseData.range *= 2f;
                            projectile.baseData.damage *= 1.5f;
                            projectile.ResetDistance();
                            AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                            projectile.AdjustPlayerProjectileTint(UnityEngine.Color.cyan, 1, 0);
                            buffed.boostedbyshotgate = true;
                        }

                    }

                }
            }

        }


        private List<Projectile> GetBullets()
        {
            PlayerController player = this.LastOwner;
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null && projectile.Owner != aiActor && projectile.Owner == this.LastOwner)
                    {
                        if (player != null)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }




        public int Current_Select = 0;
        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData BagVFXCollection;
        private static GameObject VFXScapegoat;

        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;

        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            Shotgate.BagVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "GateVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(Shotgate.BagVFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/ShotGate/Radials/Shotgate_radial_weight", Shotgate.BagVFXCollection); //right
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/ShotGate/Radials/Shotgate_radial_dura", Shotgate.BagVFXCollection); // top
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/ShotGate/Radials/Shotgate_radial_Split", Shotgate.BagVFXCollection); // left
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/ShotGate/Radials/Shotgate_radial_loop", Shotgate.BagVFXCollection); // botom

        }

        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
            }
            else
            {
                return 0;
            }

        }

        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }

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
            m_ItemSprite.SetSprite(Shotgate.BagVFXCollection, spriteID);
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
            yield return new WaitForSeconds(.5f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }



        public int LastChargeLevel = 0;
        public static GameObject Weight;
        public static GameObject Dura;
        public static GameObject Split;
        public static GameObject Loop;
        public static GameObject m_extant;

        public Vector3 center { get; private set; }
        public Vector2 bullet { get; private set; }
    }
}

