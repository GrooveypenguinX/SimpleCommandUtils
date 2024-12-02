using System;
using System.Collections.Generic;
using EFT.Interactive;
using EFT.UI;
using UnityEngine;

namespace SimpleCommandUtils.Utils
{
    public class InteractableSpawnPointMarker : InteractableObject
    {
        public readonly List<ActionsTypesClass> Actions = new List<ActionsTypesClass>();

        public void Init()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactive");
            string spawnPointName = gameObject.name;
            GameObject spawnPoint = gameObject;
            Actions.AddRange(
            new List<ActionsTypesClass>
            {
              new ActionsTypesClass
                {
                    Name = "Delete Spawn Point",
                    Action = () => RemoveSpawnPoint(spawnPointName)
                },
                new ActionsTypesClass
                {
                    Name = "Rename Spawn",
                    Action  = () => RenameSpawnPoint(spawnPoint)
                }
            }
            );
        }

        public void RenameSpawnPoint(GameObject spawnPoint)
        {
            if (RenameSpawnPointUI.Instance == null)
            {
               Console.WriteLine("RenameSpawnPointUI.Instance is null. Cannot show rename UI.");
                return;
            }

            if (spawnPoint == null)
            {
                Console.WriteLine("spawnPoint is null. Cannot rename the spawn point.");
                return;
            }

            if (RenameSpawnPointUI.Instance.showingRenameUI)
            {
                return;
            }

            RenameSpawnPointUI.Instance.Show(spawnPoint);
        }

        public bool RemoveSpawnPoint(string spawnPointName)
        {
            string currentMap = UtilityFunctions.GetCurrentMap();

            if (CustomSpawnPointMaker.CustomSpawnPoints.ContainsKey(currentMap)
                && CustomSpawnPointMaker.CustomSpawnPoints[currentMap].ContainsKey(spawnPointName))
            {
                CustomSpawnPointMaker.CustomSpawnPoints[currentMap].Remove(spawnPointName);
                CustomSpawnPointMaker.SpawnPointMarkers.Remove(spawnPointName);
                CustomSpawnPointMaker.SaveAllSpawnPoints(CustomSpawnPointMaker.CustomSpawnPoints);

                DestroySpawnPointMarker(spawnPointName);
#if DEBUG
        ConsoleScreen.Log($"Spawn point '{spawnPointName}' removed.");
#endif
                return true;
            }

            ConsoleScreen.Log($"Spawn point '{spawnPointName}' does not exist.");
            return false;
        }


        public void DestroySpawnPointMarker(string spawnPointName)
        {
            var spawnPoints = FindObjectsOfType<SpawnPointComponent>();
            foreach (var component in spawnPoints)
            {
                if (component.spawnPoint != null && component.gameObject.name == spawnPointName)
                {
#if DEBUG
                    ConsoleScreen.Log($"Destroying spawn point marker '{spawnPointName}'.");
#endif
                    SpawnPointManager.Instance.UnregisterSpawnPoint(component.gameObject);
                    InteractableSpawnPointMarker interactableComponent = component.gameObject.GetComponent<InteractableSpawnPointMarker>();
                    interactableComponent.Kill();
                    Destroy(component.gameObject);
                    SimpleCommandUtils.Player.UpdateInteractionCast();
                    break;
                }
            }
        }
    }
}
