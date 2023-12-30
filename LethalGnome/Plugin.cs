using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace LethalGnomeMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalGnomeModBase : BaseUnityPlugin
    {
        private const string modGUID = "numpties.lethal_gnome";
        private const string modName = "LethalGnome";
        private const string modVersion = "1.0.0";


        private readonly Harmony harmony = new Harmony(modGUID);

        private static LethalGnomeModBase Instance;

        public static double nextTimeToPlayAudio = 0;

        internal static AudioClip GnomeSound;

        private static byte[] GetResourceBytes(string resourceName)
        {
            string name = modName + ".Resources." + resourceName;
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

            if (stream == null)
                return null;

            byte[] array = new byte[stream.Length];

            return stream.Read(array, 0, array.Length) < array.Length ? null : array;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            byte[] resourceBytes = GetResourceBytes("gnome.assets");
            if (resourceBytes == null)
            {
                Logger.LogError($"{modGUID} was unable to load assets from memory! Aborting...");
                return;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(resourceBytes);
            GnomeSound = assetBundle.LoadAsset<AudioClip>("gnome.mp3");
            if (GnomeSound == null)
            {
                Logger.LogError($"{modGUID} was unable to load mp3 asset from bundle. Aboring...");
                return;
            }

            Logger.LogInfo($"Plugin {modGUID} is loaded!");

            harmony.PatchAll();
        }
    }
}