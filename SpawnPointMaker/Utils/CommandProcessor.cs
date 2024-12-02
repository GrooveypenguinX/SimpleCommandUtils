#if !UNITY_EDITOR
using EFT.UI;

namespace SimpleCommandUtils.Utils
{
    public class CommandProcessor
    {
        public void RegisterCommandProcessor()
        {
            // Clear console
            ConsoleScreen.Processor.RegisterCommand("clear", delegate
            {
                MonoBehaviourSingleton<PreloaderUI>.Instance.Console.Clear();
            });

            ConsoleScreen.Processor.RegisterCommand("GetPlayerWorldStats", UtilityFunctions.GetPlayerWorldStats);

            ConsoleScreen.Processor.RegisterCommand("EnableCustomSpawnPointMaker", CustomSpawnPointMaker.EnableSpawnPointMaker);

            ConsoleScreen.Processor.RegisterCommand("DisableCustomSpawnPointMaker", CustomSpawnPointMaker.DisableSpawnPointMaker);

            
            ConsoleScreen.Processor.RegisterCommandGroup<AdvancedConsoleCommands>();
        }
    }
}
#endif
