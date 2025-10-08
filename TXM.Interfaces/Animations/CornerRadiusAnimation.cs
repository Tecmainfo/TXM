namespace TXM.Interfaces.Animations
    {
    public class CornerRadiusAnimation : AnimationTimeline
        {
        public IEasingFunction EasingFunction
            {
            get => (IEasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
            }

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(nameof(EasingFunction), typeof(IEasingFunction), typeof(CornerRadiusAnimation));

        public CornerRadius From
            {
            get => (CornerRadius)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
            }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(CornerRadius), typeof(CornerRadiusAnimation));

        public CornerRadius To
            {
            get => (CornerRadius)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
            }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(CornerRadius), typeof(CornerRadiusAnimation));

        public override Type TargetPropertyType => typeof(CornerRadius);

        protected override Freezable CreateInstanceCore()
            {
            return new CornerRadiusAnimation();
            }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
            {
            CornerRadius from = From;
            CornerRadius to = To;

            double progress = animationClock.CurrentProgress ?? 0.0;
            if (EasingFunction is not null)
                {
                progress = EasingFunction.Ease(progress);
                }

            static CornerRadius Interp(CornerRadius a, CornerRadius b, double t)
                {
                return new CornerRadius(
                    a.TopLeft + ((b.TopLeft - a.TopLeft) * t),
                    a.TopRight + ((b.TopRight - a.TopRight) * t),
                    a.BottomRight + ((b.BottomRight - a.BottomRight) * t),
                    a.BottomLeft + ((b.BottomLeft - a.BottomLeft) * t));
                }

            return Interp(from, to, progress);
            }
        }
    }
