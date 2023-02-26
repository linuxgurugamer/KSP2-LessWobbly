using BepInEx;
using KSP.Game;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using KSP.IO;
using KSP.Sim.impl;

namespace LazyOrbit
{
    [BepInPlugin("io.bepis.lesswobbly", "LessWobbly", "1.0")]
    public class LessWobbly : BaseUnityPlugin
    {
        static bool loaded = false;
        private bool drawUI = false;
        private Rect windowRect;
        private int windowWidth = 350;
        private int windowHeight = 700;


        private static GUIStyle boxStyle;

        float lessWobbly = 1500f;

        public  void Initialize()
        {
            if (loaded)
            {
                Destroy(this);
            }

            loaded = true;
            //lessWobbly = PhysicsSettings.JOINT_RIGIDITY;
        }

        void Awake()
        {
            Initialize();

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
                    "Lazy Orbit",
                    GUILayout.Height(0),
                    GUILayout.Width(350));
            }
        }

        void Update()
        {
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.J))
                drawUI = !drawUI;
        }

        private void FillWindow(int windowID)
        {
            boxStyle = GUI.skin.GetStyle("Box");
            GUILayout.BeginVertical();

            GUILayout.Label($"Joint Rigidity: {PhysicsSettings.JOINT_RIGIDITY}");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{lessWobbly}:");
            lessWobbly = GUILayout.HorizontalSlider(lessWobbly, 1500f, 1500000f);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply"))
                PhysicsSettings.JOINT_RIGIDITY = lessWobbly;

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }
    }
}
