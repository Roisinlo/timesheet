using Timesheet.Core.Helpers;
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

        var error = Assert.Throws<InvalidOperationException>(() => _service.AddEntry(duplicate));
        Assert.Equal("A timesheet entry already exists for this user, project, and date.", error.Message);
    }
    
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
        var entryNotAdded = new TimesheetEntry
        {
            UserId = "2",
            ProjectId = "2",
            Date = DateTime.Today,
            HoursWorked = 3
        };

        var error = Assert.Throws<InvalidOperationException>(() => _service.UpdateEntry(entryNotAdded));
        Assert.Equal("Timesheet entry not found.", error.Message);
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
            HoursWorked = 3m
        };

        _service.AddEntry(entry2);

        var updatedEntry2 = new TimesheetEntry
        {
            Id = entry2.Id,
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 3m
        };

        var error = Assert.Throws<InvalidOperationException>(() => _service.UpdateEntry(updatedEntry2));
        Assert.Equal("Updating would create a duplicate entry.", error.Message);
    }

    [Fact]
    public void UpdateEntry_should_successfully_update_existing_entry_HoursWorked()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        var updateToEntry = new TimesheetEntry
        {
            Id = entry.Id,
            UserId = entry.UserId,
            ProjectId = entry.ProjectId,
            Date = entry.Date,
            HoursWorked = 3m
        };

        _service.UpdateEntry(updateToEntry);

        var result = _service.GetEntriesForUserWeek("1", DateTime.Today);
        var updatedEntry = result.Single();

        Assert.Equal(entry.Id, updatedEntry.Id);
        Assert.Equal(updateToEntry.HoursWorked, updatedEntry.HoursWorked);
    }

    [Fact]
    public void UpdateEntry_should_successfully_update_ProjectId()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        var updateToEntry = new TimesheetEntry
        {
            Id = entry.Id,
            UserId = entry.UserId,
            ProjectId = "2",
            Date = entry.Date,
            HoursWorked = entry.HoursWorked
        };

        _service.UpdateEntry(updateToEntry);

        var result = _service.GetEntriesForUserWeek("1", DateTime.Today);
        var updatedEntry = result.Single();

        Assert.Equal(entry.Id, updatedEntry.Id);
        Assert.Equal(updateToEntry.ProjectId, updatedEntry.ProjectId);
    }

    [Fact]
    public void UpdateEntry_should_successfully_update_Date_when_no_clashing_entry_exists()
    {
        // fixed past date to avoid future date validation error
        var fixedDate = new DateTime(2026, 2, 18);
      
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        var updateToEntry = new TimesheetEntry
        {
            Id = entry.Id,
            UserId = entry.UserId,
            ProjectId = entry.ProjectId,
            Date = fixedDate.AddDays(1),
            HoursWorked = entry.HoursWorked
        };

        _service.UpdateEntry(updateToEntry);

        var result = _service.GetEntriesForUserWeek("1", fixedDate.AddDays(1));
        var updatedEntry = result.Single();

        Assert.Equal(entry.Id, updatedEntry.Id);
        Assert.Equal(updateToEntry.Date, updatedEntry.Date.Date);
    }

    //Delete Tests
    [Fact]
    public void DeleteEntry_should_successfully_delete_entry()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry);

        _service.DeleteEntry(entry.Id);

        var result = _service.GetEntriesForUserWeek(entry.UserId, entry.Date);

        Assert.Empty(result);
    }

    [Fact]
    public void DeleteEntry_should_throw_error_if_no_entry_is_found_with_given_entry_id()
    {
        var error = Assert.Throws<InvalidOperationException>(() => _service.DeleteEntry(Guid.NewGuid()));
        Assert.Equal("Timesheet entry not found.", error.Message);
    }

    //GetEntriesForUserWeek tests
    [Fact]
    public void GetEntriesForUserWeek_should_throw_error_if_userId_param_is_invalid()
    {
        var error = Assert.Throws<ArgumentException>(() => _service.GetEntriesForUserWeek("", DateTime.Today));
        Assert.Equal("Valid User Id is required.", error.Message);

    }

    [Fact]
    public void GetEntriesForUserWeek_should_only_return_entries_for_given_user_id()
    {
        // fixed past date to avoid future date validation error
        var fixedDate = new DateTime(2026, 2, 18);

        var entry1 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate,
            HoursWorked = 7.5m
        };

        _service.AddEntry(entry1);

        var entry2 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate.AddDays(1),
            HoursWorked = 3m
        };

        _service.AddEntry(entry2);

        var entry3 = new TimesheetEntry
        {
            UserId = "2",
            ProjectId = "2",
            Date = fixedDate,
            HoursWorked = 3m
        };

        _service.AddEntry(entry3);

        var result = _service.GetEntriesForUserWeek(entry1.UserId, entry1.Date);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Id == entry1.Id);
        Assert.Contains(result, e => e.Id == entry2.Id);
        Assert.DoesNotContain(result, e => e.Id == entry3.Id);
    }

    [Fact]
    public void GetEntriesForUserWeek_should_not_return_entries_outside_of_given_week()
    {
        var fixedDate = new DateTime(2026, 2, 18);
        
        var entryThisWeek = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate,
            HoursWorked = 7.5m
        };

        var entryLastWeek = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "2",
            Date = fixedDate.AddDays(-7),
            HoursWorked = 3m
        };

        _service.AddEntry(entryThisWeek);
        _service.AddEntry(entryLastWeek);

        var result = _service.GetEntriesForUserWeek("1", fixedDate);

        Assert.Single(result);
        Assert.DoesNotContain(result, e => e.Id == entryLastWeek.Id);
    }

    [Fact]
    public void GetEntriesForUserWeek_should_return_entries_ordered_by_date()
    {
        // fixed past date to avoid future date validation error
        var fixedDate = new DateTime(2026, 2, 18);

        var entry1 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate,
            HoursWorked = 7.5m
        };

        var entry2 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate.AddDays(1),
            HoursWorked = 3m
        };

        _service.AddEntry(entry2);
        _service.AddEntry(entry1);

        var result = _service.GetEntriesForUserWeek("1", fixedDate).ToList();
        
        Assert.Equal(entry1.Id, result[0].Id);
        Assert.Equal(entry2.Id, result[1].Id);

    }

    //GetTotalHoursPerProject tests
    [Fact]
    public void GetTotalHoursPerProject_should_return_zero_if_no_entries_exist_for_given_week()
    {
        var result = _service.GetTotalHoursPerProject("1", "1", DateTime.Today);
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetTotalHoursPerProject_should_throw_error_if_userId_param_is_invalid()
    {
        var error = Assert.Throws<ArgumentException>(() => _service.GetTotalHoursPerProject("", "1", DateTime.Today));
        Assert.Equal("Valid User Id is required.", error.Message);

    }

    [Fact]
    public void GetTotalHoursPerProject_should_throw_error_if_projectId_param_is_invalid()
    {
        var error = Assert.Throws<ArgumentException>(() => _service.GetTotalHoursPerProject("1", "", DateTime.Today));
        Assert.Equal("Valid Project Id is required.", error.Message);

    }

    [Fact]
    public void GetTotalHoursPerProject_should_return_total_hours_per_project()
    {
        var fixedDate = new DateTime(2026, 2, 18);
        
        var entryUser1PreviousWeek = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate.AddDays(-7),
            HoursWorked = 3m
        };

        var entryUser1 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate,
            HoursWorked = 7.5m
        };

        var entryUser1Day2 = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = fixedDate.AddDays(1),
            HoursWorked = 4m
        };

        var entryUser2 = new TimesheetEntry
        {
            UserId = "2",
            ProjectId = "2",
            Date = fixedDate,
            HoursWorked = 3m
        };

        _service.AddEntry(entryUser1PreviousWeek);
        _service.AddEntry(entryUser1);
        _service.AddEntry(entryUser1Day2);
        _service.AddEntry(entryUser2);

        var result = _service.GetTotalHoursPerProject(entryUser1.UserId, entryUser1.ProjectId, entryUser1.Date);
        Assert.Equal(entryUser1.HoursWorked + entryUser1Day2.HoursWorked, result);
    }

    //ValidateEntry tests
    [Theory]
    [InlineData("", "1", 7.5, "User Id is required.")]
    [InlineData("1", "", 7.5, "Project Id is required.")]
    [InlineData("1", "1", 0, "Hours must be greater than 0 and less than 24.")]
    [InlineData("1", "1", 25, "Hours must be greater than 0 and less than 24.")]
    public void AddEntry_should_throw_error_if_entry_is_invalid(string userId, string projectId, decimal hoursWorked, string expectedMessage)
    {
        var entry = new TimesheetEntry
        {
            UserId = userId,
            ProjectId = projectId,
            Date = DateTime.Today,
            HoursWorked = hoursWorked
        };

        var error = Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        Assert.Equal(expectedMessage, error.Message);
    }

    [Fact]
    public void AddEntry_should_throw_error_if_date_is_in_the_future()
    {
        var entry = new TimesheetEntry
        {
            UserId = "1",
            ProjectId = "1",
            Date = DateTime.Today.AddDays(1),
            HoursWorked = 3m
        };

        var error = Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        Assert.Equal("Cannot submit timesheet for future dates.", error.Message);
    }

    [Theory]
    [InlineData("2026-02-18", "2026-02-16")]
    [InlineData("2026-02-16", "2026-02-16")]
    [InlineData("2026-02-22", "2026-02-16")]
    public void GetStartOfWeek_should_return_monday_of_given_week(string input, string expected)
    {
        var result = WeekHelper.GetStartOfWeek(DateTime.Parse(input));
        Assert.Equal(DateTime.Parse(expected), result);
    }
}