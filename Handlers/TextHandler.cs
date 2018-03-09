
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace UnityWebSocket {
  public class TextHandler : DataHandler {
    // Web Socket handler passes text event args
    public override void OnData(byte[] data) {
      RaiseOnReceivedData(this, new TextEventArgs(Encoding.UTF8.GetString(data)));
    }
  }
}
