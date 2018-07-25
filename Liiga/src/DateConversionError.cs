using System;

namespace Liiga
{
    public class DateConversionError: Exception
    {
        public string failedInput;
        public DateConversionError(string inputDate)
        {
            failedInput = inputDate;
        }

    }
}
