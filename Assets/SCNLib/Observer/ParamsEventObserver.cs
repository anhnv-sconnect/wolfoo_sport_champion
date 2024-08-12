using System;

namespace SCN.Observer
{
	internal sealed class ParamsEventObserver : EventObserver<Type, IEventParams>
	{
		public void RegisterListener<T>(Action<T> eventCallback) where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			Action<IEventParams> action = delegate(IEventParams param)
			{
				eventCallback((T)param);
			};
			AddByHashCode(typeof(T), eventCallback.GetHashCode(), action);
		}

		public void RemoveListener<T>(Action<T> eventCallback) where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			RemoveByHashCode(typeof(T), eventCallback.GetHashCode());
		}

		public void RegisterListener<T>(Action eventCallback) where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			Action<IEventParams> action = delegate
			{
				eventCallback();
			};
			AddByHashCode(typeof(T), eventCallback.GetHashCode(), action);
		}

		public void RemoveListener<T>(Action eventCallback) where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			RemoveByHashCode(typeof(T), eventCallback.GetHashCode());
		}

		public void RemoveListener<T>() where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			RemoveListener(typeof(T));
		}

		public void Dispatch<T>() where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			Dispatch(typeof(T), null);
		}

		public void Dispatch<T>(T param) where T : IEventParams
		{
			base.LogPrefix = typeof(T).FullName;
			Dispatch(typeof(T), param);
		}
	}
}
