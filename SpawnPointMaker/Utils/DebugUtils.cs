#if !UNITY_EDITOR
using Dissonance;
using EFT;
using EFT.UI;
using UnityEngine;

namespace SimpleCommandUtils.Utils
{
    public static class UtilityFunctions
    {
        public static string GetCurrentMap()
        {
            string map = SimpleCommandUtils.Player.Location;
#if DEBUG
            ConsoleScreen.Log($"Current map ID retrieved: {map}");
#endif
            return map;
        }

        public static void GetPlayerWorldStats()
        {
            if (SimpleCommandUtils.Player != null)
            {
                LogPlayerStats("Player", SimpleCommandUtils.Player);
            }
            else if (SimpleCommandUtils.HideoutPlayer != null)
            {
                LogPlayerStats("Hideout Player", SimpleCommandUtils.HideoutPlayer);
            }
            else
            {
                ConsoleScreen.Log("Player is null. You aren't in raid or hideout.");
            }
        }

        private static void LogPlayerStats(string playerType, Player player)
        {
            // Accessing UnityEngine.Vector3 Position from the Player class
            Vector3 position = ((IDissonancePlayer)player).Position;
            ConsoleScreen.Log($"{playerType} Position X: {position.x} Y: {position.y} Z: {position.z}");
    
            // Unity Transform rotation
            var rotation = player.gameObject.transform.rotation.eulerAngles;
            ConsoleScreen.Log($"{playerType} Rotation X: {rotation.x} Y: {rotation.y} Z: {rotation.z}");
        }


        public static Vector3 GetPlayerLocation()
        {
            // Access Player.Position using a local variable
            Player currentPlayer = SimpleCommandUtils.Player;
            Vector3 location = ((IDissonancePlayer)currentPlayer).Position; // This avoids any confusion
#if DEBUG
            ConsoleScreen.Log($"Player location retrieved: {location}");
#endif
            return location;
        }

    
    }
}

#endif