namespace WPR.YardiRestoreTask
{
	internal interface IWorker
	{
		void Work();
		void Cleanup();
	}
}