# Study Planner WPF Application - Architecture & Concepts Guide

## Project Overview

The **Study Planner** is a modern desktop application built with WPF and C# that helps users manage study tasks efficiently. It demonstrates professional WPF patterns and best practices using the MVVM (Model-View-ViewModel) architecture.

---

## Project Structure

```
WpfApp1/
├── Models/
│   └── Task.cs                 # Task model with enums for Priority and Status
├── ViewModels/
│   └── MainViewModel.cs        # Application logic and command handlers
├── Views/
│   └── MainWindow.xaml         # UI Layout
├── Commands/
│   └── RelayCommand.cs         # ICommand implementation
├── Converters/
│   └── EnumConverters.cs       # Value converters for binding
├── Behaviors/
│   └── TextBoxBehavior.cs      # Attached behaviors for UI enhancements
├── App.xaml                    # Application resources and styles
├── MainWindow.xaml.cs          # Code-behind for MainWindow
└── App.xaml.cs                 # Application startup logic
```

---

## Key WPF Concepts Explained

### 1. **MVVM Pattern (Model-View-ViewModel)**

The MVVM pattern separates the application into three layers:

- **Model** (`Models/Task.cs`): Contains the data structure
  - Represents a study task with properties like Title, Subject, Deadline, Priority, Status
  - Implements `INotifyPropertyChanged` to notify the UI when properties change

- **View** (`MainWindow.xaml`): The UI layer
  - Defines the user interface using XAML
  - Contains no business logic
  - Binds to the ViewModel using `{Binding}` expressions

- **ViewModel** (`ViewModels/MainViewModel.cs`): The business logic layer
  - Manages application state (tasks collection)
  - Handles user commands (Add, Edit, Delete, Filter)
  - Exposes data through properties that the View binds to
  - Acts as a bridge between Model and View

**Why MVVM?**
- Separates concerns: UI, logic, and data are independent
- Makes testing easier: You can test logic without UI
- Promotes code reuse and maintainability
- Allows designers and developers to work independently on XAML and C#

```
User → View (XAML) → ViewModel (Logic) → Model (Data)
         ↓
      Updates back through Bindings
```

---

### 2. **Data Binding**

Data Binding connects UI controls to ViewModel properties automatically. When data changes, the UI updates without manual code.

**Types of Binding Used:**

#### a) **One-Way Binding** (Default)
View reads data from ViewModel but doesn't send changes back.
```xaml
<TextBlock Text="{Binding TotalTasks}" />
```
The TextBlock displays `TotalTasks` from ViewModel. When `TotalTasks` changes, the TextBlock updates automatically.

#### b) **Two-Way Binding**
The View and ViewModel exchange data in both directions.
```xaml
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />
```
- User types → ViewModel updates
- ViewModel changes → TextBox updates
- `UpdateSourceTrigger=PropertyChanged` means update immediately as user types

#### c) **Binding with Converters**
Converters transform data before displaying.
```xaml
<TextBlock Text="{Binding Priority, Converter={StaticResource EnumToStringConverter}}" />
```
The converter transforms `TaskPriority.High` (enum) into "High" (string) for display.

**How it works:**
1. ViewModel property implements `INotifyPropertyChanged`
2. When property changes, it calls `OnPropertyChanged()`
3. WPF automatically updates all bound controls
4. No manual UI updates needed!

---

### 3. **ObservableCollection**

`ObservableCollection<T>` is a list that notifies the UI when items are added/removed/modified.

```csharp
public ObservableCollection<Task> FilteredTasks { get; set; }
```

**Why use it?**
- Regular `List<T>` doesn't notify UI of changes
- `ObservableCollection<T>` automatically updates bound controls
- DataGrid displaying tasks updates when collection changes

**Example:**
```csharp
_allTasks.Add(newTask);  // DataGrid updates automatically!
```

---

### 4. **ICommand & RelayCommand**

`ICommand` allows binding UI buttons to ViewModel methods instead of code-behind events.

**Traditional Approach (Bad):**
```csharp
// Code-behind
private void AddButton_Click(object sender, RoutedEventArgs e)
{
    AddTask();
}
```

**MVVM Approach (Good):**
```xaml
<!-- XAML: No code-behind needed -->
<Button Command="{Binding AddTaskCommand}" Content="Add Task" />
```

**ViewModel:**
```csharp
public ICommand AddTaskCommand { get; }

public MainViewModel()
{
    AddTaskCommand = new RelayCommand(_ => OpenAddTaskDialog());
}
```

**RelayCommand Implementation:**
```csharp
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public void Execute(object parameter)
    {
        _execute(parameter);  // Run the method
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;  // Enable/disable button
    }
}
```

**Benefits:**
- No code-behind code
- Command logic in ViewModel (testable)
- Can conditionally enable/disable buttons with `CanExecute`
- Commands can take parameters

**Example with CanExecute:**
```csharp
EditTaskCommand = new RelayCommand(
    _ => OpenEditTaskDialog(),           // Execute
    _ => SelectedTask != null            // Enable only if task selected
);
```
The Edit button is disabled until a task is selected!

---

### 5. **INotifyPropertyChanged**

Notifies the UI when a property changes.

