/*
 * ═══════════════════════════════════════════════════════════════
 *                          UNITY FORGE
 *                   Web Utility Action Package
 * ═══════════════════════════════════════════════════════════════
 * 
 * Author: Unity Forge
 * Github: https://github.com/unityforgedev
 * 
 */

using UnityEngine;
using System.Diagnostics;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Internet")]
    [Tooltip("Opens the persistentDataPath folder in the system file explorer.")]
    public class OpenPersistentDataFolder : FsmStateAction
    {
        // EVENTS
        public FsmEvent successEvent;
        public FsmEvent errorEvent;

        public override void Reset()
        {
            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            string path = Application.persistentDataPath;

            try
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.RevealInFinder(path); // Editor only
#elif UNITY_STANDALONE_WIN
                Process.Start("explorer.exe", path);
#elif UNITY_STANDALONE_OSX
                Process.Start("open", path);
#elif UNITY_STANDALONE_LINUX
                Process.Start("xdg-open", path);
#else
                UnityEngine.Debug.LogWarning("Opening persistentDataPath not supported on this platform.");
#endif
                Fsm.Event(successEvent);
            }
            catch
            {
                Fsm.Event(errorEvent);
            }

            Finish();
        }
    }
}
