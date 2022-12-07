using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows;

namespace MystatDesktopWpf.Domain
{
    [ContentProperty("Bindings")]
    internal class DelayedMultiBindingExtension : MarkupExtension, IMultiValueConverter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Collection<BindingBase> Bindings { get; }

        public IMultiValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public CultureInfo ConverterCulture { get; set; }

        public BindingMode Mode { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        private object undelayedValue;
        private object delayedValue;
        private DispatcherTimer timer;

        public object CurrentValue
        {
            get { return delayedValue; }
            set
            {
                delayedValue = undelayedValue = value;
                timer.Stop();
            }
        }

        public int ChangeCount { get; private set; }

        public TimeSpan Delay
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        public DelayedMultiBindingExtension()
        {
            this.Bindings = new Collection<BindingBase>();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(10);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var valueProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueProvider == null) return null;

            var bindingTarget = valueProvider.TargetObject as DependencyObject;
            var bindingProperty = valueProvider.TargetProperty as DependencyProperty;

            var multi = new MultiBinding { Converter = this, Mode = Mode, UpdateSourceTrigger = UpdateSourceTrigger };
            foreach (var binding in Bindings) multi.Bindings.Add(binding);
            multi.Bindings.Add(new Binding("ChangeCount") { Source = this, Mode = BindingMode.OneWay });

            var bindingExpression = BindingOperations.SetBinding(bindingTarget, bindingProperty, multi);

            return bindingTarget.GetValue(bindingProperty);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var newValue = Converter.Convert(values.Take(values.Length - 1).ToArray(),
                                             targetType,
                                             ConverterParameter,
                                             ConverterCulture ?? culture);

            // Фикс ошибок бинднигов
            delayedValue ??= newValue;

            if (Equals(newValue, undelayedValue)) return delayedValue;
            undelayedValue = newValue;
            timer.Stop();
            timer.Start();

            return delayedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Converter.ConvertBack(value, targetTypes, ConverterParameter, ConverterCulture ?? culture)
                            .Concat(new object[] { ChangeCount }).ToArray();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            delayedValue = undelayedValue;
            ChangeCount++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeCount)));
        }
    }
}
