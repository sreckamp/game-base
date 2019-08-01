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

        public T OldVal { get; private set; }
        public T NewVal { get; private set; }
    }

    public class ChangeValueProperty<T>
    {
        private T m_value;

        public ChangeValueProperty(T defaultVal = default(T), EventHandler valueChangeOccuredHandler = null)
        {
            m_value = defaultVal;
            if(valueChangeOccuredHandler != null)
            {
                m_valueChangeOccured += valueChangeOccuredHandler;
            }
        }

        public event EventHandler<ChangedValueArgs<T>> ValueChanged;
        private event EventHandler m_valueChangeOccured;
        public T Value
        {
            get => m_value;
            set
            {
                var old = m_value;
                m_value = value;
                ValueChanged?.Invoke(this, new ChangedValueArgs<T>(old, value));
                m_valueChangeOccured?.Invoke(this, new EventArgs());
            }
        }

        public static implicit operator T(ChangeValueProperty<T> prop)
        {
            return prop.Value;
        }
    }
}
