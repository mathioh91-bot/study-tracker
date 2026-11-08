# Quick Start - Study Planner WPF App

## ⚡ 2-Minute Setup

### 1. Build
```powershell
# Visual Studio: Ctrl+Shift+B
```

### 2. Run
```powershell
# Visual Studio: F5 (or Ctrl+F5 for release)
```

### 3. Use
- **Add Task**: Click "➕ Add Task" button
- **Edit Task**: Select task → Click "✏️ Edit"
- **Delete Task**: Click "🗑️ Delete" in DataGrid
- **Search**: Type in search box (updates as you type)
- **Filter**: Click status/priority buttons to filter
- **Clear**: Click "🔄 Clear Filters" to reset

---

## 🎯 5 Key Concepts

### 1. **Data Binding** - Automatic UI Updates
```xaml
<!-- Type → ViewModel updates automatically -->
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />

<!-- ViewModel property changes → UI updates automatically -->
<TextBlock Text="{Binding TotalTasks}" />
```

### 2. **ObservableCollection** - Collection Auto-Updates
```csharp
_allTasks.Add(newTask);  // DataGrid refreshes automatically!
```

### 3. **ICommand** - Button Click Handling
```xaml
<Button Command="{Binding AddTaskCommand}" Content="Add" />
```

### 4. **INotifyPropertyChanged** - Property Change Notification
```csharp
private int _count;
public int Count
{
    get { return _count; }
    set { _count = value; OnPropertyChanged(nameof(Count)); }
}
```

### 5. **Value Converters** - Data Transformation
```xaml
<TextBlock Text="{Binding Priority, Converter={StaticResource EnumToStringConverter}}" />
```

---

## 📂 Key Files

| File | What It Does |
|------|--------------|
| **Models/Task.cs** | Defines task data structure |
| **ViewModels/MainViewModel.cs** | Contains all business logic |
| **MainWindow.xaml** | UI layout |
| **App.xaml** | Global styles and colors |
| **Commands/RelayCommand.cs** | Makes buttons work |

---

## 🧪 Quick Test

1. **Add**: Click "Add Task" → Type title → Click "Save" → Appears in grid ✅
2. **Search**: Type "Math" → Only math tasks show ✅
3. **Filter**: Click "High" priority → Only high priority ✅
4. **Edit**: Select task → Click "Edit" → Change values → "Save" ✅
5. **Delete**: Click "Delete" in grid → Task removed ✅
6. **Stats**: Sidebar counts update automatically ✅

---

## 🎨 Color Scheme

| Use | Color |
|-----|-------|
| Sidebar | Dark Navy (#001F3F) |
| Buttons | Blue (#00A4EF) |
| Cards | White (#FFFFFF) |
| Background | Light Gray (#F5F5F5) |

---

## 📚 Documentation

- **README.md** - Project overview
- **PROJECT_GUIDE.md** - Learn MVVM & binding (start here!)
- **QUICK_REFERENCE.md** - Code snippets
- **ARCHITECTURE_DIAGRAMS.md** - Visual flowcharts
- **TESTING_GUIDE.md** - How to test
- **IMPLEMENTATION_SUMMARY.md** - What was built

---

## 🔧 Common Tasks

### To Add a New Property to Tasks
1. Add to `Models/Task.cs`
2. Add ViewModel property
3. Update `MainWindow.xaml` form
4. Done! Binding handles the rest.

### To Change Colors
Edit `App.xaml` resources:
```xaml
<SolidColorBrush x:Key="PrimaryColor" Color="#NEW_COLOR" />
```

### To Add a New Filter
1. Add property to ViewModel
2. Add `FilterByX` method
3. Create button with Command binding
4. Done!

---

## ⚠️ Troubleshooting

| Problem | Solution |
|---------|----------|
| App won't build | Clean solution → Rebuild |
| Binding errors in output | Check property names for typos |
| Tasks don't save | Check title isn't empty (required) |
| Filters not working | Click "Clear Filters" then try again |

---

## 🎓 Learning Path

1. **Run the app** - See it working
2. **Read README.md** - Understand features
3. **Read PROJECT_GUIDE.md** - Learn concepts
4. **Read code comments** - See how it works
5. **Modify code** - Try changes and see results
6. **Add features** - Extend it with your ideas

---

## ✅ You're Ready!

```
✓ App built
✓ App running
✓ Features working
✓ UI beautiful
✓ Code clean
✓ Documented
✓ Ready to learn/extend
```

**Now explore the code and documentation!** 🚀

