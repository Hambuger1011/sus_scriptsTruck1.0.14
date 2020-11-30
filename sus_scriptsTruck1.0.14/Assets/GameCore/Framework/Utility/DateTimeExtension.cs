using Framework;

using System;

public static class DateTimeExtension
{

    public static enSgameDayOfWeek GetDayOfWeek(this DateTime timeNow)
	{
		if (timeNow.DayOfWeek == DayOfWeek.Sunday)
		{
			return enSgameDayOfWeek.enSunday;
		}
		return (enSgameDayOfWeek)timeNow.DayOfWeek;
	}
}

public enum enSgameDayOfWeek
{
    enMonday = 1,
    enTuesday,
    enWednesday,
    enThursday,
    enFriday,
    enSaturday,
    enSunday
}
