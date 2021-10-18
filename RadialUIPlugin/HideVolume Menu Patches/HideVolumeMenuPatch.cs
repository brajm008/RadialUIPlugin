﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.TaleSpire.AssetManagement;
using Bounce.TaleSpire.Physics;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Reflection_Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Hide Volumes
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)>();

        public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolumeItem, bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));
        public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(GMHideVolumeMenuBoardTool), "Begin")]
    internal class HideVolumeMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, HideVolumeItem ____selectedVolume)
        {
            var targetId = ____selectedVolume;
            return true;
        }

        internal static void Postfix(MapMenu map, object obj, HideVolumeItem ____selectedVolume, Vector3 ____selectedPos)
        {
            MapMenu mapMenu = MapMenuManager.OpenMenu(____selectedPos, true);

            var toggleTiles = Reflections.GetMenuAction<GMHideVolumeMenuBoardTool>("ToggleTiles");
            var deleteBlock = Reflections.GetMenuAction<GMHideVolumeMenuBoardTool>("DeleteBlock");

            mapMenu.AddItem(toggleTiles, "Toggle Visibility");
            mapMenu.AddItem(deleteBlock, "Delete", closeMenuOnActivate: true);
        }
    }
}