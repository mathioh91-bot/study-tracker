using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    /// <summary>
    /// ViewModel for the Study Planner application.
    /// Manages tasks, statistics, filtering, searching, and user interactions.
    /// Uses MVVM pattern to separate UI logic from business logic.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Task> _allTasks;
        private ObservableCollection<Task> _filteredTasks;
        private Task _selectedTask;
        private string _searchText;
        private TaskStatus? _selectedStatusFilter;
        private TaskPriority? _selectedPriorityFilter;

        // Dashboard statistics
        private int _totalTasks;
        private int _completedTasks;
        private int _pendingTasks;
        private int _inProgressTasks;

        // Dialog properties
        private bool _isTaskDialogOpen;
        private string _dialogTitle;
        private Task _currentEditingTask;

        // ICommand properties
        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand CancelTaskCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand FilterStatusCommand { get; }
        public ICommand FilterPriorityCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public MainViewModel()
        {
            // Initialize collections
            _allTasks = new ObservableCollection<Task>();
            _filteredTasks = new ObservableCollection<Task>();

            // Initialize commands
            AddTaskCommand = new RelayCommand(_ => OpenAddTaskDialog());
            EditTaskCommand = new RelayCommand(_ => OpenEditTaskDialog(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand(_ => DeleteSelectedTask(), _ => SelectedTask != null);
            SaveTaskCommand = new RelayCommand(_ => SaveTask());
            CancelTaskCommand = new RelayCommand(_ => CloseTaskDialog());
            SearchCommand = new RelayCommand(_ => ApplyFilters());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch());
            FilterStatusCommand = new RelayCommand(param => FilterByStatus(param));
            FilterPriorityCommand = new RelayCommand(param => FilterByPriority(param));
            ClearFiltersCommand = new RelayCommand(_ => ClearAllFilters());

            // Load sample data
            LoadSampleData();
        }

        // Expose enum values for ComboBox binding
        public Array PriorityValues => Enum.GetValues(typeof(TaskPriority));
        public Array StatusValues => Enum.GetValues(typeof(TaskStatus));

        #region Properties with Notification

        public ObservableCollection<Task> FilteredTasks
        {
            get { return _filteredTasks; }
            set { _filteredTasks = value; OnPropertyChanged(nameof(FilteredTasks)); }
        }

        public Task SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; OnPropertyChanged(nameof(SelectedTask)); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        public TaskStatus? SelectedStatusFilter
        {
            get { return _selectedStatusFilter; }
            set { _selectedStatusFilter = value; OnPropertyChanged(nameof(SelectedStatusFilter)); }
        }

        public TaskPriority? SelectedPriorityFilter
        {
            get { return _selectedPriorityFilter; }
            set { _selectedPriorityFilter = value; OnPropertyChanged(nameof(SelectedPriorityFilter)); }
        }

        // Dashboard Statistics
        public int TotalTasks
        {
            get { return _totalTasks; }
            set { _totalTasks = value; OnPropertyChanged(nameof(TotalTasks)); }
        }

        public int CompletedTasks
        {
            get { return _completedTasks; }
            set { _completedTasks = value; OnPropertyChanged(nameof(CompletedTasks)); }
        }

        public int PendingTasks
        {
            get { return _pendingTasks; }
            set { _pendingTasks = value; OnPropertyChanged(nameof(PendingTasks)); }
        }

        public int InProgressTasks
        {
            get { return _inProgressTasks; }
            set { _inProgressTasks = value; OnPropertyChanged(nameof(InProgressTasks)); }
        }

        public bool IsTaskDialogOpen
        {
            get { return _isTaskDialogOpen; }
            set { _isTaskDialogOpen = value; OnPropertyChanged(nameof(IsTaskDialogOpen)); }
        }

        public string DialogTitle
        {
            get { return _dialogTitle; }
            set { _dialogTitle = value; OnPropertyChanged(nameof(DialogTitle)); }
        }

        public Task CurrentEditingTask
        {
            get { return _currentEditingTask; }
            set { _currentEditingTask = value; OnPropertyChanged(nameof(CurrentEditingTask)); }
        }

        #endregion

        #region Methods

        private void OpenAddTaskDialog()
        {
            CurrentEditingTask = new Task
            {
                Id = _allTasks.Count > 0 ? _allTasks.Max(t => t.Id) + 1 : 1,
                Deadline = DateTime.Now.AddDays(1),
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium
            };
            DialogTitle = "Add New Task";
            IsTaskDialogOpen = true;
        }

        private void OpenEditTaskDialog()
        {
            if (SelectedTask == null) return;

            CurrentEditingTask = new Task
            {
                Id = SelectedTask.Id,
                Title = SelectedTask.Title,
                Subject = SelectedTask.Subject,
                Description = SelectedTask.Description,
                Deadline = SelectedTask.Deadline,
                Priority = SelectedTask.Priority,
                Status = SelectedTask.Status
            };
            DialogTitle = "Edit Task";
            IsTaskDialogOpen = true;
        }

        private void SaveTask()
        {
            if (string.IsNullOrWhiteSpace(CurrentEditingTask.Title))
            {
                return; // Validation: title is required
            }

            var existingTask = _allTasks.FirstOrDefault(t => t.Id == CurrentEditingTask.Id);

            if (existingTask != null)
            {
                // Update existing task
                existingTask.Title = CurrentEditingTask.Title;
                existingTask.Subject = CurrentEditingTask.Subject;
                existingTask.Description = CurrentEditingTask.Description;
                existingTask.Deadline = CurrentEditingTask.Deadline;
                existingTask.Priority = CurrentEditingTask.Priority;
                existingTask.Status = CurrentEditingTask.Status;
            }
            else
            {
                // Add new task
                _allTasks.Add(CurrentEditingTask);
            }

            CloseTaskDialog();
            UpdateStatistics();
            ApplyFilters();
        }

        private void DeleteSelectedTask()
        {
            if (SelectedTask != null)
            {
                _allTasks.Remove(SelectedTask);
                UpdateStatistics();
                ApplyFilters();
            }
        }

        private void CloseTaskDialog()
        {
            IsTaskDialogOpen = false;
            CurrentEditingTask = null;
        }

        private void ApplyFilters()
        {
            var query = _allTasks.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(t =>
                    t.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    t.Subject.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Apply status filter
            if (SelectedStatusFilter.HasValue)
            {
                query = query.Where(t => t.Status == SelectedStatusFilter.Value);
            }

            // Apply priority filter
            if (SelectedPriorityFilter.HasValue)
            {
                query = query.Where(t => t.Priority == SelectedPriorityFilter.Value);
            }

            FilteredTasks.Clear();
            foreach (var task in query)
            {
                FilteredTasks.Add(task);
            }
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            ApplyFilters();
        }

        private void FilterByStatus(object param)
        {
            if (param == null)
            {
                SelectedStatusFilter = null;
            }
            else if (param is string statusStr && Enum.TryParse<TaskStatus>(statusStr, out var status))
            {
                SelectedStatusFilter = SelectedStatusFilter == status ? (TaskStatus?)null : status;
            }
            ApplyFilters();
        }

        private void FilterByPriority(object param)
        {
            if (param == null)
            {
                SelectedPriorityFilter = null;
            }
            else if (param is string priorityStr && Enum.TryParse<TaskPriority>(priorityStr, out var priority))
            {
                SelectedPriorityFilter = SelectedPriorityFilter == priority ? (TaskPriority?)null : priority;
            }
            ApplyFilters();
        }

        private void ClearAllFilters()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = null;
            SelectedPriorityFilter = null;
            ApplyFilters();
        }

        private void UpdateStatistics()
        {
            TotalTasks = _allTasks.Count;
            CompletedTasks = _allTasks.Count(t => t.Status == TaskStatus.Completed);
            PendingTasks = _allTasks.Count(t => t.Status == TaskStatus.Pending);
            InProgressTasks = _allTasks.Count(t => t.Status == TaskStatus.InProgress);
        }

        private void LoadSampleData()
        {
            _allTasks.Add(new Task
            {
                Id = 1,
                Title = "Study Mathematics Chapter 5",
                Subject = "Mathematics",
                Description = "Review calculus concepts and solve practice problems",
                Deadline = DateTime.Now.AddDays(2),
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress
            });

            _allTasks.Add(new Task
            {
                Id = 2,
                Title = "Read Literature Book Chapter 3",
                Subject = "Literature",
                Description = "Read and take notes on chapter 3",
                Deadline = DateTime.Now.AddDays(5),
                Priority = TaskPriority.Medium,
                Status = TaskStatus.Pending
            });

            _allTasks.Add(new Task
            {
                Id = 3,
                Title = "Physics Lab Report",
                Subject = "Physics",
                Description = "Complete and submit the lab report",
                Deadline = DateTime.Now.AddDays(1),
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress
            });

            _allTasks.Add(new Task
            {
                Id = 4,
                Title = "History Essay",
                Subject = "History",
                Description = "Write an essay on World War II",
                Deadline = DateTime.Now.AddDays(7),
                Priority = TaskPriority.Medium,
                Status = TaskStatus.Pending
            });

            _allTasks.Add(new Task
            {
                Id = 5,
                Title = "Chemistry Quiz Preparation",
                Subject = "Chemistry",
                Description = "Prepare for the upcoming quiz",
                Deadline = DateTime.Now.AddDays(3),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending
            });

            UpdateStatistics();
            ApplyFilters();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
