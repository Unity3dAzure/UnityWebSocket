using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityWebSocket {
  // Drop this script onto a TextMesh gameobject to make it update
  public class TextMeshReceiver : DataReceiver {

    private string text;
    private TextMesh textMesh;
    private bool needsUpdated = false;

    void Awake() {
      textMesh = gameObject.GetComponent<TextMesh>();
    }

    void Update() {
      // update any text components on this gameobject
      if (textMesh != null) {
        textMesh.text = text;
        needsUpdated = false;
      }
    }

    // Override this method in your own subclass to process the received event data
    override public void OnReceivedData(object sender, EventArgs args) {
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
