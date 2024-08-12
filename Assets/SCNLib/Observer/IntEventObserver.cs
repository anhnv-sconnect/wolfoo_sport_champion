namespace SCN.Observer
{
	internal sealed class IntEventObserver : EventObserver<int, object>
	{
		public void SetLogPrefix(string prefix)
		{
			base.LogPrefix = prefix;
		}
	}
}
