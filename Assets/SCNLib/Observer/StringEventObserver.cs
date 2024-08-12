namespace SCN.Observer
{
	internal sealed class StringEventObserver : EventObserver<string, object>
	{
		public void SetLogPrefix(string prefix)
		{
			base.LogPrefix = prefix;
		}
	}
}
