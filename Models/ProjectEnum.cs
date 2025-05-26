namespace server.Models;

public class ProjectEnum
{
    public enum SmtpSettings
    {
        SmtpSettings,
        Host,
        Port,
        Username,
        Password
    }

    public enum NotifyTempData
    {
        NotifySuccess,
        NotfiyFailure
    }
}