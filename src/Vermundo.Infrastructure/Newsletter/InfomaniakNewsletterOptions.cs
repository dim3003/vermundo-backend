namespace Vermundo.Infrastructure.Newsletter;

public class InfomaniakNewsletterOptions
{
    public const string SectionName = "Infomaniak:Newsletter";

    public string ApiToken { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty; 
}

