using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using System;

// This handler takes the web socket data bytes and raises a received data event.
// This allows us to parse the bytes data in one place and then raise an event to feed game object recievers or a controller to target multiple game objects.
namespace UnityWebSocket {
  public abstract class DataHandler : MonoBehaviour, IDataHandler {
    // A game object can implement this received data event to get updates.
    public delegate void ReceivedData(object sender, EventArgs e);
    public static event ReceivedData OnReceivedData;

    // Override this method in your own subclass to pass any custom event args
    public virtual void OnData(byte[] data) {
      Debug.LogFormat("Received data:\n{0}", data.Length);
      RaiseOnReceivedData(this, new DataEventArgs(data));
    }

    protected void RaiseOnReceivedData(object sender, EventArgs e) {
      // Raise the event
      OnReceivedData(this, e);
    }

    #region Unity lifecycle

    // Web Socket data handler
    void OnEnable() {
      UnityWebSocket.OnData += OnData;
    }

    void OnDisable() {
      UnityWebSocket.OnData -= OnData;
    }

    #endregion
  }
}
