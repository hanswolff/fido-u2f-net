using System.Collections.Generic;
using FidoU2f.Models;

namespace FidoU2f.Demo.Services
{
	public interface IFidoRepository
	{
		void StoreStartedRegistration(string userName, FidoStartedRegistration startedRegistration);

		FidoStartedRegistration GetStartedRegistration(string userName, string challenge);

		IEnumerable<FidoStartedRegistration> GetAllStartedRegistrationsOfUser(string userName);

		void RemoveStartedRegistration(string userName, string challenge);

		void StoreDeviceRegistration(string userName, FidoDeviceRegistration deviceRegistration);

		IEnumerable<FidoDeviceRegistration> GetDeviceRegistrationsOfUser(string userName);
	}
}
