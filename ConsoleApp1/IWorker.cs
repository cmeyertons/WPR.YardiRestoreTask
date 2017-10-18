namespace WPR.YardiRestoreTask
{
	public interface IWorker
	{
		void Work();
		void Cleanup();
	}
}