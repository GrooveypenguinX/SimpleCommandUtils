#if !UNITY_EDITOR
using System.IO;
using System.Reflection;
using BepInEx;
using Comfort.Common;
using EFT;
using EFT.UI;
using SimpleCommandUtils.Patches;
using SimpleCommandUtils.Utils;
using UnityEngine;

namespace SimpleCommandUtils
{
    [BepInPlugin("com.SimpleCommandUtils.Core", "Simple Command Utils", "0.0.5")]

    internal class SimpleCommandUtils : BaseUnityPlugin
    {
        public static CommandProcessor CommandProcessor;

        private static GameWorld _gameWorld;
        public static Player Player;
        public static HideoutPlayer HideoutPlayer;
        private static GameUI _gameUI;

        public static string PluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal void Awake()
        {
            new GetAvailableActionsPatch().Enable();
            new GameEndedPatch().Enable();
            new GameStartPatch().Enable();
            new ItemSpecificationsPanelModMenuPatch().Enable();
        }
        internal void Start()
        {
            Init();
        }

        internal void Update()
        {
            if (Singleton<GameWorld>.Instantiated && (_gameWorld == null || _gameUI == null || Player == null))
            {
                _gameWorld = Singleton<GameWorld>.Instance;
                _gameUI = MonoBehaviourSingleton<GameUI>.Instance;
                Player = Singleton<GameWorld>.Instance.MainPlayer;
            }
        }

        internal void OnGUI()
        {
        }
        public void Init()
        {
            if (CommandProcessor == null)
            {
                CommandProcessor = new CommandProcessor();
                CommandProcessor.RegisterCommandProcessor();
            }
            InitializeSpawnPointManager();
        }
        public void InitializeSpawnPointManager()
        {
            GameObject spawnPointManagerObject = new GameObject("SpawnPointManager");
            spawnPointManagerObject.AddComponent<SpawnPointManager>();

        }
    }
}
#endif