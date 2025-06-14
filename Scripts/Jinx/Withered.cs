using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;


namespace Knives
{
    public class WitheringRose : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Withering Rose";

            string resourceName = "Knives/Resources/withering_rose";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<WitheringRose>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "By Any Other Name";
            string longDesc = "Lowers health between floors, Gives you thorns. This rose infests itself into the holder draining their life and giving the host its properties." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
            item.quality = PickupObject.ItemQuality.C;
            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gives you thorns, but lose half a heart each floor.";
        }
        public static int itemID;
        public static List<GameObject> m_extantThorns = new List<GameObject> { };
        public bool HasSyn = false;
        public override void Pickup(PlayerController player)
        {
            
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            InitThorns(player);
            base.Pickup(player);

            owner = player;
        }

        public override void Update()
        {
            if(this.owner != null)
            {
                if(this.owner.PlayerHasActiveSynergy("Every Rose Has Thorns"))
                {
                    if(HasSyn == false)
                    {
                        HasSyn = true;
                        DestroyThorns();
                        InitThorns(this.owner);
                    }
                }
                else
                {
                    if (HasSyn == true)
                    {
                        HasSyn = false;
                        DestroyThorns();
                        InitThorns(this.owner);
                    }
                }
            }
            base.Update();
        }

        public void DestroyThorns()
        {
            for (int i = m_extantThorns.Count - 1; i >= 0; i--)
            {
                UnityEngine.GameObject.Destroy(m_extantThorns[i]);
            }
        }

        public void InitThorns(PlayerController player)
        {
            int num = 6;
            if (player.PlayerHasActiveSynergy("Every Rose Has Thorns"))
            {
                num = 8;
            }
            for (int i = 0; i < num; i++)
            {
                GameObject thorn = PlayerOrbitalItem.CreateOrbital(player, Thornstone.orbitalPrefab.gameObject, false);
                SpikeOrbit orbit = thorn.GetOrAddComponent<SpikeOrbit>();
                if (i % 2 == 0) orbit.LerpPoint = 200;
                SpeculativeRigidbody spec = thorn.GetComponent<SpeculativeRigidbody>();
                spec.OnPreRigidbodyCollision += this.BodyColision;

                m_extantThorns.Add(thorn);
            }

        }

        private void BodyColision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            PlayerController player = this.Owner;
            PhysicsEngine.SkipCollision = true;
            RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
            AIActor component = otherRigidbody.GetComponent<AIActor>();
            bool flag = component != null;
            if (flag)
            {
                if (component.healthHaver && component.CenterPosition.GetAbsoluteRoom() == currentRoom)
                {
                    if (didDamage == false && !player.IsStealthed)
                    {
                        didDamage = true;
                        GameManager.Instance.StartCoroutine(this.DamageTimer());

                        if (player.PlayerHasActiveSynergy("Every Rose Has Thorns"))
                        {
                            component.healthHaver.ApplyDamage(15f, Vector2.zero, "Poke", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        }
                        else
                        {
                            component.healthHaver.ApplyDamage(10f, Vector2.zero, "Poke", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        }

                        this.owner.DidUnstealthyAction();
                    }
                }
            }
        }
        private IEnumerator DamageTimer()
        {
            yield return new WaitForSeconds(0.5f);
            didDamage = false;
            yield break;
        }


        public PlayerController owner;

        bool half_toggle = false;
        private bool didDamage;

        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                DestroyThorns();
                InitThorns(player);
                if(player.healthHaver.GetCurrentHealth() > .5f)
                {
                    if (player.healthHaver.Armor != 0)
                    {
                        GameManager.Instance.StartCoroutine(armorlost());

                    }
                    else
                    {
                        player.healthHaver.ForceSetCurrentHealth(player.healthHaver.GetCurrentHealth() - .5f);
                    }
                   
                }
                if(player.characterIdentity == PlayableCharacters.Robot)
                {
                    if(player.healthHaver.Armor > 1)
                    {
                        GameManager.Instance.StartCoroutine(armorlost());
                    }
                }
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        private IEnumerator armorlost()
        {
            yield return new WaitForSeconds(.01f);
            float armor = this.Owner.healthHaver.Armor;
            this.Owner.healthHaver.Armor *= 0;
            int adjarmor = (int)armor - 1;
            this.Owner.healthHaver.Armor = adjarmor;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DestroyThorns();
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }
    }
}