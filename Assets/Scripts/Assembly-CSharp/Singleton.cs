using UnityEngine;

public class Singleton<T> : RDBase where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	private static bool applicationIsQuitting = false;

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T)?.ToString() + "' already destroyed on application quit. Won't create again - returning null.\n");
				return null;
			}
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)Object.FindObjectOfType(typeof(T));
					if (Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.\n");
						return _instance;
					}
					if (_instance == null)
					{
						GameObject obj = new GameObject();
						_instance = obj.AddComponent<T>();
						obj.name = "(singleton) " + typeof(T).ToString();
						Object.DontDestroyOnLoad(obj);
					}
				}
				return _instance;
			}
		}
	}

	public void OnDestroy()
	{
		applicationIsQuitting = true;
	}

	public virtual void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
