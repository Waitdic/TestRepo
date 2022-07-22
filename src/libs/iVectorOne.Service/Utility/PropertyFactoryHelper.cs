namespace iVectorOne.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyFactoryHelper
    {
        /// <summary>
        /// Split the given integer into a list with elements of given digits
        /// </summary>
        /// <param name="number">A number which is going to be split</param>
        /// <param name="digit">A number of digits per element in the list</param>
        /// <returns>A list with number as an element of given length</returns>
        public static List<int> SplitNumberToNDigitList(int number, int digit)
        {
            var list = new List<int>();
            var upperBound = Math.Pow(10, digit) - 1; // a max number which given n digit number can be i.e. if 2 digits then upperBound is 99
            var numOfDigits = (int)Math.Log10(number) + 1;

            if (number > upperBound)
            {
                var numberString = number.ToString();
                for (int i = 0; list.Count() < Math.Ceiling((decimal)numOfDigits / digit);)
                {
                    if (i == 0 && numOfDigits % digit != 0)
                    {
                        list.Add(int.Parse(numberString.Substring(i, 1)));
                        ++i;
                    }
                    else
                    {
                        list.Add(int.Parse(numberString.Substring(i, digit)));
                        i += digit;
                    }
                }
            }
            else
            {
                list.Add(number);
            }

            return list;
        }

        // todo - make this part of the token logic
        /// <summary>
        /// Get a combination of all numbers stored in the list as like string concatination 
        /// Pads a number with zeros if given number is less then specified num of digits
        /// </summary>
        /// <param name="list">A list storing numbers</param>
        /// <param name="digits">A number of digits of a number stored in each element</param>
        /// <returns>A number which is concatination of all numbers in a list</returns>
        public static int GetNumFromList(List<int> list, int digits)
        {
            int numOfDecimals = (int)Math.Pow(10, digits);
            var number = 0;
            for (int i = 0; i < list.Count(); ++i)
            {
                number = number * numOfDecimals + list[i];
            }
            return number;
        }
    }
}