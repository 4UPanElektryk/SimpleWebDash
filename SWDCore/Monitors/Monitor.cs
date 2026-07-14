namespace SWDCore.Monitors;

public abstract class Monitor
{
	public Monitor() { Clock.Tick += OnEvent; }
	public virtual void OnEvent(object sender, ClockTickEventArgs e) { }
}
