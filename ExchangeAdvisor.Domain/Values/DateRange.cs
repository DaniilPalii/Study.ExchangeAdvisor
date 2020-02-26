using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Values
{
    public struct DateRange
    {
        public readonly DateTime Start;

        public readonly DateTime End;

        public IEnumerable<DateTime> Days
        {
            get
            {
                for (var day = Start; day < End; day = day.AddDays(1))
                    yield return day;
            }
        }

        public DateRange(DateTime end) : this(DateTime.MinValue, end) { }

        public DateRange(DateTime start, DateTime end)
        {
            start = start.Date;
            end = end.Date;

            if (end < start)
            {
                throw new ArgumentException(
                    $"Start date is {start} and end is {end} "
                        + "but end date should be greater or equal to start date");
            }

            Start = start;
            End = end;
        }

        public bool DoesContain(DateTime date)
        {
            date = date.Date;

            return Start <= date && End >= date;
        }

        public override bool Equals(object obj)
        {
            return obj is DateRange dateRange
                && dateRange.Start == Start
                && dateRange.End == End;
        }

        public override int GetHashCode() => (Start, End).GetHashCode();

        public static bool operator ==(DateRange left, DateRange right) => left.Equals(right);

        public static bool operator !=(DateRange left, DateRange right) => !(left == right);
    }
}
