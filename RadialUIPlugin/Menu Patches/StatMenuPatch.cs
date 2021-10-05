﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Menu_Patches;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Internal DB
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onStatCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, (string,ShouldShowMenu)> _hideStat = new Dictionary<string, (string, ShouldShowMenu)>();

        // Public Accessor
        public static void AddCustomCharacterSubmenuStat(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onStatCallback.Add(key, (value, externalCheck));
        public static bool RemoveCustomCharacterSubmenuStat(string key) => _onStatCallback.Remove(key);

        /// <summary>
        /// Prevent a default stat from appearing
        /// </summary>
        /// <param name="key">Id of plugin that wants to hide stat</param>
        /// <param name="value">0 to 7 (id of stat being hidden)</param>
        /// <param name="callback">Optional callback to hide stat</param>
        public static void HideDefaultCharacterSubmenuStat(string key, string value, ShouldShowMenu callback = null) => _hideStat.Add( key, (value, callback));

        /// <summary>
        /// Re-allows a stat to show (useful for script engine on unpatch)
        /// </summary>
        /// <param name="key">Id of plugin that hid the stat</param>
        /// <returns></returns>
        public static bool ShowDefaultCharacterSubmenuStat(string key) => _hideStat.Remove(key);
    }
}

namespace RadialUI.Menu_Patches
{
    

    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_Stats")]
    internal class StatMenuPatch
    {
        public static bool Show(string menuText, string miniId, string targetId)
        {
            return false;
        }

        internal static bool Prefix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;

            var statNames = CampaignSessionManager.StatNames;
            for (var i = 0; i < statNames.Length; i++)
            {
                if (RadialUIPlugin._hideStat.CanAdd(i.ToString(), miniId.ToString(), targetId.ToString())) map.AddStat(statNames[i], ____selectedCreature.CreatureId, i);
            }

            foreach (var key in RadialUIPlugin._onStatCallback.Keys.Where(key => RadialUIPlugin._onStatCallback[key].Item2 == null || RadialUIPlugin._onStatCallback[key].Item2(miniId,targetId)))
            {
                map.AddItem(RadialUIPlugin._onStatCallback[key].Item1);
            }

            return false;
        }
    }
}
