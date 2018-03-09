using System;

namespace UnityWebSocket {
  public class DataEventArgs : EventArgs {
    public byte[] Data { get; private set; }

    public DataEventArgs(byte[] data) {
      this.Data = data;
    }
  }
}
