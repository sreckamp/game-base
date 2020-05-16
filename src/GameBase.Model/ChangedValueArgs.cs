using System;

namespace GameBase.Model
{
    public class ChangedValueArgs<T>:EventArgs
    {
        public ChangedValueArgs(T oldVal, T newVal)
        {
            OldVal = oldVal;
            NewVal = newVal;
        }

        public T OldVal { get; }
        public T NewVal { get; }
    }

    public class ChangeValueProperty<T>
    {
        private T m_value;

        public ChangeValueProperty(T defaultVal = default, EventHandler valueChangeOccuredHandler = null)
        {
            m_value = defaultVal;
            ValueChanged += (sender, args) => { };
            ValueChangeOccured += valueChangeOccuredHandler ?? ((sender, args) => { });
        }

        public event EventHandler<ChangedValueArgs<T>> ValueChanged;
        private event EventHandler ValueChangeOccured;
        private T Value
        {
            get => m_value;
            set
            {
                var old = m_value;
                m_value = value;
                ValueChanged?.Invoke(this, new ChangedValueArgs<T>(old, value));
                ValueChangeOccured?.Invoke(this, new EventArgs());
            }
        }

        public static implicit operator T(ChangeValueProperty<T> prop)
        {
            return prop.Value;
        }

        public static implicit operator ChangeValueProperty<T>(T value)
        {
            var prop = new ChangeValueProperty<T> {Value = value};
            return prop;
        }
    }
}
