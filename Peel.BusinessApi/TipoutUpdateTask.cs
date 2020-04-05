using System;
using System.Threading.Tasks;
using Peel.BusinessContracts;

namespace Peel.BusinessApi
{
	public class TipOutUpdateTask : IScheduledTask
	{
		public Task Run()
		{
			Console.WriteLine("running tipout");

			return Task.CompletedTask;
		}
	}
}
