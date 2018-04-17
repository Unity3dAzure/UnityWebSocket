using System;

namespace Unity3dAzure.WebSockets {
  public class WebSocketCloseEventArgs : EventArgs {
    public ushort Code;
    public string Reason;
    public bool WasClean;

    public WebSocketCloseEventArgs(string reason="Unknown", ushort code=0) {
      this.Code = code;
      this.Reason = reason;
      this.WasClean = (code == 1000) ? true : false;
    }
  }
}
