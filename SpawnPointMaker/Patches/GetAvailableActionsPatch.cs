using System.Reflection;
using HarmonyLib;
using SimpleCommandUtils.Utils;
using SPT.Reflection.Patching;

namespace SimpleCommandUtils.Patches { 
    internal class GetAvailableActionsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.FirstMethod(typeof(GetActionsClass), x => x.Name == nameof(GetActionsClass.GetAvailableActions) && x.GetParameters()[0].Name == "owner");
    }

    [PatchPrefix]
    public static bool PatchPrefix(object[] __args, ref ActionsReturnClass __result)
    {
        // __args[1] is a GInterface called "interactive", it represents the component that enables interaction

        if (!(__args[1] is InteractableSpawnPointMarker)) return true;

        var customInteractable = __args[1] as InteractableSpawnPointMarker;
        if (customInteractable == null) return true;
        __result = new ActionsReturnClass
        {
            Actions = customInteractable.Actions
        };

        return false;
    }
}
}
