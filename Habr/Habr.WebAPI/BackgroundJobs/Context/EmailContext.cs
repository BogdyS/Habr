namespace Habr.WebAPI.BackgroundJobs.Context;

public class EmailContext
{
    public string EmailFrom { get; set; }
    public string EmailTo { get; set; }
    public string Password  { get; set; }
    public string Name { get; set; }
    public DateTime SendingTime { get; set; }
}