using System;
using UnityEngine;

namespace SCN.Observer
{
	public class EventListenerBase : MonoBehaviour
	{
		protected Action<bool> listener;

		public void SetListener<T>(Action<T> Action) where T : IEventParams
		{
			listener = delegate(bool active)
			{
				if (active)
				{
					EventDispatcher.Instance.RegisterListener(Action);
				}
				else
				{
					EventDispatcher.Instance.RemoveListener(Action);
				}
			};
			listener(obj: true);
		}

		public void SetListener<T>(Action Action) where T : IEventParams
		{
			listener = delegate(bool active)
			{
				if (active)
				{
					EventDispatcher.Instance.RegisterListener<T>(Action);
				}
				else
				{
					EventDispatcher.Instance.RemoveListener<T>(Action);
				}
			};
			listener(obj: true);
		}

		public void SetListener(int key, Action Action)
		{
			listener = delegate(bool active)
			{
				if (active)
				{
					EventDispatcher.Instance.RegisterListener(key, Action);
				}
				else
				{
					EventDispatcher.Instance.RemoveListener(key, Action);
				}
			};
			listener(obj: true);
		}

		public void SetListener(int key, Action<object> Action)
		{
			listener = delegate(bool active)
			{
				if (active)
				{
					EventDispatcher.Instance.RegisterListener(key, Action);
				}
				else
				{
					EventDispatcher.Instance.RemoveListener(key, Action);
				}
			};
			listener(obj: true);
		}

	}
}
