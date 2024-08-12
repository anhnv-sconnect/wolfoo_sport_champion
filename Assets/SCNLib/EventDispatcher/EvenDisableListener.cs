using SCN.Observer;

namespace SCN
{
	public class EvenDisableListener : EventListenerBase
	{
		private void OnEnable()
		{
			if (listener != null)
			{
				listener(obj: true);
			}
		}

		private void OnDisable()
		{
			if (listener != null)
			{
				listener(obj: false);
			}
		}
	}
}
