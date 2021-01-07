// Coded by Lunar Mods | https://github.com/Lunar-Mods
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using UnityEngine;

namespace Crewcam
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [ReactorPluginSide(PluginSide.ClientOnly)]

    public class CrewcamPlugin : BasePlugin
    {
        public static PlayerControl players;
        public const string Id = "crewcam";
        public static int playerR = 0;
        public static float zoom = 3f;
        public static bool shadows = true;

        public static int count = 0;

        public Harmony Harmony { get; } = new Harmony(Id);


        public override void Load()
        {
            Harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {

            public static void Postfix()
            {
                if (shadows)
                {
                    Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 100), Camera.main.transform.rotation);
                }
                else
                {
                    Camera.main.transform.SetParent(null);
                }

                Camera.main.orthographicSize = zoom;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (count == playerR)
                    {
                        players = player;

                        if (players == PlayerControl.LocalPlayer)
                        {
                            Camera.main.transform.SetParent(null);
                        }
                        else
                        {
                            Camera.main.transform.position = players.transform.position;
                            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 100), Camera.main.transform.rotation);
                        }
                        count = 0;
                        return;
                    }
                    else
                    {
                        count = count + 1;
                        continue;
                    }
                }

            }

        }

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class KeyboardJoystickUpdatePatchPatch
        {
            public static void Postfix()
            {

                if (Input.GetKeyDown(KeyCode.F5)) //Change player
                {
                    playerR = playerR + 1;
                }

                if (Input.GetKeyDown(KeyCode.F6)) //Change zoom
                {
                    zoom = zoom + 1f;
                }

                if (Input.GetKeyDown(KeyCode.F7)) //Change zoom
                {
                    zoom = zoom - 1f;
                }

                if (Input.GetKeyDown(KeyCode.F8)) //Change zoom
                {
                    shadows = !shadows;
                }
            }
        }

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerUpdatePatch
        {
            public static void Postfix(PingTracker __instance)
            {
                //Focused Player
                __instance.text.Centered = true;
                if (players == PlayerControl.LocalPlayer)
                {
                    __instance.text.Text += $"\nFocused: [FF696969FF]{players.nameText.Text} (You)[]";
                }
                else
                {
                    __instance.text.Text += $"\nFocused: [FF696969FF]{players.nameText.Text}[]";
                }

                //Shadows
                __instance.text.Centered = true;
                if (shadows)
                {
                    __instance.text.Text += $"\nShow Shadows: [00FF00FF]{shadows.ToString()}[]";
                }
                else
                {
                    __instance.text.Text += $"\nShow Shadows: [FF0000FF]{shadows.ToString()}[]";
                }

                //Zoom
                __instance.text.Centered = true;
                __instance.text.Text += $"\nZoom: [FF696969FF]{zoom.ToString()}[]";
            }
        }
    }
}
