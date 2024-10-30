using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using Player = Exiled.Events.Handlers.Player;

namespace SnivysCustomItems.Items
{
    [CustomItem(ItemType.GunRevolver)]
    public class ExplosiveRoundRevolver : CustomWeapon
    {
        public override uint Id { get; set; } = 21;
        public override string Name { get; set; } = "Explosive Round Revolver";
        public override string Description { get; set; } = "This revolver fires explosive rounds.";
        public override float Weight { get; set; } = 1f;
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

        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 2;
        public bool UseGrenades { get; set; } = true;
        public float FuseTime { get; set; } = 2.5f;
        public float ScpGrenadeDamageMultiplier { get; set; } = .5f;

        protected override void SubscribeEvents()
        {
            Player.Shot += OnShot;
        }

        protected override void UnsubscribeEvents()
        {
            Player.Shot -= OnShot;
        }
        

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (UseGrenades)
            {
                ev.IsAllowed = false;
                if (!(ev.Player.CurrentItem is Firearm firearm) || firearm.Ammo >= ClipSize)
                    return;
                foreach (Item item in ev.Player.Items.ToList())
                {
                    if (item.Type != ItemType.GrenadeHE)
                        continue;
                    ev.Player.DisableEffect(EffectType.Invisible);
                    ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));
                    Timing.CallDelayed(3f, () => firearm.Ammo = ClipSize);
                    ev.Player.RemoveItem(item);
                    return;
                }
            }
        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.Player.CurrentItem is Firearm firearm)
                firearm.Ammo -= 1;
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = FuseTime;
            grenade.ScpDamageMultiplier = ScpGrenadeDamageMultiplier;
            grenade.SpawnActive(ev.Position);
        }
    }
}