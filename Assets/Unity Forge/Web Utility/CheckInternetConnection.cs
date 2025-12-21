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
using System.Collections;
using UnityEngine.Networking;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Internet")]
    [Tooltip("Checks if there is an active internet connection by pinging a URL.")]
    public class CheckInternetConnection : FsmStateAction
    {
        [Tooltip("URL to test the internet connection. Default: https://www.google.com")]
        public FsmString testURL;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optional: stores the result of the internet check (true = connected, false = not connected)")]
        public FsmBool isConnected;

        [Tooltip("Event to send if internet is available")]
        public FsmEvent connectedEvent;

        [Tooltip("Event to send if internet is not available")]
        public FsmEvent disconnectedEvent;

        public override void Reset()
        {
            testURL = "https://www.google.com";
            isConnected = null;
            connectedEvent = null;
            disconnectedEvent = null;
        }

        public override void OnEnter()
        {
            Fsm.Owner.StartCoroutine(CheckConnection());
        }

        private IEnumerator CheckConnection()
        {
            string url = string.IsNullOrEmpty(testURL.Value) ? "https://www.google.com" : testURL.Value;

            using (UnityWebRequest request = UnityWebRequest.Head(url))
            {
                request.timeout = 5; // 5 seconds timeout
                yield return request.SendWebRequest();

                bool result = request.result == UnityWebRequest.Result.Success;

                if (!isConnected.IsNone)
                    isConnected.Value = result;

                if (result)
                    Fsm.Event(connectedEvent);
                else
                    Fsm.Event(disconnectedEvent);
            }

            Finish();
        }
    }
}
