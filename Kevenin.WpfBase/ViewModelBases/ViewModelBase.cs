using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kevenin.WpfBase.ViewModelBases
{
    public enum WindowDialogType { Information, Avertissement, Erreur }

    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Private Fields

        private Dictionary<string, object> propertyValueMap;

        #endregion Private Fields

        #region Protected Constructors

        protected ViewModelBase()
        {
            propertyValueMap = new Dictionary<string, object>();
        }

        #endregion Protected Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Protected Methods

        protected static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression body = expression.Body;
            MemberExpression memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
            }
            return memberExpression.Member.Name;
        }

        protected T Get<T>(Expression<Func<T>> path)
        {
            return Get(path, default(T));
        }

        protected virtual T Get<T>(Expression<Func<T>> path, T defaultValue)
        {
            var propertyName = GetPropertyName(path);
            if (propertyValueMap.ContainsKey(propertyName))
            {
                return (T)propertyValueMap[propertyName];
            }
            else
            {
                propertyValueMap.Add(propertyName, defaultValue);
                return defaultValue;
            }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> path)
        {
            string propertyName = GetPropertyName(path);
            InternalPropertyUpdate(propertyName);
        }

        protected void Set<T>(Expression<Func<T>> path, T value)
        {
            Set(path, value, false);
        }

        protected virtual void Set<T>(Expression<Func<T>> path, T value, bool forceUpdate)
        {
            var oldValue = Get(path);
            var propertyName = GetPropertyName(path);

            if (!object.Equals(value, oldValue) || forceUpdate)
            {
                propertyValueMap[propertyName] = value;
                OnPropertyChanged(path);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void InternalPropertyUpdate(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Private Methods
    }
}