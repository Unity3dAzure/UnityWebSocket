using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Unity3dAzure.WebSockets {
  public abstract class UnityWebSocket : MonoBehaviour {
    // Web Socket data delegate
    public delegate void Data (byte[] rawData, string data, Boolean isBinary);
    public static Data OnData;

    protected string WebSocketUri;
    protected List<UnityKeyValue> Headers;

    protected IWebSocket _ws;

    protected bool isAttached = false;

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

    protected virtual void OnWebSocketClose (object sender, WebSocketCloseEventArgs e) {
      Debug.Log ("Web socket closed with reason: " + e.Reason);
      DettachHandlers();
    }

    protected virtual void OnWebSocketMessage (object sender, WebSocketMessageEventArgs e) {
      Debug.LogFormat ("Web socket {1} message:\n{0}", e.Data, e.IsBinary ? "binary" : "string");
      // Raise web socket data handler event
      if (OnData != null) {
        OnData (e.RawData, e.Data, e.IsBinary);
      }
    }
    
    protected virtual void OnWebSocketError (object sender, WebSocketErrorEventArgs e) {
      Debug.LogError ("Web socket error: " + e.Message);
      DisconnectWebSocket ();
    }

#endregion

    public void SendText (string text, Action<bool> callback = null) {
      if (_ws == null || !_ws.IsOpen()) {
        Debug.LogWarning("Web socket is not available to send text message. Try connecting?");
        return;
      }
      _ws.SendAsync (text, callback);
    }

    public void SendUTF8Text (string text, Action<bool> callback = null) {
      byte[] data = Encoding.UTF8.GetBytes (text);
      SendBytes (data, callback);
    }

    public virtual void SendInputText (InputField inputField) {
      SendText (inputField.text);
    }

    public void SendBytes (byte[] data, Action<bool> callback = null) {
      if (_ws == null || !_ws.IsOpen()) {
        Debug.LogWarning("Web socket is not available to send bytes. Try connecting?");
        return;
      }
      _ws.SendAsync (data, callback);
    }

    protected void ConnectWebSocket () {
      if (string.IsNullOrEmpty (WebSocketUri)) {
        Debug.LogError ("WebSocketUri must be set");
        return;
      }

      if (_ws == null || !_ws.IsConfigured()) {
        var customHeaders = new List<KeyValuePair<string, string>>();
        if (Headers != null) {
          foreach (UnityKeyValue header in Headers) {
            customHeaders.Add(new KeyValuePair<string, string>(header.key, header.value));
          }
        }

        Debug.Log ("Create Web Socket: " + WebSocketUri);
#if ENABLE_WINMD_SUPPORT
        Debug.Log ("Using UWP Web Socket");
        _ws = new WebSocketUWP();
#elif UNITY_EDITOR || ENABLE_MONO
        Debug.Log("Using Mono Web Socket");
        _ws = new WebSocketMono();
#endif
        _ws.ConfigureWebSocket(WebSocketUri, customHeaders);
      }

      if (!isAttached) {
        Debug.Log ("Connect Web Socket: " + _ws.Url());
        AttachHandlers();
        _ws.ConnectAsync ();
      }
    }

    protected void DisconnectWebSocket () {
      if (_ws != null && isAttached) {
        Debug.Log ("Disconnect Web Socket");
        _ws.CloseAsync ();
      }
    }

    protected void AttachHandlers() {
      if (isAttached) {
        return;
      }
      isAttached = true;
      _ws.OnError += OnWebSocketError;
      _ws.OnOpen += OnWebSocketOpen;
      _ws.OnMessage += OnWebSocketMessage;
      _ws.OnClose += OnWebSocketClose;
    }

    protected void DettachHandlers() {
      if (!isAttached) {
        return;
      }
      isAttached = false;
      _ws.OnError -= OnWebSocketError;
      _ws.OnOpen -= OnWebSocketOpen;
      _ws.OnMessage -= OnWebSocketMessage;
      _ws.OnClose -= OnWebSocketClose;
    }

  }

}
