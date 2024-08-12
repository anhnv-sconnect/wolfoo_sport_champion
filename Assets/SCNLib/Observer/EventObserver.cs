using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Observer
{
	internal class EventObserver<K, V>
	{
		private readonly Dictionary<K, Dictionary<int, Action<V>>> observerDictionary = new Dictionary<K, Dictionary<int, Action<V>>>();


		protected string LogPrefix
		{
			get;
			set;
		}

		protected void DebugLog(string msg)
		{
				LogPrefix = string.Empty;
		}

		public void RegisterListener(K key, Action<V> eventCallback)
		{
			AddByHashCode(key, eventCallback.GetHashCode(), eventCallback);
		}

		public void RegisterListener(K key, Action eventCallback)
		{
			int hashCode = eventCallback.GetHashCode();
			Action<V> action = delegate
			{
				eventCallback();
			};
			AddByHashCode(key, hashCode, action);
		}

		public void RemoveListener(K key, Action<V> eventCallback)
		{
			RemoveByHashCode(key, eventCallback.GetHashCode());
		}

		public void RemoveListener(K key, Action eventCallback)
		{
			RemoveByHashCode(key, eventCallback.GetHashCode());
		}

		public void RemoveListener(K key)
		{
			RemoveByKey(key);
		}

		public void RemoveAllListener()
		{
			observerDictionary.Clear();
		}

		protected void AddByHashCode(K key, int hashCode, Action<V> action)
		{
	
			if (!observerDictionary.TryGetValue(key, out var value))
			{
				value = new Dictionary<int, Action<V>>();
				observerDictionary[key] = value;
			}
			value[hashCode] = action;
		}

		protected void RemoveByHashCode(K key, int hashCode)
		{
		
			if (observerDictionary.TryGetValue(key, out var value))
			{
				value.Remove(hashCode);
			}
		}

		private void RemoveByKey(K key)
		{
			if (observerDictionary.Remove(key))
			{
				DebugLog($"UnRegister All of Key: {key}");
			}
		}

		public void Dispatch(K key, V obj = default(V))
		{
			if (observerDictionary.TryGetValue(key, out var value))
			{
				Dictionary<int, Action<V>>.ValueCollection values = value.Values;

                foreach (Action<V> item in values)
				{
					if (item != null)
					{
						item(obj);
					}
					else
					{
						DebugLog($"Action {item} has been REMOVED !!!");
					}
                }
            }
		}
	}
}