**Implementation in Task.cs:**
```csharp
public class Task : INotifyPropertyChanged
{
    private string _title;  // Private backing field

    public string Title
    {
        get { return _title; }
        set 
        { 
            _title = value; 
            OnPropertyChanged(nameof(Title));  // Notify UI
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

**How it works:**
1. When a property changes, `OnPropertyChanged()` is called
2. This raises the `PropertyChanged` event
3. WPF listens to this event and updates bound controls
4. All bindings to that property update automatically

---

### 6. **XAML Styles & Resources**

Styles define consistent look-and-feel for controls without repeating code.

**In App.xaml:**
```xaml
<SolidColorBrush x:Key="PrimaryColor" Color="#001F3F" />
<SolidColorBrush x:Key="AccentColor" Color="#00A4EF" />

<Style x:Key="ModernButton" TargetType="Button">
    <Setter Property="Background" Value="{StaticResource AccentColor}" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="Padding" Value="12,8" />
    <Setter Property="BorderThickness" Value="0" />
</Style>
```

**Using the style in XAML:**
```xaml
<Button Style="{StaticResource ModernButton}" Content="Add Task" />
```

**Benefits:**
- Apply consistent styling to all buttons
- Change color globally by updating one resource
- Reduces XAML code duplication
- Professional appearance

---

### 7. **Value Converters**

Converters transform data during binding for display purposes.

**Example: EnumToStringConverter**
```csharp
public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum enumValue)
            return enumValue.ToString();  // Convert.Pending → "Pending"
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Convert string back to enum if binding updates from UI
        if (value is string stringValue && targetType.IsEnum)
            return Enum.Parse(targetType, stringValue);
        return null;
    }
}
```

**Usage in XAML:**
```xaml
<TextBlock Text="{Binding Status, Converter={StaticResource EnumToStringConverter}}" />
```

**Data Flow:**
```
ViewModel (TaskStatus.Completed)
         ↓
Converter.Convert()  → "Completed"
         ↓
TextBlock displays "Completed"
```

---

### 8. **Attached Behaviors**

Attached Behaviors extend control functionality without subclassing.

**TextBoxBehavior Example:**
```csharp
public static class TextBoxBehavior
{
    public static string GetPlaceholder(DependencyObject obj)
    {
        return (string)obj.GetValue(PlaceholderProperty);
    }

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached(
            "Placeholder",
            typeof(string),
            typeof(TextBoxBehavior),
            new PropertyMetadata(string.Empty, OnPlaceholderChanged));
}
```

**Usage in XAML:**
```xaml
<TextBox behaviors:TextBoxBehavior.Placeholder="Search..." />
```

**Benefits:**
- Add functionality to existing controls without subclassing
- Reusable across different control types
- Keeps XAML clean

---

## Feature Walkthrough

### Dashboard (Sidebar)
- **Statistics Cards**: Display Total, Completed, Pending, and In-Progress task counts
- **Real-time Updates**: Statistics update automatically when tasks change
- **Navigation**: Quick access buttons for adding tasks and clearing filters

### Task Management (Main Area)
1. **Search**: Filter tasks by title or subject with `UpdateSourceTrigger=PropertyChanged`
2. **Multi-Filter**: Filter by Status and Priority simultaneously
3. **DataGrid**: Display all filtered tasks
4. **CRUD Operations**:
   - Add: Click "Add Task" button → Modal dialog → Save
   - Edit: Select task → Click "Edit" → Modify → Save
   - Delete: Select task → Click "Delete"

### Modal Dialog
- Uses `Visibility` binding with `BooleanToVisibilityConverter`
- Form validation (Title is required)
- ComboBox bindings for Priority and Status enums

---

## Data Flow Example: Adding a Task

```
User clicks "Add Task" button
         ↓
Command binding triggers AddTaskCommand.Execute()
         ↓
ViewModel.OpenAddTaskDialog() executes
         ↓
IsTaskDialogOpen = true
         ↓
Modal dialog becomes visible (Visibility binding)
         ↓
User fills form and clicks Save
         ↓
SaveTaskCommand.Execute() triggers
         ↓
ViewModel.SaveTask() validates and adds to _allTasks
         ↓
ObservableCollection raises notification
         ↓
FilteredTasks collection updates
         ↓
DataGrid re-renders automatically
         ↓
UpdateStatistics() recalculates counts
         ↓
Dashboard statistics update via Property bindings
```

---

## Color Palette

The application uses a professional Windows 11-inspired design:

```
Primary: #001F3F (Dark Navy)      - Sidebar background
Accent:  #00A4EF (Blue)           - Buttons, highlights
Background: #F5F5F5              - Main area background
Card:    #FFFFFF (White)          - Card backgrounds
Text Primary: #1A1A1A             - Main text
Text Secondary: #666666           - Secondary text
Border: #E0E0E0                   - Dividers
```

---

## Key Takeaways

1. **MVVM separates concerns**: UI (View), Logic (ViewModel), Data (Model)
2. **Data Binding eliminates manual UI updates**: Use `{Binding}` expressions
3. **ObservableCollection** notifies UI of collection changes
4. **ICommand** enables button/action binding without code-behind
5. **INotifyPropertyChanged** enables automatic UI updates
6. **XAML Styles** provide consistent, maintainable design
7. **Value Converters** transform data during binding
8. **Attached Behaviors** extend control functionality

---

## Running the Application

1. Open `WpfApp1.sln` in Visual Studio
2. Build the solution
3. Press F5 to run
4. The application displays with sample tasks
5. Try adding, editing, deleting, and filtering tasks

---

## Future Enhancements

- Save tasks to a local database (SQLite)
- Add task categories and tags
- Implement deadline notifications
- Add progress tracking with completion percentage
- Export tasks to PDF/Excel
- Dark mode toggle
- User authentication and cloud sync

