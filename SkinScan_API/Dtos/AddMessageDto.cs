namespace SkinScan_API.Dtos
{
	public class AddMessageDto
	{
		public String text { get; set; }
		public String ReceiverId { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
