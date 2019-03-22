namespace Zebble
{
    using System;
    using System.Threading.Tasks;

    public class SearchInput : Canvas
    {
        float CancelButtonActualWidth;
        bool FirstRun = true;
        public readonly ImageView Icon = new ImageView { Id = "Icon", Enabled = false, Absolute = true };
        public readonly TextInput TextBox = new TextInput { KeyboardActionType = KeyboardActionType.Search, Id = "TextBox", Placeholder = "Search" };
        public readonly Button CancelButton = new Button { Text = "Cancel", Id = "CancelButton", Absolute = true };
        public readonly AsyncEvent Searched = new AsyncEvent();
        public readonly AsyncEvent Searching = new AsyncEvent();

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            await Add(TextBox);
            TextBox.UserFocusChanged.Handle(FocusChanged);
            TextBox.UserTextChanged.Handle(Searching.Raise);
            TextBox.UserTextChangeSubmitted.Handle(Searched.Raise);

            await Add(Icon);

            CancelButton.PreRendered.Handle(Arrange);
            await Add(CancelButton);

            this.On(x => x.Shown, WhenShown);
        }

        Task WhenShown()
        {
            CancelButtonActualWidth = CancelButton.ActualWidth;
            var iconXPosition = CalculateIconX(focused: false);
            Icon.X(iconXPosition);

            return Task.CompletedTask;
        }

        Task Arrange()
        {
            CancelButton.Width(CancelButton.Font.GetTextWidth(CancelButton.Text.OrEmpty()) +
                CancelButton.Padding.Horizontal());

            var cancelButtonXPosition = CalculateCancelButtonX(focused: false);
            var iconXPosition = CalculateIconX(focused: false);

            CancelButton.X(cancelButtonXPosition);
            Icon.X(iconXPosition);

            return Task.CompletedTask;
        }

        public void Focus() => TextBox.Focus();

        public void UnFocus() => TextBox.UnFocus();

        float CalculateIconX(bool focused)
        {
            var iconX = Padding.Left();
            if (TextBox.TextAlignment.ToHorizontalAlignment() == HorizontalAlignment.Center)
            {
                var textWidth = TextBox.Font.GetTextWidth(TextBox.Text.Or(TextBox.Placeholder));
                var textStart = (TextBox.ActualWidth + TextBox.Padding.Horizontal() - textWidth) / 2;
                if (!FirstRun && !focused)
                {
                    textStart += CancelButtonActualWidth / 2;
                }

                iconX = textStart - TextBox.Padding.Left() - TextBox.Margin.Left();

                iconX -= Icon.ActualWidth;
                iconX -= Icon.Margin.Right();

                iconX = Math.Max(iconX, Padding.Left());
            }

            return iconX;
        }

        float CalculateCancelButtonX(bool focused)
        {
            if (focused)
            {
                return (Padding.Horizontal() + TextBox.ActualWidth + TextBox.Margin.Horizontal() + CancelButton.Margin.Left()) - CancelButtonActualWidth;
            }

            return ActualWidth + Margin.Horizontal();
        }

        public string Text
        {
            get => TextBox.Text.TrimOrEmpty();
            set => TextBox.Text = value.TrimOrEmpty();
        }

        async Task FocusChanged(bool focused)
        {
            FirstRun = false;

            SetPseudoCssState("focus", focused).RunInParallel();
            var cancelButtonXPosition = CalculateCancelButtonX(focused);
            var iconXPosition = CalculateIconX(focused);

            CancelButton.Width(focused ? CancelButtonActualWidth : 0);
            CancelButton.Animate(cb => cb.X(cancelButtonXPosition)).RunInParallel();
            Icon.Animate(i => i.X(iconXPosition)).RunInParallel();

            await TextBox.Animate(txt => txt.Width(ActualWidth - Padding.Horizontal() - TextBox.Margin.Horizontal() -
                (focused ? CancelButton.ActualWidth + CancelButton.Margin.Horizontal() : 0)));
        }

        public override void Dispose()
        {
            Searched?.Dispose();
            Searching?.Dispose();
            base.Dispose();
        }
    }
}