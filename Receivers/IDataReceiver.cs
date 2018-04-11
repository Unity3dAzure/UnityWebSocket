using System;

namespace Unity3dAzure.WebSockets {
  public interface IDataReceiver {
    void OnReceivedData (object sender, EventArgs args);
  }
}
