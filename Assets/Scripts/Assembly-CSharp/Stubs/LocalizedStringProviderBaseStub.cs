using UnityEngine;

public abstract class LocalizedStringProviderBase
{
    protected virtual bool initialized { get { return true; } }
    protected virtual bool Initialize() { return true; }
    protected virtual bool TryGetLocalizedString(string key, out string value) { value = key; return true; }
    protected virtual string GetLocalizedString(string key) { return key; }
}
