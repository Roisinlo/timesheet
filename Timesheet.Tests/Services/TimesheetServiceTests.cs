using Timesheet.Core.Models;
using Timesheet.Core.Services;

public class TimesheetServiceTests
{
    private readonly ITimesheetService _service;

    public TimesheetServiceTests()
    {
        _service = new TimesheetService();
    }

    //AddEntry tests
    [Fact]
    public void AddEntry_should_throw_error_when_duplicate_exists()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        var duplicate = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 4
        };

        Assert.Throws<InvalidOperationException>(() => _service.AddEntry(duplicate));
    }

    // public void AddEntry_should_call_ValidateEntry()
    // {
    //     var entry = new TimesheetEntry
    //     {
    //         UserId = "1",
    //         ProjectId = "1",
    //         Date = DateTime.Today,
    //         HoursWorked = 7.5m
    //     };

        

    //     Assert.True(TimesheetService.)
    // }
    [Fact]
    public void AddEntry_should_successfully_add_an_entry()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        var result = _service.GetEntriesForUserWeek("1", DateTime.Today);
        var addedEntry = result.Single();

        Assert.Single(result);
        Assert.Equal(entry.Id, addedEntry.Id);
        Assert.Equal(entry.UserId, addedEntry.UserId);
        Assert.Equal(entry.ProjectId, addedEntry.ProjectId);
        Assert.Equal(entry.Date, addedEntry.Date.Date);
        Assert.Equal(entry.HoursWorked, addedEntry.HoursWorked);
    }

    //Update tests
    [Fact]
    public void UpdateEntry_should_throw_error_if_existing_entry_is_not_found()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        var entryNotAdded = new TimesheetEntry
        {
            UserId = "2",
            ProjectId = "2",
            Date = DateTime.Today,
            HoursWorked = 3
        };

        _service.AddEntry(entry);

        Assert.Throws<InvalidOperationException>(() => _service.UpdateEntry(entryNotAdded));

    }

        [Fact]
    public void UpdateEntry_should_throw_error_if_updating_an_existing_entry_would_cause_a_duplicate_entry()
    {
        var entry1 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry1);

        var entry2 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "2",
            Date = DateTime.Today,
            HoursWorked = 3
        };

        _service.AddEntry(entry2);
        entry2.ProjectId = "1";

        Assert.Throws<InvalidOperationException>(() => _service.UpdateEntry(entry2));

    }

}