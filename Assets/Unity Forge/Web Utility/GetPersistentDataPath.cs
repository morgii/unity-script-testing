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
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("File")]
    [Tooltip("Gets the persistent data path and stores it in a string variable.")]
    public class GetPersistentDataPath : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the persistent data path")]
        public FsmString storePath;

        [Tooltip("Event to send after storing path")]
        public FsmEvent finishEvent;

        public override void Reset()
        {
            storePath = null;
            finishEvent = null;
        }

        public override void OnEnter()
        {
            if (!storePath.IsNone)
                storePath.Value = Application.persistentDataPath;

            if (finishEvent != null)
                Fsm.Event(finishEvent);

            Finish();
        }
    }
}
