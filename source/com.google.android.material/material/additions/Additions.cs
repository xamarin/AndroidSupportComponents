using System;

namespace Google.Android.Material.Animation
{
    public partial class ArgbEvaluatorCompat
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (Java.Lang.Integer)startValue, (Java.Lang.Integer)endValue);
    }

    public partial class MatrixEvaluator
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (global::Android.Graphics.Matrix)startValue, (global::Android.Graphics.Matrix)endValue);
    }
}

namespace Google.Android.Material.AppBar
{
    public partial class CollapsingToolbarLayout
    {
        public override global::Android.Views.ViewStates Visibility
        {
            get => base.Visibility;
            set => base.Visibility = value;
        }

        public void SetTitle (string title) =>
            Title = title;

        public void SetVisibility (global::Android.Views.ViewStates visibility) =>
            Visibility = visibility;
    }
}

namespace Google.Android.Material.BottomNavigation
{
    public partial class BottomNavigationItemView 
    {
        void global::AndroidX.AppCompat.View.Menu.IMenuViewItemView.SetEnabled(bool enabled) =>
            Enabled = enabled;
    }
}

namespace Google.Android.Material.CircularReveal
{
    public partial class CircularRevealWidgetCircularRevealEvaluator
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (CircularRevealWidgetRevealInfo)startValue, (CircularRevealWidgetRevealInfo)endValue);
    }

    public partial class CircularRevealWidgetCircularRevealProperty 
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) => 
            Get((Google.Android.Material.CircularReveal.ICircularRevealWidget)@object);
    }

    public partial class CircularRevealWidgetCircularRevealScrimColorProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((Google.Android.Material.CircularReveal.ICircularRevealWidget)@object);
    }
}

namespace Google.Android.Material.Internal
{
    public partial class NavigationMenuItemView
    {
        void global::AndroidX.AppCompat.View.Menu.IMenuViewItemView.SetEnabled(bool enabled) =>
            Enabled = enabled;
    }
}

namespace Google.Android.Material.Snackbar
{
    public partial class Snackbar
    {
        public Snackbar SetAction(string text, Action<global::Android.Views.View> clickHandler) =>
            SetAction(text, new SnackbarActionClickImplementor(clickHandler));

        public Snackbar SetAction(Java.Lang.ICharSequence text, Action<global::Android.Views.View> clickHandler) =>
            SetAction(text, new SnackbarActionClickImplementor(clickHandler));

        public Snackbar SetAction(int resId, Action<global::Android.Views.View> clickHandler) =>
            SetAction(resId, new SnackbarActionClickImplementor(clickHandler));

        internal class SnackbarActionClickImplementor : Java.Lang.Object, global::Android.Views.View.IOnClickListener
        {
            private Action<global::Android.Views.View> handler;

            public SnackbarActionClickImplementor(Action<global::Android.Views.View> handler) =>
                this.handler = handler;

            public void OnClick(global::Android.Views.View v) =>
                handler?.Invoke(v);
        }
    }
}
