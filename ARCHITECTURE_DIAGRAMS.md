# Study Planner - Visual Architecture Overview

## Application Structure Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      STUDY PLANNER                          │
│                   WPF Application                           │
└─────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (Views & XAML)                               │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  MainWindow.xaml                                           │  │
│  │  ├─ Sidebar (Navigation & Statistics)                      │  │
│  │  ├─ Search & Filter Controls                              │  │
│  │  ├─ Task DataGrid                                         │  │
│  │  └─ Modal Task Dialog                                     │  │
│  │                                                            │  │
│  │  App.xaml (Global Styles & Resources)                     │  │
│  │  ├─ Color Palette                                         │  │
│  │  ├─ Button Styles                                         │  │
│  │  ├─ TextBox Styles                                        │  │
│  │  └─ Effects (Shadows)                                     │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↕ (Data Binding)
┌──────────────────────────────────────────────────────────────────┐
│  VIEWMODEL LAYER (Business Logic)                                │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  MainViewModel.cs                                          │  │
│  │  ├─ Properties (Bindable Data)                            │  │
│  │  │  ├─ FilteredTasks (ObservableCollection<Task>)        │  │
│  │  │  ├─ SelectedTask                                       │  │
│  │  │  ├─ TotalTasks, CompletedTasks, etc.                  │  │
│  │  │  ├─ SearchText, StatusFilter, PriorityFilter          │  │
│  │  │  └─ IsTaskDialogOpen, CurrentEditingTask              │  │
│  │  │                                                        │  │
│  │  └─ Commands (User Actions)                              │  │
│  │     ├─ AddTaskCommand                                     │  │
│  │     ├─ EditTaskCommand                                    │  │
│  │     ├─ DeleteTaskCommand                                  │  │
│  │     ├─ SaveTaskCommand                                    │  │
│  │     ├─ FilterStatusCommand                                │  │
│  │     ├─ FilterPriorityCommand                              │  │
│  │     └─ ClearFiltersCommand                                │  │
│  │                                                            │  │
│  │  Methods (Implementation)                                 │  │
│  │  ├─ OpenAddTaskDialog()                                   │  │
│  │  ├─ SaveTask()                                            │  │
│  │  ├─ DeleteSelectedTask()                                  │  │
│  │  ├─ ApplyFilters()                                        │  │
│  │  └─ UpdateStatistics()                                    │  │
│  │                                                            │  │
│  │  INotifyPropertyChanged Implementation                    │  │
│  │  └─ OnPropertyChanged() → Notifies View of changes       │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              ↕ (Data Operations)
┌──────────────────────────────────────────────────────────────────┐
│  MODEL LAYER (Data)                                              │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  Task.cs                                                   │  │
│  │  ├─ Properties                                             │  │
│  │  │  ├─ Id                                                 │  │
│  │  │  ├─ Title                                              │  │
│  │  │  ├─ Subject                                            │  │
│  │  │  ├─ Description                                        │  │
│  │  │  ├─ Deadline                                           │  │
│  │  │  ├─ Priority (TaskPriority enum)                       │  │
│  │  │  └─ Status (TaskStatus enum)                           │  │
│  │  │                                                        │  │
│  │  ├─ TaskPriority Enum: Low, Medium, High                 │  │
│  │  ├─ TaskStatus Enum: Pending, InProgress, Completed      │  │
│  │  │                                                        │  │
│  │  └─ INotifyPropertyChanged Implementation                │  │
│  │     └─ Each property change notifies listeners           │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ➞ In-Memory Storage (ObservableCollection<Task>)              │
│    ├─ Automatically notifies UI of changes                     │
│    └─ No database - data reset on app restart                 │
└──────────────────────────────────────────────────────────────────┘
```

---

## Data Flow Diagram: Adding a Task

```
┌─────────────────────────────────────────────────────────────────┐
│  USER INTERACTION                                               │
│  Click "Add Task" Button                                        │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  XAML BINDING                                                   │
│  <Button Command="{Binding AddTaskCommand}" />                 │
│  ➜ WPF detects button click                                    │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  RELAYCOMMAND EXECUTION                                         │
│  AddTaskCommand.Execute(parameter) called                       │
│  ➜ RelayCommand invokes the lambda: _ => OpenAddTaskDialog()   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  VIEWMODEL LOGIC                                                │
│  MainViewModel.OpenAddTaskDialog()                              │
│  ├─ Create new Task object                                     │
│  ├─ Set CurrentEditingTask = newTask                           │
│  ├─ Set DialogTitle = "Add New Task"                           │
│  └─ Set IsTaskDialogOpen = true                                │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  PROPERTY CHANGE NOTIFICATION                                   │
│  OnPropertyChanged("IsTaskDialogOpen")                          │
│  ➜ PropertyChanged event raised                                │
│  ➜ All bound controls receive notification                     │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  UI UPDATE (Data Binding)                                       │
│  <Grid Visibility="{Binding IsTaskDialogOpen, ...}">           │
│  ➜ Binding detects IsTaskDialogOpen = true                     │
│  ➜ Modal dialog becomes visible                                │
│                                                                 │
│  <TextBox Text="{Binding CurrentEditingTask.Title, ...}">      │
│  ➜ Binding displays empty Title field                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  USER ENTERS DATA                                               │
│  User types task details and clicks Save                        │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  SAVE COMMAND EXECUTION                                         │
│  SaveTaskCommand.Execute() called                               │
│  ➜ MainViewModel.SaveTask() executed                            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  BUSINESS LOGIC                                                 │
│  SaveTask() Method:                                             │
│  1. Validate Title is not empty                                │
│  2. Add CurrentEditingTask to _allTasks collection             │
│  3. Call UpdateStatistics()                                    │
│  4. Call ApplyFilters()                                        │
│  5. Call CloseTaskDialog()                                     │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  COLLECTION CHANGE NOTIFICATION                                 │
│  _allTasks.Add(newTask)                                         │
│  ➜ ObservableCollection raises CollectionChanged event         │
│  ➜ All bound ItemsSource properties receive notification       │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  FILTER & STATISTICS UPDATE                                     │
│  ApplyFilters() refreshes FilteredTasks                         │
│  UpdateStatistics() recalculates counts:                        │
│  ├─ TotalTasks++                                               │
│  ├─ PendingTasks++ (if new task is Pending)                    │
│  ├─ OnPropertyChanged("TotalTasks")                            │
│  └─ OnPropertyChanged("PendingTasks")                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  MULTIPLE UI UPDATES (All Automatic!)                          │
│  1. DataGrid Updates                                            │
│     <DataGrid ItemsSource="{Binding FilteredTasks}">           │
│     ➜ New row appears in DataGrid                              │
│                                                                 │
│  2. Sidebar Statistics Update                                   │
│     <TextBlock Text="{Binding TotalTasks}">                    │
│     ➜ "Total Tasks: 6" (was 5)                                 │
│                                                                 │
│  3. Modal Dialog Closes                                         │
│     IsTaskDialogOpen = false                                    │
│     ➜ Dialog becomes invisible                                  │
│                                                                 │
│  ALL without any manual UI refresh code!                        │
└─────────────────────────────────────────────────────────────────┘
```

---

## Binding Architecture

```
                    XAML (View Layer)
                          ↑
                          │ Binding Property Changed
                          │
    ┌──────────────────────┴─────────────────────────┐
    │                                                 │
    ▼                                                 ▼
