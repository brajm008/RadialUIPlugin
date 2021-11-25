﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.Unmanaged;

namespace RadialUI.Extensions
{
    public static class Mapmenu
    {
        public static void AddItems(this MapMenu map, Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> list, NGuid targetId)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            foreach (var key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onSubmenuSize[key].Item1);
            }
        }
    }
}