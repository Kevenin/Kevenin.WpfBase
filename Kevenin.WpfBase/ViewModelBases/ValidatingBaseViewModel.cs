using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kevenin.WpfBase.ViewModelBases
{
    public class ValidatingBaseViewModel : BaseViewModel, IDataErrorInfo
    {
        #region Private Fields

        private Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();

        #endregion Private Fields

        #region Public Properties

        public string Error
        {
            get
            {
                var errors = from b in ruleMap.Values where b.HasError select b.Error;

                return string.Join("\n", errors);
            }
        }

        public bool HasErrors
        {
            get
            {
                var values = ruleMap.Values.ToList();
                values.ForEach(b => b.Update());

                return values.Any(b => b.HasError);
            }
        }

        #endregion Public Properties

        #region Public Indexers

        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return null;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        public void AddRule<T>(Expression<Func<T>> expression, Func<bool> ruleDelegate, string errorMessage)
        {
            var name = GetPropertyName(expression);

            ruleMap.Add(name, new Binder(ruleDelegate, errorMessage));
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Set<T>(Expression<Func<T>> path, T value, bool forceUpdate)
        {
            ruleMap[GetPropertyName(path)].IsDirty = true;
            base.Set<T>(path, value, forceUpdate);
        }

        #endregion Protected Methods

        #region Private Classes

        private class Binder
        {
            #region Private Fields

            private readonly string message;
            private readonly Func<bool> ruleDelegate;

            #endregion Private Fields



            #region Internal Constructors

            internal Binder(Func<bool> ruleDelegate, string message)
            {
                this.ruleDelegate = ruleDelegate;
                this.message = message;

                IsDirty = true;
            }

            #endregion Internal Constructors



            #region Internal Properties

            internal string Error { get; set; }
            internal bool HasError { get; set; }

            internal bool IsDirty { get; set; }

            #endregion Internal Properties



            #region Internal Methods

            internal void Update()
            {
                if (!IsDirty)
                    return;

                Error = null;
                HasError = false;
                try
                {
                    if (!ruleDelegate())
                    {
                        Error = message;
                        HasError = true;
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    HasError = true;
                }
            }

            #endregion Internal Methods
        }

        #endregion Private Classes
    }
}
