using System;

namespace UnityWebSocket {
  public interface IDataReceiver {
    void OnReceivedData(object sender, EventArgs args);
  }
}
