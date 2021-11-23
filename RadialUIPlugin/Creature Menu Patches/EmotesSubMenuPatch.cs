﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Reflection_Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Emotes Submenu Extra Components
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuEmotes = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuEmotes = new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonEmotesSubmenu(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuEmotes.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonEmotesSubmenu(string key) => _onSubmenuEmotes.Remove(key);
        public static void HideDefaultEmotesSubmenuItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuEmotes, key, value, callback);
        public static void UnHideDefaultEmotesSubmenuItem(string key, string value) => RemoveRemoveOn(_removeOnSubmenuEmotes, key, value);

        // Old obsolete Methods
        [Obsolete("This method signature will be replaced with AddCustomButtonEmotesSubmenu on Version 2.1.0.0")]
        public static void AddOnSubmenuEmotes(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuEmotes.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtoEmotesSubmenu on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuEmotes(string key) => _onSubmenuEmotes.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultEmotesSubmenuItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuEmotes(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuEmotes, key, value, callback);
        [Obsolete("This method signature will be replaced with UnHideDefaultEmotesSubmenuItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuEmotes(string key, string value) => RemoveRemoveOn(_removeOnSubmenuEmotes, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Emote_Menu")]
    internal class EmotesSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, Creature ____selectedCreature, List<ActionTimeline> ____emotes, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var CallEmote = Reflections.GetMenuItemAction("CallEmote", __instance);

            for (int index = 0; index < ____emotes.Count; ++index)
            {
                ActionTimeline emote = ____emotes[index];
                if (RadialUIPlugin._removeOnSubmenuEmotes.CanAdd(emote.name, miniId.ToString(), targetId.ToString()))
                    map.AddItem(CallEmote, emote.DisplayName, icon: Icons.GetIconSprite(emote.IconName), obj: emote.name, fadeName: false);
            }
            return false;
        }

        internal static void Postfix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;
            
            foreach (var key in RadialUIPlugin._onSubmenuEmotes.Keys.Where(key => RadialUIPlugin._onSubmenuEmotes[key].Item2 == null || RadialUIPlugin._onSubmenuEmotes[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onSubmenuEmotes[key].Item1);
            }
        }
    }
}