using System;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace SnivysCustomItems
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;
        public override string Name { get; } = "Snivys Custom Items";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVCustomItems";
        public override Version Version { get; } = new Version(1, 2, 5);
        public override Version RequiredExiledVersion { get; } = new Version(8, 14, 0);

        public override void OnEnabled()
        {
            Instance = this;
            CustomItem.RegisterItems(overrideClass: Config);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            Instance = null;
            base.OnDisabled();
        }
    }
}