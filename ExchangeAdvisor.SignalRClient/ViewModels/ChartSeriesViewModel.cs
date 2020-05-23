using System;
using System.Collections.Generic;
using System.Drawing;
using ExchangeAdvisor.Domain.Extensions;

namespace ExchangeAdvisor.SignalRClient.ViewModels
{
    public class ChartSeriesViewModel<T>
    {
        public string Name { get; }

        public IReadOnlyCollection<T> DataSource { get; }

        public string ColorHexCode { get; set; }

        public ChartSeriesViewModel(string name, IReadOnlyCollection<T> dataSource, Color color)
            : this(name, dataSource)
        {
            ColorHexCode = color.ToHexCode();
        }

        public ChartSeriesViewModel(string name) : this(name, dataSource: Array.Empty<T>()) { }

        public ChartSeriesViewModel(string name, IReadOnlyCollection<T> dataSource)
        {
            Name = name;
            DataSource = dataSource ?? Array.Empty<T>();
        }
    }
}