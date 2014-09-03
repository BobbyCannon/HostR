#region References

using Hostr.Extensions;

#endregion

namespace Hostr.Triggers
{
	public class TriggerSettings : Settings
	{
		#region Methods

		public ITrigger CreateInstance()
		{
			return TargetFullName.CreateInstance(new object[] { this }) as ITrigger;
		}

		#endregion
	}
}