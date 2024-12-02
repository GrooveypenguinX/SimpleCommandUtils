using System;
using System.Collections.Generic;
using System.IO;
using EFT.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TMPro;
using UnityEngine;

namespace SimpleCommandUtils.Utils
{
    public static class CustomSpawnPointMaker
    {
        private static bool _enableSpawnPointMaker;

        public static Dictionary<string, Dictionary<string, SpawnPoint>> CustomSpawnPoints;

        public static Dictionary<string, GameObject> SpawnPointMarkers = new Dictionary<string, GameObject>();

        public static void EnableSpawnPointMaker()
        {
            _enableSpawnPointMaker = true;
#if DEBUG
            ConsoleScreen.Log("Spawn point maker enabled.");
#endif
            CustomSpawnPoints = LoadCustomSpawnPointsJson();

            LoadAndDisplaySpawnPoints();
        }

        public static void DisableSpawnPointMaker()
        {
            _enableSpawnPointMaker = false;
#if DEBUG
            ConsoleScreen.Log("Spawn point maker disabled.");
#endif
            foreach (var marker in SpawnPointMarkers.Values)
            {
                marker.SetActive(false); // Deactivate instead of destroy
            }
#if DEBUG
            ConsoleScreen.Log("All spawn point markers deactivated.");
#endif

        }

        public static Dictionary<string, Dictionary<string, SpawnPoint>> LoadCustomSpawnPointsJson()
        {
            string filePath = Path.Combine(SimpleCommandUtils.PluginPath, "player_spawnpoints.json");

            if (File.Exists(filePath))
            {
#if DEBUG
                ConsoleScreen.Log("Loading custom spawn points from JSON file.");
#endif
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        var jsonSerializer = new JsonSerializer
                        {
                            Converters = { new CustomJsonConverter() },
                            // Handle case-insensitive deserialization
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            }
                        };

                        var spawnPoints = jsonSerializer.Deserialize<Dictionary<string, Dictionary<string, SpawnPoint>>>(jsonReader);

                        if (spawnPoints == null)
                        {
                            ConsoleScreen.Log("Deserialization failed, spawnPoints is null.");
                            return new Dictionary<string, Dictionary<string, SpawnPoint>>();
                        }
#if DEBUG
                        ConsoleScreen.Log("Deserialization successful.");
#endif
                        return spawnPoints;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleScreen.Log($"Error loading JSON file: {ex.Message}");
                    return new Dictionary<string, Dictionary<string, SpawnPoint>>();
                }
            }

