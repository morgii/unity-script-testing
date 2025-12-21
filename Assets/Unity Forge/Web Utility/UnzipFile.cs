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
using System.IO;
using System.IO.Compression;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("File")]
    [Tooltip("Unzips a zip file to a target folder.")]
    public class UnzipFile : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Full path to the zip file")]
        public FsmString zipFilePath;

        [RequiredField]
        [Tooltip("Target folder to extract files to")]
        public FsmString extractFolderPath;

        [Tooltip("Event to send if unzip succeeds")]
        public FsmEvent successEvent;

        [Tooltip("Event to send if unzip fails")]
        public FsmEvent errorEvent;

        public override void Reset()
        {
            zipFilePath = null;
            extractFolderPath = null;
            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(zipFilePath.Value) || string.IsNullOrEmpty(extractFolderPath.Value))
            {
                Fsm.Event(errorEvent);
                Finish();
                return;
            }

            try
            {
                if (!File.Exists(zipFilePath.Value))
                {
                    Debug.LogError("Zip file does not exist: " + zipFilePath.Value);
                    Fsm.Event(errorEvent);
                    Finish();
                    return;
                }

                // Ensure target folder exists
                if (!Directory.Exists(extractFolderPath.Value))
                    Directory.CreateDirectory(extractFolderPath.Value);

                // Extract all
                ZipFile.ExtractToDirectory(zipFilePath.Value, extractFolderPath.Value);

                Debug.Log("Zip extracted to: " + extractFolderPath.Value);
                Fsm.Event(successEvent);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unzip failed: " + e.Message);
                Fsm.Event(errorEvent);
            }

            Finish();
        }
    }
}
