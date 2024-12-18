namespace SimpleWebDash.Monitors
{
	public class Monitor
	{
		public Monitor() { Clock.Tick += OnEvent; }
		public virtual void OnEvent(object sender, ClockTickEventArgs e)
		{
			
		}
	}
}
