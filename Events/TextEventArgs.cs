using System;

namespace UnityWebSocket {
  public class TextEventArgs : EventArgs {
    public string Text { get; private set; }

    public TextEventArgs(string text) {
      this.Text = text;
    }
  }
}
