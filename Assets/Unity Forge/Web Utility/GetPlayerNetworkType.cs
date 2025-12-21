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
    [ActionCategory("Network")]
    [Tooltip("Gets the player network type: Wi-Fi, LAN, Mobile, or Unknown.")]
    public class GetPlayerNetworkType : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Stores network type as string")]
        public FsmString storeNetworkType;

        [Tooltip("Event if network type detected")]
        public FsmEvent successEvent;

        [Tooltip("Event if no network detected")]
        public FsmEvent errorEvent;

        public override void Reset()
        {
            storeNetworkType = null;
            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            string networkType = "Unknown";

#if UNITY_EDITOR || UNITY_STANDALONE
            networkType = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? "LAN" : "None";
#elif UNITY_IOS || UNITY_ANDROID
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    networkType = "Wi-Fi";
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    networkType = "Mobile";
                    break;
                default:
                    networkType = "None";
                    break;
            }
#endif

            if (!storeNetworkType.IsNone)
                storeNetworkType.Value = networkType;

            if (networkType != "None")
                Fsm.Event(successEvent);
            else
                Fsm.Event(errorEvent);

            Finish();
        }
    }
}
