using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Dungeonator;
using System.Collections;

namespace Knives
{

    class MiscToolMethods
    {

        public static void AddItemToBasegameSynergy(int synergyId, int itemId)
        {
            var synergies = GameManager.Instance.SynergyManager.synergies;

            if (synergies == null || synergies.Length <= synergyId)
                return;

            var synergy = synergies[synergyId];
            var obj = PickupObjectDatabase.GetById(itemId);

            if (obj is Gun)
                synergy.OptionalGunIDs.Add(itemId);
            else if (obj is PassiveItem || obj is PlayerItem)
                synergy.OptionalItemIDs.Add(itemId);
        }
        public static RoomHandler GetAbsoluteRoomFromProjectile(Projectile bullet)
        {
            Vector2 worldCenter = bullet.sprite.WorldCenter;
            IntVector2 pos = worldCenter.ToIntVector2(VectorConversions.Round);
            return GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(pos);
        }

        public static bool CheckForWalls(PlayerController player)
        {
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(player.transform.position, VectorConversions.Round));
            CellData cellaim = room.GetNearestCellToPosition(player.transform.position);


            if (cellaim != null && room != null)
            {
                for (int i = 0; i <= offsets.Count - 1; i++)
                {
                    CellData targetcell = room.GetNearestCellToPosition((Vector2)(cellaim.position + offsets[i]));
                    if (targetcell != null)
                    {
                        if (targetcell.type == CellType.WALL || targetcell.isOccupied)
                        {
                            return true;
                        }
                    }

                }

            }

            return false;
        }

        public static Dictionary<int, IntVector2> offsets = new Dictionary<int, IntVector2>
        {
            { 0,new IntVector2(0,0)},
            { 1,new IntVector2(0,1)},
            { 2,new IntVector2(1,0)},
            { 3,new IntVector2(0,-1)},
            { 4,new IntVector2(-1,0)},
            { 5,new IntVector2(1,1)},
            { 6,new IntVector2(-1,1)},
            { 7,new IntVector2(-1,-1)},
            { 8,new IntVector2(1,-1)},
        };

