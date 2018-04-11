using System;

namespace Unity3dAzure.WebSockets {
  public class DataEventArgs : EventArgs {
    public byte[] Data { get; private set; }

    public DataEventArgs (byte[] data) {
      this.Data = data;
    }
  }
}
