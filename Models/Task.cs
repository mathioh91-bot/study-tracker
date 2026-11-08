using System;
using System.ComponentModel;

namespace WpfApp1.Models
{
    /// <summary>
    /// Represents a study task with title, subject, description, deadline, priority, and status.
    /// Implements INotifyPropertyChanged for data binding support.
    /// </summary>
    public class Task : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _subject;
        private string _description;
        private DateTime _deadline;
        private TaskPriority _priority;
        private TaskStatus _status;

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; OnPropertyChanged(nameof(Subject)); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime Deadline
        {
            get { return _deadline; }
            set { _deadline = value; OnPropertyChanged(nameof(Deadline)); }
        }

        public TaskPriority Priority
        {
            get { return _priority; }
            set { _priority = value; OnPropertyChanged(nameof(Priority)); }
        }

        public TaskStatus Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Enum for task priority levels
    /// </summary>
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Enum for task status
    /// </summary>
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
