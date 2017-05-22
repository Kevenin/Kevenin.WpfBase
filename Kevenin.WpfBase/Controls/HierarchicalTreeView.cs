using Kevenin.WpfBase.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevenin.WpfBase.Controls
{
    public class TreeViewModel : ViewModelBase
    {
        #region Private Fields

        private bool? _isChecked = false;
        private TreeViewModel _parent;

        #endregion Private Fields

        #region Private Constructors

        private TreeViewModel(string name, TreeViewModel Parent, object data, bool enabled = true)
        {
            this.Name = name;
            this.Parent = Parent;
            this.Children = new ObservableCollection<TreeViewModel>();
            this.IsEnabled = enabled;
            this.Data = data;
        }

        #endregion Private Constructors

        #region Public Properties

        public ObservableCollection<TreeViewModel> Children { get; private set; }

        public object Data { get; set; }

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        public bool IsEnabled { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public TreeViewModel Parent { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the TreeView split by the chosen levels
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="data">All the objects to be put in the treeview</param>
        /// <param name="rootname">Name displayed for the root Node</param>
        /// <param name="displayproperty">The property to be displayed in the treeview</param>
        /// <param name="levels">The name of the properties to use as levels in the treeview (exclude the display property)</param>
        /// <returns></returns>
        public static ObservableCollection<TreeViewModel> CreateTreeView<T>(IEnumerable<T> data, string rootname, string displayproperty, params string[] levels)
        {
            TreeViewModel root;
            root = new TreeViewModel(rootname, null, null);

            foreach (T node in data)
                addnode(root, 0, node, displayproperty, levels);

            root.Initialize();

            ObservableCollection<TreeViewModel> results = new ObservableCollection<TreeViewModel>();
            results.Add(root);
            return results;
        }

        /// <summary>
        /// Get a list of all the checkeditems in the treeview
        /// </summary>
        /// <typeparam name="T">the type of the result</typeparam>
        /// <param name="root">the root of the treeview</param>
        /// <returns></returns>
        public static ObservableCollection<T> GetCheckedItems<T>(TreeViewModel root)
        {
            ObservableCollection<T> result = new ObservableCollection<T>();
            FindCheckedItem(result, root);
            return result;
        }

        /// <summary>
        /// Search for an item in the treeview
        /// </summary>
        /// <typeparam name="T">Type of the data in the treeview to search</typeparam>
        /// <param name="searchterm">the value to search</param>
        /// <param name="searchproperty">the parameter to search in</param>
        /// <param name="root">the root of the treeview</param>
        /// <returns></returns>
        public static T SearchItem<T>(object searchterm, string searchproperty, TreeViewModel root)
        {
            object r = null;
            if (root.Children.Count > 0)
                foreach (TreeViewModel t in root.Children)
                {
                    r = SearchItem<T>(searchterm, searchproperty, t);
                    if (r != null)
                        break;
                }
            else
            {
                if (root.Data.GetType().GetProperty(searchproperty).GetValue(root.Data, null).ToString() == searchterm.ToString())
                    r = root.Data;
            }
            return (T)r;
        }

        /// <summary>
        /// Search for a treenode in the treeview
        /// </summary>
        /// <param name="searchterm">the object to search for</param>
        /// <param name="searchproperty">the property to search in</param>
        /// <param name="root">the root of the treeview</param>
        /// <returns></returns>
        public static TreeViewModel SearchItem(object searchterm, string searchproperty, TreeViewModel root)
        {
            TreeViewModel r = null;

            if (root.Children.Count > 0)
            {
                foreach (TreeViewModel t in root.Children)
                {
                    r = SearchItem(searchterm, searchproperty, t);
                    if (r != null)
                        break;
                }
            }
            else
            {
                if (root.Data.GetType().GetProperty(searchproperty).GetValue(root.Data, null).ToString() == searchterm.ToString())
                    r = root;
            }
            return r;
        }

        #endregion Public Methods

        #region Private Methods

        private static void addnode<T>(TreeViewModel root, int level, T node, string displayproperty, params string[] levels)
        {
            if (level < levels.Count())
            {
                string group = node.GetType().GetProperty(levels[level].ToString()).GetValue(node, null).ToString();

                foreach (TreeViewModel t in root.Children)
                {
                    if (t.Name == group && level < levels.Count())
                    {
                        addnode(t, level + 1, node, displayproperty, levels);
                        return;
                    }
                }
                TreeViewModel g = new TreeViewModel(group, root, node);
                if (level < levels.Count())
                    addnode(g, level + 1, node, displayproperty, levels);
                root.Children.Add(g);
            }
            else
                root.Children.Add(new TreeViewModel(node.GetType().GetProperty(displayproperty).GetValue(node, null).ToString(), root, node));
        }

        private static void FindCheckedItem<T>(ObservableCollection<T> result, TreeViewModel root)
        {
            foreach (TreeViewModel t in root.Children)
                if (t.Children.Count > 0)
                    FindCheckedItem(result, t);
                else
                    if (t.IsChecked == true)
                    result.Add((T)t.Data);
        }

        private void Initialize()
        {
            foreach (TreeViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;
            if (IsEnabled)
            {
                _isChecked = value;

                if (updateChildren && _isChecked.HasValue)
                    foreach (TreeViewModel c in this.Children)
                        c.SetIsChecked(_isChecked, true, false);

                if (updateParent && _parent != null)
                    _parent.VerifyCheckState();

                this.OnPropertyChanged("IsChecked");
            }
        }

        private void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion Private Methods
    }
}
