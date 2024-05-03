using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using GungeonAPI;

namespace Knives
{
    class Sheila : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Sheila";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/tur_collection/tur_nonmounted_right";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Sheila>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Queen Of All LMGs";
            string longDesc = "Place the turrent mount on the battlefield and interact to commence the Mazza! \n\n" +
                "The turret can not be placed beside walls. \nInteract with the mounting brace to equip sheila. \nDismounting can be done by, getting knocked off the turret, pressing roll, or pressing interact." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 60f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            //Set the rarity of the item



            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            Sheila.BuildPrefab();
            user = player;
            base.Pickup(player);
        }
       
        public override void  DoEffect(PlayerController user)
        {
            
            //walls_check
            RoomHandler room;
            room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(user.CenterPosition, VectorConversions.Round));
            CellData cellaim = room.GetNearestCellToPosition(user.CenterPosition);
            CellData cellaimmunis = room.GetNearestCellToPosition(user.CenterPosition - new Vector2(0,1));

            //if not in or near wall
            if (cellaim.HasWallNeighbor(true,true) == false && cellaimmunis.HasWallNeighbor(true,true) == false)
            {

                //Destroy last instance of object if availible
                if (m_Gun != null)
                {
                    UnityEngine.GameObject.DestroyObject(m_Gun);
                   
                }
                

                //make new gun
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Sheila.Gun, this.LastOwner.transform.position + new Vector3(0.6f, 1.05f, -6f), Quaternion.identity);
                gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(this.LastOwner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
               
                gameObject.GetOrAddComponent<TurretController>();
                m_Gun_position = this.LastOwner.transform.position + new Vector3(0.6f, 1.05f, -6f);
                m_Gun = gameObject;
                AkSoundEngine.PostEvent("Play_tur_place", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_place", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_place", base.gameObject);
                AkSoundEngine.PostEvent("Play_tur_place", base.gameObject);

            }
            else
            {
                this.ClearCooldowns();
            }
        }
        
        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/tur_collection/tur_mounted_up", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            Sheila.Gun = gameObject;

        }
        public bool startup = false;
        public PlayerController user;
        public Gun onturretgun;
        public bool dismount = false;
        
        //update is used as the mount state controller so the code can interact with player item code and interact code at the same time
        //the isOnTurret bool is the interact press state.
        public override void Update()
        {
            if(this.LastOwner != null)
            {

                if (m_Gun != null && m_Gun_position != null)
                {
                    TurretController turrrret = m_Gun.GetComponent<TurretController>();

                    //initializes on turret state
                    if (turrrret.isOnTurret && startup == false)
                    {
                        user.MovementModifiers += this.NoMotionModifier;
                        user.IsStationary = true;
                        user.OnPreDodgeRoll += this.HandleDodgeRollStarted;
                        Gun sheila = user.inventory.AddGunToInventory(PickupObjectDatabase.GetById(Sheila_LMG.ID) as Gun, true);
                        sheila.CanBeDropped = false;
                        sheila.CanBeSold = false;
                        onturretgun = sheila;
                        user.inventory.GunLocked.SetOverride("on turret", true, null);
                        startup = true;
                    }

                    //three sections to control ways to get off turret.

                    //knock off mechanic for explosives and rubberkin
                    if (turrrret.isOnTurret && startup == true)
                    {
                        // calculates knock off by distance from turretbase to player
                        if (Vector2.Distance(user.CenterPosition, m_Gun_position) > 1)
                        {
                            dismount = true;
                        }
                    }

                    // special dodgeroll case for mounted state
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(user.PlayerIDX);
                    if (instanceForPlayer.ActiveActions.DodgeRollAction.IsPressed && turrrret.isOnTurret && startup)
                    {
                        user.IsStationary = false;

                        user.MovementModifiers -= this.NoMotionModifier;
                        dismount = true;
                    }

                    //secondary interact controller that allows for player to use E to exit turret
                    if (turrrret.isOnTurret == false && startup == true)
                    {
                        dismount = true;
                        startup = false;
                    }

                    // essentally a method to control dismount procedures
                    if (dismount)
                    {
                        // dismount controller
                        user.IsStationary = false;
                        user.MovementModifiers -= this.NoMotionModifier;
                        user.OnPreDodgeRoll -= this.HandleDodgeRollStarted;
                        turrrret.isOnTurret = false;
                        dismount = false;
                        if (onturretgun != null)
                        {
                            user.inventory.GunLocked.RemoveOverride("on turret");
                            user.inventory.DestroyGun(onturretgun);
                            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                        }
                    }
                }
            }
            else 
            {
                
            }
           
            base.Update();
        }
      
      
        private void HandleDodgeRollStarted(PlayerController obj)
        {
            dismount = true;
        }
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }
      
       
 
        public override void  OnPreDrop(PlayerController user)
        {
            //ensures no sheila entities exist when owner does not have sheila item
            if (m_Gun != null)
            {


                if (m_Gun != null && m_Gun_position != null)
                {
                    TurretController turrrret = m_Gun.GetComponent<TurretController>();
                    if (turrrret.isOnTurret && startup)
                    {
                        // dismount controller
                        user.IsStationary = false;
                        user.MovementModifiers -= this.NoMotionModifier;
                        user.OnPreDodgeRoll -= this.HandleDodgeRollStarted;
                        turrrret.isOnTurret = false;
                        dismount = false;
                        if (onturretgun != null)
                        {
                            user.inventory.GunLocked.RemoveOverride("on turret");
                            user.inventory.DestroyGun(onturretgun);
                            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                            AkSoundEngine.PostEvent("Play_tur_dismount", base.gameObject);
                        }
                    }

                }

                UnityEngine.GameObject.DestroyObject(m_Gun);
                m_Gun = null;

               
            }
            base.OnPreDrop(user);
        }

       
        public static GameObject Gun;
        public static GameObject m_Gun = null;
        public Vector3 m_Gun_position;
    }
}
