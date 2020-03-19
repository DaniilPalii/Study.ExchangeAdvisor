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

        public static IDateRangeFrom FromMinDate() => From(DateTime.MinValue);

        public static IDateRangeFrom FromToday() => From(DateTime.Today);

        public static IDateRangeFrom From(int year, int month, int day) => From(new DateTime(year, month, day));

        public static IDateRangeFrom From(DateTime start) => new DateRangeFrom(start);

        private DateRange(DateTime start, DateTime end)
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

        private struct DateRangeFrom : IDateRangeFrom
        {
            public DateRangeFrom(DateTime start) => Start = start;

            public DateRange UntilMaxDate() => Until(DateTime.MaxValue);

            public DateRange UntilToday() => Until(DateTime.Today);

            public DateRange Until(int year, int month, int day) => Until(new DateTime(year, month, day));

            public DateRange Until(DateTime end) => new DateRange(Start, end);

            private readonly DateTime Start;
        }
    }

    public interface IDateRangeFrom
    {
        DateRange Until(DateTime end);

        DateRange Until(int year, int month, int day);

        DateRange UntilToday();

        DateRange UntilMaxDate();
    }
}
