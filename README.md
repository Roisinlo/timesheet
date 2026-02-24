# Timesheet project

A simple timesheet app built with .NET 8 and Blazor Server, allowing users to add, edit, delete and view timesheet entries by week and total hours per project.

### Prerequisites

- .NET 8

### Run project

```bash
cd Timesheet.Web
dotnet watch
```

### Run Tests

```bash
cd Timesheet.Tests
dotnet test
```

---

### Structure

#### Timesheet.Core

Contains business logic with no UI dependency:

- `Models/TimesheetEntry.cs` — core data model
- `Services/TimesheetService.cs` — add, edit, delete, list and total hours logic
- `Services/ITimesheetService.cs` — service interface
- `Helpers/WeekHelper.cs` — calculates the Monday of a given week

#### Timesheet.Web

Blazor frontend with components split by responsibility:

- `Components/Pages/Home.razor` — main page, handles view switching
- `Components/Shared/EntryForm.razor` — reusable form for add and edit
- `Components/Shared/EntryTable.razor` — displays entries with edit and delete actions
- `Components/Shared/HoursPerProjectTable.razor` — displays total hours per project

---

### Decisions

- Blazor vs Razor - I decided to go with Blazor in the end because it enabled a single page experience without full page reloads on each interaction. Coming from a React background, the component model felt familiar and appropriate for this kind of interactive, form based UI.

- Since the brief requires in-memory storage only, the service is registered as a singleton so data persists across requests within a session. Using `AddScoped` would create a new instance per request, losing all data between page loads.

- `TimesheetEntry.Date` is normalised to `Date.Date` on both add and update to remove the time component. This ensures the check for duplicate works correctly regardless of the time value passed in.

- The User Id field is disabled on the edit form. In a production system, changing the user on an existing entry would require elevated permissions and an audit trail. The brief does not restrict this, so it is permitted on add but prevented on edit.

---

### Future Improvements

- During manual testing I found a bug where a user could log more than 24 hours in a day across multiple projects. The current validation only checks that a single entry doesn't exceed 24 hours. In production this would require an additional validation to sum all hours for a user on a given day, across all projects.

- In production, `CreatedAt` and `LastModifiedAt` fields would be added to `TimesheetEntry` to for audit to track when entries were created and modified, and by who.

- In a real system, role based permissions would determine whether a user can edit their own entries only, or also manage entries for others. Manager roles could be introduced to give higher level permissions.

- The Blazor components could be broken down further in a larger application, for example extracting the search filter into its own component.

- The service uses `DateTime.Today` directly for future date validation, therefore in unit tests I have hard coded a date in the past to test from. In production this would be abstracted behind an `IDateTimeProvider` interface to allow better testing.

- I would make the UI clearer- where the values for User Id and Week Starting are still held in the form when the entries and hours data is presented I would have that presented too e.g. \*Timesheet for **UserId** Week Commencing **Date\***
