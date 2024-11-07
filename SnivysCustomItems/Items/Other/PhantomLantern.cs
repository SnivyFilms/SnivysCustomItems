using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace SnivysCustomItems.Items.Other
{
    [CustomItem(ItemType.Lantern)]
    public class PhantomLantern : CustomItem
    {
        public override uint Id { get; set; } = 24;
        public override string Name { get; set; } = "Phantom Lantern";
        public override string Description { get; set; } = "'Limbo is no place for a soul like yours'";
        public override float Weight { get; set; } = 0.5f;
        public float EffectDuration { get; set; } = 150f;
        private bool _effectActive = false;
        private CoroutineHandle phantomLanternCoroutine;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHid,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside096,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideGr18,
                },
            },
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Role = RoleTypeId.Scp106
                }
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                }
            }
        };

        protected override void SubscribeEvents()
        {
            PlayerEvent.TogglingFlashlight += UsingFlashlight;
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.InteractingElevator += OnInteractingElevator;
            PlayerEvent.InteractingLocker += OnInteractingLocker;
            PlayerEvent.Interacted += OnInteracted;
            PlayerEvent.ChangingItem += OnChangingItem;
            PlayerEvent.Died += OnDied;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.TogglingFlashlight -= UsingFlashlight;
            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.InteractingElevator -= OnInteractingElevator;
            PlayerEvent.InteractingLocker -= OnInteractingLocker;
            PlayerEvent.Interacted -= OnInteracted;
            PlayerEvent.ChangingItem -= OnChangingItem;
            PlayerEvent.Died -= OnDied;
            base.UnsubscribeEvents();
        }

        private void UsingFlashlight(TogglingFlashlightEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            _effectActive = true;
            ev.Player.EnableEffect(EffectType.Ghostly);
            ev.Player.EnableEffect(EffectType.Invisible);
            ev.Player.EnableEffect(EffectType.FogControl, 5);
            ev.Player.EnableEffect(EffectType.Slowness, 50);
            phantomLanternCoroutine = Timing.RunCoroutine(PhantomLanternCoroutine(ev.Player));
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) 
                return;
            if (!_effectActive)
                return;
            Timing.CallDelayed(.5f, () =>
            {
                ev.Player.EnableEffect(EffectType.Invisible);
            });
        }

        private void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }

        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }
        private void OnInteracted(InteractedEventArgs ev)
        {
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) 
                return;
            if (!_effectActive)
                return;
            ev.IsAllowed = false;
        }
        private void OnDied(DiedEventArgs ev)
        {
            Timing.KillCoroutines(phantomLanternCoroutine);
            _effectActive = false;
        }
        public IEnumerator<float> PhantomLanternCoroutine(PlayerAPI player)
        {
            float durationRemaining = EffectDuration;
            while (durationRemaining > 0)
            {
                player.Stamina = 0;
                durationRemaining -= 1f;
                yield return Timing.WaitForSeconds(1f);
            }
            EndOfEffect(player);
        }

        public void EndOfEffect(PlayerAPI player)
        {
            player.DisableEffect(EffectType.Ghostly);
            player.DisableEffect(EffectType.Invisible);
            player.DisableEffect(EffectType.Slowness);
            player.DisableEffect(EffectType.FogControl);
            player.CurrentItem.Destroy();
            _effectActive = false;
        }
    }
}