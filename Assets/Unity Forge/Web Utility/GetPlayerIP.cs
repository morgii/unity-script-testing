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
using System.Net;
using System.Net.Sockets;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Network")]
    [Tooltip("Gets the player's local or public IP address.")]
    public class GetPlayerIP : FsmStateAction
    {
        [Tooltip("Use local IP (LAN) if true, or public IP if false")]
        public FsmBool useLocalIP;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the IP address as a string")]
        public FsmString storeIP;

        [Tooltip("Event to send if IP is found")]
        public FsmEvent successEvent;

        [Tooltip("Event to send if failed")]
        public FsmEvent errorEvent;

        public override void Reset()
        {
            useLocalIP = true;
            storeIP = null;
            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            string ip = null;

            if (useLocalIP.Value)
            {
                ip = GetLocalIPAddress();
            }
            else
            {
                // Public IP via web request
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        ip = wc.DownloadString("https://api.ipify.org").Trim();
                    }
                }
                catch
                {
                    ip = null;
                }
            }

            if (!string.IsNullOrEmpty(ip))
            {
                if (!storeIP.IsNone)
                    storeIP.Value = ip;

                Fsm.Event(successEvent);
            }
            else
            {
                Fsm.Event(errorEvent);
            }

            Finish();
        }

        private string GetLocalIPAddress()
        {
            string localIP = null;

            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch
            {
                localIP = null;
            }

            return localIP;
        }
    }
}
