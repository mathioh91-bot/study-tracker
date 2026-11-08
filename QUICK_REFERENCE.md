# Study Planner - Quick Reference Guide

## File Organization

### Models (Data)
- **Task.cs**: Defines the Task data structure
  - Properties: Id, Title, Subject, Description, Deadline, Priority, Status
  - Implements `INotifyPropertyChanged` for binding
  - Enums: `TaskPriority` (Low, Medium, High), `TaskStatus` (Pending, InProgress, Completed)

### ViewModels (Business Logic)
- **MainViewModel.cs**: Central logic hub
  - Manages task collection (`ObservableCollection<Task>`)
  - Handles commands: Add, Edit, Delete, Search, Filter
  - Maintains dashboard statistics
  - Exposes data for View binding

### Views (User Interface)
- **MainWindow.xaml**: Main UI layout
  - Sidebar with statistics
  - Task DataGrid
  - Search and filter controls
  - Modal task dialog
- **MainWindow.xaml.cs**: Code-behind (minimal)
  - Sets DataContext = new MainViewModel()

### Commands
- **RelayCommand.cs**: ICommand implementation
  - Enables command binding in XAML
  - Supports CanExecute for button enable/disable

### Converters
- **EnumConverters.cs**: Convert enums to strings for display
  - `EnumToStringConverter`: Enum → String
  - `PriorityToColorConverter`: TaskPriority → Color code
  - `StatusToColorConverter`: TaskStatus → Color code

### Behaviors
- **TextBoxBehavior.cs**: Adds placeholder text to TextBox
  - Attached behavior pattern
  - Usage: `behaviors:TextBoxBehavior.Placeholder="..."`

### Styling
- **App.xaml**: Global resources and styles
  - Color scheme resources
  - Button styles (Modern, Secondary)
  - TextBox, ComboBox styles
  - DataGrid row styles
  - Effects (shadows)

---

## Common Patterns

### 1. Creating a Bindable Property in ViewModel

```csharp
private int _taskCount;

public int TaskCount
{
    get { return _taskCount; }
    set { _taskCount = value; OnPropertyChanged(nameof(TaskCount)); }
}

// In XAML:
<TextBlock Text="{Binding TaskCount}" />
```

### 2. Creating a Command

```csharp
public ICommand DeleteTaskCommand { get; }

public MainViewModel()
{
    // Basic command
    DeleteTaskCommand = new RelayCommand(_ => DeleteTask());

    // Command with CanExecute
    EditTaskCommand = new RelayCommand(
        _ => EditTask(),
        _ => SelectedTask != null  // Only enable if task selected
    );
}
```

### 3. Two-Way Binding with Immediate Updates

```xaml
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />
```

### 4. Binding with Converter

```xaml
<TextBlock Text="{Binding Status, Converter={StaticResource EnumToStringConverter}}" />
```

### 5. Using ObservableCollection

```csharp
private ObservableCollection<Task> _tasks = new ObservableCollection<Task>();

public ObservableCollection<Task> Tasks
{
    get { return _tasks; }
}

// Adding updates UI automatically
_tasks.Add(newTask);

// Binding in XAML:
<DataGrid ItemsSource="{Binding Tasks}" />
```

---

## How Binding Works (Step by Step)

### Property Change Flow
```
1. User updates TextBox
   ↓
2. TextBox text changes
   ↓
3. Because Binding UpdateSourceTrigger=PropertyChanged:
   WPF calls ViewModel property setter
   ↓
4. Property raises PropertyChanged event
   ↓
5. All bound controls receive notification
   ↓
6. Bound controls update their display
```

### Collection Change Flow
```
1. Code adds item to ObservableCollection
   _tasks.Add(newItem)
   ↓
2. ObservableCollection raises CollectionChanged event
   ↓
3. DataGrid receives notification
   ↓
4. DataGrid refreshes display automatically
```

---

## Debugging Tips

### To trace binding issues:
1. Open Visual Studio Output Window
2. Go to Debug → Windows → Output
3. Check for binding errors like:
   - "BindingExpression path error"
   - Property not found
   - Type mismatch

### Common binding errors:
- **"Path error"**: Property name doesn't exist on ViewModel
  - Check spelling: `{Binding TaskCount}` not `{Binding taskcount}`

- **"Type mismatch"**: Binding type doesn't match property type
  - Use Converter to transform types

