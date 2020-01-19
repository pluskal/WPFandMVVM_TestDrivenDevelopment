﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FriendStorage.UI.ViewModel;

namespace FriendStorage.UI.Wrapper
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IRevertibleChangeTracking
    {
        private readonly Dictionary<string, object> _originalValues;
        private readonly List<IRevertibleChangeTracking> _trackingObjects;

        public ModelWrapper(T model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IRevertibleChangeTracking>();
        }

        public T Model { get; }

        public bool IsChanged
        {
            get { return _originalValues.Count > 0 || _trackingObjects.Any(t => t.IsChanged); }
        }

        public bool IsValid => throw new NotImplementedException();


        public void AcceptChanges()
        {
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects) trackingObject.AcceptChanges();
            OnPropertyChanged("");
        }

        public void RejectChanges()
        {
            foreach (var originalValueEntry in _originalValues)
                typeof(T).GetProperty(originalValueEntry.Key).SetValue(Model, originalValueEntry.Value);
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects) trackingObject.RejectChanges();
            OnPropertyChanged("");
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue) propertyInfo.GetValue(Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName)
                ? (TValue) _originalValues[propertyName]
                : GetValue<TValue>(propertyName);
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue newValue,
            [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);
            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName);
                propertyInfo.SetValue(Model, newValue);
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + "IsChanged");
            }
        }

        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(_originalValues[propertyName], newValue))
                {
                    _originalValues.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        protected void RegisterCollection<TWrapper, TModel>(
            ChangeTrackingCollection<TWrapper> wrapperCollection,
            List<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                modelCollection.AddRange(wrapperCollection.Select(w => w.Model));
            };
            RegisterTrackingObject(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
            where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
        {
            if (_trackingObjects.Contains(trackingObject)) return;

            _trackingObjects.Add(trackingObject);
            trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged)) OnPropertyChanged(nameof(IsChanged));
        }
    }
}