using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FriendOrganizer.UITests.Extentions
{
    public static class NotifyPropertyChangedExtentions
    {
        public static bool  IsPropertyChangedFired(
            this INotifyPropertyChanged notifyPropertyChanged,
            Action action, string propertyName)
        {
            var fired = false;
            notifyPropertyChanged.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    fired = true;
                }
            };
            action();

            return fired;

        }
    }
}
