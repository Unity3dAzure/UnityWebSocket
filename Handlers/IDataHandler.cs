using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWebSocket {
  public interface IDataHandler {
    void OnData(byte[] data);
  }
}
