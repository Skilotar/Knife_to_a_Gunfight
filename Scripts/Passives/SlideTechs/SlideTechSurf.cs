using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using ItemAPI;
using MultiplayerBasicExample;
using UnityEngine;
namespace Knives
{
    class Slide_Surf : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Surf";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_surf";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_Surf>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Shredding";
            string longDesc = "One of the cult of the slide's more simple spells. It greatly reduces the friction of tables you slide over for a short time.\n\n" +
                "Something about this table and flipper is TOOTALLY wrong maan." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            Slide_Surf.ElectricImmunity = new DamageTypeModifier();
            Slide_Surf.ElectricImmunity.damageMultiplier = 0f;
            Slide_Surf.ElectricImmunity.damageType = CoreDamageTypes.Electric;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }
        bool doingreloadroutine = false;
        bool iselectricimmune = false;
        float rampup = .001f;
        public override void Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {
                    
                    if (onepertable == false)
                    {
                        if (this.Owner.CurrentRoom != null)
                        {
                            IPlayerInteractable nearestInteractable = this.Owner.CurrentRoom.GetNearestInteractable(this.Owner.specRigidbody.UnitCenter, .5f, this.Owner);
                            if (nearestInteractable != null)
                            {
                                if (nearestInteractable is FlippableCover)
                                {
                                    FlippableCover Table = nearestInteractable as FlippableCover;
                                    CurrentTable = Table;
                                    StartCoroutine(SurfRoutine(this.Owner, Table));
                                    rampup = .001f;
                                }
                            }
                        }
                    }
                    float synergymult = 1;
                    if (this.Owner.PlayerHasActiveSynergy("Surge Surfer")) synergymult = 1.2f;
                    this.Owner.knockbackDoer.ApplyKnockback(this.Owner.NonZeroLastCommandedDirection, (((.5f + rampup )* (this.Owner.stats.MovementSpeed / 7)) * synergymult), true);
                    rampup += .001f;
                    if (rampup >= .6) rampup = .6f;
                }
                else
                {
                    onepertable = false;
                }

                if (this.Owner.PlayerHasActiveSynergy("Surge Surfer"))
                {
                    if (iselectricimmune == false)
                    {
                        this.Owner.healthHaver.damageTypeModifiers.Add(Slide_Surf.ElectricImmunity);
                        iselectricimmune = true; 
                    }
                    
                }
                else
                {
                    if (iselectricimmune == true)
                    {
                        this.Owner.healthHaver.damageTypeModifiers.Remove(Slide_Surf.ElectricImmunity);
                        iselectricimmune = false;
                    }

                }

            }
        }
        public bool onepertable = false;
        public bool cleanup = false;    
        int loops = 0;
        FlippableCover CurrentTable;
        public static DamageTypeModifier ElectricImmunity { get; private set; }
        int KnockbackID;

        private IEnumerator SurfRoutine(PlayerController player, FlippableCover Table)
        {
            onepertable = true;
            cleanup = true; // do cleanup unless Click is done to kill early.
            if (Table != null)
            {
                if (!Table.IsFlipped && !Table.IsBroken)
                {
                    // get near middle
                    while (Vector2.Distance(player.CenterPosition,Table.specRigidbody.UnitCenter) > 1)
                    {
                        yield return new WaitForEndOfFrame();
                        yield return null;
                    }

                    // setup section
                    yield return new WaitForSeconds(.07f);
                    PassiveItem.IncrementFlag((PlayerController)player, typeof(LiveAmmoItem));
                    Table.specRigidbody.Velocity = player.specRigidbody.Velocity;
                    player.specRigidbody.OnPreRigidbodyCollision += this.OnPreCollision;
                    Table.specRigidbody.OnPreRigidbodyCollision += this.OnPreCollision;
                    Table.PreventPitFalls = true;
                    if(player.IsSlidingOverSurface) player.ForceStartDodgeRoll();

                    // Loop
                    while (Table != null && !Table.IsFlipped && !Table.IsBroken && player.IsSlidingOverSurface)
                    {
                       
                        if (loops == 20)
                        {

                            this.DoDaGoo();
                            loops = 0;
                        }
                        loops++;
                        if (Table != null)
                        {
                            if (Table.specRigidbody != null)
                            {
                                Table.specRigidbody.Velocity = player.specRigidbody.Velocity;
                                if (KeyTime(GungeonActions.GungeonActionType.DodgeRoll) > .25f || Key(GungeonActions.GungeonActionType.Shoot))
                                {

                                    if (Table != null)
                                    {
                                        Table.specRigidbody.Velocity = new Vector2(0, 0);
                                        Table.specRigidbody.OnPreRigidbodyCollision -= this.OnPreCollision;
                                        Table.DestroyCover();
                                    }
                                    cleanup = false;
                                }
                            }
                        }
                        yield return null;
                    }

                    // Cleanup
                    if (Table != null && cleanup == true)
                    {
                        if (Table.specRigidbody != null)
                        {
                            Table.specRigidbody.OnPreRigidbodyCollision -= this.OnPreCollision;
                            Table.specRigidbody.Velocity = new Vector2(0, 0);
                            Table.PreventPitFalls = false;
                        }
                    }
                    PassiveItem.DecrementFlag((PlayerController)player, typeof(LiveAmmoItem));
                    
                    player.specRigidbody.OnPreRigidbodyCollision -= this.OnPreCollision;

                }
            }
            onepertable = true;
        }

        public void DoDaGoo()
        {

            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            string text = "assets/data/goops/water goop.asset";
            GoopDefinition goopDefinition;
            try
            {
                GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
                goopDefinition = gameObject.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
            }
            goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDefinition);
            goopManagerForGoopType.TimedAddGoopCircle(this.Owner.sprite.WorldCenter, 2f, 3f, false);

            if (this.Owner.PlayerHasActiveSynergy("Surge Surfer")) goopManagerForGoopType.ElectrifyGoopCircle(this.Owner.CenterPosition, 1);

        }


        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(otherRigidbody.aiActor != null)
            {
                if(otherRigidbody.aiActor.healthHaver != null)
                {
                    otherRigidbody.aiActor.healthHaver.ApplyDamage(30, myRigidbody.Velocity, "Shredded", CoreDamageTypes.Water);
                    otherRigidbody.aiActor.healthHaver.TriggerInvulnerabilityPeriod(.25f);
                    if(CurrentTable != null) CurrentTable.DestroyCover(); // How the heck would this ever be null?
                    if (this.Owner != null) this.Owner.healthHaver.TriggerInvulnerabilityPeriod(1);
                }
            }
        }

        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && (this.Owner as PlayerController) != null)
            {
                return BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }

        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel && (this.Owner as PlayerController) != null)
            {
                return BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
            }
            else
            {
                return 0;
            }

        }
    }
}