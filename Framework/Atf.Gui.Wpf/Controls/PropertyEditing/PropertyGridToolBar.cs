﻿//Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;

namespace Sce.Atf.Wpf.Controls.PropertyEditing
{
    /// <summary>
    /// PropertyGrid ToolBar</summary>
    public class PropertyGridToolBar : Control
    {
        /// <summary>
        /// ShowCategorized ICommand routed through element tree</summary>
        public static readonly ICommand ShowCategorized = new RoutedCommand("ShowCategorized", typeof(PropertyGridToolBar));

        /// <summary>
        /// ShowAlphaSorted ICommand routed through element tree</summary>
        public static readonly ICommand ShowAlphaSorted = new RoutedCommand("ShowAlphaSorted", typeof(PropertyGridToolBar));

        #region Properties Property

        /// <summary>
        /// Gets or sets PropertyGridToolBar's Properties dependency property</summary>
        public IEnumerable Properties
        {
            get { return (IEnumerable)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        /// <summary>
        /// PropertyGridToolBar's Properties dependency property</summary>
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(IEnumerable), typeof(PropertyGridToolBar), new FrameworkPropertyMetadata(new PropertyChangedCallback(PropertiesProperty_Changed)));

        private static void PropertiesProperty_Changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var tb = o as PropertyGridToolBar;
            if (tb != null && tb.Properties != null)
            {
                if (tb.m_isCateegorized)
                {
                    if (tb.GetCollectionView().CanSort)
                    {
                        tb.ExecuteSort(null, null);
                    }
                }
                else if (tb.GetCollectionView().CanGroup)
                {
                    tb.ExecuteGroup(null, null);
                }
            }
        }

        #endregion

        /// <summary>
        /// Static constructor</summary>
        static PropertyGridToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridToolBar), new FrameworkPropertyMetadata(typeof(PropertyGridToolBar)));
        }

        /// <summary>
        /// Constructor</summary>
        public PropertyGridToolBar()
        {
            base.CommandBindings.Add(new CommandBinding(ShowAlphaSorted, new ExecutedRoutedEventHandler(ExecuteSort), new CanExecuteRoutedEventHandler(CanExecuteSort)));
            base.CommandBindings.Add(new CommandBinding(ShowCategorized, new ExecutedRoutedEventHandler(ExecuteGroup), new CanExecuteRoutedEventHandler(CanExecuteGroup)));
        }

        private void CanExecuteSort(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Properties != null && GetCollectionView().CanSort;
        }

        private void ExecuteSort(object sender, ExecutedRoutedEventArgs e)
        {
            Clear();
            var cv = GetCollectionView();
            cv.SortDescriptions.Add(s_alphaSort);
            m_isCateegorized = true;
        }

        private void CanExecuteGroup(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Properties != null && GetCollectionView().CanGroup;
        }

        private void ExecuteGroup(object sender, ExecutedRoutedEventArgs e)
        {
            Clear();
            var cv = GetCollectionView();
            cv.GroupDescriptions.Add(DefaultPropertyGrouping.ByCategory);
            m_isCateegorized = false;
        }

        private void Clear()
        {
            if (Properties != null)
            {
                var cv = GetCollectionView();
                cv.SortDescriptions.Clear();
                cv.GroupDescriptions.Clear();
            }
        }

        private ICollectionView GetCollectionView()
        {
            return CollectionViewSource.GetDefaultView(Properties);
        }

        private bool m_isCateegorized;
        private static SortDescription s_alphaSort = new SortDescription("Descriptor.DisplayName", ListSortDirection.Ascending);
    }
}
