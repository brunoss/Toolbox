using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Windows.Input;

namespace ToolBox.MV
{
    public interface ICanExecuteNotifier
    {
        bool CanExecute();
        ICommand Command { get; set; }
    }

    public class CollectionNotEmptyNotifier
    {
        public static CollectionNotEmptyNotifier<T> Create<T>(ObservableCollection<T> values)
        {
            return new CollectionNotEmptyNotifier<T>(values);
        }
    }

    public class CollectionNotEmptyNotifier<T> : ICanExecuteNotifier
    {
        private readonly ObservableCollection<T> _values; 
        public CollectionNotEmptyNotifier(ObservableCollection<T> values)
        {
            _values = values;
            values.CollectionChanged += CollectionOnCollectionChanged;
        }

        private void CollectionOnCollectionChanged(object sender, 
            NotifyCollectionChangedEventArgs args)
        {
            if (args.NewStartingIndex == -1 || 
                args.NewStartingIndex == 0 && args.Action == NotifyCollectionChangedAction.Add ||
                args.Action == NotifyCollectionChangedAction.Reset)
            {
                Command?.CanExecute(this);
            }
        }

        public bool CanExecute()
        {
            return _values.Count > 0;
        }

        public ICommand Command { get; set; }
    }

    public abstract class PropertyNotifier<T, V> : ICanExecuteNotifier
        where T : INotifyPropertyChanged
    {
        protected readonly T _model;
        protected readonly Func<T, V> _prop;
        protected readonly string _propName;

        protected PropertyNotifier(T value, Func<T, V> prop, string propName)
        {
            _model = value;
            _model.PropertyChanged += ModelOnPropertyChanged;
            _prop = prop;
            _propName = propName;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == _propName)
            {
                Command?.CanExecute(this);
            }
        }

        public ICommand Command { get; set; }

        public abstract bool CanExecute();
    }

    public class NotNullNotifier<T> : PropertyNotifier<T, object>
        where T : INotifyPropertyChanged
    {
        public NotNullNotifier(T value, Func<T, object> prop, string propName) 
            : base(value, prop, propName)
        {
            
        }
        public override bool CanExecute()
        {
            return _prop(_model) != null;
        }
    }

    public class BolleanNotifier<T> : PropertyNotifier<T, bool>
        where T : INotifyPropertyChanged
    {
        public BolleanNotifier(T value, Func<T, bool> prop, string propName)
            : base(value, prop, propName)
        {
            
        }

        public override bool CanExecute()
        {
            return _prop(_model);
        }
    }
}
