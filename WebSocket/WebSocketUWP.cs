#if ENABLE_WINMD_SUPPORT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using Windows.Web;
using UnityEngine;

namespace Unity3dAzure.WebSockets {
  public class WebSocketUWP : IWebSocket {
    private MessageWebSocket socket;
    private DataWriter dataWriter;
    private Uri uri;

    public event OnError OnError;
    public event OnOpen OnOpen;
    public event OnMessage OnMessage;
    public event OnClose OnClose;

    private bool isAttached = false;
    private bool isOpened = false;

    private string url;
    private List<KeyValuePair<string, string>> headers;

    public void ConfigureWebSocket(string url) {
      ConfigureWebSocket(url, null);
    }

    public void ConfigureWebSocket(string url, List<KeyValuePair<string, string>> headers) {
      if (socket != null) {
        throw new Exception("WebSocket is already configured!");
      }

      this.url = url;
      this.headers = headers;

      socket = new MessageWebSocket();
      uri = WebSocketUri(url);

      if (headers != null) {
        foreach (var header in headers) {
          socket.SetRequestHeader(header.Key, header.Value);
        }
      }
    }

    public async void ConnectAsync() {
      if (socket == null) {
        Debug.Log("Configure MessageWebSocket");
        ConfigureWebSocket(url, headers);
      }
      AttachHandlers();
      try {
        await socket.ConnectAsync(uri);
        dataWriter = new DataWriter(socket.OutputStream);
        isOpened = true;
        RaiseOpen();
      } catch (Exception ex) {
        WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
        if (status.Equals(WebErrorStatus.Unknown)) {
          Debug.LogError("An unknown WebErrorStatus exception occurred.");
        } else {
          RaiseError("Error: MessageWebSocket failed to connect: " + status.ToString());
        }
      }

    }

    public void CloseAsync() {
      if (socket == null) {
        return;
      }
      try {
        isOpened = false;
        socket.Close(1000, "User closed");
      } catch (Exception ex) {
        RaiseError(ex.Message);
      }
    }

    public bool IsConfigured() {
      if (socket == null) {
        return false;
      }
      return true;
    }

    public bool IsOpen() {
      if (socket != null && isOpened) {
        return true;
      }
      return false;
    }

    public string Url() {
      return uri.AbsoluteUri;
    }

    public async void SendAsync(string text, Action<bool> completed = null) {
      await SendAsyncText(text, completed);
    }

    public async void SendAsync(byte[] data, Action<bool> completed = null) {
      await SendAsyncData(data, completed);
    }

    private async Task SendAsyncText(string message, Action<bool> completed = null) {
      try {
        await dataWriter.FlushAsync();
        socket.Control.MessageType = SocketMessageType.Utf8;
        dataWriter.WriteString(message);
        await dataWriter.StoreAsync();
      } catch (Exception ex) {
        WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
        switch (status) {
          case WebErrorStatus.OperationCanceled:
            RaiseError("Write message canceled.");
            break;
          default:
            RaiseError("Error: " + status);
            break;
        }
      }
    }

    private async Task SendAsyncData(byte[] Data, Action<bool> completed = null) {
      try {
        await dataWriter.FlushAsync();
        socket.Control.MessageType = SocketMessageType.Binary;
        dataWriter.WriteBytes(Data);
        await dataWriter.StoreAsync();
      } catch (Exception ex) {
        WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
        switch (status) {
          case WebErrorStatus.OperationCanceled:
            RaiseError("Write message canceled.");
            break;
          default:
            RaiseError("Error: " + status);
            break;
        }
      }
    }

#region WebSocket Handlers

    private void AttachHandlers() {
      if (isAttached) {
        return;
      }
      isAttached = true;
      socket.MessageReceived += HandleOnMessage;
      socket.Closed += HandleOnClose;
    }

    private void UnattachHandlers() {
      if (!isAttached) {
        return;
      }
      isAttached = false;
      if (isOpened) {
        socket.MessageReceived -= HandleOnMessage;
        socket.Closed -= HandleOnClose;
      }
    }

    private void Dispose() {
      dataWriter.Dispose();
      dataWriter = null;
      socket.Dispose();
      socket = null;
      isAttached = false;
      isOpened = false;
    }

    private void RaiseError(string message) {
      if (OnError != null) {
        OnError.Invoke(this, new WebSocketErrorEventArgs(message));
      }
    }

    private void RaiseOpen() {
      if (OnOpen != null) {
        OnOpen.Invoke(this, new EventArgs());
      }
    }

    private void HandleOnMessage(object sender, MessageWebSocketMessageReceivedEventArgs args) {
      if (OnMessage == null) {
        return;
      }
      try {
        using (var reader = args.GetDataReader()) {
          if (args.MessageType == SocketMessageType.Utf8) {
            string text = reader.ReadString(reader.UnconsumedBufferLength);
            OnMessage.Invoke(sender, new WebSocketMessageEventArgs(text));
          } else if (args.MessageType == SocketMessageType.Binary) {
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);
            OnMessage.Invoke(sender, new WebSocketMessageEventArgs(data));
          }
        }
      } catch (Exception ex) {
        WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
        RaiseError(status.ToString() + " " + ex.Message);
      }

    }

    private void HandleOnClose(object sender, WebSocketClosedEventArgs args) {
      if (OnClose != null) {
        OnClose.Invoke(sender, new WebSocketCloseEventArgs(args.Reason, args.Code));
      }
      UnattachHandlers();
      Dispose();
    }

#endregion

    private Uri WebSocketUri(string url) {
      Uri webSocketUri;

      if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out webSocketUri)) {
        throw new Exception("Error: Invalid Web Socket URI");
      }

      if (!webSocketUri.Scheme.Equals("ws") && !webSocketUri.Scheme.Equals("wss")) {
        throw new Exception("Error: Web Socket URI must use 'ws://' or 'wss://' scheme.");
      }

      return webSocketUri;
    }

  }
}

#endif
