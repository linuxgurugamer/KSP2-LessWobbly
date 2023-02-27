using BepInEx;
using KSP.Game;
using UnityEngine;
using UnityEngine.Serialization;

using System.Collections.Generic;
using System.Linq;
using KSP.IO;
using KSP.Sim.impl;
using System.Linq.Expressions;
using System;

namespace LessWobbly
{
    internal class WobblySettings
    {
        [SerializeField]
        internal float jointRigidity = 1500f;
        [SerializeField]
        internal float jointBreakForceFactor = 50f;

        internal WobblySettings(float jointRigidity, float jointBreakForceFactor)
        {
            this.jointRigidity = jointRigidity;
            this.jointBreakForceFactor = jointBreakForceFactor;
        }

        internal WobblySettings() { }
    }


    [BepInPlugin("io.bepis.lesswobbly", "LessWobbly", "1.0")]
    public class LessWobbly : BaseUnityPlugin
    {
        static bool loadedData = false;
        private bool drawUI = false;
        private Rect windowRect;
        private int windowWidth = 350;
        private int windowHeight = 700;


        private static GUIStyle boxStyle;

        WobblySettings wobblySettings;

        internal void Initialize(WobblySettings lw)
        {
            wobblySettings = new WobblySettings(lw.jointRigidity, lw.jointBreakForceFactor);
        }

        void Awake()
        {
            LoadWobblySettings(false);

            windowRect = new Rect((Screen.width * 0.85f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);
        }

        void OnGUI()
        {
            if (drawUI)
            {
                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    windowRect,
                    FillWindow,
                    "Less Wobbly",
                    GUILayout.Height(0),
                    GUILayout.Width(350));
            }
        }

        public void LoadWobblySettings(bool create)
        {
            bool wobblySettingsFileExists = IOProvider.JsonFileExists(IOProvider.DataLocation.Global, "WobblySetting");
            WobblySettings parsedValue;
            if (wobblySettingsFileExists)
            {
                if (!IOProvider.FromJsonFile<WobblySettings>(IOProvider.DataLocation.Global, "WobblySetting", out parsedValue))
                {
                    parsedValue = new WobblySettings();
                }
            }
            else
            {
                parsedValue = new WobblySettings();
            }
            Initialize(parsedValue);
            //if (create)
            //    SaveData();
        }

        void SaveData()
        {
            WobblySettings parsedValue = new WobblySettings();
            parsedValue.jointBreakForceFactor = wobblySettings.jointBreakForceFactor;
            parsedValue.jointRigidity = wobblySettings.jointRigidity;
            IOProvider.ToJsonFile(IOProvider.DataLocation.Global, "WobblySetting", parsedValue);
        }

        int cnt = 0;
        void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.LOADING && !loadedData && cnt > 10)
            {
                LoadWobblySettings(false);
                try
                {
                    PhysicsSettings.JOINT_RIGIDITY = wobblySettings.jointRigidity;
                    PhysicsSettings.JointBreakForceFactor = wobblySettings.jointBreakForceFactor;
                    loadedData = true;
                }
                catch (NullReferenceException e)
                {
                    cnt = 0;
                }
            }
            if (!loadedData)
                cnt++;

            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.J))
            {
                drawUI = !drawUI;
            }
        }

        private void FillWindow(int windowID)
        {
            boxStyle = GUI.skin.GetStyle("Box");
            GUILayout.BeginVertical();

            GUILayout.Label($"Joint Rigidity: {PhysicsSettings.JOINT_RIGIDITY}");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{wobblySettings.jointRigidity}:", GUILayout.Width(50));
            wobblySettings.jointRigidity = GUILayout.HorizontalSlider(wobblySettings.jointRigidity, 1500f, 1500000f);
            GUILayout.EndHorizontal();

            GUILayout.Label($"Joint Break Force Factor: {PhysicsSettings.JointBreakForceFactor}");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{wobblySettings.jointBreakForceFactor}:", GUILayout.Width(50));
            wobblySettings.jointBreakForceFactor = GUILayout.HorizontalSlider(wobblySettings.jointBreakForceFactor, 10f, 50f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset"))
            {
                wobblySettings = new WobblySettings();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply"))
            {
                PhysicsSettings.JOINT_RIGIDITY = wobblySettings.jointRigidity;
                PhysicsSettings.JointBreakForceFactor = wobblySettings.jointBreakForceFactor;
                SaveData();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }
    }
}
