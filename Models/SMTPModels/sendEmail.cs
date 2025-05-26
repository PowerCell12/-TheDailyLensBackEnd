namespace server.Models.SMTPModels
{
    public class sendEmail
    {
        public string subject { get; set; }
        public string body { get; set; }

        public string currentEmail { get; set; }
    }
}