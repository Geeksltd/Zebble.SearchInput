namespace Zebble
{
    using System;
    using System.Threading.Tasks;

    public class SearchInput : Canvas
    {
        float CancelButtonActualWidth;
        bool FirstRun = true;
        public readonly ImageView Icon = new ImageView { Id = "Icon", ZIndex = 100, Enabled = false }.Height(100.Percent()).Absolute().Stretch(Stretch.OriginalRatio);
        public readonly TextInput TextBox = new TextInput { KeyboardActionType = KeyboardActionType.Search, Id = "TextBox", Placeholder = "Search", ZIndex = 1 };
        public readonly Button CancelButton = new Button { Text = "Cancel", Id = "CancelButton", ZIndex = 1, Absolute = true };
        public readonly AsyncEvent Searched = new AsyncEvent();

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            await Add(TextBox);
            TextBox.UserFocusChanged.Handle(FocusChanged);
            TextBox.UserTextChangeSubmitted.Handle(Searched.Raise);

            await Add(Icon);

            CancelButton.PreRendered.Handle(Arrange);
            await Add(CancelButton);

            this.On(x => x.Shown, (Action)WhenShown);
        }

        void WhenShown()
        {
            CancelButtonActualWidth = CancelButton.ActualWidth;
            Icon.X(CalculateIconX(focused: false));
        }

        Task Arrange()
        {
            CancelButton.Width(CancelButton.Font.GetTextWidth(CancelButton.Text.OrEmpty()) +
                CancelButton.Padding.Horizontal());

            CancelButton.X(CalculateCancelButtonX(focused: false));

            Icon.X(CalculateIconX(focused: false));

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
                return Padding.Left() + TextBox.ActualWidth + TextBox.Margin.Horizontal() + CancelButton.Margin.Left();
            }

            return ActualWidth + Margin.Horizontal();
        }

        public string Text
        {
            get => TextBox.Text.TrimOrEmpty();
            set => TextBox.Text = value.TrimOrEmpty();
        }

        Task FocusChanged(bool focused)
        {
            FirstRun = false;
            void change()
            {
                CancelButton.Width(focused ? CancelButtonActualWidth : 0);
                SetPseudoCssState("focus", focused).RunInParallel();
                Icon.X(CalculateIconX(focused));
                TextBox.Width(ActualWidth - Padding.Horizontal() - TextBox.Margin.Horizontal() - (focused ? CancelButton.ActualWidth + CancelButton.Margin.Horizontal() : 0));
                CancelButton.X(CalculateCancelButtonX(focused));
            }

            // Ensure animations on all objects will apply:
            return TextBox.Animate(t => Icon.Animate(i => CancelButton.Animate(c => change()).RunInParallel()).RunInParallel());
        }

        public override void Dispose()
        {
            Searched?.Dispose();
            base.Dispose();
        }
    }
}