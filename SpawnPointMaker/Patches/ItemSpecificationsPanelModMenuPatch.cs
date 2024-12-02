using System.Reflection;
using EFT.UI;
using SPT.Reflection.Patching;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleCommandUtils.Patches
{
    internal class ItemSpecificationsPanelModMenuPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ItemSpecificationPanel).GetMethod("method_13", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        public static void PatchPostfix(ItemSpecificationPanel __instance)
        {
            // Use reflection to access the private _modsContainer field
            var modsContainerField = typeof(ItemSpecificationPanel).GetField("_modsContainer", BindingFlags.Instance | BindingFlags.NonPublic);
            RectTransform modsContainer = (RectTransform)modsContainerField?.GetValue(__instance);

            // Use reflection to access the private layoutElement_0 field
            var layoutElementField = typeof(ItemSpecificationPanel).GetField("layoutElement_0", BindingFlags.Instance | BindingFlags.NonPublic);
            LayoutElement layoutElement = (LayoutElement)layoutElementField?.GetValue(__instance);

            // Assuming you want to increase the rows by changing the division factor in the original calculation

            if (modsContainer == null)
            {
                return;
            }
            int num = Mathf.CeilToInt(modsContainer.childCount / 4f); // Change from 3f to 4f for more rows
            int num3 = Mathf.Max(6, num);

            GridLayoutGroup component2 = modsContainer.GetComponent<GridLayoutGroup>();
            RectTransform rectTransform = modsContainer;
            int num2 = 0;

            // Calculate the width as in the original method
            RectTransform y = __instance.gameObject.RectTransform();
            while (rectTransform != y && !(rectTransform == null))
            {
                LayoutGroup component = rectTransform.GetComponent<LayoutGroup>();
                if (component != null)
                {
                    RectOffset padding = component.padding;
                    num2 += padding.left + padding.right;
                }
                rectTransform = rectTransform.parent.gameObject.RectTransform();
            }

            float minWidth = num2 + component2.cellSize.x * num3 + component2.spacing.x * (num3 - 1);

            if (layoutElement == null)
            {
                return;
            }
            layoutElement.minWidth = minWidth;
        }
    }

}
