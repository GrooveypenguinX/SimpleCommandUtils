using System;
using System.Collections.Generic;
using EFT.UI;
using UnityEngine;

namespace SimpleCommandUtils.Utils
{
    [Serializable]
    public class SpawnPointData
    {
        public Dictionary<string, SpawnPoint> SpawnPoints = new Dictionary<string, SpawnPoint>();
    }

    [Serializable]
    public class SpawnPoint
    {
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }

        public SpawnPoint(Vector3 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        // Parameterless constructor for JSON deserialization
        public SpawnPoint() { }
    }

    public class SpawnPointComponent : MonoBehaviour
        {
            public SpawnPoint spawnPoint;

            public void Initialize(SpawnPoint thisSpawnPoint)
            {
                spawnPoint = thisSpawnPoint;
            }

            private void OnTriggerEnter(Collider other)
            {
                // Check if the other collider is another spawn point
                if (other.GetComponent<SpawnPointComponent>() != null)
                {
                    ConsoleScreen.Log("Collision with another spawn point marker detected.");
                }
            }
        }

    public class SpawnPointManager : MonoBehaviour
    {
        public static SpawnPointManager Instance { get; private set; }
        private readonly HashSet<GameObject> _spawnPoints = new HashSet<GameObject>();

        private void Awake()
        {
            // Ensure there's only one instance of SpawnPointManager
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterSpawnPoint(GameObject spawnPoint)
        {
            _spawnPoints.Add(spawnPoint);
        }

        public void UnregisterSpawnPoint(GameObject spawnPoint)
        {
            _spawnPoints.Remove(spawnPoint);
        }

        public bool IsPlayerCollidingWithAnySpawnPoint(Vector3 playerPosition, float radius)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                if (Vector3.Distance(spawnPoint.transform.position, playerPosition) < radius)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSpawnPointColliding(GameObject spawnPoint, float radius)
        {
            foreach (var otherSpawnPoint in _spawnPoints)
            {
                if (otherSpawnPoint != spawnPoint && Vector3.Distance(otherSpawnPoint.transform.position, spawnPoint.transform.position) < radius)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
