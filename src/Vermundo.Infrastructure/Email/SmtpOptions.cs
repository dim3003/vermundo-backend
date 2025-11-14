namespace Vermundo.Infrastructure.Email;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public bool UseStartTls { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FromAddress { get; set; } = default!;
    public string FromName { get; set; } = default!;
}
