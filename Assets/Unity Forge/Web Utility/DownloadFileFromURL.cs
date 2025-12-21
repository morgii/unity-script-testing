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
using UnityEngine.Networking;
using System.Collections;
using System.IO;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Internet")]
    [Tooltip("Downloads any file from a URL and optionally saves it to a file with its original name and extension.")]
    public class DownloadFileFromURL : FsmStateAction
    {
        // INPUT
        [RequiredField]
        public FsmString url;

        // SAVE OPTIONS
        [Tooltip("Enable to save the downloaded file to persistentDataPath")]
        public FsmBool saveToFile;

        [Tooltip("File name or relative path inside persistentDataPath. If empty, will use original file name from URL.")]
        public FsmString saveFilePath;

        // DOWNLOAD PROGRESS (0 → 1)
        [UIHint(UIHint.Variable)]
        public FsmFloat downloadProgress;

        // EVENTS
        public FsmEvent successEvent;
        public FsmEvent errorEvent;

        public override void Reset()
        {
            url = null;

            saveToFile = false;
            saveFilePath = null;

            downloadProgress = null;

            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(url.Value))
            {
                Debug.LogError("DownloadFileFromURL: URL is empty!");
                Fsm.Event(errorEvent);
                Finish();
                return;
            }

            if (!downloadProgress.IsNone)
                downloadProgress.Value = 0f;

            Fsm.Owner.StartCoroutine(DownloadFile());
        }

        private IEnumerator DownloadFile()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url.Value))
            {
                UnityWebRequestAsyncOperation op = request.SendWebRequest();

                while (!op.isDone)
                {
                    if (!downloadProgress.IsNone)
                        downloadProgress.Value = request.downloadProgress;

                    yield return null;
                }

                if (!downloadProgress.IsNone)
                    downloadProgress.Value = 1f;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (saveToFile.Value)
                    {
                        string fileName;

                        // Use provided saveFilePath, or extract from URL if empty
                        if (!string.IsNullOrEmpty(saveFilePath.Value))
                        {
                            fileName = saveFilePath.Value;
                        }
                        else
                        {
                            // Extract file name from URL
                            fileName = Path.GetFileName(url.Value);
                            if (string.IsNullOrEmpty(fileName))
                                fileName = "downloadedFile";
                        }

                        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

                        // Ensure directory exists only if subfolders are specified
                        string dir = Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        byte[] fileData = request.downloadHandler.data;

                        try
                        {
                            File.WriteAllBytes(fullPath, fileData);
                            Debug.Log($"File saved to: {fullPath}");
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Failed to save file: {e.Message}");
                            Fsm.Event(errorEvent);
                            Finish();
                            yield break;
                        }
                    }

                    Fsm.Event(successEvent);
                }
                else
                {
                    Debug.LogError($"Download failed: {request.error}");
                    Fsm.Event(errorEvent);
                }
            }

            Finish();
        }
    }
}
