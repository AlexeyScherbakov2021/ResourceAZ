﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ResourceAZ//.Resources.Styles
{
    public class DataGridSelected : DataGrid
    {
        public static readonly DependencyProperty SelectedItemsListProperty =
       DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(DataGridSelected), 
           new PropertyMetadata(default(IList)/*, OnSelectedItemsPropertyChanged*/));


        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SetValue(SelectedItemsListProperty, base.SelectedItems);
        }

        public IList SelectedItemsList
        {
            get => (IList)GetValue(SelectedItemsListProperty); 
            set => throw new Exception("This property is read-only. To bind to it you must use 'Mode=OneWayToSource'.");
            //set { SetValue(SelectedItemsListProperty, value); }
        }


        //private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((DataGridSelected)d).OnSelectedItemsChanged((IList)e.OldValue, (IList)e.NewValue);
        //}

        //protected virtual void OnSelectedItemsChanged(IList oldSelectedItems, IList newSelectedItems)
        //{
        //}
    }
}