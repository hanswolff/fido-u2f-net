using System.Collections.Generic;
using FidoU2f.Models;

namespace FidoU2f.Demo.Models
{
	public class RegistrationsViewModel
	{
		public ICollection<FidoStartedRegistration> StartedRegistrations { get; set; }

		public ICollection<FidoDeviceRegistration> DeviceRegistrations { get; set; }

		public RegistrationsViewModel()
		{
			StartedRegistrations = new List<FidoStartedRegistration>();
			DeviceRegistrations = new List<FidoDeviceRegistration>();
		}
	}
}