            {
#if DEBUG
                ConsoleScreen.Log("Custom spawn points JSON file not found. Creating a new file.");
#endif
                var spawnPoints = new Dictionary<string, Dictionary<string, SpawnPoint>>();
                SaveAllSpawnPoints(spawnPoints);
                return spawnPoints;
            }
        }

        public static void SaveAllSpawnPoints(Dictionary<string, Dictionary<string, SpawnPoint>> spawnPoints)
        {
            string filePath = Path.Combine(SimpleCommandUtils.PluginPath, "player_spawnpoints.json");
            string json = JsonConvert.SerializeObject(spawnPoints, Formatting.Indented);
            File.WriteAllText(filePath, json);
#if DEBUG
            ConsoleScreen.Log("All spawn points saved to JSON file.");
#endif
        }

        public static void CreateCustomSpawnPointMarker(string spawnPointName, Vector3 position, float rotation)
        {
#if DEBUG
            ConsoleScreen.Log($"Attempting to create spawn point marker: {spawnPointName} at position {position} with rotation {rotation}");
#endif
            GameObject cube = CreateCubeMarker(spawnPointName, position, rotation);

            if (cube == null)
            {
                ConsoleScreen.Log("Failed to create cube marker.");
                return;
            }
#if DEBUG
            ConsoleScreen.Log($"Cube marker created successfully: {cube.name}");
#endif
            // Add the spawn point component and register with manager
            var spawnPointComponent = cube.AddComponent<SpawnPointComponent>();
            spawnPointComponent.Initialize(new SpawnPoint(position, rotation));

            if (SpawnPointManager.Instance.IsSpawnPointColliding(cube, 1.0f))
            {
                ConsoleScreen.Log("Spawn point marker is colliding with another marker. Removing.");
                GameObject.Destroy(cube);
                return;
            }

            SpawnPointManager.Instance.RegisterSpawnPoint(cube);

            SpawnPointMarkers[spawnPointName] = cube;

            SaveSpawnPointData(spawnPointName, position, rotation);
#if DEBUG
            ConsoleScreen.Log($"Spawn point '{spawnPointName}' created and saved.");
#endif
        }

        public static GameObject CreateCubeMarker(string spawnPointName, Vector3 position, float rotation)
        {
#if DEBUG
            ConsoleScreen.Log($"Creating cube marker: {spawnPointName} at position {position} with rotation {rotation}");
#endif
            try
            {
                // Create the cube marker
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.GetComponent<Renderer>().enabled = true;
                cube.name = spawnPointName;
                cube.transform.position = position;
                cube.transform.rotation = Quaternion.Euler(0, rotation, 0);

                // Add the spawn point name above the cube using TextMeshPro
                GameObject textObject = new GameObject("SpawnPointLabel");
                TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
                textMeshPro.text = spawnPointName;
                textMeshPro.fontSize = 2; // Set font size to be smaller
                textMeshPro.color = Color.white;
                textMeshPro.alignment = TextAlignmentOptions.Center; // Center text alignment

                // Adjust text object position above the cube
                textObject.transform.position = position + Vector3.up * 1.2f;
                textObject.transform.SetParent(cube.transform);

                // Attach the FaceCameraScript script to make the text face the player
                textObject.AddComponent<FaceCameraScript>();


                cube.AddComponent<InteractableSpawnPointMarker>();
                InteractableSpawnPointMarker interactableSpawnPointMarker = cube.GetComponent<InteractableSpawnPointMarker>();
                interactableSpawnPointMarker.Init();
#if DEBUG
                ConsoleScreen.Log($"Cube marker created successfully: {cube.name}");
#endif
                return cube;
            }
            catch (Exception ex)
            {
                ConsoleScreen.LogError($"Exception in CreateCubeMarker: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public static void SaveSpawnPointData(string spawnPointName, Vector3 position, float rotation)
        {
            string currentMap = UtilityFunctions.GetCurrentMap();
            if (!CustomSpawnPoints.ContainsKey(currentMap))
            {
                CustomSpawnPoints[currentMap] = new Dictionary<string, SpawnPoint>();
            }

            CustomSpawnPoints[currentMap][spawnPointName] = new SpawnPoint(position, rotation);

            SaveAllSpawnPoints(CustomSpawnPoints);
        }

        public static void CreateSpawnPoint(string spawnPointName)
        {
            if (!_enableSpawnPointMaker)
            {
                ConsoleScreen.Log("Spawn point maker is not enabled.");
                return;
            }

            if (SpawnPointManager.Instance == null)
            {
                ConsoleScreen.Log("SpawnPointManager.Instance is not set.");
                return;
            }

            if (SimpleCommandUtils.Player == null)
            {
                ConsoleScreen.Log("Player object is not set.");
                return;
            }

            Vector3 position = UtilityFunctions.GetPlayerLocation();

            float rotation = SimpleCommandUtils.Player.gameObject.transform.rotation.eulerAngles.y;

            string currentMap = UtilityFunctions.GetCurrentMap();

            if (CustomSpawnPoints == null)
            {
#if DEBUG
                ConsoleScreen.Log("Custom spawn points data is not loaded.");
#endif
                CustomSpawnPoints = new Dictionary<string, Dictionary<string, SpawnPoint>>();
            }
            if (CustomSpawnPoints.ContainsKey(currentMap) && CustomSpawnPoints[currentMap].ContainsKey(spawnPointName))
            {
                ConsoleScreen.Log($"Spawn point '{spawnPointName}' already exists.");
                return;
            }

            if (SpawnPointManager.Instance.IsPlayerCollidingWithAnySpawnPoint(UtilityFunctions.GetPlayerLocation(), 1.0f))
            {
                ConsoleScreen.Log("Cannot create a spawn point here, another spawn point marker is too close.");
                return;
            }

            CreateCustomSpawnPointMarker(spawnPointName, position, rotation);

        }

        public static void RemoveSpawnPoint(string spawnPointName)
        {
            string currentMap = UtilityFunctions.GetCurrentMap();

            if (CustomSpawnPoints.ContainsKey(currentMap) && CustomSpawnPoints[currentMap].ContainsKey(spawnPointName))
            {
                CustomSpawnPoints[currentMap].Remove(spawnPointName);
                SpawnPointMarkers.Remove(spawnPointName);
                DestroySpawnPointMarker(spawnPointName);
                SaveAllSpawnPoints(CustomSpawnPoints);
#if DEBUG
                ConsoleScreen.Log($"Spawn point '{spawnPointName}' removed.");
#endif
            }
            else
            {
                ConsoleScreen.Log($"Spawn point '{spawnPointName}' does not exist.");
            }
        }

        public static void DestroySpawnPointMarker(string spawnPointName)
        {
            var spawnPoints = GameObject.FindObjectsOfType<SpawnPointComponent>();
            foreach (var component in spawnPoints)
            {
                if (component.spawnPoint != null && component.gameObject.name == spawnPointName)
                {
#if DEBUG
                    ConsoleScreen.Log($"Destroying spawn point marker '{spawnPointName}'.");
#endif
                    SpawnPointManager.Instance.UnregisterSpawnPoint(component.gameObject);
                    GameObject.Destroy(component.gameObject);
                    break;
                }
            }
        }

        public static void LoadAndDisplaySpawnPoints()
        {
            string currentMap = UtilityFunctions.GetCurrentMap();
#if DEBUG
            ConsoleScreen.Log($"Loading and displaying spawn points for map '{currentMap}'.");
#endif
            try
            {
                if (CustomSpawnPoints.ContainsKey(currentMap))
                {
                    int spawnPointCount = CustomSpawnPoints[currentMap].Count;
#if DEBUG
                    ConsoleScreen.Log($"Found {spawnPointCount} spawn points for map '{currentMap}'.");
#endif
                    foreach (var spawnPoint in CustomSpawnPoints[currentMap])
                    {
                        try
                        {
                            var position = ParsePosition(spawnPoint.Value.Position);
                            var rotation = spawnPoint.Value.Rotation;
                            var spawnPointName = spawnPoint.Key;

                            // Check if the marker already exists in the dictionary
                            if (SpawnPointMarkers.TryGetValue(spawnPointName, out GameObject existingCube))
                            {
#if DEBUG
                                ConsoleScreen.Log($"Found existing visual marker for spawn point '{spawnPointName}'. Turning it on.");
#endif
                                existingCube.SetActive(true);
                            }
                            else
                            {
#if DEBUG
                                ConsoleScreen.Log($"Creating new visual marker for spawn point '{spawnPointName}' at position {position} with rotation {rotation}.");
#endif
                                // Create new visual marker and add it to the dictionary
                                GameObject cube = CreateCubeMarker(spawnPointName, position, rotation);
                                var spawnPointComponent = cube.AddComponent<SpawnPointComponent>();
                                spawnPointComponent.Initialize(new SpawnPoint(position, rotation));
                                SpawnPointManager.Instance.RegisterSpawnPoint(cube);

                                // Add the new marker to the dictionary
                                SpawnPointMarkers[spawnPointName] = cube;
                            }
                        }
                        catch (Exception ex)
                        {
                            ConsoleScreen.LogError($"Error while processing spawn point '{spawnPoint.Key}' for map '{currentMap}': {ex.Message}");
                        }
                    }
                }
                else
                {
                    ConsoleScreen.Log($"No spawn points found for map '{currentMap}'.");
                }
            }
            catch (Exception ex)
            {
                ConsoleScreen.LogError($"Error while loading and displaying spawn points for map '{currentMap}': {ex.Message}");
            }
        }

        public static Vector3 ParsePosition(object positionData)
        {
            try
            {
                float x, y, z;



                // If positionData is a Vector3, use it directly
                if (positionData is Vector3 vector3)
                {
#if DEBUG
                    ConsoleScreen.Log($"Parsed position as Vector3: x={vector3.x}, y={vector3.y}, z={vector3.z}");
#endif
                    return vector3;
                }

                // If positionData is a JObject (i.e., { "x": 58.67002, "y": 0.233123064, "z": 57.4049568 })
                if (positionData is JObject positionObject)
                {
                    x = positionObject["x"]?.Value<float>() ?? throw new FormatException("Missing x value");
                    y = positionObject["y"]?.Value<float>() ?? throw new FormatException("Missing y value");
                    z = positionObject["z"]?.Value<float>() ?? throw new FormatException("Missing z value");
#if DEBUG
                    ConsoleScreen.Log($"Parsed position as object: x={x}, y={y}, z={z}");
#endif
                }
                // If positionData is a JArray (i.e., [674.5625, 5.60742664, 123.641823])
                else if (positionData is JArray positionArray)
                {
                    if (positionArray.Count < 3)
                        throw new FormatException("Array does not contain enough elements");

                    x = positionArray[0].Value<float>();
                    y = positionArray[1].Value<float>();
                    z = positionArray[2].Value<float>();
#if DEBUG
                    ConsoleScreen.Log($"Parsed position as array: x={x}, y={y}, z={z}");
#endif
                }
                else
                {
                    throw new FormatException("Unknown format for Position.");
                }

                return new Vector3(x, y, z);
            }
            catch (Exception ex)
            {
                ConsoleScreen.LogError($"Failed to parse position data: {positionData}. positionData Type {positionData.GetType()}. Error: {ex.Message}");
                throw; // Re-throw to ensure the caller is aware of the failure
            }
        }

    }

}
