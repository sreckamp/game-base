using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static void Trigger(EventHandler<ChangedValueArgs<T>> handler, object sender, T oldVal, T newVal)
        {
            handler?.Invoke(sender, new ChangedValueArgs<T>(oldVal, newVal));
        }
    }
}
