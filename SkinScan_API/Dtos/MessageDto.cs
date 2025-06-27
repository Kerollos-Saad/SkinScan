namespace SkinScan_API.Dtos
{
	public class MessageDto
	{
		public int Id { get; set; }
		public String Text { get; set; }
		public String? SenderId { get; set; }
		public String ReceiverId { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
