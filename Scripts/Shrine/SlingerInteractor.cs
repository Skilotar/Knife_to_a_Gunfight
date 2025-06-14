using System;
using System.Collections;
using System.Collections.Generic;
using SaveAPI;
using System.Linq;
using System.Text;
using GungeonAPI;
using UnityEngine;
using Knives;
using ItemAPI;
using Dungeonator;

namespace Knives
{
    public class SlingerInteractor : SimpleInteractable, IPlayerInteractable
    {
        bool m_allowMeToIntroduceMyself = true;
        void Start()
        {
            talkPoint = transform.Find("talkpoint");
            m_isToggled = false;
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
            m_canUse = true;

            tk2dSpriteAnimator animator = base.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.Play(SlingerMinigame.sleepanim);

            
            conversation = new List<string>() {
                "Howdy Stranger",
                "I'm practicing for a rematch with the fella who done killed me.",
                "Care for a little Duel? I've got some nice prizes if ya get me."
            };

            acceptText = "Sure. (may lose [sprite \"heart_big_idle_001\"])";
            declineText = "No thanks.";
           
            SlingerMinigame.Start();

            /*
            var shadowobj = ItemAPI.SpriteBuilder.SpriteFromResource("genericbarrel_shadow_001");
            shadowobj.transform.SetParent(base.transform);
            shadowobj.transform.localPosition = new Vector3(0f, -1f / 16f);
            tk2dSprite shadow = shadowobj.GetComponent<tk2dSprite>();
            shadow.HeightOffGround = -5f;
            shadow.SortingOrder = 0;
            shadow.IsPerpendicular = false;
            shadow.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTilted");
            shadow.usesOverrideMaterial = true;
            */

            SlingerInteractor.one = SpriteBuilder.SpriteFromResource(SlingerInteractor.Icon, null);
            SlingerInteractor.one.name = SlingerInteractor.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(SlingerInteractor.one);
            Alexandria.ItemAPI.FakePrefab.MarkAsFakePrefab(SlingerInteractor.one);
            SlingerInteractor.one.SetActive(false);

            RoomHandler room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.sprite.WorldCenter.ToIntVector2());
            
            Minimap.Instance.RegisterRoomIcon(room, one, false);
        }
        public PlayerController player;
        public bool issleeping = true;

        public void Interact(PlayerController interactor)
        {

            if (SlingerMinigame.InGame == false)
            {
                if (SlingerMinigame.Playedgameonce == true)
                {

                    bool flag = TextBoxManager.HasTextBox(this.talkPoint);
                    if (!flag)
                    {
                        TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, "No more for today", interactor.characterAudioSpeechTag, instant: false, showContinueText: true);
                    }
                }
                else
                {
                    bool flag = TextBoxManager.HasTextBox(this.talkPoint);
                    if (!flag)
                    {
                        this.m_canUse = ((this.CanUse != null) ? this.CanUse(interactor, base.gameObject) : this.m_canUse);
                        bool flag2 = !this.m_canUse;
                        base.StartCoroutine(this.HandleConversation(interactor));
                        player = interactor;
                    }

                }
            }
            

        }

        public bool right = false;
        private IEnumerator HandleConversation(PlayerController interactor)
        {
            if(interactor.transform.position.x > base.gameObject.transform.position.x) // player is to the right of him
            {
                right = true;
               
                tk2dSpriteAnimator animator = base.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.Play(SlingerMinigame.idleRight);
                
            }

            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
            //base.spriteAnimator.PlayForDuration("talk_start", 1, "talk");
            interactor.SetInputOverride("npcConversation");
            Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
            yield return null;

            int conversationIndex = m_allowMeToIntroduceMyself ? 0 : conversation.Count - 1;
            while (conversationIndex < conversation.Count - 1)
            {
                GungeonAPI.Tools.Print($"Index: {conversationIndex}");
                TextBoxManager.ClearTextBox(this.talkPoint);
                TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, -1f, conversation[conversationIndex], interactor.characterAudioSpeechTag, instant: false, showContinueText: true);
                float timer = 0;
                while (!BraveInput.GetInstanceForPlayer(interactor.PlayerIDX).ActiveActions.GetActionFromType(GungeonActions.GungeonActionType.Interact).WasPressed || timer < 0.4f)
                {
                    timer += BraveTime.DeltaTime;
                    yield return null;
                }
                conversationIndex++;
            }
            m_allowMeToIntroduceMyself = false;
            TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, -1f, conversation[conversation.Count - 1], interactor.characterAudioSpeechTag, instant: false, showContinueText: true);

            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, acceptText, declineText);
            int selectedResponse = -1;
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
                yield return null;

            if (selectedResponse == 0)
            {
                TextBoxManager.ClearTextBox(this.talkPoint);
                if (right) // reset back
                {
                    right = false;
                }
                OnAccept?.Invoke(interactor, this.gameObject);
                //base.spriteAnimator.Play("talk");
                yield return new WaitForSeconds(1f);
                string response = "Wait for the " + "{wb}!!!{w}" + " and shoot me before I shoot you!";

                TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 4f, response, interactor.characterAudioSpeechTag, instant: false);
                yield return new WaitForSeconds(4f);
            }
            else
            {
                string response = "I'll be here.. if ya change your mind.";
                if (right) // reset back
                {
                    right = false;
                    tk2dSpriteAnimator animator = base.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                    animator.Play(SlingerMinigame.idleanim);
                }
                TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, response, interactor.characterAudioSpeechTag, instant: false);
                OnDecline?.Invoke(interactor, this.gameObject);
                TextBoxManager.ClearTextBox(this.talkPoint);
                Pixelator.Instance.LerpToLetterbox(1, 0.25f);
            }

            // Free player and run OnAccept/OnDecline actions
            interactor.ClearInputOverride("npcConversation");

            if (right) // reset back
            {
                tk2dSpriteAnimator animator = base.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.Play(SlingerMinigame.idleanim);
                right = false;
            }


            //base.spriteAnimator.Play("idle");
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 1f);
            base.sprite.UpdateZDepth();

            if(issleeping)
            {
                StartCoroutine(wakeUp());
                issleeping = false;
            }
        }

        private IEnumerator wakeUp()
        {
            result = 1;
            tk2dSpriteAnimator animator = base.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.Play(SlingerMinigame.wakeanim);

            yield return new WaitForSeconds(1.2f);

            animator.Play(SlingerMinigame.idleanim);

        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 1f);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (base.sprite == null)
            {
                return 100f;
            }
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, v) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            
            return result;
        }
        public float result = 4f;

        public static void Notify(string header, string text)
        {
            var sprite = PickupObjectDatabase.GetById(ChaoticComedy.ID).sprite;
            GameUIRoot.Instance.notificationController.DoCustomNotification(
                header,
                text,
                sprite.Collection,
                sprite.spriteId,
                UINotificationController.NotificationColor.SILVER,
                false,
                false);
        }

        private static string Icon = "Knives/Resources/Deadeye/Duel_MapIcon";
        private static GameObject one;
        private static string vfxName = "Icon";

    }
}