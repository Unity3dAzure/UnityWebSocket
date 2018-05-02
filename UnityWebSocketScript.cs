using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3dAzure.WebSockets {
  public sealed class UnityWebSocketScript : UnityWebSocket {
    // Unity Inspector fields
    [SerializeField]
    private string webSocketUri = "ws://127.0.0.1:8080";

    [SerializeField]
    private bool AutoConnect = false;

    [Header ("Optional config")]
    [SerializeField]
    private List<UnityKeyValue> headers;

    #region Web Socket connection

    void Start () {
      // Config Websocket
      WebSocketUri = webSocketUri;
      Headers = headers;

      if (AutoConnect) {
        Connect ();
      }
    }

    void OnDisable () {
      Close ();
    }

    #endregion

    #region Web Socket methods

    /*

    public override void Connect () {
      ConnectWebSocket ();
    }

    public override void Close () {
      DisconnectWebSocket ();
    }

    */

    #endregion

    #region Web Socket handlers

    /*

    protected override void OnWebSocketOpen (object sender, EventArgs e) {
      Debug.Log ("Web socket is open");
    }

    protected override void OnWebSocketClose (object sender, WebSocketCloseEventArgs e) {
      Debug.Log ("Web socket closed with reason: " + e.Reason);
      if (!e.WasClean) {
        DisconnectWebSocket ();
      }
      DettachHandlers();
    }

    protected override void OnWebSocketMessage (object sender, WebSocketMessageEventArgs e) {
      Debug.LogFormat ("Web socket {1} message:\n{0}", e.Data, e.IsBinary ? "binary" : "string");
      // Raise web socket data handler event
      if (OnData != null) {
        OnData (e.RawData, e.Data, e.IsBinary);
      }
    }

    protected override void OnWebSocketError (object sender, WebSocketErrorEventArgs e) {
      Debug.LogError ("Web socket error: " + e.Message);
      DisconnectWebSocket ();
    }

    */

    #endregion

  }

}
