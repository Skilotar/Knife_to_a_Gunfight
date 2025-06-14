using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class BannerItem : PlayerItem
    {
        // Call setup sprites first then setup banner sprites.
        public void SetSpriteNames(string playerUpSprite, string playerDownSprite, string playerLeftSprite, string playerRightSprite, string playerUpLeftSprite, string playerUpRightSprite)
        {
            playerUpSpriteName = playerUpSprite;
            playerDownSpriteName = playerDownSprite;
            playerLeftSpriteName = playerLeftSprite;
            playerRightSpriteName = playerRightSprite;
            playerUpLeftSpriteName = playerUpLeftSprite;
            playerUpRightSpriteName = playerUpRightSprite;

            offsetMapping = new Dictionary<string, Vector2>()
        {
            { playerUpSpriteName, new Vector2(8 / 16f, 2 / 16f) },
            { playerDownSpriteName, new Vector2(5 / 16f, 4 / 16f) },
            { playerLeftSpriteName, new Vector2(9 / 16f, 7 / 16f) },
            { playerRightSpriteName, new Vector2(3 / 16f, 5 / 16f) },
            { playerUpRightSpriteName, new Vector2(7 / 16f, 2 / 16f) },
            { playerUpLeftSpriteName, new Vector2(10 / 16f, 2 / 16f) },
        };
        }
        public void SetupBannerSprites(string name, string upResourcePath, string downResourcePath, string leftResourcePath, string rightResourcePath, string upLeftResourcePath, string upRightResourcePath)
        {
            backpackVfx =
                VFXBuilder.CreateVFX(name, new List<string> { upResourcePath }, 10, new IntVector2(16, 20), tk2dBaseSprite.Anchor.MiddleCenter,
                false, 0, -1, null, tk2dSpriteAnimationClip.WrapMode.Once, persist: true);

            tk2dSprite sprite = backpackVfx.GetComponent<tk2dSprite>();
            SpriteBuilder.AddSpriteToCollection(upResourcePath, sprite.collection, playerUpSpriteName);
            SpriteBuilder.AddSpriteToCollection(downResourcePath, sprite.collection, playerDownSpriteName);
            SpriteBuilder.AddSpriteToCollection(upLeftResourcePath, sprite.collection, playerUpLeftSpriteName);
            SpriteBuilder.AddSpriteToCollection(upRightResourcePath, sprite.collection, playerUpRightSpriteName);
            SpriteBuilder.AddSpriteToCollection(leftResourcePath, sprite.collection, playerLeftSpriteName);
            SpriteBuilder.AddSpriteToCollection(rightResourcePath, sprite.collection, playerRightSpriteName);
        }
        public string playerUpSpriteName, playerDownSpriteName, playerLeftSpriteName, playerRightSpriteName, playerUpLeftSpriteName, playerUpRightSpriteName;
        public GameObject backpackVfx;
        private GameObject backpackInstance;
        private tk2dSprite sprite_instance;
        bool currentlyHidden = false;

        public Dictionary<string, Vector2> offsetMapping;
        public override void Update()
        {
            if (LastOwner == null || sprite_instance == null)
            {
                base.Update();
                return;
            }

            bool shouldBeHidden = LastOwner.IsFalling || LastOwner.IsDodgeRolling;
            if (!currentlyHidden && shouldBeHidden)
            {
                currentlyHidden = true;
                sprite_instance.renderer.enabled = false;
            }
            else if (currentlyHidden && !shouldBeHidden)
            {
                currentlyHidden = false;
                sprite_instance.renderer.enabled = true;
            }

            if (!currentlyHidden)
            {
                sprite_instance.HeightOffGround = LastOwner.IsBackfacing() ? 0.2f : -0.6f;

                string spriteName = GetSpriteNameFromPlayerDirection();
                sprite_instance.SetSprite(spriteName);
                Vector2 offset;
                if (offsetMapping.TryGetValue(spriteName, out offset))
                {
                    sprite_instance.PlaceAtLocalPositionByAnchor(offset, tk2dBaseSprite.Anchor.LowerLeft);
                }
            }
            sprite_instance.UpdateZDepth();
            base.Update();
        }
        string GetSpriteNameFromPlayerDirection()
        {
            float currentAngle = LastOwner.FacingDirection;
            if (currentAngle < -120f)
                return playerLeftSpriteName;
            else if (currentAngle < -60f)
                return playerDownSpriteName;
            else if (currentAngle < 25f)
                return playerRightSpriteName;
            else if (currentAngle < 60f)
                return playerUpRightSpriteName;
            else if (currentAngle < 120f)
                return playerUpSpriteName;
            else if (currentAngle < 155f)
                return playerUpLeftSpriteName;
            // either angle above 155 (therefore should be facing left) or is out of range (somehow)
            return playerLeftSpriteName;
        }
        void EnableVisuals()
        {
            //backpackInstance = LastOwner.RegisterAttachedObject(backbackVfx, "jetpack", -0.1f);
            backpackInstance = GameObject.Instantiate(backpackVfx, LastOwner.CenterPosition, Quaternion.identity, LastOwner.transform);

            sprite_instance = backpackInstance.GetComponent<tk2dSprite>();
            Vector3 currentPosition = sprite_instance.transform.localPosition;
            //sprite_instance.transform.localPosition = currentPosition + new Vector3(currentPosition.x - 4f / 16f, currentPosition.y - 8f/16f, 0);
            //sprite_instance.PlaceAtPositionByAnchor(LastOwner.CenterPosition + new Vector2(currentPosition.x + 7f / 16f, currentPosition.y + 14f / 16f), tk2dSprite.Anchor.MiddleCenter);
            sprite_instance.PlaceAtLocalPositionByAnchor(new Vector2(8 / 16f, 4 / 16f), tk2dSprite.Anchor.LowerLeft);
            sprite_instance.HeightOffGround = 0.2f;
            sprite_instance.UpdateZDepth();

        }
        void DisableVisuals()
        {
            if (backpackInstance == null)
                return;

            Destroy(backpackInstance);
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            EnableVisuals();
        }
        public override void OnPreDrop(PlayerController user)
        {
            DisableVisuals();
            base.OnPreDrop(user);
        }
        public override void OnDestroy()
        {
            DisableVisuals();
            base.OnDestroy();
        }
    }
}
