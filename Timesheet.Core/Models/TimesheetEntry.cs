namespace Timesheet.Core.Models;

public class TimesheetEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    /// <summary>
    /// Hours worked in decimal format (e.g., 7.5). Must be > 0 and <= 24.
    /// </summary>
    public decimal HoursWorked { get; set; }
}