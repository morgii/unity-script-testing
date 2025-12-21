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
using System.Net.NetworkInformation;
using UnityEngine.Networking;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Internet")]
    [Tooltip("Pings a server URL or IP address continuously while the 'checkEveryFrame' bool is true.")]
    public class PingCounter : FsmStateAction
    {
        [Tooltip("Server URL to ping (e.g., https://mygame.com)")]
        public FsmString serverURL;

        [Tooltip("IP address to ping (e.g., 8.8.8.8)")]
        public FsmString ipAddress;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the last ping in milliseconds (-1 if failed)")]
        public FsmFloat lastPing;

        [Tooltip("Event to send if ping succeeded")]
        public FsmEvent successEvent;

        [Tooltip("Event to send if ping failed")]
        public FsmEvent errorEvent;

        [Tooltip("Timeout for each ping in milliseconds")]
        public FsmInt timeout;


        [Tooltip("Check ping every frame if true")]
        public FsmBool EveryFrame;

        

        public override void Reset()
        {
            serverURL = null;
            ipAddress = null;
            lastPing = null;
            successEvent = null;
            errorEvent = null;
            EveryFrame = false;
            timeout = 5000;
        }

        public override void OnEnter()
        {
            DoPing();
        }

        public override void OnUpdate()
        {
            if (EveryFrame.Value)
            {
                DoPing();
            }
        }

        private void DoPing()
        {
            Fsm.Owner.StartCoroutine(PingRoutine());
        }

        private IEnumerator PingRoutine()
        {
            float pingMs = -1f;

            // Ping by URL
            if (!string.IsNullOrEmpty(serverURL.Value))
            {
                using (UnityWebRequest request = UnityWebRequest.Head(serverURL.Value))
                {
                    float start = Time.realtimeSinceStartup;
                    request.timeout = Mathf.Max(1, timeout.Value / 1000); // seconds
                    yield return request.SendWebRequest();
                    float end = Time.realtimeSinceStartup;

                    if (request.result == UnityWebRequest.Result.Success)
                        pingMs = (end - start) * 1000f;
                }
            }
            // Ping by IP
            else if (!string.IsNullOrEmpty(ipAddress.Value))
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = null;

                try
                {
                    reply = ping.Send(ipAddress.Value, timeout.Value);
                }
                catch
                {
                    reply = null;
                }

                if (reply != null && reply.Status == IPStatus.Success)
                    pingMs = reply.RoundtripTime;
            }

            if (!lastPing.IsNone)
                lastPing.Value = pingMs;

            if (pingMs >= 0)
                Fsm.Event(successEvent);
            else
                Fsm.Event(errorEvent);
        }
    }
}
