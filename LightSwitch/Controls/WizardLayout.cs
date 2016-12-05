using System;
using Xamarin.Forms;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace LightSwitch
{
    /// <summary>
    /// Wizard layout.
    /// </summary>
	[ContentProperty("Pages")]
	public class WizardLayout: Grid
    {
        #region Private Members

        readonly WizardStackLayout _contentStack;
		readonly RelativeLayout _layout;
		readonly PagerControl _pager;
		readonly ObservableCollection<View> _pages = new ObservableCollection<View>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HomeDrive.App.WizardLayout"/> class.
        /// </summary>
        public WizardLayout()
        {
			_pages.CollectionChanged += PagesChanged;
			
            // Wrapping layout
            _layout = new RelativeLayout();
            Children.Add(_layout);

            // Content
            _contentStack = new WizardStackLayout();

			// Pager
			_pager = new PagerControl();

			var bottomHeight = PagerControl.PagerHeight;

            _layout.Children.Add(_contentStack, () => new Rectangle(0, 0, _layout.Bounds.Width * GetChildCount(),
                _layout.Bounds.Height - bottomHeight));

            _layout.Children.Add(_pager, () => new Rectangle(0, _layout.Bounds.Height - bottomHeight, 
                _layout.Bounds.Width, bottomHeight));
			         
        }

        #region Properties

        /// <summary>
        /// The page property.
        /// </summary>
        public static BindableProperty PageProperty = BindableProperty.Create(nameof(Page),
            typeof(int), typeof(WizardLayout), 0, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) => { 
            var ctrl = (WizardLayout)bindable;
            ctrl.Page = (int)newValue;
        });

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        public int Page
        {
            get { return (int)GetValue(PageProperty); }
            set 
            { 
                if (value >= GetChildCount())
                    return;
                
                SetValue(PageProperty, value);

                // focus first entry
                FocusFirstEntry();

                Device.BeginInvokeOnMainThread(async () => await _contentStack.TranslateTo(-(Width * value), 0));

				_pager.Page = value;
            }                
        }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
		public ObservableCollection<View> Pages
        {
            get { return _pages; }            
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Pageses the changed.
        /// </summary>
        /// <returns>The changed.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void PagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePages();      
        }

        /// <summary>
        /// Childs the property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Child_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
            {
                _contentStack.ForceLayout();
                _layout.ForceLayout();

                UpdatePager();
                FocusFirstEntry();
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Updates the pager.
        /// </summary>
        private void UpdatePager()
        {
        	_pager.PageCount = GetChildCount();
            _pager.IsVisible = _pager.PageCount > 1;
        }

        /// <summary>
        /// Updates the pages.
        /// </summary>
        /// <returns>The pages.</returns>
        private void UpdatePages()
        {
            foreach(var child in _contentStack.Children)
                child.PropertyChanged -= Child_PropertyChanged;
            
            _contentStack.Children.Clear();

            if (Pages != null)
            {
                foreach (var item in Pages)
                {
                    item.PropertyChanged += Child_PropertyChanged;                
                    _contentStack.Children.Add(new ContentView { Content = item });
                }            

                UpdatePager();
                FocusFirstEntry();
            }
            else
            {
				_pager.PageCount = 0;
            }
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <returns>The child count.</returns>
        private int GetChildCount()
        {
            return _contentStack.Children.Count(c => (c as ContentView).Content.IsVisible);
        }

        /// <summary>
        /// Focuses the first entry.
        /// </summary>
        /// <returns>The first entry.</returns>
        private void FocusFirstEntry()
        {
            if (Page < GetChildCount())
            {
                var activeControl = _contentStack.Children.ElementAt(Page);
                var firstEntry = GetFirstEntry((activeControl as ContentView).Content as IViewContainer<View>);
                if (firstEntry != null)
                    firstEntry.Focus();
            }
        }

        /// <summary>
        /// Gets the first entry.
        /// </summary>
        /// <returns>The first entry.</returns>
        /// <param name="viewContainer">View container.</param>
        private Entry GetFirstEntry(IViewContainer<View> viewContainer)
        {
            if (viewContainer == null)
                return null;
            
            var firstEntry = viewContainer.Children.FirstOrDefault(mx => mx is Entry) as Entry;
            if(firstEntry != null)
                return firstEntry;

            foreach (var child in viewContainer.Children)
                if (child is IViewContainer<View>)
                    return GetFirstEntry(child as IViewContainer<View>);

            return null;
        }

        #endregion

        #region Overridden Members

        /// <param name="x">A value representing the x coordinate of the child region bounding box.</param>
        /// <param name="y">A value representing the y coordinate of the child region bounding box.</param>
        /// <param name="width">A value representing the width of the child region bounding box.</param>
        /// <param name="height">A value representing the height of the child region bounding box.</param>
        /// <summary>
        /// Positions and sizes the children of a Layout.
        /// </summary>
        /// <remarks>Implementors wishing to change the default behavior of a Layout should override this method. It is suggested
        /// to still call the base method and modify its calculated results.</remarks>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            // Ensure that the current page is reflected in the current transform
            var translation = -(Width * Page);
			if(!_contentStack.TranslationX.Equals(translation))
                Device.BeginInvokeOnMainThread(async () => await _contentStack.TranslateTo(translation, 0, 0));
        }
        #endregion

        #region Nested Classes

        /// <summary>
        /// Wizard scroll view.
        /// </summary>
        private class WizardStackLayout : Layout<View>
        {
            /// <summary>
            /// Layouts the children.
            /// </summary>
            /// <returns>The children.</returns>
            /// <param name="x">The x coordinate.</param>
            /// <param name="y">The y coordinate.</param>
            /// <param name="width">Width.</param>
            /// <param name="height">Height.</param>
            protected override void LayoutChildren(double x, double y, double width, double height)
            {
                if (Children.Count == 0)
                    return;

                var visibleChildren = Children.Where(ch => (ch as ContentView).Content.IsVisible);

                var childWidth = (width / visibleChildren.Count());
                foreach (var child in visibleChildren)
                {
                    child.Layout(new Rectangle(x, y, childWidth, height));
                    x += childWidth;
                }
            }
        }
        #endregion
    }

	
}

