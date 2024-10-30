﻿using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using SnivysCustomItems.Items;

namespace SnivysCustomItems
{
    public class Config : IConfig
    {
        [Description("Is plugin enabled?")] 
        public bool IsEnabled { get; set; } = true;

        [Description("Is debug printouts enabled?")]
        public bool Debug { get; set; } = false;

        public List<SmokeGrenade> SmokeGrenades { get; private set; } = new()
        {
            new SmokeGrenade()
        };

        public List<ExplosiveRoundRevolver> ExplosiveRoundRevolvers { get; private set; } = new()
        {
            new ExplosiveRoundRevolver()
        };
        
        public List<NerveAgentGrenade> NerveAgentGrenades { get; set; } = new()
        {
            new NerveAgentGrenade()
        };
    }
}