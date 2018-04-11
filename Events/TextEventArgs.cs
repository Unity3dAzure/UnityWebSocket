using System;

namespace Unity3dAzure.WebSockets {
  public class TextEventArgs : EventArgs {
    public string Text { get; private set; }

    public TextEventArgs (string text) {
      this.Text = text;
    }
  }
}