Control.Text                               ViewModel.PropertyName
   (TextBox)                                    (String Property)
    ▲                                                 ▲
    │                                                 │
    │ User types text                         OnPropertyChanged()
    │                                                 │
    └──────────────────────────────────────────────┬─┘
                                                   │
                                          INotifyPropertyChanged
                                          Implementation


FLOW:
1. User types in TextBox
2. TextBox text property changes
3. UpdateSourceTrigger=PropertyChanged triggers
4. Binding updates ViewModel property (setter called)
5. Property setter calls OnPropertyChanged()
6. Event fires, other bound controls update
7. All updates happen automatically!
```

---

## Command Execution Flow

```
┌────────────────┐
│   XAML View    │
│   <Button>     │
│   Command=     │
│   "AddTask"    │
└────────┬───────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Binding System (in WPF)            │
│  Locates AddTaskCommand on          │
│  DataContext (MainViewModel)        │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  User clicks button                 │
│  ➜ WPF calls:                       │
│  AddTaskCommand.Execute(null)       │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  ICommand.Execute() Method          │
│  public class RelayCommand:         │
│    public void Execute(obj param)   │
│    {                                │
│      _execute(param);               │
│    }                                │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Lambda Expression Executes         │
│  _ => OpenAddTaskDialog()           │
│                                     │
│  Now ViewModel method runs!         │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  ViewModel Method Execution         │
│  OpenAddTaskDialog() {...}          │
│  ├─ Business logic                  │
│  ├─ Data operations                 │
│  └─ Property updates                │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Property Notifications             │
│  OnPropertyChanged(name)            │
│  ➜ Event fires                      │
│  ➜ Bound UI controls update         │
└─────────────────────────────────────┘
```

---

## Converter Chain During Binding

```
Data Source                Converter              UI Control
(ViewModel)               (Binding)                 (XAML)

