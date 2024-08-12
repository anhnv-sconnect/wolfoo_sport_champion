using System;
using SCN.Observer;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SCN
{
	public class EventDispatcher : Singleton<EventDispatcher>
	{
	
		private IntEventObserver intEventObserver;

		private ParamsEventObserver paramsEventObserver;

		private IntEventObserver IntEventObserver
		{
			get
			{
				if (intEventObserver != null)
				{
					return intEventObserver;
				}
				intEventObserver = new IntEventObserver();
				return intEventObserver;
			}
		}

		private ParamsEventObserver ParamsEventObserver
		{
			get
			{
				if (paramsEventObserver != null)
				{
					return paramsEventObserver;
				}
				paramsEventObserver = new ParamsEventObserver();
				return paramsEventObserver;
			}
		}

		public void RemoveAllListener()
		{
			if (paramsEventObserver != null)
			{
				paramsEventObserver.RemoveAllListener();
			}
			GC.Collect();
		}


		public void Dispatch(int key, object value = null)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.Dispatch(key, value);
		}

		public void RegisterListener(int key, Action<object> eventCallback)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.RegisterListener(key, eventCallback);
		}

		public void RegisterListener(int key, Action eventCallback)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.RegisterListener(key, eventCallback);
		}

		public void RemoveListener(int key, Action<object> eventCallback)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.RemoveListener(key, eventCallback);
		}

		public void RemoveListener(int key, Action eventCallback)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.RemoveListener(key, eventCallback);
		}

		public void RemoveListener(int key)
		{
			IntEventObserver.SetLogPrefix(key.ToString());
			IntEventObserver.RemoveListener(key);
		}

		public void Dispatch<T>() where T : IEventParams
		{
			ParamsEventObserver.Dispatch<T>();
		}

		public void Dispatch<T>(T eventParams) where T : IEventParams
		{
			ParamsEventObserver.Dispatch(eventParams);
		}

		public void RegisterListener<T>(Action<T> eventCallback) where T : IEventParams
		{
			ParamsEventObserver.RegisterListener(eventCallback);
		}

		public void RegisterListener<T>(Action eventCallback) where T : IEventParams
		{
			ParamsEventObserver.RegisterListener<T>(eventCallback);
		}

		public void RemoveListener<T>(Action<T> eventCallback) where T : IEventParams
		{
			ParamsEventObserver.RemoveListener(eventCallback);
		}

		public void RemoveListener<T>(Action eventCallback) where T : IEventParams
		{
			ParamsEventObserver.RemoveListener<T>(eventCallback);
		}

		public void RemoveListener<T>() where T : IEventParams
		{
			ParamsEventObserver.RemoveListener<T>();
		}
	}
}
