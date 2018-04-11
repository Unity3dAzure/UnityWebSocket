using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3dAzure.WebSockets {
  public interface IDataHandler {
    void OnData (byte[] rawData, string data, Boolean isBinary);
  }
}
