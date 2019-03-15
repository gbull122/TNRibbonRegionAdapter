using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;

namespace TNRibbonRegionAdapter
{
    public class RibbonBehavoir : RegionBehavior
    {
        public static readonly string BehaviorKey = "RibbonBehavior";
        public const double DefaultMergeOrder = 10000d;

        public Ribbon MainRibbon { get; set; }

        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
            Region.Views.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {


                    var ribbonView = newItem as UserControl;
                    var thing = GetRibbonFromView(ribbonView);

                    MergeRibbon(newItem, thing, MainRibbon);
                }
            }
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public Ribbon GetRibbonFromView(UserControl thing)
        {
            var foundControl = thing.FindName("MainMenu") as Ribbon;
            return foundControl;
        }

        protected void MergeRibbon(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            MergeApplicationMenu(sourceView, moduleRibbon, ribbon);
            MergeQuickAccessToolbar(sourceView, moduleRibbon, ribbon);
            MergeContextualTabGroups(sourceView, moduleRibbon, ribbon);
            MergeItemsControl(sourceView, moduleRibbon, ribbon);
        }

        protected void MergeQuickAccessToolbar(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            if (moduleRibbon.QuickAccessToolBar != null)
            {
                if (ribbon.QuickAccessToolBar == null)
                    ribbon.QuickAccessToolBar = new RibbonQuickAccessToolBar();
                MergeItemsControl(sourceView, moduleRibbon.QuickAccessToolBar, ribbon.QuickAccessToolBar);
            }
        }

        protected void MergeApplicationMenu(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            if (moduleRibbon.ApplicationMenu != null)
            {
                if (ribbon.ApplicationMenu == null)
                    ribbon.ApplicationMenu = new RibbonApplicationMenu();
                MergeItemsControl(sourceView, moduleRibbon.ApplicationMenu, ribbon.ApplicationMenu);
            }
        }

        protected void MergeContextualTabGroups(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            foreach (RibbonContextualTabGroup group in moduleRibbon.ContextualTabGroups)
            {
                if (!ribbon.ContextualTabGroups.Any(t => UiElementsHaveSameId(t, group)))
                {
                    DisconnectFromParent(group);
                    if (group.DataContext == null)
                        group.DataContext = moduleRibbon.DataContext;
                    InsertItem(sourceView, group, ribbon.ContextualTabGroups);
                }
            }
        }

        protected void MergeItems(IList list, ItemsControl target)
        {
            if (list == null)
                return;

            foreach (object item in list)
            {
                if (item is ItemsControl)
                    MergeItemsControl(item, (ItemsControl)item, target);
                else
                    MergeNonItemsControl(item as UIElement, target);
            }
        }

        protected void MergeNonItemsControl(UIElement item, ItemsControl target)
        {
            if (item == null)
                return;
            InsertItem(item, item, target.Items);
        }

        protected void MergeItemsControl(object sourceView, ItemsControl source, ItemsControl target)
        {
            var items = source.Items.Cast<UIElement>().ToArray();
            foreach (UIElement item in items)
            {
                if (item is ItemsControl)
                {
                    var existingItem = target.Items
                        .OfType<ItemsControl>()
                        .FirstOrDefault(t => UiElementsHaveSameId(t, item));
                    if (existingItem == null)
                        InsertItem(sourceView, item, target.Items);
                    else
                        MergeItemsControl(sourceView, (ItemsControl)item, existingItem);
                }
                else
                {
                    InsertItem(sourceView, item, target.Items);
                }
            }
        }

        protected void InsertItem(object sourceView, UIElement item, IList target)
        {
            DisconnectFromParent(item);
            InsertSorted(item, target);
        }

        protected internal bool UiElementsHaveSameId(UIElement item1, UIElement item2)
        {
            var item1Id = GetUiElementHeaderOrName(item1);
            var item2Id = GetUiElementHeaderOrName(item2);

            return item1Id.Equals(item2Id);
        }

        protected string GetUiElementHeaderOrName(UIElement item)
        {
            var key = string.Empty;

            if (item is HeaderedItemsControl)
                key = ((HeaderedItemsControl)item).Header as string;
            if (string.IsNullOrEmpty(key) && item is FrameworkElement)
                key = ((FrameworkElement)item).Name;

            return key;
        }

        public void InsertSorted(UIElement item, IList collection)
        {
            var order = UIElementExtension.GetMergeOrder(item);
            if (Math.Abs(order - DefaultMergeOrder) < 0.001)
            {
                order = DefaultMergeOrder;
                UIElementExtension.SetMergeOrder(item, order);
            }

            int insertPosition = 0;
            foreach (UIElement t in collection)
            {
                var curOrder = UIElementExtension.GetMergeOrder(t);
                if (curOrder > order)
                    break;
                insertPosition++;
            }
            DisconnectFromParent(item);
            collection.Insert(insertPosition, item);
        }

        public T GetChildOfType<T>(DependencyObject depObj)
                where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public DependencyObject GetParent(UIElement child)
        {
            DependencyObject parent = null;
            if (child is FrameworkElement)
                parent = ((FrameworkElement)child).Parent;

            if (parent == null)
                parent = VisualTreeHelper.GetParent(child);

            return parent;
        }

        public object GetDataContext(UIElement child, DependencyObject parent)
        {
            object dataContext = null;
            if (child is FrameworkElement)
            {
                dataContext = ((FrameworkElement)child).DataContext;
                if (dataContext == null && parent is FrameworkElement)
                    dataContext = ((FrameworkElement)parent).DataContext;
            }

            return dataContext;
        }

        public void DisconnectFromParent(UIElement child)
        {
            var parent = GetParent(child);

            var dataContext = GetDataContext(child, parent);

            try
            {
                //var parentAsPresenter = parent as System.Windows.Controls.ContentPresenter;
                //if (parentAsPresenter != null)
                //{
                //    parentAsPresenter.Content = null;
                //    return;
                //}
                //var parentAsPanel = parent as System.Windows.Controls.Panel;
                //if (parentAsPanel != null)
                //{
                //    parentAsPanel.Children.Remove(child);
                //    return;
                //}
                //var parentAsContentControl = parent as System.Windows.Controls.ContentControl;
                //if (parentAsContentControl != null)
                //{
                //    parentAsContentControl.Content = null;
                //    return;
                //}
                //var parentAsDecorator = parent as System.Windows.Controls.Decorator;
                //if (parentAsDecorator != null)
                //{
                //    parentAsDecorator.Child = null;
                //    return;
                //}
                var parentAsItemsControl = parent as System.Windows.Controls.ItemsControl;
                if (parentAsItemsControl != null)
                {
                    parentAsItemsControl.Items.Remove(child);
                }
            }
            finally
            {
                if ( child is FrameworkElement && dataContext != null)
                    ((FrameworkElement)child).DataContext = dataContext;
            }
        }

    }
}
