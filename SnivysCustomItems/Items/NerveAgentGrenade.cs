using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;
using PlayerAPI = Exiled.API.Features.Player;

namespace SnivysCustomItems.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class NerveAgentGrenade : CustomGrenade
    {
        public override uint Id { get; set; } = 22;
        public override string Name { get; set; } = "Nerve Agent Grenade";
        public override string Description { get; set; } = "Deploys Nerve Agent";
        public override float Weight { get; set; } = 1f;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public float NerveAgentDuration { get; set; } = 25f;
        public float NerveAgentRadius { get; set; } = 5f;
        public float NerveAgentPoisonDuration { get; set; } = 30f;
        public float NerveAgentImmediateDamage { get; set; } = 1f;
        private Vector3 grenadePosition;
        private Pickup pickup;
        private CoroutineHandle nerveAgentHandle;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 1,
                    Location = SpawnLocationType.InsideHid,
                },
            }
        };

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            grenadePosition = ev.Position;
            Scp244 scp244 = (Scp244)Item.Create(ItemType.SCP244a);
            scp244.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            scp244.Primed = true;
            scp244.MaxDiameter = 0f;
            pickup = scp244.CreatePickup(grenadePosition);
            nerveAgentHandle = Timing.RunCoroutine(NerveAgentCoroutine());
            Timing.CallDelayed(NerveAgentDuration, () =>
            {
                Timing.KillCoroutines(nerveAgentHandle);
                pickup.Position += Vector3.down;
                pickup.Position += Vector3.down;
                pickup.Position += Vector3.down;
                pickup.Position += Vector3.down;
                pickup.Position += Vector3.down;
                Timing.CallDelayed(5, () =>
                {
                    scp244.Destroy();
                });
            });
        }

        public IEnumerator<float> NerveAgentCoroutine()
        {
            for (;;)
            {
                foreach (PlayerAPI player in PlayerAPI.List)
                {
                    if(Vector3.Distance(player.Position, grenadePosition) <= NerveAgentRadius)
                    {
                        player.EnableEffect(EffectType.Poisoned, NerveAgentPoisonDuration);
                        player.Hurt(NerveAgentImmediateDamage, DamageType.Poison);
                    }
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}