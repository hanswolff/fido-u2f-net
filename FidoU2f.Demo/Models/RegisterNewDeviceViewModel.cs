namespace FidoU2f.Demo.Models
{
	public class RegisterNewDeviceViewModel
	{
		public string AppId { get; set; }
		public string Challenge { get; set; }
		public string UserName { get; set; }

		public string RawRegisterResponse { get; set; }
	}
}
