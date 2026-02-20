using Timesheet.Core.Helpers;
using Timesheet.Core.Models;

namespace Timesheet.Core.Services;

public class TimesheetService : ITimesheetService
{
    private readonly List<TimesheetEntry> _entries = new();

    public void AddEntry(TimesheetEntry entry)
    {
        ValidateEntry(entry);

        if(IsDuplicate(entry)) throw new InvalidOperationException("A timesheet entry already exists for this user, project, and date.");
        _entries.Add(entry);
    }

    public void UpdateEntry(TimesheetEntry entry)
    {
        ValidateEntry(entry);

        var existingEntry = _entries.FirstOrDefault(e => e.Id == entry.Id);
        if(existingEntry is null) throw new InvalidOperationException("Timesheet entry not found.");

        if(_entries.Any(e => 
            e.Id != entry.Id && 
            e.UserId == entry.UserId &&
            e.ProjectId == entry.ProjectId &&
            e.Date.Date == entry.Date.Date))
        {
            throw new InvalidOperationException("Updating would create a duplicate entry.");
        }

        existingEntry.UserId = entry.UserId;
        existingEntry.ProjectId = entry.ProjectId;
        existingEntry.Date = entry.Date.Date;
        existingEntry.HoursWorked = entry.HoursWorked;
    }

    public void DeleteEntry(Guid id)
    {
        var existingEntry = _entries.FirstOrDefault(e => e.Id == id);
        if(existingEntry is null) throw new InvalidOperationException("Timesheet entry not found");

        _entries.Remove(existingEntry);
    }

    public IEnumerable<TimesheetEntry> GetEntriesForUserWeek(string userId, DateTime weekStart)
    {
        var start = WeekHelper.GetStartOfWeek(weekStart);
        var end = start.AddDays(7);

        return _entries.Where(e =>
                e.UserId == userId &&
                e.Date >= start &&
                e.Date < end).OrderBy(e => e.Date).ToList();
    }

    public decimal GetTotalHoursPerProject(string userId, string projectId, DateTime weekStart)
    {
        var start = WeekHelper.GetStartOfWeek(weekStart);
        var end = start.AddDays(7);

        return _entries.Where(e =>
                e.UserId == userId &&
                e.ProjectId == projectId &&
                e.Date >= start &&
                e.Date < end).Sum(e => e.HoursWorked); 
    }

    //Helpers
    private static void ValidateEntry(TimesheetEntry entry)
    {
        if(string.IsNullOrWhiteSpace(entry.UserId)) throw new ArgumentException("User Id is required.");
        if(string.IsNullOrWhiteSpace(entry.ProjectId)) throw new ArgumentException("Project Id is required.");
        if(entry.HoursWorked <= 0 || entry.HoursWorked > 24) throw new ArgumentException("Hours must be greater than 0 and less than 24.");
        if(entry.Date.Date > DateTime.Today) throw new ArgumentException("Cannot submit timesheet for future dates.");
    }

    private bool IsDuplicate(TimesheetEntry entry)
    {
        return _entries.Any(e => 
                e.UserId == entry.UserId &&
                e.ProjectId == entry.ProjectId &&
                e.Date == entry.Date.Date);
    }
}