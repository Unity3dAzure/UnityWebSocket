using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity3dAzure.WebSockets {
  // Updates TextMesh component text with received data
  [RequireComponent (typeof (TextMesh))]
  public class TextMeshReceiver : DataReceiver {

    protected string text;
    protected bool needsUpdated = false;

    private TextMesh textMesh;

    void Awake () {
      textMesh = gameObject.GetComponent<TextMesh> ();
    }

    void Update () {
      if (!needsUpdated) {
        return;
      }
      // update TextMesh component on this gameObject
      textMesh.text = text;
      needsUpdated = false;
    }

    // Override this method in your own subclass to process the received event data
    override public void OnReceivedData (object sender, EventArgs args) {
      if (args == null) {
        return;
      }

      // return early if wrong type of EventArgs
      var myArgs = args as TextEventArgs;
      if (myArgs == null) {
        return;
      }

      text = myArgs.Text;
      needsUpdated = true;
    }
  }

}
