using HarmonyLib;
using UnityEngine;
using LethalGnomeMod;

[HarmonyPatch]
internal class RoundManagerPatch
{
    private const float timeBetweenSounds = 30f;
    private const float playSoundJitter = 0.5f;

    [HarmonyPatch(typeof(RoundManager), "Update")]
    [HarmonyPostfix]
    public static void Update(ref EnemyVent[] ___allEnemyVents)
    {
        PlayGnomeSoundIfReady(___allEnemyVents);
    }

    public static void PlayGnomeSoundIfReady(EnemyVent[] enemyVents)
    {
        if (RoundManager.Instance == null)
        {
            return;
        }
        if (enemyVents.Length == 0)
        {
            return;
        }
        if (LethalGnomeModBase.nextTimeToPlayAudio == 0)
        {
            LethalGnomeModBase.nextTimeToPlayAudio = GetNextTimeToPlay();
        }
        if (Time.time < LethalGnomeModBase.nextTimeToPlayAudio)
        {
            return;
        }

        AudioSource source = GetAudioSource(enemyVents);
        if (source == null)
        {
            return;
        }
        PlayGnomeSound(source);
        LethalGnomeModBase.nextTimeToPlayAudio = GetNextTimeToPlay();
        Debug.Log($"Playing again at {LethalGnomeModBase.nextTimeToPlayAudio}");
    }

    public static AudioSource GetAudioSource(EnemyVent[] enemyVents)
    {
        if (enemyVents.Length == 0)
        {
            return null;
        }
        return enemyVents[Random.Range(0, enemyVents.Length)].ventAudio;
    }

    public static void PlayGnomeSound(AudioSource source)
    {
        Debug.Log($"Playing gnome sound at {source.transform.position}!");
        // TODO set audibleNoiseID to play over the server
        RoundManager.PlayRandomClip(source, new AudioClip[]{ LethalGnomeModBase.GnomeSound });
        Debug.Log("Gnome sound is played!");
    }

    public static double GetNextTimeToPlay()
    {
        System.Random rng = new System.Random();
        // Take the jitter and multiply by a random variable between -1 and 1
        double jitterMultiplier = playSoundJitter * (rng.NextDouble() * 2 - 1);
        // Get the actual jitter to apply
        double jitterToApply = jitterMultiplier * timeBetweenSounds;
        // Calculate the next time to play the audio with the jitter
        return Time.time + timeBetweenSounds + jitterToApply;
    }
}