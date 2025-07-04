using System.ComponentModel;

namespace NirvanaMAUI.ViewModels
{
    public class InputStatus : INotifyPropertyChanged
    {
        public string Name { get; }
        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) > 0.001f)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public InputStatus(string name, float initial = 0f)
        {
            Name = name;
            _value = initial;
        }
    }
}
