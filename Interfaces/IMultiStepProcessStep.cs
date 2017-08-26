using System;
namespace AircraftForSale
{
	public interface IMultiStepProcessStep : IDisposable
	{
		int StepIndex { get; set; }
		event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;
	}


	public class MultiStepProcessStepEventArgs
	{
		public int Index { get; set; }
	}
}

