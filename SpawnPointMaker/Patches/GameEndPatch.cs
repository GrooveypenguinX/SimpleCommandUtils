using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SimpleCommandUtils.Utils;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;

namespace SimpleCommandUtils.Patches
{
    internal class GameEndedPatch : ModulePatch
    {
        private static Type _targetClassType;

        protected override MethodBase GetTargetMethod()
        {
            _targetClassType = PatchConstants.EftTypes.Single(targetClass =>
                !targetClass.IsInterface &&
                !targetClass.IsNested &&
                targetClass.GetMethods().Any(method => method.Name == "LocalRaidEnded") &&
                targetClass.GetMethods().Any(method => method.Name == "ReceiveInsurancePrices")
            );

            return AccessTools.Method(_targetClassType.GetTypeInfo(), "LocalRaidEnded");
        }

        [PatchPostfix]
        public static void Postfix()
        {
            RenameSpawnPointUI.Instance.DestroyUIElements();
        }
    }

}
