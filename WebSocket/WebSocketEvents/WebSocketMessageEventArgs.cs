using System;

namespace Unity3dAzure.WebSockets {
  public class WebSocketMessageEventArgs : EventArgs {
    public string Data = null;
    public byte[] RawData = null;
    public bool IsBinary;

    public WebSocketMessageEventArgs(string text) {
      this.Data = text;
      this.IsBinary = false;
    }

    public WebSocketMessageEventArgs(byte[] data) {
      this.RawData = data;
      this.IsBinary = true;
    }
  }
}
