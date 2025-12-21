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
    [Tooltip("Downloads an image from a URL, stores it as a Texture or Sprite, and optionally saves it to a file with its original name and extension.")]
    public class GetImageFromURL : FsmStateAction
    {
        // INPUT
        [RequiredField]
        public FsmString url;

        // OPTIONS
        public FsmBool createSprite;
        public FsmFloat pixelsPerUnit;

        // SAVE OPTIONS
        [Tooltip("Enable to save the downloaded image to persistentDataPath")]
        public FsmBool saveToFile;

        [Tooltip("File name or relative path inside persistentDataPath (e.g., 'myImage.png'). If empty, will use original file name from URL.")]
        public FsmString saveFilePath;

        // OUTPUT - TEXTURE
        [UIHint(UIHint.Variable)]
        public FsmTexture storeTexture;

        // OUTPUT - SPRITE
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(Sprite))]
        public FsmObject storeSprite;

        // IMAGE INFO
        [UIHint(UIHint.Variable)]
        public FsmInt imageWidth;

        [UIHint(UIHint.Variable)]
        public FsmInt imageHeight;

        // DOWNLOAD PROGRESS (0 → 1)
        [UIHint(UIHint.Variable)]
        public FsmFloat downloadProgress;

        // EVENTS
        public FsmEvent successEvent;
        public FsmEvent errorEvent;

        public override void Reset()
        {
            url = null;

            createSprite = false;
            pixelsPerUnit = 100f;

            saveToFile = false;
            saveFilePath = null;

            storeTexture = null;
            storeSprite = null;
            imageWidth = null;
            imageHeight = null;
            downloadProgress = null;

            successEvent = null;
            errorEvent = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(url.Value))
            {
                Debug.LogError("GetImageFromURL: URL is empty!");
                Fsm.Event(errorEvent);
                Finish();
                return;
            }

            if (!downloadProgress.IsNone)
                downloadProgress.Value = 0f;

            Fsm.Owner.StartCoroutine(DownloadImage());
        }

        private IEnumerator DownloadImage()
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url.Value))
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
                    Texture2D tex = DownloadHandlerTexture.GetContent(request);

                    // Store texture
                    if (!storeTexture.IsNone)
                        storeTexture.Value = tex;

                    // Store image size
                    if (!imageWidth.IsNone)
                        imageWidth.Value = tex.width;

                    if (!imageHeight.IsNone)
                        imageHeight.Value = tex.height;

                    // Create & store sprite
                    if (createSprite.Value && !storeSprite.IsNone)
                    {
                        Sprite sprite = Sprite.Create(
                            tex,
                            new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f),
                            pixelsPerUnit.Value
                        );

                        storeSprite.Value = sprite;
                    }

                    // Save to persistent data folder
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
                            fileName = Path.GetFileName(url.Value.Split('?')[0]); // remove query params
                            if (string.IsNullOrEmpty(fileName))
                                fileName = "downloadedImage.png";
                        }

                        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

                        // Ensure directory exists only if subfolders are specified
                        string dir = Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        byte[] pngData = tex.EncodeToPNG();

                        try
                        {
                            File.WriteAllBytes(fullPath, pngData);
                            Debug.Log($"Image saved to: {fullPath}");
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Failed to save image: {e.Message}");
                            Fsm.Event(errorEvent);
                            Finish();
                            yield break;
                        }
                    }

                    Fsm.Event(successEvent);
                }
                else
                {
                    Debug.LogError($"Image download failed: {request.error}");
                    Fsm.Event(errorEvent);
                }
            }

            Finish();
        }
    }
}
