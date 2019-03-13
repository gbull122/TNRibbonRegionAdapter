using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace TNRibbonRegionAdapter
{
    public class RibbonBehavoir : RegionBehavior
    {
        public static readonly string BehaviorKey = "RibbonBehavior";

        private Ribbon mainRibbon;

        public Ribbon MainRibbon
        {
            get { return mainRibbon; }
            set { mainRibbon = value; }
        }

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

        public Ribbon GetRibbonFromView(UserControl thing)
        {
            var foundControl = thing.FindName("MainMenu") as Ribbon;
            return foundControl;
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        protected virtual internal void MergeRibbon(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            MergeApplicationMenu(sourceView, moduleRibbon, ribbon);
            MergeQuickAccessToolbar(sourceView, moduleRibbon, ribbon);
            MergeContextualTabGroups(sourceView, moduleRibbon, ribbon);
            MergeItemsControl(sourceView, moduleRibbon, ribbon);
        }

        protected virtual internal void MergeQuickAccessToolbar(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            if (moduleRibbon.QuickAccessToolBar != null)
            {
                if (ribbon.QuickAccessToolBar == null)
                    ribbon.QuickAccessToolBar = new RibbonQuickAccessToolBar();
                MergeItemsControl(sourceView, moduleRibbon.QuickAccessToolBar, ribbon.QuickAccessToolBar);
            }
        }

        protected virtual internal void MergeApplicationMenu(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            if (moduleRibbon.ApplicationMenu != null)
            {
                if (ribbon.ApplicationMenu == null)
                    ribbon.ApplicationMenu = new RibbonApplicationMenu();
                MergeItemsControl(sourceView, moduleRibbon.ApplicationMenu, ribbon.ApplicationMenu);
            }
        }

        protected virtual internal void MergeContextualTabGroups(object sourceView, Ribbon moduleRibbon, Ribbon ribbon)
        {
            foreach (RibbonContextualTabGroup group in moduleRibbon.ContextualTabGroups)
            {
                if (!ribbon.ContextualTabGroups.Any(t => ItemsMatch(t, group)))
                {
                    group.DisconnectFromParent();
                    if (group.DataContext == null)
                        group.DataContext = moduleRibbon.DataContext;
                    InsertItem(sourceView, group, ribbon.ContextualTabGroups);
                }
            }
        }

        protected virtual void MergeItems(IList list, ItemsControl target)
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

        protected virtual internal void MergeNonItemsControl(UIElement item, ItemsControl target)
        {
            if (item == null)
                return;
            InsertItem(item, item, target.Items);
        }

        protected virtual internal void MergeItemsControl(object sourceView, ItemsControl source, ItemsControl target)
        {
            var items = source.Items.Cast<UIElement>().ToArray();
            foreach (UIElement item in items)
            {
                if (item is ItemsControl)
                {
                    var existingItem = target.Items
                        .OfType<ItemsControl>()
                        .FirstOrDefault(t => ItemsMatch(t, item));
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

        private readonly Dictionary<object, List<UIElement>> _mergedItemsByView = new Dictionary<object, List<UIElement>>();

        protected void InsertItem(object sourceView, UIElement item, IList target)
        {
            RememberMergedItem(sourceView, item);
            item.DisconnectFromParent();
            UIElementExtension.InsertSorted(item, target);
        }

        private void RememberMergedItem(object sourceView, UIElement item)
        {
            if (!_mergedItemsByView.ContainsKey(sourceView))
                _mergedItemsByView.Add(sourceView, new List<UIElement>());
            var list = _mergedItemsByView[sourceView];
            list.Add(item);
        }

        protected virtual internal void UnmergeItems(IList list, ItemsControl target)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                Unmerge(item, target);
            }
        }

        protected virtual void Unmerge(object view, ItemsControl target)
        {
            var list = GetMergedItemsByView(view);
            if (list == null)
                return;
            list.ForEach(i => i.DisconnectFromParent(false));
            _mergedItemsByView.Remove(view);
        }

        protected virtual internal List<UIElement> GetMergedItemsByView(object view)
        {
            if (!_mergedItemsByView.ContainsKey(view))
                return null;
            var list = _mergedItemsByView[view];
            return list;
        }

        protected virtual internal bool ItemsMatch(UIElement item1, UIElement item2)
        {
            var tab1Id = GetMergeKey(item1);
            var tab2Id = GetMergeKey(item2);

            return tab1Id.Equals(tab2Id);
        }

        protected virtual internal string GetMergeKey(UIElement item)
        {
            var key = UIElementExtension.GetMergeKey(item);
            if (string.IsNullOrEmpty(key))
            {
                if (item is HeaderedItemsControl)
                    key = ((HeaderedItemsControl)item).Header as string;
                if (string.IsNullOrEmpty(key) && item is FrameworkElement)
                    key = ((FrameworkElement)item).Name;
                if (string.IsNullOrEmpty(key))
                    key = Guid.NewGuid().ToString();
                UIElementExtension.SetMergeKey(item, key);
            }
            return key;
        }
    }
}
