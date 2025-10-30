using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDo
{
    public class TaskItem : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private bool _isDone;

        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }

        public bool IsDone
        {
            get => _isDone;
            set => SetField(ref _isDone, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}