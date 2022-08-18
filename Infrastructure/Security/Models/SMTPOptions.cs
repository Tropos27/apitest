namespace Launcher.Infrastructure.Security.Models
{
    public class SMTPOptions
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }

        public Mail Mail { get; set; }
    }
}