        public static Projectile standardproj = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0].projectile;
        public static Projectile SpawnProjAtPosi(Projectile proj, Vector2 posi,PlayerController player,Gun gun, float var = 0,float speedmult = 1,bool postprocess = true)
        {
            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, posi, Quaternion.Euler(0f, 0f, ((gun.CurrentOwner as PlayerController).CurrentGun == null) ? 0f : (gun.CurrentOwner as PlayerController).CurrentGun.CurrentAngle + var), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.speed *= speedmult;
                component.UpdateSpeed();
                if (postprocess)
                {
                    player.DoPostProcessProjectile(proj);
                }

            }
            
            return component;
        }

        public static Projectile SpawnProjAtPosi(Projectile proj, Vector2 posi, PlayerController player, float Angle = 0, float var = 0, float speedmult = 1, bool postprocess = true)
        {
            
            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, posi, Quaternion.Euler(0f, 0f, Angle + var), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.speed *= speedmult;
                component.UpdateSpeed();
                if (postprocess)
                {
                    player.DoPostProcessProjectile(proj);
                }

            }

            return component;
        }

        public static void TrimAllGunSprites(Gun gun)
        {
            Alexandria.ItemAPI.GunTools.TrimGunSprites(gun);
        }

        public static void ResetMusic(Dungeon d)
        {
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            string m_cachedMusicEventCore;
            if (flag)
            {
                m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
            AkSoundEngine.PostEvent(m_cachedMusicEventCore, GameManager.Instance.gameObject);

        }

        public static List<GameObject> GenerateOverheadFromResourceList(List<string> resourceList, List<string> namesList)
        {
            List<GameObject> returnList = new List<GameObject>();
            int num = 0;
            foreach(String resource in resourceList)
            {

                GameObject create = SpriteBuilder.SpriteFromResource(resource, null);
                create.name = namesList[num];
                UnityEngine.Object.DontDestroyOnLoad(create);
                FakePrefab.MarkAsFakePrefab(create);
                create.SetActive(false);

                returnList.Add(create);
                num++;
            }

            return returnList;
        }

        public static List<T> RandomNoRepeats<T>(List<T> candidates, int count)
        {
            List<T> outcomes = new List<T> { };
            int i = 0;
            do
            {
                i++;
                int V = UnityEngine.Random.Range(0, candidates.Count);
                if (!outcomes.Contains(candidates[V]))
                {
                    outcomes.Add(candidates[V]);
                }


            } while (i < count * 3 && outcomes.Count < count);

            return outcomes;
        }


        public static float getPlayerDepth()
        {
            float DepthMult = 1;
            
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)//0
            {
                DepthMult = 1f;
                
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)//.5
            {
                DepthMult = 1.5f;
                
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON)//1
            {
                DepthMult = 2f;
                
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.BELLYGEON)//1.5
            {
                DepthMult = 2.5f;
               
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)//2
            {
                DepthMult = 3f;
               
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)//2.5
            {
                DepthMult = 3.5f;
                
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON)//3
            {
                DepthMult = 4f;
               
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)//3.5
            {
                DepthMult = 4.5f;
               
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)//4
            {
                DepthMult = 5f;
                
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)//past
            {
                DepthMult = 5.5f;
               
            }
            return DepthMult;

        }

        public static IEnumerable<PlayerItem> GetCurrentlySelectedActiveItem(PlayerController player)
        {
            foreach (PlayerItem active in player.activeItems)
            {
                if (active.IsCurrentlyActive)
                {
                    yield return active;
                    break;
                }
            }

        }


        public static VFXComplex CreateVFXComplex(string name, List<string> spritePaths, int fps, IntVector2 Dimensions, tk2dBaseSprite.Anchor anchor, bool usesZHeight, float zHeightOffset, bool persist = false, VFXAlignment alignment = VFXAlignment.NormalAligned, float emissivePower = -1, Color? emissiveColour = null, VFXPoolType type = VFXPoolType.All)
        {

            //Use this to create multiple muzzleflashes into a VFX pool on the other side.
            GameObject Obj = new GameObject(name);
            
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            Obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(Obj);
            UnityEngine.Object.DontDestroyOnLoad(Obj);

            tk2dSpriteCollectionData VFXSpriteCollection = SpriteBuilder.ConstructCollection(Obj, (name + "_Collection"));
            int spriteID = SpriteBuilder.AddSpriteToCollection(spritePaths[0], VFXSpriteCollection);

            tk2dSprite sprite = Obj.GetOrAddComponent<tk2dSprite>();
            sprite.SetSprite(VFXSpriteCollection, spriteID);
            tk2dSpriteDefinition defaultDef = sprite.GetCurrentSpriteDef();
            defaultDef.colliderVertices = new Vector3[]{
                      new Vector3(0f, 0f, 0f),
                      new Vector3((Dimensions.x / 16), (Dimensions.y / 16), 0f)
                  };

            tk2dSpriteAnimator animator = Obj.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = Obj.GetOrAddComponent<tk2dSpriteAnimation>();
            animation.clips = new tk2dSpriteAnimationClip[0];
            animator.Library = animation;
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = fps };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < spritePaths.Count; i++)
            {
                tk2dSpriteCollectionData collection = VFXSpriteCollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection(spritePaths[i], collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(anchor);
                frameDef.colliderVertices = defaultDef.colliderVertices;
                if (emissivePower > 0) frameDef.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                if (emissivePower > 0) frameDef.material.SetFloat("_EmissiveColorPower", emissivePower);
                if (emissiveColour != null) frameDef.material.SetColor("_EmissiveColor", (Color)emissiveColour);
                if (emissivePower > 0) frameDef.materialInst.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                if (emissivePower > 0) frameDef.materialInst.SetFloat("_EmissiveColorPower", emissivePower);
                if (emissiveColour != null) frameDef.materialInst.SetColor("_EmissiveColor", (Color)emissiveColour);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            if (emissivePower > 0) sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            if (emissivePower > 0) sprite.renderer.material.SetFloat("_EmissiveColorPower", emissivePower);
            if (emissiveColour != null) sprite.renderer.material.SetColor("_EmissiveColor", (Color)emissiveColour);
            clip.frames = frames.ToArray();
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animation.clips = animation.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            if (!persist)
            {
                SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
                kill.fadeTime = -1f;
                kill.animator = animator;
                kill.delayDestructionTime = -1f;
            }
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("start");
            vfObj.attached = true;
            vfObj.persistsOnDeath = persist;
            vfObj.usesZHeight = usesZHeight;
            vfObj.zHeight = zHeightOffset;
            vfObj.alignment = alignment;
            vfObj.destructible = false;
            vfObj.effect = Obj;
            complex.effects = new VFXObject[] { vfObj };
            
            return complex;
        }

        public static void GenerateCloudTileAtPosition(Dungeon dungeon, RoomHandler parentRoom, IntVector2 position, float CorruptionOdds = 0.5f, bool AllowGlitchShader = true, float GlitchShaderOdds = 0.2f)
        {

            

            if (dungeon == null | parentRoom == null | UnityEngine.Random.value > CorruptionOdds) { return; }

            tk2dSpriteCollectionData dungeonCollection = dungeon.tileIndices.dungeonCollection;

            bool isWallCell = false;

            if (dungeon.data.isWall(position.x, position.y) | dungeon.data.isAnyFaceWall(position.x, position.y) | dungeon.data.isWall(position.x, position.y - 1))
            {
                isWallCell = true;
            }

            GameObject m_GlitchTile = new GameObject("GlitchTile_" + UnityEngine.Random.Range(1000000, 9999999)) { layer = 22 };
            m_GlitchTile.transform.position = position.ToVector2();
            m_GlitchTile.transform.parent = parentRoom.hierarchyParent;

            m_GlitchTile.AddComponent<tk2dSprite>();

            tk2dSprite m_GlitchSprite = m_GlitchTile.GetComponent<tk2dSprite>();
            m_GlitchSprite.Collection = dungeonCollection;
            int SpriteID_001 = SpriteBuilder.AddSpriteToCollection("Knives/Resources/cloud_001", m_GlitchSprite.Collection);
            int SpriteID_002 = SpriteBuilder.AddSpriteToCollection("Knives/Resources/cloud_002", m_GlitchSprite.Collection);
            float num = UnityEngine.Random.Range(0, 2.0f);
            m_GlitchSprite.SetSprite(m_GlitchSprite.Collection, SpriteID_001);
            if (num > 1)
            {
                m_GlitchSprite.SetSprite(m_GlitchSprite.Collection, SpriteID_002);
            }
            m_GlitchSprite.ignoresTiltworldDepth = false;
            m_GlitchSprite.depthUsesTrimmedBounds = false;
            m_GlitchSprite.allowDefaultLayer = false;
            m_GlitchSprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.NONE;
            m_GlitchSprite.independentOrientation = false;
            m_GlitchSprite.hasOffScreenCachedUpdate = false;
            if (isWallCell)
            {
                m_GlitchSprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            }
            else
            {
                m_GlitchSprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            }
            m_GlitchSprite.SortingOrder = 2;
            m_GlitchSprite.IsBraveOutlineSprite = false;
            m_GlitchSprite.IsZDepthDirty = false;
            m_GlitchSprite.ApplyEmissivePropertyBlock = false;
            m_GlitchSprite.GenerateUV2 = false;
            m_GlitchSprite.LockUV2OnFrameOne = false;
            m_GlitchSprite.StaticPositions = false;

            m_GlitchTile.AddComponent<DebrisObject>();
            DebrisObject m_GlitchDebris = m_GlitchTile.GetComponent<DebrisObject>();
            m_GlitchDebris.angularVelocity = 0;
            m_GlitchDebris.angularVelocityVariance = 0;
            m_GlitchDebris.animatePitFall = false;
            m_GlitchDebris.bounceCount = 0;
            m_GlitchDebris.breakOnFallChance = 0;
            m_GlitchDebris.breaksOnFall = false;
            m_GlitchDebris.canRotate = false;
            m_GlitchDebris.changesCollisionLayer = false;
            m_GlitchDebris.collisionStopsBullets = false;
            m_GlitchDebris.followupBehavior = DebrisObject.DebrisFollowupAction.None;
            m_GlitchDebris.IsAccurateDebris = true;
            m_GlitchDebris.IsCorpse = false;
            m_GlitchDebris.lifespanMax = 1200;
            m_GlitchDebris.lifespanMin = 1100;
            m_GlitchDebris.motionMultiplier = 0;
            m_GlitchDebris.pitFallSplash = false;
            m_GlitchDebris.playAnimationOnTrigger = false;
            m_GlitchDebris.PreventAbsorption = true;
            m_GlitchDebris.PreventFallingInPits = true;
            m_GlitchDebris.Priority = EphemeralObject.EphemeralPriority.Ephemeral;
            m_GlitchDebris.shouldUseSRBMotion = false;
            m_GlitchDebris.usesDirectionalFallAnimations = false;
            m_GlitchDebris.usesLifespan = true;

            DepthLookupManager.ProcessRenderer(m_GlitchSprite.renderer, DepthLookupManager.GungeonSortingLayer.BACKGROUND);
            m_GlitchTile.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            m_GlitchSprite.IsPerpendicular = false;
            // m_GlitchSprite.HeightOffGround = -4f;
            m_GlitchSprite.HeightOffGround = -1.7f;
            m_GlitchSprite.SortingOrder = 2;
            m_GlitchSprite.UpdateZDepth();
            
            
            return;
        }

        public static void EasyAnimate(Projectile proj, List<string> projectileNames, int frames, IntVector2 dimensions, int fps, bool light, tk2dSpriteAnimationClip.WrapMode wrapmode, string name = "idle")
        {
            MiscToolMethods.AnimateProjectile(proj,projectileNames, fps, wrapmode, MiscToolMethods.DupeList<IntVector2>(dimensions, frames), MiscToolMethods.DupeList<bool>(light, frames), MiscToolMethods.DupeList<tk2dBaseSprite.Anchor>(tk2dBaseSprite.Anchor.MiddleCenter, frames), MiscToolMethods.DupeList<bool>(true, frames), MiscToolMethods.DupeList<bool>(false, frames), MiscToolMethods.DupeList<Vector3?>(null, frames), MiscToolMethods.DupeList<IntVector2?>(null, frames), MiscToolMethods.DupeList<IntVector2?>(null, frames), MiscToolMethods.DupeList<Projectile>(null, frames), 0,name);
        }
        public static void AnimateProjectile(Projectile proj, List<string> names, int fps, tk2dSpriteAnimationClip.WrapMode wrapmode, List<IntVector2> pixelSizes, List<bool> lighteneds, List<tk2dBaseSprite.Anchor> anchors, List<bool> anchorsChangeColliders, List<bool> fixesScales, List<Vector3?> manualOffsets, List<IntVector2?> overrideColliderPixelSizes, List<IntVector2?> overrideColliderOffsets, List<Projectile> overrideProjectilesToCopyFrom, int framesToLoopAfter, string name = "idle")
        {
            tk2dSpriteAnimationClip tk2dSpriteAnimationClip = new tk2dSpriteAnimationClip();
            tk2dSpriteAnimationClip.name = name;
            tk2dSpriteAnimationClip.fps = (float)fps;
            List<tk2dSpriteAnimationFrame> list = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < names.Count; i++)
            {
                string text = names[i];
                IntVector2 intVector = pixelSizes[i];
                IntVector2? intVector2 = overrideColliderPixelSizes[i];
                IntVector2? intVector3 = overrideColliderOffsets[i];
                Vector3? vector = manualOffsets[i];
                bool flag = anchorsChangeColliders[i];
                bool flag2 = fixesScales[i];
                bool flag3 = vector == null;
                if (flag3)
                {
                    vector = new Vector3?(Vector2.zero);
                }
                tk2dBaseSprite.Anchor anchor = anchors[i];
                bool flag4 = lighteneds[i];
                Projectile projectile = overrideProjectilesToCopyFrom[i];
                tk2dSpriteAnimationFrame tk2dSpriteAnimationFrame = new tk2dSpriteAnimationFrame();
                tk2dSpriteAnimationFrame.spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(text);
                tk2dSpriteAnimationFrame.spriteCollection = ETGMod.Databases.Items.ProjectileCollection;
                list.Add(tk2dSpriteAnimationFrame);
                int? num = null;
                int? num2 = null;
                bool flag5 = intVector2 != null;
                if (flag5)
                {
                    num = new int?(intVector2.Value.x);
                    num2 = new int?(intVector2.Value.y);
                }
                int? num3 = null;
                int? num4 = null;
                bool flag6 = intVector3 != null;
                if (flag6)
                {
                    num3 = new int?(intVector3.Value.x);
                    num4 = new int?(intVector3.Value.y);
                }
                tk2dSpriteDefinition tk2dSpriteDefinition = GunTools.SetupDefinitionForProjectileSprite(text, tk2dSpriteAnimationFrame.spriteId, intVector.x, intVector.y, flag4, num, num2, num3, num4, projectile);
                GunTools.ConstructOffsetsFromAnchor(tk2dSpriteDefinition, anchor, new Vector2?(tk2dSpriteDefinition.position3), flag2, flag);
                tk2dSpriteDefinition.position0 += vector.Value;
                tk2dSpriteDefinition.position1 += vector.Value;
                tk2dSpriteDefinition.position2 += vector.Value;
                tk2dSpriteDefinition.position3 += vector.Value;
                bool flag7 = i == 0;
                if (flag7)
                {
                    ETGMod.GetAnySprite(proj).SetSprite(tk2dSpriteAnimationFrame.spriteCollection, tk2dSpriteAnimationFrame.spriteId);
                }
            }
            tk2dSpriteAnimationClip.wrapMode = wrapmode;
            bool flag8 = wrapmode == tk2dSpriteAnimationClip.WrapMode.LoopSection;
            if (flag8)
            {
                tk2dSpriteAnimationClip.loopStart = framesToLoopAfter;
            }
            tk2dSpriteAnimationClip.frames = list.ToArray();
            bool flag9 = proj.sprite.spriteAnimator == null;
            if (flag9)
            {
                proj.sprite.spriteAnimator = proj.sprite.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            proj.sprite.spriteAnimator.playAutomatically = true;
            bool flag10 = proj.sprite.spriteAnimator.Library == null;
            bool flag11 = flag10;
            if (flag11)
            {
                proj.sprite.spriteAnimator.Library = proj.sprite.spriteAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
                proj.sprite.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
                proj.sprite.spriteAnimator.Library.enabled = true;
            }
            proj.sprite.spriteAnimator.Library.clips = proj.sprite.spriteAnimator.Library.clips.Concat(new tk2dSpriteAnimationClip[]
            {
                tk2dSpriteAnimationClip
            }).ToArray<tk2dSpriteAnimationClip>();
            proj.sprite.spriteAnimator.DefaultClipId = proj.sprite.spriteAnimator.Library.GetClipIdByName("idle");
            proj.sprite.spriteAnimator.deferNextStartClip = false;
        }
        public static List<T> DupeList<T>(T value, int length)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < length; i++)
            {
                list.Add(value);
            }
            return list;
        
        }

        public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector2 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);

            // return the vector info
            return _returnVector;
        }

        public static IEnumerator DoReverseDistortionWaveLocal(Vector2 center, float distortionIntensity, float distortionRadius, float maxRadius, float duration)
        {
            Material distMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
            Vector4 distortionSettings = MiscToolMethods.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
            distMaterial.SetVector("_WaveCenter", distortionSettings);
            Pixelator.Instance.RegisterAdditionalRenderPass(distMaterial);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                t = BraveMathCollege.LinearToSmoothStepInterpolate(0f, 1f, t);
                distortionSettings = MiscToolMethods.GetCenterPointInScreenUV(center, distortionIntensity, distortionRadius);
                distortionSettings.w = Mathf.Lerp(distortionSettings.w, 0f, t);
                distMaterial.SetVector("_WaveCenter", distortionSettings);
                float currentRadius = Mathf.Lerp(maxRadius, 0f, t);
                distMaterial.SetFloat("_DistortProgress", currentRadius / (maxRadius * 5f));
                yield return null;
            }
            Pixelator.Instance.DeregisterAdditionalRenderPass(distMaterial);
            UnityEngine.Object.Destroy(distMaterial);
            yield break;
        }

        private static Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, dRadius, dIntensity);
        }


    }
}



