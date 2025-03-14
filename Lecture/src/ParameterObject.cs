
public class ParameterObject
{
    class Account
    {
        public double GetFlowBetween(DateRange range)
        {
            double result = 0;
            List<DateTime> entries = new List<DateTime>();
            foreach (var each in entries)
            {
                if (range.Includes(each.GetDate()))
                {
                    result += each.GetValue();
                }
            }

            return result;
        }
    }
}

// client code...
// double flow = anAccount.getFlowBetween(startDate, endDate);