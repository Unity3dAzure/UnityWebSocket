using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

// This handler takes the web socket data bytes and raises a received data event.
// This allows us to parse the bytes data in one place and then raise an event to feed game object recievers or a controller to target multiple game objects.
namespace Unity3dAzure.WebSockets {
  public sealed class MessageHandler : DataHandler {

    // Override OnData method in your own subclass to pass any custom event args
    /*
    public override void OnData (byte[] rawData, string data, Boolean isBinary) {
      Debug.LogFormat ("Received {1} data:\n{0}", isBinary ? rawData.Length.ToString () : data, isBinary ? "binary" : "string");
      if (isBinary) {
        RaiseOnReceivedData (this, new DataEventArgs (rawData));
      } else {
        RaiseOnReceivedData (this, new TextEventArgs (data));
      }
    }
    */

  }
}
