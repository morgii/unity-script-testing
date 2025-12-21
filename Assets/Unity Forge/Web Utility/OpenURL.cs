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
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Network")]
    [Tooltip("Opens a URL in the default web browser or application.")]
    public class OpenURL : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Full URL or combined with Base URL + Endpoint")]
        public FsmString url;

        [Tooltip("Base URL (optional)")]
        public FsmString baseUrl;

        [Tooltip("Endpoint path to append to base URL")]
        public FsmString endpointPath;

        [Title("Query Parameters")]
        [Tooltip("Query parameters (format: Key=Value, one per line)")]
        [UIHint(UIHint.TextArea)]
        public FsmString queryParameters;

        [Title("Options")]
        [Tooltip("Validate URL format before opening")]
        public FsmBool validateURL = true;

        [Tooltip("URL encode query parameters automatically")]
        public FsmBool encodeParameters = true;

        [Title("Events")]
        [Tooltip("Event sent when URL opens successfully")]
        public FsmEvent successEvent;

        [Tooltip("Event sent when URL validation fails")]
        public FsmEvent errorEvent;

        [Title("Debug")]
        [Tooltip("Log URL opening to console")]
        public FsmBool logURL = false;

        [Tooltip("Enable verbose debug logging")]
        public FsmBool debugMode = false;

        public override void Reset()
        {
            url = null;
            baseUrl = null;
            endpointPath = null;
            queryParameters = null;
            validateURL = true;
            encodeParameters = true;
            successEvent = null;
            errorEvent = null;
            logURL = false;
            debugMode = false;
        }

        public override void OnEnter()
        {
            OpenURLInBrowser();
            Finish();
        }

        private void OpenURLInBrowser()
        {
            string finalUrl = BuildUrl();

            if (debugMode.Value)
            {
                Debug.Log($"[Open URL] Built URL: {finalUrl}");
            }

            // Validate URL if requested
            if (validateURL.Value)
            {
                if (!IsValidURL(finalUrl))
                {
                    if (logURL.Value || debugMode.Value)
                    {
                        Debug.LogError($"[Open URL] Invalid URL format: {finalUrl}");
                    }
                    Fsm.Event(errorEvent);
                    return;
                }
            }

            // Open URL
            try
            {
                if (logURL.Value || debugMode.Value)
                {
                    Debug.Log($"[Open URL] Opening: {finalUrl}");
                }

                Application.OpenURL(finalUrl);

                if (debugMode.Value)
                {
                    Debug.Log("[Open URL] URL opened successfully");
                }

                Fsm.Event(successEvent);
            }
            catch (System.Exception e)
            {
                if (logURL.Value || debugMode.Value)
                {
                    Debug.LogError($"[Open URL] Error opening URL: {e.Message}");
                }
                Fsm.Event(errorEvent);
            }
        }

        private string BuildUrl()
        {
            string finalUrl = "";

            // Use direct URL or build from base + endpoint
            if (!string.IsNullOrEmpty(url.Value))
            {
                finalUrl = url.Value;
            }
            else
            {
                if (!string.IsNullOrEmpty(baseUrl.Value))
                {
                    finalUrl = baseUrl.Value.TrimEnd('/');
                }

                if (!string.IsNullOrEmpty(endpointPath.Value))
                {
                    string path = endpointPath.Value.TrimStart('/');
                    finalUrl = string.IsNullOrEmpty(finalUrl) ? path : $"{finalUrl}/{path}";
                }
            }

            // Add query parameters if provided
            if (!string.IsNullOrEmpty(queryParameters.Value))
            {
                string queryString = BuildQueryString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    char separator = finalUrl.Contains("?") ? '&' : '?';
                    finalUrl = $"{finalUrl}{separator}{queryString}";
                }
            }

            return finalUrl;
        }

        private string BuildQueryString()
        {
            var parameters = new List<string>();
            string[] lines = queryParameters.Value.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.Contains("="))
                {
                    string[] parts = trimmed.Split(new[] { '=' }, 2);
                    string key = parts[0].Trim();
                    string value = parts.Length > 1 ? parts[1].Trim() : "";

                    if (encodeParameters.Value)
                    {
                        key = UnityEngine.Networking.UnityWebRequest.EscapeURL(key);
                        value = UnityEngine.Networking.UnityWebRequest.EscapeURL(value);
                    }

                    parameters.Add($"{key}={value}");
                }
            }

            return string.Join("&", parameters);
        }

        private bool IsValidURL(string urlString)
        {
            if (string.IsNullOrEmpty(urlString))
            {
                return false;
            }

            // Check if URL starts with a valid protocol
            if (!urlString.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase) &&
                !urlString.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase) &&
                !urlString.StartsWith("ftp://", System.StringComparison.OrdinalIgnoreCase) &&
                !urlString.StartsWith("file://", System.StringComparison.OrdinalIgnoreCase) &&
                !urlString.StartsWith("mailto:", System.StringComparison.OrdinalIgnoreCase))
            {
                if (debugMode.Value)
                {
                    Debug.LogWarning($"[Open URL] URL missing protocol (http://, https://, etc.): {urlString}");
                }
                return false;
            }

            // Try to parse as URI
            try
            {
                System.Uri uri = new System.Uri(urlString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}