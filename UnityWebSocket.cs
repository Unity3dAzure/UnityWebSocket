using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

#if !NETFX_CORE || UNITY_ANDROID
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif

namespace Unity3dAzure.WebSockets {
  public abstract class UnityWebSocket : MonoBehaviour {
    // Web Socket data delegate
    public delegate void Data (byte[] rawData, string data, Boolean isBinary);
    public static Data OnData;

    protected string WebSocketUri;
    protected string Origin;
    protected List<UnityKeyValue> Headers;
    protected uint WaitTime = 0; // seconds

    private WebSocket _ws;
    protected bool isActivated = false;

    #region Web Socket methods

    public virtual void Connect () {
      ConnectWebSocket ();
    }

    public virtual void Close () {
      DisconnectWebSocket ();
    }

    #endregion

    #region Web Socket handlers

    protected virtual void OnWebSocketOpen (object sender, EventArgs e) {
      Debug.Log ("Web socket is open");
    }

    protected virtual void OnWebSocketClose (object sender, CloseEventArgs e) {
      Debug.Log ("Web socket closed with reason: " + e.Reason);
      if (!e.WasClean) {
        DisconnectWebSocket ();
      }
    }

    protected virtual void OnWebSocketMessage (object sender, MessageEventArgs e) {
      Debug.LogFormat ("Web socket {1} message:\n{0}", e.Data, e.IsBinary ? "binary" : "string");
      // Raise web socket data handler event
      if (OnData != null) {
        OnData (e.RawData, e.Data, e.IsBinary);
      }
    }

    protected virtual void OnWebSocketError (object sender, WebSocketSharp.ErrorEventArgs e) {
      Debug.LogError ("Web socket error: " + e.Message);
      DisconnectWebSocket ();
    }

    #endregion

    public void SendText (string text, Action<bool> callback = null) {
      if (_ws == null || _ws.ReadyState != WebSocketSharp.WebSocketState.Open) {
        Debug.LogWarning ("Web socket is not available to send message. Try connecting?");
        return;
      }
      _ws.SendAsync (text, callback);
    }

    public void SendUTF8Text (string text, Action<bool> callback = null) {
      byte[] data = Encoding.UTF8.GetBytes (text);
      SendBytes (data, callback);
    }

    public void SendInputText (InputField inputField) {
      SendText (inputField.text);
    }

    public void SendBytes (byte[] data, Action<bool> callback = null) {
      if (_ws == null || _ws.ReadyState != WebSocketSharp.WebSocketState.Open) {
        Debug.LogWarning ("Web socket is not available to send message. Try connecting?");
        return;
      }
      _ws.SendAsync (data, callback);
    }

    public void SendFile (FileInfo fileInfo, Action<bool> callback = null) {
      if (_ws == null || _ws.ReadyState != WebSocketSharp.WebSocketState.Open) {
        Debug.LogWarning ("Web socket is not available to send message. Try connecting?");
        return;
      }
      _ws.SendAsync (fileInfo, callback);
    }

    protected void ConnectWebSocket () {
      if (string.IsNullOrEmpty (WebSocketUri)) {
        Debug.LogError ("WebSocketUri must be set");
        return;
      }

      if (_ws == null) {
        Debug.Log ("Create web socket uri: " + WebSocketUri);
        _ws = new WebSocket (WebSocketUri);
        // add origin
        if (!string.IsNullOrEmpty (Origin)) {
          _ws.Origin = Origin;
        }
        // set properties
        if (WaitTime > 0) {
          _ws.WaitTime = TimeSpan.FromSeconds (WaitTime);
        }
        // add headers
        if (Headers != null || Headers.Count > 0) {
          var customHeaders = new List<KeyValuePair<string, string>> ();
          foreach (UnityKeyValue header in Headers) {
            customHeaders.Add (new KeyValuePair<string, string> (header.key, header.value));
          }
          _ws.CustomHeaders = customHeaders;
        }
      }

      if (!isActivated) {
        Debug.Log ("Connect web socket");
        isActivated = true;
        _ws.OnError += OnWebSocketError;
        _ws.OnOpen += OnWebSocketOpen;
        _ws.OnMessage += OnWebSocketMessage;
        _ws.OnClose += OnWebSocketClose;
        _ws.ConnectAsync ();
      }
    }

    protected void DisconnectWebSocket () {
      if (_ws != null && isActivated) {
        Debug.Log ("Disconnect web socket");
        _ws.CloseAsync ();
        _ws.OnError -= OnWebSocketError;
        _ws.OnOpen -= OnWebSocketOpen;
        _ws.OnMessage -= OnWebSocketMessage;
        _ws.OnClose -= OnWebSocketClose;
        isActivated = false;
      }
    }

    #region Server Certificate Validation

    protected void ValidateServerCertificate () {
      // required for running in Windows and Android
#if !NETFX_CORE || UNITY_ANDROID
      ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
#endif
    }

    private static string HostName (string url) {
      var match = Regex.Match (url, @"^(https:\/\/|http:\/\/|wss:\/\/|ws:\/\/)(www\.)?([a-z0-9-_]+\.[a-z]+)", RegexOptions.IgnoreCase);
      if (match.Groups.Count == 4 && match.Groups[3].Value.Length > 0) {
        return match.Groups[3].Value;
      }
      return url;
    }

#if !NETFX_CORE || UNITY_ANDROID
    private bool RemoteCertificateValidationCallback (System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
      // Check the certificate to see if it was issued from host
      if (certificate.Subject.Contains (HostName (WebSocketUri))) {
        return true;
      } else {
        return false;
      }
    }
#endif

    #endregion

  }

}
