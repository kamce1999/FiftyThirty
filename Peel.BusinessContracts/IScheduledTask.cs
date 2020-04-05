using System.Threading.Tasks;

namespace Peel.BusinessContracts
{
	public interface IScheduledTask
	{
		Task Run();
	}
}
