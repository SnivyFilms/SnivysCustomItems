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
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

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
                ZoneType playerZone = ev.Player.Zone;
                switch (playerZone)
                {
                    case ZoneType.Surface:
                        ev.Player.Teleport(Room.List.Where(r => r.Type is RoomType.Surface).GetRandomValue());
                        break;
                    case ZoneType.LightContainment:
                        ev.Player.Teleport(Room.List.Where(r => r.Zone is ZoneType.LightContainment).GetRandomValue());
                        break;
                    case ZoneType.HeavyContainment:
                        ev.Player.Teleport(Room.List.Where(r => r.Zone is ZoneType.HeavyContainment).GetRandomValue());
                        break;
                    case ZoneType.Entrance:
                        ev.Player.Teleport(Room.List.Where(r => r.Zone is ZoneType.Entrance).GetRandomValue());
                        break;
                    default:
                        ev.Player.Teleport(Room.List.GetRandomValue());
                        break;
                }
            });
        }
    }
}