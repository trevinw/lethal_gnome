using HarmonyLib;
using UnityEngine;
using LethalGnomeMod;
using Unity.Netcode;
using System.Reflection;

[HarmonyPatch]
internal class EnemyVentPatch
{
    [HarmonyPatch(typeof(EnemyVent), "InitializeRPCS_EnemyVent")]
    [HarmonyPostfix]
    internal static void InitializeRPCS_EnemyVent()
    {
        NetworkManager.__rpc_func_table.Add(2024010469u, __rpc_handler_2024010469);
    }
    public static void PlayGnomeSound(EnemyVent vent)
    {
        NetworkManager networkManager = vent.NetworkManager;
        if ((object)networkManager == null || !networkManager.IsListening)
        {
            Debug.Log("network manager not initialized");
            return;
        }
        FieldInfo rpcExecStage = vent.GetType().GetField("__rpc_exec_stage", BindingFlags.NonPublic | BindingFlags.Instance);
        if ((int)rpcExecStage.GetValue(vent) != /* __RpcExecStage.Client */ 2 && (networkManager.IsServer || networkManager.IsHost))
        {
            Debug.Log("Is server");
            ClientRpcParams clientRpcParams = default(ClientRpcParams);
            MethodInfo beginRPC = vent.GetType().GetMethod("__beginSendClientRpc", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Log("Send RPC!");
            FastBufferWriter bufferWriter = (FastBufferWriter)beginRPC.Invoke(vent, new object[] { 2024010469u, clientRpcParams, RpcDelivery.Reliable });
            MethodInfo endRPC = vent.GetType().GetMethod("__endSendClientRpc", BindingFlags.NonPublic | BindingFlags.Instance);
            endRPC.Invoke(vent, new object[] { bufferWriter, 2024010469u, clientRpcParams, RpcDelivery.Reliable });
            Debug.Log("RPC sent!");
        }
        if ((int)rpcExecStage.GetValue(vent) == /* __RpcExecStage.Client */ 2 && (networkManager.IsClient || networkManager.IsHost))
        {
            Debug.Log("Is client");
            RoundManager.PlayRandomClip(vent.ventAudio, new AudioClip[] { LethalGnomeModBase.GnomeSound });
        }
    }

    public static void __rpc_handler_2024010469(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
    {
        NetworkManager networkManager = target.NetworkManager;
        if ((object)networkManager != null && networkManager.IsListening)
        {
            Debug.Log("RPC in!");
            FieldInfo rpcExecStage = target.GetType().GetField("__rpc_exec_stage", BindingFlags.NonPublic | BindingFlags.Instance);
            rpcExecStage.SetValue(target, /*NetworkBehaviour.__RpcExecStage.Client*/ 2);
            PlayGnomeSound((EnemyVent) target);
            rpcExecStage.SetValue(target, /*NetworkBehaviour.__RpcExecStage.None*/ 0);
        }
    }
}