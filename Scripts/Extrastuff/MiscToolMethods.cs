using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Dungeonator;

namespace Knives
{


    class MiscToolMethods
    {

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
    }

}

