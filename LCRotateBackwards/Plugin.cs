using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LCRotateBackwards
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new (PluginInfo.PLUGIN_GUID);
        
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    
    [HarmonyPatch]
    class RotationPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ShipBuildModeManager), "Update");
        }
        
        static void Postfix(ref ShipBuildModeManager __instance)
        {
            if (__instance.InBuildMode &&
                (IngamePlayerSettings.Instance.playerInput.actions.FindAction("Sprint", false).IsPressed()) &&
                (IngamePlayerSettings.Instance.playerInput.actions.FindAction("ReloadBatteries", false).IsPressed() ||
                 StartOfRound.Instance.localPlayerUsingController && __instance.playerActions.Movement.InspectItem.IsPressed()))
            {
                var angles = __instance.ghostObject.eulerAngles;
                
                __instance.ghostObject.eulerAngles = new Vector3(
                    angles.x, angles.y - Time.deltaTime * 2 * 155f, angles.z);
            }
        }
    }
}