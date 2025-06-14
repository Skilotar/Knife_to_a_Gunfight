using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using Dungeonator;

namespace Knives
{
    class Eye_of_the_tiger : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Tiger Eye";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Eye_of_the_tiger";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Eye_of_the_tiger>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Blind Rage";
            string longDesc = "It's the eye of that tiger\n" +
                "It's the thrill of the fright\n" +
                "Running away from the rage of that tiger\n" +
                "And the last known survivor is fleeing the fight\n" +
                "It's all from stealing the eyeeeeeeeee of that tiger!!!!\n\n______________________________\n"+
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            //Adds the actual passive effect to the item

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.D;

            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            Eye_of_the_tiger.fleeData = new FleePlayerData();
            Eye_of_the_tiger.fleeData.Player = player;
            Eye_of_the_tiger.fleeData.StartDistance = 1f;
            Eye_of_the_tiger.fleeData.StopDistance = 50;
            Gun gun = ((Gun)PickupObjectDatabase.GetById(369));
            TigerProj = gun.DefaultModule.chargeProjectiles[0].Projectile.GetComponent<SummonTigerModifier>().TigerProjectilePrefab;


            base.Pickup(player);
        }
        public override void  DoEffect(PlayerController user)
        {
            RoomHandler absoluteRoom = user.CurrentRoom;
            if(user.CurrentRoom != null)
            {
                List<AIActor> actors = new List<AIActor> { };
                if (absoluteRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    actors = MiscToolMethods.RandomNoRepeats(absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear), UnityEngine.Random.Range(2, 6));
                }
                for (int i = 0; i <= actors.Count -1; i++)
                {
                    RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
                    Vector2? idealPosition = null;
                    if (actors[i] != null)
                    {
                        idealPosition = new Vector2?(actors[i].CenterPosition);
                    }
                    IntVector2 value = new IntVector2(4, 2);
                    if (base.sprite)
                    {
                        value = Vector2.Scale(new Vector2(4f, 2f), base.sprite.scale.XY()).ToIntVector2(VectorConversions.Ceil);
                    }
                    IntVector2? intVector = absoluteRoomFromPosition.GetOffscreenCell(new IntVector2?(value), new CellTypes?(CellTypes.FLOOR), false, idealPosition);
                    if (intVector == null)
                    {
                        intVector = absoluteRoomFromPosition.GetRandomAvailableCell(new IntVector2?(value), new CellTypes?(CellTypes.FLOOR), false, null);
                    }
                    if (intVector != null)
                    {
                        if (actors[i].CenterPosition != null)
                        {
                            this.ShootSingleProjectile(intVector.Value.ToVector2(), BraveMathCollege.Atan2Degrees(actors[i].CenterPosition - intVector.Value.ToVector2()));
                        }
                        else
                        {
                            this.ShootSingleProjectile(intVector.Value.ToVector2(), BraveMathCollege.Atan2Degrees(absoluteRoomFromPosition.GetCenterCell().ToVector2() - intVector.Value.ToVector2()));
                        }
                    }
                    ETGModConsole.Log(actors[i]);
                    actors[i].behaviorSpeculator.FleePlayerData = Eye_of_the_tiger.fleeData;
                    GameManager.Instance.StartCoroutine(Eye_of_the_tiger.RemoveFear(actors[i]));
                }


                fearVFX = user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_fear") as GameObject, new Vector3(0,.5f,0), true, false, false);
                user.CurrentStoneGunTimer = 4;
                float dura = 4;
                base.CanBeDropped = false;
                m_speedMod = StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.ADDITIVE, 2.5f);
                m_spaceMod = StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 99);
                user.ownerlessStatModifiers.Add(m_speedMod);
                user.ownerlessStatModifiers.Add(m_spaceMod);
                StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));

            }
            
        }
        public GameObject fearVFX;
        private void ShootSingleProjectile(Vector2 spawnPosition, float angle)
        {
            GameObject gameObject = SpawnManager.SpawnProjectile(this.TigerProj.gameObject, spawnPosition.ToVector3ZUp(spawnPosition.y), Quaternion.Euler(0f, 0f, angle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = this.LastOwner;
            component.Shooter = component.Owner.specRigidbody;
            if (component.Owner is PlayerController)
            {
                PlayerStats stats = (component.Owner as PlayerController).stats;
                component.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
                component.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                component.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                (component.Owner as PlayerController).DoPostProcessProjectile(component);
            }
        }
        public Projectile TigerProj;
        public StatModifier m_speedMod;
        public StatModifier m_spaceMod;
        private static IEnumerator RemoveFear(AIActor aiactor)
        {
            yield return new WaitForSeconds(5f);
            aiactor.behaviorSpeculator.FleePlayerData = null;
            yield break;
        }

        protected void EndEffect(PlayerController user)
        {
            UnityEngine.GameObject.Destroy(fearVFX);
            user.ownerlessStatModifiers.Remove(m_speedMod);
            user.ownerlessStatModifiers.Remove(m_spaceMod);
        }
        private static FleePlayerData fleeData;
    }
}

