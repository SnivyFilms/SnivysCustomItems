using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;
using Random = System.Random;

namespace SnivysCustomItems.Items.Injections
{
    [CustomItem(ItemType.Adrenaline)]
    public class DeadringerSyringe : CustomItem
    {
        public override uint Id { get; set; } = 23;
        public override string Name { get; set; } = "Phantom Decoy Device";
        public override string Description { get; set; } = "When injected. You become light headed, which will eventually cause other effects";
        public override float Weight { get; set; } = 1.15f;
        public String OnUseMessage { get; set; } = "You become incredibly light headed";
        public String RagdollDeathReason { get; set; } = "Totally A Intentional Fatal Injection";
        public bool UsableAfterNuke { get; set; } = false;
        public bool TeleportToLightAfterDecom { get; set; } = false;
        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            LockerSpawnPoints = new List<LockerSpawnPoint>()
            {
                new LockerSpawnPoint()
                {
                    Chance = 25,
                    UseChamber = true,
                    Offset = Vector3.zero,
                    Type = LockerType.Misc
                }
            }
        };
        public List<RoomType> ExcludedRooms { get; set; } = new List<RoomType>()
        {
            RoomType.EzShelter,
            RoomType.Lcz173,
            RoomType.Hcz049,
            RoomType.HczNuke,
            RoomType.EzCollapsedTunnel
        };
        protected override void SubscribeEvents()
        {
            Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            if (!UsableAfterNuke && Warhead.IsDetonated)
                return;
            ev.Player.Broadcast(new Exiled.API.Features.Broadcast(OnUseMessage, 3));
            ev.Player.EnableEffect(EffectType.Blinded, 15f, true);
            Timing.CallDelayed(3, () =>
            {
                ev.Player.EnableEffect(EffectType.Flashed, 5f, true);
                ev.Player.EnableEffect(EffectType.Invisible, 5f, true);
                ev.Player.EnableEffect(EffectType.Ensnared, 5f, true);
                ev.Player.EnableEffect(EffectType.Disabled, 60f, true);
                ev.Player.EnableEffect(EffectType.Exhausted, 15f, true);
                ev.Player.EnableEffect(EffectType.AmnesiaItems, 30f, true);
                ev.Player.EnableEffect(EffectType.AmnesiaVision, 30f, true);
                Ragdoll ragdoll = Ragdoll.CreateAndSpawn(ev.Player.Role, ev.Player.Nickname, RagdollDeathReason, ev.Player.Position, ev.Player.ReferenceHub.PlayerCameraReference.rotation);
                Random random = new Random();
                List<Room> rooms = Room.List.Where(room => !ExcludedRooms.Contains(room.Type)).ToList();
                if (rooms.Count > 0)
                {
                     if (!TeleportToLightAfterDecom && Map.DecontaminationState == DecontaminationState.Finish)
                     { 
                         ev.Player.Teleport(Room.List.Where(r => r.Zone is not ZoneType.LightContainment && !ExcludedRooms.Contains(r.Type)).GetRandomValue());
                     }
                     else
                     {
                         Room randomRoom = rooms[random.Next(rooms.Count)];
                         Vector3 teleportPosition = randomRoom.Position + Vector3.up;
                         ev.Player.Position = teleportPosition;
                     }
                }
            });
        }
    }
}