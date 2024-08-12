using SCN.Observer;

namespace SCN
{
	public class EventDestroyListener : EventListenerBase
	{
		private void Awake()
		{
			if (listener != null)
			{
				listener(obj: true);
			}
		}

		private void OnDestroy()
		{
			if (listener != null)
			{
				listener(obj: false);
			}
		}
	}
}
