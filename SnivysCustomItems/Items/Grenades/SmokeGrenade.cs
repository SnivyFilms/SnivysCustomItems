using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using JetBrains.Annotations;
using UnityEngine;

namespace SnivysCustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class SmokeGrenade : CustomGrenade
    {
        public override uint Id { get; set; } = 20;
        public override string Name { get; set; } = "Obscurus Veil-5";

        public override string Description { get; set; } =
            "This flash is a smoke grenade, when detonated, a smoke cloud will be deployed";

        public override float Weight { get; set; } = 1.15f;

        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 6,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideNukeArmory,
                },
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            var pos = ev.Position;
            Scp244 smoke = (Scp244)Item.Create(ItemType.SCP244a);
            smoke.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            smoke.Primed = true;
            smoke.MaxDiameter = 0;
            smoke.CreatePickup(pos);
        }
    }
}