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
using System.Net.Sockets;
using System.Threading.Tasks;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Network")]
    [Tooltip("Checks if a server IP and port are available.")]
    public class CheckServerPort : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Server IP or hostname")]
        public FsmString serverIP;

        [RequiredField]
        [Tooltip("Server port")]
        public FsmInt port;

        [Tooltip("Event if server is reachable")]
        public FsmEvent successEvent;

        [Tooltip("Event if server is not reachable")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores true if reachable, false otherwise")]
        public FsmBool isReachable;

        public override void Reset()
        {
            serverIP = null;
            port = 0;
            successEvent = null;
            errorEvent = null;
            isReachable = null;
        }

        public override void OnEnter()
        {
            CheckPortAsync();
        }

        private async void CheckPortAsync()
        {
            bool reachable = false;

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var task = client.ConnectAsync(serverIP.Value, port.Value);
                    var timeoutTask = Task.Delay(3000); // 3 seconds timeout

                    if (await Task.WhenAny(task, timeoutTask) == task && client.Connected)
                        reachable = true;
                }
            }
            catch
            {
                reachable = false;
            }

            if (!isReachable.IsNone)
                isReachable.Value = reachable;

            Fsm.Event(reachable ? successEvent : errorEvent);
            Finish();
        }
    }
}
