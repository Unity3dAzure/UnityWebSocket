#if UNITY_EDITOR || ENABLE_MONO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using WebSocketSharp;

// Implementation for websocket-sharp
// .NET Framework 3.5 or later (including Mono)
namespace Unity3dAzure.WebSockets {
  public class WebSocketMono : IWebSocket {
    private WebSocket socket;

    public event OnOpen OnOpen;
    public event OnError OnError;
    public event OnMessage OnMessage;
    public event OnClose OnClose;

    private bool isAttached = false;

    public WebSocketMono() {
      ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidCertificateCallback);
    }

    public void ConfigureWebSocket(string url) {
      ConfigureWebSocket(url, null);
    }

    public void ConfigureWebSocket(string url, List<KeyValuePair<string, string>> headers) {
      if (socket != null) {
        throw new Exception("WebSocket is already configured!");
      }

      socket = new WebSocket(url);

      // add custom web socket headers
      if (headers != null && headers.Count > 0) {
        socket.CustomHeaders = headers;
      }
    }

    public void ConnectAsync() {
      if (socket == null) {
        Debug.LogError("WebSocket not configured!");
        return;
      }
      AttachHandlers();
      socket.ConnectAsync();
    }

    public void CloseAsync() {
      if (socket == null) {
        return;
      }
      socket.CloseAsync();
    }

    public bool IsConfigured() {
      if (socket == null) {
        return false;
      }
      return true;
    }

    public bool IsOpen() {
      if (socket == null || socket.ReadyState != WebSocketState.Open) {
        return false;
      }
      return true;
    }

    public string Url() {
      return socket.Url.ToString();
    }

    public void SendAsync(byte[] data, Action<bool> completed = null) {
      socket.SendAsync(data, completed);
    }

    public void SendAsync(string text, Action<bool> completed = null) {
      socket.SendAsync(text, completed);
    }

    #region WebSocket Handlers

    private void AttachHandlers() {
      if (isAttached) {
        return;
      }
      isAttached = true;
      socket.OnError += HandleOnError;
      socket.OnOpen += HandleOnOpen;
      socket.OnMessage += HandleOnMessage;
      socket.OnClose += HandleOnClose;
    }

    private void UnattachHandlers() {
      if (!isAttached) {
        return;
      }
      isAttached = false;
      socket.OnError -= HandleOnError;
      socket.OnOpen -= HandleOnOpen;
      socket.OnMessage -= HandleOnMessage;
      socket.OnClose -= HandleOnClose;
    }

    private void Dispose() {
      ((IDisposable) socket).Dispose();
      socket = null;
      isAttached = false;
    }

    private void HandleOnError(object sender, WebSocketSharp.ErrorEventArgs e) {
      if (OnError != null) {
        OnError.Invoke(sender, new WebSocketErrorEventArgs(e.Message));
      }
    }

    private void HandleOnOpen(object sender, EventArgs e) {
      if (OnOpen != null) {
        OnOpen.Invoke(sender, e);
      }
    }

    private void HandleOnMessage(object sender, WebSocketSharp.MessageEventArgs e) {
      if (OnMessage != null) {
        var args = e.IsBinary ? new WebSocketMessageEventArgs(e.RawData) : new WebSocketMessageEventArgs(e.Data);
        OnMessage.Invoke(sender, args);
      }
    }

    private void HandleOnClose(object sender, WebSocketSharp.CloseEventArgs e) {
      if (OnClose != null) {
        OnClose.Invoke(sender, new WebSocketCloseEventArgs(e.Reason, e.Code));
      }
      UnattachHandlers();
      Dispose();
    }

    #endregion

    public bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
      bool valid = true;

      // If there are errors in the certificate chain, look at each error to determine the cause.
      if (sslPolicyErrors != SslPolicyErrors.None) {
        for (int i = 0; i < chain.ChainStatus.Length; i++) {
          if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            bool chainIsValid = chain.Build((X509Certificate2) certificate);
            if (!chainIsValid) {
              valid = false;
            }
          }
        }
      }
      return valid;
    }


  }
}
#endif
