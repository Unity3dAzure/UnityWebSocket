using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3dAzure.WebSockets {
  public abstract class DataReceiver : MonoBehaviour, IDataReceiver {

    // Override this method in your own subclass to process the received event data
    virtual public void OnReceivedData (object sender, EventArgs args) {
      Debug.Log ("Hey we got some bytes to do something with!");
    }

    #region Unity lifecycle

    virtual public void OnEnable () {
      DataHandler.OnReceivedData += OnReceivedData;
    }

    virtual public void OnDisable () {
      DataHandler.OnReceivedData -= OnReceivedData;
    }

    #endregion
  }
}
