namespace Unity3dAzure.WebSockets {
  [System.Serializable]
  public class UnityKeyValue {
    public string key;
    public string value;

    public UnityKeyValue (string key, string value) {
      this.key = key;
      this.value = value;
    }
  }
}
