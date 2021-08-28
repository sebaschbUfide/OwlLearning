using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mysqltest.Models
{
    public class RandomPassword
    {

        const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "123456789";
        const string SPECIALS = @"!@£$%^&*()#€";

        public string generateRandomPassword(int passwordSize)
        {
            char[] _password = new char[passwordSize];
            string charSet = ""; // Initialise to blank
            System.Random _random = new Random();
            int counter;

            charSet += LOWER_CASE;
            charSet += UPPER_CAES;
            charSet += NUMBERS;
            charSet += SPECIALS;


            for (counter = 0; counter < passwordSize; counter++)
            {
                _password[counter] = charSet[_random.Next(charSet.Length - 1)];
            }

            return String.Join(null, _password);

        }

    }
}