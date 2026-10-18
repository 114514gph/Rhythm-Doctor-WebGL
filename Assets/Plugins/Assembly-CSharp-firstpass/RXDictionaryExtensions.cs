using System.Collections.Generic;

public static class RXDictionaryExtensions
{
	public static bool TryGetValueAs<Key, Value, ValueAs>(this IDictionary<Key, Value> dictionary, Key key, out ValueAs valueAs, ValueAs _default = default(ValueAs)) where ValueAs : Value
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			valueAs = (ValueAs)(object)value;
			return true;
		}
		valueAs = _default;
		return false;
	}
}
