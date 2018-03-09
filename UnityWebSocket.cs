using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Text;
using UnityEngine.UI;

namespace UnityWebSocket {
  public class UnityWebSocket : MonoBehaviour {
    // Web Socket data delegate
    public delegate void Data(byte[] data);
    public static Data OnData;

    // Unity Inspector fields
    [SerializeField]
    private string webSocketUri = "ws://127.0.0.1:8080";
    [SerializeField]
    private bool AutoConnect = false;

    private WebSocket _ws;
    private bool isReady = false;

    void OnEnable() {
      if (AutoConnect) {
        ConnectWebSocket();
      }
    }

    void OnDisable() {
      DisconnectWebSocket();
    }

    public void Connect() {
      ConnectWebSocket();
    }

    public void Close() {
      DisconnectWebSocket();
    }

    public void SendText(string text) {
      byte[] data = Encoding.UTF8.GetBytes(text);
      SendBytes(data, null);
    }

    public void SendInputText(InputField inputField) {
      SendText(inputField.text);
    }

    public void SendBytes(byte[] data, Action<bool> callback = null) {
      if (_ws == null || _ws.ReadyState != WebSocketSharp.WebSocketState.Open) {
        Debug.LogWarning("Web socket is not available to send message. Try connecting?");
        return;
      }
      _ws.SendAsync(data, callback);
    }

    private void ConnectWebSocket() {
      if (_ws == null) {
        Debug.Log("Create web socket uri: " + webSocketUri);
        _ws = new WebSocket(webSocketUri);
      }

      if (!isReady) {
        Debug.Log("Connect web socket");
        isReady = true;
        _ws.OnError += OnWebSocketError;
        _ws.OnOpen += OnWebSocketOpen;
        _ws.OnMessage += OnWebSocketMessage;
        _ws.OnClose += OnWebSocketClose;
        _ws.ConnectAsync();
      }
    }

    private void DisconnectWebSocket() {
      if (_ws != null && isReady) {
        Debug.Log("Disconnect web socket");
        _ws.CloseAsync();
        _ws.OnError -= OnWebSocketError;
        _ws.OnOpen -= OnWebSocketOpen;
        _ws.OnMessage -= OnWebSocketMessage;
        _ws.OnClose -= OnWebSocketClose;
        isReady = false;
      }
    }

    private void OnWebSocketOpen(object sender, EventArgs e) {
      Debug.Log("Web socket is open");
    }

    private void OnWebSocketClose(object sender, CloseEventArgs e) {
      Debug.Log("Web socket closed with reason: " + e.Reason);
      if (!e.WasClean) {
        DisconnectWebSocket();
      }
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e) {
      Debug.Log("Web socket message:\n" + e.Data);
      // Raise web socket data handler event
      OnData(e.RawData);
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e) {
      Debug.LogError("Web socket error: " + e.Message);
      DisconnectWebSocket();
    }

  }

}

