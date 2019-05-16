namespace fifty.DataModel.Models
{
	public class Employee
	{
		public string Id { get; set; }
		public string First_name { get; set; }
		public string Last_name { get; set; }
		public string Email_address { get; set; }
		public string Employee_identifier { get; set; }
		public string Pincode { get; set; }
		public EmployeeStatus? Status { get; set; }
		public System.Collections.Generic.ICollection<EmployeeRole> Roles { get; set; }
	}
}