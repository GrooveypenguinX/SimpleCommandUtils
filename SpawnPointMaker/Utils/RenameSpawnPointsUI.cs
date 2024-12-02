using System;
using System.IO;
using System.Reflection;
using EFT;
using EFT.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SimpleCommandUtils.Utils
{
    public class RenameSpawnPointUI : MonoBehaviour
    {
        public static RenameSpawnPointUI Instance;
        public InputField inputField;
        public Button enterButton;
        public Button closeButton;

        public string oldName;
        public GameObject spawnPoint;
        [FormerlySerializedAs("canvasGO")] public GameObject canvasGo;

        private AssetBundle _loadedBundle;
        private bool _isUIBundleLoaded;
        public bool showingRenameUI;

        public void Awake()
        {
            // Ensure there's only one instance of RenameSpawnPointUI
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

        private void Update()
        {
            if (showingRenameUI)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CursorSettings.SetCursor(ECursorType.Idle);
                UIEventSystem.Instance.SetTemporaryStatus(true);
                GamePlayerOwner.IgnoreInputWithKeepResetLook = true;
                GamePlayerOwner.IgnoreInputInNPCDialog = true;

            }
        }

    public void InitializeUI()
    {
        if (_isUIBundleLoaded)
        {
            Console.WriteLine("UI is already loaded.");
            return;
        }

        // Load the embedded resource stream
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "SimpleCommandUtils.Bundles.renameui.bundle"; // Replace with the actual namespace and path
        using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream == null)
            {
                Console.WriteLine($"Failed to find embedded resource: {resourceName}");
                return;
            }

            // Read the resource stream into a byte array
            byte[] bundleData = new byte[resourceStream.Length];
            resourceStream.Read(bundleData, 0, bundleData.Length);

            // Load the asset bundle from memory
            _loadedBundle = AssetBundle.LoadFromMemory(bundleData);

            if (_loadedBundle == null)
            {
                Console.WriteLine($"Failed to load AssetBundle from embedded resource: {resourceName}");
                return;
            }
        }

        // Load the Canvas prefab from the AssetBundle
        GameObject canvasPrefab = _loadedBundle.LoadAsset<GameObject>("RenameSpawnPointUI");

        if (canvasPrefab == null)
        {
            Console.WriteLine("Failed to load Canvas prefab from embedded resource.");
            _loadedBundle.Unload(false);
            return;
        }

        canvasGo = Instantiate(canvasPrefab, gameObject.transform, false);
        canvasGo.name = "RenameSpawnPointUI";

        inputField = canvasGo.transform.Find("RenameInputField").GetComponent<InputField>();
        enterButton = canvasGo.transform.Find("EnterButton").GetComponent<Button>();
        closeButton = canvasGo.transform.Find("CloseButton").GetComponent<Button>();

        if (inputField == null || enterButton == null || closeButton == null)
        {
            Console.WriteLine("Failed to find UI elements in the prefab.");
            _loadedBundle.Unload(false);
            return;
        }

        SetupButtonActions();
        canvasGo.SetActive(false);
        _isUIBundleLoaded = true;
        _loadedBundle.Unload(false);

    #if DEBUG
        Console.WriteLine("Successfully created RenameSpawnPointUI");
    #endif
    }
        private void SetupButtonActions()
        {
#if DEBUG
            Console.WriteLine("Setting up button actions.");
#endif
            enterButton.onClick.AddListener(() => RenameSpawnPoint(inputField.text));
            closeButton.onClick.AddListener(Hide);
#if DEBUG
            Console.WriteLine("Button actions set up.");
#endif
        }

        public void Show(GameObject spawnPointGo)
        {
            if (spawnPointGo == null)
            {
                Console.WriteLine("spawnPointGO is null.");
                return;
            }

            if (!_isUIBundleLoaded)
            {
                InitializeUI();
            }

            if (canvasGo == null)
            {
                Console.WriteLine("canvasGO is null.");
                return;
            }

            if (SimpleCommandUtils.Player == null)
            {
                Console.WriteLine("player is null");
                return;
            }

            InteractableSpawnPointMarker interactableComponent = spawnPointGo.GetComponent<InteractableSpawnPointMarker>();
            interactableComponent.Kill();

            spawnPoint = spawnPointGo;
            oldName = spawnPoint.name;
            inputField.text = oldName;

            canvasGo.SetActive(true);
            showingRenameUI = true;
        }

        public void Hide()
        {
            if (canvasGo != null)
            {
                canvasGo.SetActive(false);
                showingRenameUI = false;

                UIEventSystem.Instance.SetTemporaryStatus(false);
                GamePlayerOwner.IgnoreInputWithKeepResetLook = false;
                GamePlayerOwner.IgnoreInputInNPCDialog = false;
                SimpleCommandUtils.Player.UpdateInteractionCast();
            }
        }

        public void RenameSpawnPoint(string newName)
        {
            if (spawnPoint != null)
            {
                Vector3 position = spawnPoint.transform.position;
                float rotation = spawnPoint.transform.rotation.eulerAngles.y;

                var interactableSpawnPointMarker = spawnPoint.GetComponent<InteractableSpawnPointMarker>();

                // Attempt to remove the old spawn point
                bool removalSuccessful = interactableSpawnPointMarker.RemoveSpawnPoint(oldName);

                // Only proceed if the removal was successful
                if (removalSuccessful)
                {
                    CustomSpawnPointMaker.CreateCustomSpawnPointMarker(newName, position, rotation);
                    Hide();
                }
                else
                {
                    // Handle the failure case, if necessary
                    ConsoleScreen.Log($"Failed to remove spawn point '{oldName}'. Rename aborted.");
                }
            }
        }


        public void DestroyUIElements()
        {
            if (canvasGo != null)
            {
                Destroy(canvasGo);
                canvasGo = null;
            }

            if (_loadedBundle != null)
            {
                _loadedBundle.Unload(true);
                _loadedBundle = null;
            }

            _isUIBundleLoaded = false;
        }

    }

}
