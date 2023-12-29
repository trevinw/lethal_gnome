using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalGnomeMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalGnomeModBase : BaseUnityPlugin
    {
        private const string modGUID = "numpties.lethal_gnome";
        private const string modName = "Lethal Gnome";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static LethalGnomeModBase Instance;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Logger.LogInfo($"Plugin {modGUID} is loaded!");

            harmony.PatchAll(typeof(LethalGnomeModBase));
            // harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}