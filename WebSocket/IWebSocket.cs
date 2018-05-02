using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unity3dAzure.WebSockets {
  public interface IWebSocket {
    void ConfigureWebSocket(string url);
    void ConfigureWebSocket(string url, List<KeyValuePair<string, string>> headers);

    void ConnectAsync();
    void CloseAsync();

    void SendAsync(byte[] data, Action<bool> completed = null);
    void SendAsync(string text, Action<bool> completed = null);

    bool IsConfigured();
    bool IsOpen();
    string Url();

    event OnError OnError;
    event OnOpen OnOpen;
    event OnMessage OnMessage;
    event OnClose OnClose;
  }

  public delegate void OnError(object sender, WebSocketErrorEventArgs e);
  public delegate void OnOpen(object sender, EventArgs e);
  public delegate void OnMessage(object sender, WebSocketMessageEventArgs e);
  public delegate void OnClose(object sender, WebSocketCloseEventArgs e);
}
