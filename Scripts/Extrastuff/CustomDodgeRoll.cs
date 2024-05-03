using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;
using MonoMod.RuntimeDetour;

namespace Knives
{
    public interface ICustomDodgeRoll
    {
        bool DodgeButtonHeld { get; set; }
        bool isDodging { get; set; }
        PlayerController owner { get; set; }

        bool canDodge { get; }  // if false, disables a CustomDodgeRoll from activating
        bool canMultidodge { get; }  // if true, enables dodging while already mid-dodge

        void BeginDodgeRoll();  // called once before a dodge roll begins
        IEnumerator ContinueDodgeRoll();  // called every frame until dodge roll ends
        void FinishDodgeRoll(); // called once after a dodge roll ends
        void AbortDodgeRoll(); // called if the dodge roll is interrupted prematurely
    }

    public class CustomDodgeRoll : MonoBehaviour, ICustomDodgeRoll
    {
        public bool DodgeButtonHeld   { get; set; }
        public bool isDodging         { get; set; }
        public PlayerController owner { get; set; }

        public virtual bool canDodge      => true;
        public virtual bool canMultidodge => false;

        public static Hook customDodgeRollHook = null;

        public static void InitCustomDodgeRollHooks()
        {
            customDodgeRollHook = new Hook(
                typeof(PlayerController).GetMethod("HandleStartDodgeRoll", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(CustomDodgeRoll).GetMethod("CustomDodgeRollHook"));
        }

        public static bool CustomDodgeRollHook(Func<PlayerController,Vector2,bool> orig, PlayerController player, Vector2 direction)
        {
            if (player.DodgeRollIsBlink)
            {
                return orig(player, direction);
            }
            List<CustomDodgeRoll> overrides = new List<CustomDodgeRoll>();
            foreach (PassiveItem p in player.passiveItems)
            {
                CustomDodgeRoll overrideDodgeRoll = p.GetComponent<CustomDodgeRoll>();
                if (overrideDodgeRoll)
                    overrides.Add(overrideDodgeRoll);
            }
            if (overrides.Count == 0)  // fall back to default behavior if we don't have overrides
                return orig(player,direction);

            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
            if (instanceForPlayer.ActiveActions.DodgeRollAction.IsPressed)
            {
                // if (!(this.owner.IsDodgeRolling || this.owner.IsFalling || this.owner.IsInputOverridden || this.dodgeButtonHeld || this.isDodging))
                // if (player.AcceptingNonMotionInput && !(player.IsDodgeRolling || this.dodgeButtonHeld || this.isDodging))
                
                foreach (CustomDodgeRoll customDodgeRoll in overrides)
                {
                    if (customDodgeRoll.DodgeButtonHeld)
                        continue;
                    customDodgeRoll.DodgeButtonHeld = true;
                    if (!player.IsDodgeRolling) 
                    {
                        customDodgeRoll.TryDodgeRoll();
                    }
                    else
                    {
                        customDodgeRoll.StartCoroutine(BufferButtonHold(player, customDodgeRoll));
                    }
                    return true;

                }
                
            }
            else
            {
                foreach (CustomDodgeRoll customDodgeRoll in overrides)
                    customDodgeRoll.DodgeButtonHeld = false;
            }
            return false;
        }
        public bool rollattemptwasBuffered;
        private static IEnumerator BufferButtonHold(PlayerController player, CustomDodgeRoll customDodgeRoll)
        {
            
            while (player.IsDodgeRolling && customDodgeRoll.isDodging) // wait...
            {
                yield return null;
            }
            
            if (Key(GungeonActions.GungeonActionType.DodgeRoll, player)) // still holding?
            {


                customDodgeRoll.DodgeButtonHeld = true;
                customDodgeRoll.AbortDodgeRoll();
                customDodgeRoll.TryDodgeRoll();// okay go
                
            }
            yield break;
        }

        public virtual void BeginDodgeRoll()
        {
            // any dodge setup code should be here
        }

        public virtual void FinishDodgeRoll()
        {
            // any succesful dodge cleanup code should be here
        }

        public virtual void AbortDodgeRoll()
        {
            // any aborted dodge cleanup code should be here
            isDodging = false;
        }

        public virtual IEnumerator ContinueDodgeRoll()
        {
            // code to execute while dodge rolling should be here
            yield break;
        }

        private IEnumerator DoDodgeRollWrapper()
        {
            
            isDodging = true;
            BeginDodgeRoll();
            IEnumerator script = ContinueDodgeRoll();
            while(isDodging && script.MoveNext())
                yield return script.Current;
            FinishDodgeRoll();
            isDodging = false;
            
            yield break;
        }

        private bool TryDodgeRoll()
        {
            
            if (!owner || !canDodge || (isDodging && !canMultidodge))
                return false;
            owner.StartCoroutine(DoDodgeRollWrapper());
            
            return true;
        }

        public static bool Key(GungeonActions.GungeonActionType action, PlayerController player)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }
    }
}

