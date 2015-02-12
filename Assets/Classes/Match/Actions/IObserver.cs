namespace Match.Actions {
	public interface IObserver {
		void OnActionStart (IAction action);
		void OnActionEnd   (IAction action);
	}
}