using System;

namespace Unity3dAzure.WebSockets {
  public class WebSocketErrorEventArgs : EventArgs {
    public string Message;

    public WebSocketErrorEventArgs(string error) {
      this.Message = error;
    }
  }
}
