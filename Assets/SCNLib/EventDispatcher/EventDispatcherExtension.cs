using System;
using SCN.Observer;
using UnityEngine;

namespace SCN
{
	public static class EventDispatcherExtension
	{
		public static void RegisterListener(this MonoBehaviour mono, int key, Action<object> action, bool untilDisable = true)
		{
			if (untilDisable)
			{
				GetOrAddComponent<EvenDisableListener>(mono, key, action);
			}
			else
			{
				GetOrAddComponent<EventDestroyListener>(mono, key, action);
			}
		}

		public static void RegisterListener(this MonoBehaviour mono, int key, Action action, bool untilDisable = true)
		{
			if (untilDisable)
			{
				GetOrAddComponent<EvenDisableListener>(mono, key, action);
			}
			else
			{
				GetOrAddComponent<EventDestroyListener>(mono, key, action);
			}
		}

		public static void RegisterListener<T>(this MonoBehaviour mono, Action<T> action, bool untilDisable = true) where T : IEventParams
		{
			if (untilDisable)
			{
				GetOrAddComponent<T, EvenDisableListener>(mono, action);
			}
			else
			{
				GetOrAddComponent<T, EventDestroyListener>(mono, action);
			}
		}

		public static void RegisterListener<T>(this MonoBehaviour mono, Action action, bool untilDisable = true) where T : IEventParams
		{
			if (untilDisable)
			{
				GetOrAddComponent<T, EvenDisableListener>(mono, action);
			}
			else
			{
				GetOrAddComponent<T, EventDestroyListener>(mono, action);
			}
		}

		public static void Dispatch<T>(this MonoBehaviour mono) where T : IEventParams
		{
			EventDispatcher.Instance.Dispatch<T>();
		}

		public static void Dispatch<T>(this MonoBehaviour mono, T para) where T : IEventParams
		{
			EventDispatcher.Instance.Dispatch(para);
		}

		public static void Dispatch(this MonoBehaviour mono, int key)
		{
			EventDispatcher.Instance.Dispatch(key);
		}

		public static void Dispatch(this MonoBehaviour mono, int key, object param)
		{
			EventDispatcher.Instance.Dispatch(key, param);
		}

		private static void GetOrAddComponent<TS>(MonoBehaviour mono, int key, Action<object> action) where TS : EventListenerBase
		{
			GetOrAddComponent<TS>(mono).SetListener(key, action);
		}

		private static void GetOrAddComponent<TS>(MonoBehaviour mono, int key, Action action) where TS : EventListenerBase
		{
			GetOrAddComponent<TS>(mono).SetListener(key, action);
		}

		private static void GetOrAddComponent<T, TS>(MonoBehaviour mono, Action<T> action) where T : IEventParams where TS : EventListenerBase
		{
			GetOrAddComponent<TS>(mono).SetListener(action);
		}

		private static void GetOrAddComponent<T, TS>(MonoBehaviour mono, Action action) where T : IEventParams where TS : EventListenerBase
		{
			GetOrAddComponent<TS>(mono).SetListener<T>(action);
		}

		private static T GetOrAddComponent<T>(MonoBehaviour mono) where T : EventListenerBase
		{
			T val = ((Component)mono).GetComponent<T>();
			if ((UnityEngine.Object)null == (UnityEngine.Object)(object)val)
			{
				val = ((Component)mono).gameObject.AddComponent<T>();
			}
			return val;
		}
	}
}
