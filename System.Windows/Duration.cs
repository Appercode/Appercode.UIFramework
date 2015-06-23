using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows
{
    public struct Duration
    {
        private TimeSpan timeSpan;

        private Duration.DurationType durationType;

        public Duration(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("", "timeSpan");
            }
            this.durationType = Duration.DurationType.TimeSpan;
            this.timeSpan = timeSpan;
        }

        private enum DurationType
        {
            Automatic,
            TimeSpan,
            Forever
        }

        public static Duration Automatic
        {
            get
            {
                Duration duration = new Duration();
                duration.durationType = Duration.DurationType.Automatic;
                return duration;
            }
        }

        public static Duration Forever
        {
            get
            {
                Duration duration = new Duration();
                duration.durationType = Duration.DurationType.Forever;
                return duration;
            }
        }

        public bool HasTimeSpan
        {
            get
            {
                return this.durationType == Duration.DurationType.TimeSpan;
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                if (!this.HasTimeSpan)
                {
                    throw new InvalidOperationException();
                }
                return this.timeSpan;
            }
        }

        public static int Compare(Duration t1, Duration t2)
        {
            if (t1.durationType == Duration.DurationType.Automatic)
            {
                if (t2.durationType == Duration.DurationType.Automatic)
                {
                    return 0;
                }
                return -1;
            }
            if (t2.durationType == Duration.DurationType.Automatic)
            {
                return 1;
            }
            if (t1 < t2)
            {
                return -1;
            }
            if (t1 > t2)
            {
                return 1;
            }
            return 0;
        }

        public static bool Equals(Duration t1, Duration t2)
        {
            return t1.Equals(t2);
        }

        public static Duration operator +(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return new Duration(t1.timeSpan + t2.timeSpan);
            }
            if (t1.durationType != Duration.DurationType.Automatic && t2.durationType != Duration.DurationType.Automatic)
            {
                return Duration.Forever;
            }
            return Duration.Automatic;
        }

        public static bool operator ==(Duration t1, Duration t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator >(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return t1.timeSpan > t2.timeSpan;
            }
            if (t1.HasTimeSpan && t2.durationType == Duration.DurationType.Forever)
            {
                return false;
            }
            if (t1.durationType == Duration.DurationType.Forever && t2.HasTimeSpan)
            {
                return true;
            }
            return false;
        }

        public static bool operator >=(Duration t1, Duration t2)
        {
            if (t1.durationType == Duration.DurationType.Automatic && t2.durationType == Duration.DurationType.Automatic)
            {
                return true;
            }
            if (t1.durationType == Duration.DurationType.Automatic || t2.durationType == Duration.DurationType.Automatic)
            {
                return false;
            }
            return !(t1 < t2);
        }

        public static implicit operator Duration(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("", "timeSpan");
            }
            return new Duration(timeSpan);
        }

        public static bool operator !=(Duration t1, Duration t2)
        {
            return !t1.Equals(t2);
        }

        public static bool operator <(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return t1.timeSpan < t2.timeSpan;
            }
            if (t1.HasTimeSpan && t2.durationType == Duration.DurationType.Forever)
            {
                return true;
            }
            if (t1.durationType == Duration.DurationType.Forever && t2.HasTimeSpan)
            {
                return false;
            }
            return false;
        }

        public static bool operator <=(Duration t1, Duration t2)
        {
            if (t1.durationType == Duration.DurationType.Automatic && t2.durationType == Duration.DurationType.Automatic)
            {
                return true;
            }
            if (t1.durationType == Duration.DurationType.Automatic || t2.durationType == Duration.DurationType.Automatic)
            {
                return false;
            }
            return !(t1 > t2);
        }

        public static Duration operator -(Duration t1, Duration t2)
        {
            if (t1.HasTimeSpan && t2.HasTimeSpan)
            {
                return new Duration(t1.timeSpan - t2.timeSpan);
            }
            if (t1.durationType == Duration.DurationType.Forever && t2.HasTimeSpan)
            {
                return Duration.Forever;
            }
            return Duration.Automatic;
        }

        public static Duration operator +(Duration duration)
        {
            return duration;
        }

        public static Duration Plus(Duration duration)
        {
            return duration;
        }

        public Duration Add(Duration duration)
        {
            return this + duration;
        }

        public override bool Equals(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (!(value is Duration))
            {
                return false;
            }
            return this.Equals((Duration)value);
        }

        public bool Equals(Duration duration)
        {
            if (!this.HasTimeSpan)
            {
                return this.durationType == duration.durationType;
            }
            if (!duration.HasTimeSpan)
            {
                return false;
            }
            return this.timeSpan == duration.timeSpan;
        }

        public override int GetHashCode()
        {
            if (this.HasTimeSpan)
            {
                return this.timeSpan.GetHashCode();
            }
            return this.durationType.GetHashCode() + 17;
        }

        public Duration Subtract(Duration duration)
        {
            return this - duration;
        }

        public override string ToString()
        {
            if (this.HasTimeSpan)
            {
                return this.timeSpan.ToString();
            }
            if (this.durationType == Duration.DurationType.Forever)
            {
                return "Forever";
            }
            return "Automatic";
        }

        internal static Duration Create(object o)
        {
            if (o == null)
            {
                TimeSpan timeSpan = new TimeSpan();
                return new Duration(timeSpan);
            }
            string str = o as string;
            if (str == null)
            {
                return new Duration(TimeSpan.FromSeconds((double)o));
            }
            if (str.Equals("Forever"))
            {
                return Duration.Forever;
            }
            return Duration.Automatic;
        }
    }
}