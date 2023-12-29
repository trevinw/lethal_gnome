using BepInEx;

namespace LethalCompanyTemplate
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "numpties.lethal_gnome";
        private const string modName = "Lethal Gnome";
        private const string modVersion = "1.0.0";

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {modGUID} is loaded!");
        }
    }
}