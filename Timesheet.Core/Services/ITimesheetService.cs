using Timesheet.Core.Models;

namespace Timesheet.Core.Services;
public interface ITimesheetService
{
    /// <summary>
    /// Adds a new timesheet entry. Throws if duplicate or invalid hours.
    /// </summary>
    void AddEntry(TimesheetEntry entry);

    /// <summary>
    /// Updates an existing timesheet entry. Throws if not found or would create duplicate.
    /// </summary>
    void UpdateEntry(TimesheetEntry entry);

    /// <summary>
    /// Deletes a timesheet entry by ID. Throws if not found.
    /// </summary>
    void DeleteEntry(Guid id);

    /// <summary>
    /// Returns all entries for a given user and week (weekStart = Monday).
    /// </summary>
    IEnumerable<TimesheetEntry> GetEntriesForUserWeek(string userId, DateTime weekStart);

    /// <summary>
    /// Returns total hours worked per project for a user in a week.
    /// </summary>
    decimal GetTotalHoursPerProject(string userId, string projectId, DateTime weekStart);
}