TaskPriority.High
   │
   ├─ Binding.Convert() called
   │
   └─ Convert(
        value: TaskPriority.High,
        targetType: string,
        parameter: null,
        culture: en-US
      )
      {
        return ((Enum)value).ToString()
               // = "High"
      }
      │
      └─> TextBlock.Text = "High"
          (Displayed to user)


REVERSE FLOW (ComboBox Selection):
User selects "High" from ComboBox
   │
   ├─ ComboBox.SelectedItem = "High" (string)
   │
   ├─ Binding.ConvertBack() called
   │
   └─ ConvertBack(
        value: "High",
        targetType: TaskPriority,
        parameter: null,
        culture: en-US
      )
      {
        return Enum.Parse<TaskPriority>("High")
               // = TaskPriority.High
      }
      │
      └─> ViewModel.Priority = TaskPriority.High
          (Value stored as enum)
```

---

## Memory and Performance Model

```
┌─────────────────────────────────────────────────────┐
│  Data Persistence                                   │
│                                                     │
│  MainViewModel                                      │
│  ├─ _allTasks (ObservableCollection<Task>)        │
│  │  └─ In Memory during application runtime        │
│  │                                                 │
│  └─ _filteredTasks (ObservableCollection<Task>)   │
│     └─ Filtered view of _allTasks                  │
│                                                     │
│  Application Startup:                              │
│  ├─ MainViewModel instantiated                     │
│  ├─ LoadSampleData() populates _allTasks          │
│  └─ ApplyFilters() creates FilteredTasks          │
│                                                     │
│  Application Shutdown:                             │
│  ├─ All collections cleared                        │
│  ├─ All tasks lost (not saved to file/DB)         │
│  └─ Next run will reload sample data              │
└─────────────────────────────────────────────────────┘

OPTIMIZATION:
- DataGrid Virtualization: Only renders visible rows
- Binding Caching: Compiled bindings for performance
- ObservableCollection: Efficient updates
- Lazy Filtering: Only when filter changes
```

---

## Error Handling Flow

```
┌────────────────────────────────────────┐
│  Binding Error in XAML                 │
│  <TextBlock Text="{Binding BadName}" > │
└────────┬───────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────┐
│  WPF Binding Engine                                    │
│  1. Looks for "BadName" on DataContext                │
│  2. Property not found                                │
│  3. Binding fails                                     │
└────────┬───────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────┐
│  Output Window - Debug Messages                        │
│  "BindingExpression path error: 'MainViewModel'       │
│   object has no 'BadName' property"                   │
│                                                       │
│  UI Effect: TextBlock displays blank                 │
│  (No exception, no crash)                            │
└────────────────────────────────────────────────────────┘

PREVENTION:
1. Enable binding error output
2. Check property names carefully
3. Verify DataContext is set
4. Use IntelliSense in XAML
```

---

## UI Responsive Pattern

```
User Action
    │
    ├─ Button Click / Text Change / Selection
    │
    ▼
Binding Detects Change
    │
    ├─ Command.Execute() or Property update
    │
    ▼
ViewModel Method Executes
    │
    ├─ Business logic runs
    ├─ Collections modified
    ├─ Properties updated
    │
    ▼
OnPropertyChanged() or CollectionChanged Event
    │
    ├─ Event propagates through WPF system
    │
    ▼
Bound Controls Receive Notification
    │
    ├─ DataGrid refreshes rows
    ├─ TextBlock updates text
    ├─ Sidebar statistics update
    │
    ▼
USER SEES UPDATED UI
    │
    └─ All without manual refresh calls!
```

---

## File Size Reference

```
Models/
├── Task.cs                          ~80 lines

ViewModels/
├── MainViewModel.cs                 ~280 lines

Commands/
├── RelayCommand.cs                  ~40 lines

Converters/
├── EnumConverters.cs                ~80 lines

Behaviors/
├── TextBoxBehavior.cs               ~60 lines

Views/
├── MainWindow.xaml                  ~300 lines
├── MainWindow.xaml.cs               ~10 lines

Configuration/
├── App.xaml                         ~100 lines
├── App.xaml.cs                      ~10 lines

Total: ~960 lines of clean, well-organized code
```

---

This visual architecture shows:
- How MVVM separates concerns
- How data flows through the application
- How binding automatically updates UI
- How commands execute user actions
- How converters transform data
- How everything works together seamlessly!