- **"CanExecute called on null object"**: SelectedItem is null
  - Check `CanExecute` predicate handles null

### Enable binding error messages:
Add to App.xaml.cs:
```csharp
PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.All;
```

---

## Styling Guide

### Using existing styles:
```xaml
<!-- Modern blue button -->
<Button Style="{StaticResource ModernButton}" Content="Save" />

<!-- Light secondary button -->
<Button Style="{StaticResource SecondaryButton}" Content="Cancel" />

<!-- Formatted text input -->
<TextBox Style="{StaticResource ModernTextBox}" />

<!-- Styled dropdown -->
<ComboBox Style="{StaticResource ModernComboBox}" />
```

### Color resources available:
```xaml
{StaticResource PrimaryColor}      <!-- #001F3F -->
{StaticResource AccentColor}       <!-- #00A4EF -->
{StaticResource BackgroundColor}   <!-- #F5F5F5 -->
{StaticResource CardBackground}    <!-- #FFFFFF -->
{StaticResource TextPrimary}       <!-- #1A1A1A -->
{StaticResource TextSecondary}     <!-- #666666 -->
{StaticResource BorderColor}       <!-- #E0E0E0 -->
```

---

## Task Addition Workflow

```
1. User clicks "Add Task" button
   ↓
2. AddTaskCommand triggers
   ↓
3. ViewModel.OpenAddTaskDialog() executes
   - Creates new Task
   - Sets DialogTitle = "Add New Task"
   - Sets IsTaskDialogOpen = true
   ↓
4. Modal dialog appears (visibility binding)
   ↓
5. CurrentEditingTask bound to form fields
   ↓
6. User fills form and clicks Save
   ↓
7. SaveTaskCommand triggers
   - Validates Title is not empty
   - Adds/updates in _allTasks collection
   - Updates statistics
   - Refreshes filtered list
   ↓
8. IsTaskDialogOpen = false
   ↓
9. Dialog closes, DataGrid updates
```

---

## Filter & Search Logic

### Search Implementation
```csharp
if (!string.IsNullOrWhiteSpace(SearchText))
{
    // Case-insensitive search in Title and Subject
    query = query.Where(t =>
        t.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
        t.Subject.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0
    );
}
```

### Status Filter
```csharp
if (SelectedStatusFilter.HasValue)
{
    query = query.Where(t => t.Status == SelectedStatusFilter.Value);
}
```

### Priority Filter
```csharp
if (SelectedPriorityFilter.HasValue)
{
    query = query.Where(t => t.Priority == SelectedPriorityFilter.Value);
}
```

**All filters can be combined and toggled independently!**

---

## Testing Checklist

- [ ] Add Task: Click Add → Fill form → Save → Task appears in grid
- [ ] Edit Task: Select task → Click Edit → Modify → Save → Changes appear
- [ ] Delete Task: Select task → Click Delete → Task removed
- [ ] Search: Type in search → Only matching tasks appear
- [ ] Filter Status: Click status button → Shows only that status
- [ ] Filter Priority: Click priority button → Shows only that priority
- [ ] Multi-filter: Search + filter by status/priority together
- [ ] Clear Filters: All tasks reappear
- [ ] Statistics: Add/complete/delete tasks → Counts update automatically
- [ ] Modal: Dialog appears/closes correctly
- [ ] Required Fields: Try saving empty title → Should not save

---

## Performance Considerations

- **ObservableCollection**: Efficient for < 1000 items
- **DataGrid virtualization**: Enabled by default for large lists
- **Binding**: One-way binding is faster than two-way
- **Converters**: Keep lightweight, avoid heavy processing
- **Events**: Unsubscribe to avoid memory leaks

---

## Architecture Decision Rationale

### Why MVVM?
- ViewModel is testable without UI
- UI changes don't require logic changes
- Multiple views can share one ViewModel
- Supports data binding naturally

### Why Commands?
- No code-behind logic
- Reusable across controls
- Can enable/disable dynamically with CanExecute

### Why ObservableCollection?
- Automatic UI updates when collection changes
- No manual refresh() calls needed
- Standard WPF pattern

### Why Converters?
- Transform data without ViewModel complexity
- Reusable and testable
- Separate display logic from business logic

---

## Resources for Learning

- WPF MVVM Pattern
- Data Binding in WPF
- ICommand and RelayCommand
- ObservableCollection usage
- Value Converters tutorial
- XAML styling and resources

