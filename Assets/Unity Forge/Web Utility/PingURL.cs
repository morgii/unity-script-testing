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
    [Tooltip("Pings a URL and returns the latency in milliseconds.")]
    public class PingURL : FsmStateAction
    {
        [RequiredField]
        [Tooltip("URL to ping (e.g., https://www.google.com)")]
        public FsmString url;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the ping result in milliseconds. -1 if failed.")]
        public FsmFloat pingTime;

        [Tooltip("Event to send if ping was successful")]
        public FsmEvent successEvent;

        [Tooltip("Event to send if ping failed")]
        public FsmEvent errorEvent;

        [Tooltip("Timeout in seconds")]
        public FsmFloat timeout;

        public override void Reset()
        {
            url = "https://www.google.com";
            pingTime = null;
            successEvent = null;
            errorEvent = null;
            timeout = 5f;
        }

        public override void OnEnter()
        {
            Fsm.Owner.StartCoroutine(PingCoroutine());
        }

        private IEnumerator PingCoroutine()
        {
            string targetUrl = string.IsNullOrEmpty(url.Value) ? "https://www.google.com" : url.Value;

            using (UnityWebRequest request = UnityWebRequest.Head(targetUrl))
            {
                request.timeout = Mathf.Max(1, (int)timeout.Value);

                float startTime = Time.realtimeSinceStartup;
                yield return request.SendWebRequest();
                float endTime = Time.realtimeSinceStartup;

                bool success = request.result == UnityWebRequest.Result.Success;

                if (!pingTime.IsNone)
                    pingTime.Value = success ? (endTime - startTime) * 1000f : -1f;

                if (success)
                    Fsm.Event(successEvent);
                else
                    Fsm.Event(errorEvent);
            }

            Finish();
        }
    }
}
