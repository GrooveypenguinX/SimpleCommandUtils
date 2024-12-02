using System.Reflection;
using EFT;
using SimpleCommandUtils.Utils;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SimpleCommandUtils.Patches
{
    internal class GameStartPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        public static void PatchPostfix()
        {
            GameObject renameSpawnPointUIGo = new GameObject("RenameSpawnPointUI");
            renameSpawnPointUIGo.AddComponent<RenameSpawnPointUI>();
            RenameSpawnPointUI renameSpawnPointUI = renameSpawnPointUIGo.GetComponent<RenameSpawnPointUI>();
            renameSpawnPointUI.InitializeUI();
        }
    }
}
