using EFT.Console.Core;
using EFT.UI;

namespace SimpleCommandUtils.Utils
{
    public class AdvancedConsoleCommands
    {
        [ConsoleCommand("CreateSpawnPoint", "Create a spawn point with the specified name", "<SpawnPointName>")]
        public static void CreateSpawnPoint(string spawnPointName)
        {
            if (string.IsNullOrEmpty(spawnPointName))
            {
                ConsoleScreen.Log("Error: Spawn point name cannot be empty.");
                return;
            }

            CustomSpawnPointMaker.CreateSpawnPoint(spawnPointName);

        }

        [ConsoleCommand("RemoveSpawnPoint", "Remove a spawn point with the specified name", "<SpawnPointName>")]
        public static void RemoveSpawnPoint(string spawnPointName)
        {
            if (string.IsNullOrEmpty(spawnPointName))
            {
                ConsoleScreen.Log("Error: Spawn point name cannot be empty.");
                return;
            }

            CustomSpawnPointMaker.RemoveSpawnPoint(spawnPointName);
#if DEBUG
            ConsoleScreen.Log($"Spawn point '{spawnPointName}' removed successfully.");
#endif
        }
    }
}
