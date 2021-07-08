using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Utils;

namespace POS
{
    class Program
    {        
        static string keyName = "currency";
        static string keyValue = string.Empty;
        static string currencySymbol = string.Empty;

        static bool bandera;
        static double itemPrice; 
        static int itemPayment;
        static List<double> denomination;
        static CountriesInfo countriesInfo = new CountriesInfo();
        static DenominationLists denominationLists = new DenominationLists();

        static void Main(string[] args)
        {            
            // Check configuration
            UpdateAppConfig();
            string currency = ConfigurationManager.AppSettings.Get("currency");
            string[] countryInfo = countriesInfo.countries[currency];
            Denomination(countryInfo[0]);
            currencySymbol = countryInfo[1];

            bandera = true;
            while (bandera)
            {
                Console.WriteLine("==> Enter the item price:");
                itemPrice = ReadConsole();
            }

            //
            Console.WriteLine("==> Item payment <==");
            double paymentAccum = 0.00;
            double paymentMissing = 0.00;
            bandera = true;
            while (bandera)
            {
                string valuesLine = string.Empty;
                Console.WriteLine(string.Format("Item price:          {0} {1}", currencySymbol, itemPrice.ToString("N2")));
                Console.WriteLine(string.Format("Payment accumulated: {0} {1}", currencySymbol, paymentAccum.ToString("N2")));
                Console.WriteLine(string.Format("Missing payment:     {0} {1}", currencySymbol, paymentMissing.ToString("N2")));
                Console.WriteLine("********************************************");
                Console.WriteLine("Select denomination:");
                for (int val = 0; val < denomination.Count; val++)
                {
                    double value = denomination[val];
                    Console.WriteLine(string.Format("{0}) {1} {2}  ", val + 1, currencySymbol, value));
                }
                                
                Console.WriteLine(valuesLine);

                itemPayment = (Int32) ReadConsole();

                var totalValues = denomination.Count;
                if(itemPayment <= totalValues)
                {
                    paymentAccum += denomination[itemPayment - 1];
                    if(paymentAccum < itemPrice)
                    {
                        paymentMissing = itemPrice - paymentAccum;
                        bandera = true;
                    }
                    else if(paymentAccum > itemPrice)
                    {
                        //your change
                        string changeLine = "";
                        var change = paymentAccum - itemPrice;
                        
                        for(int x= denomination.Count-1; x >= 0; x--)
                        {
                            if (change >= denomination[x])
                            {
                                int res = Convert.ToInt32(change / denomination[x]);
                                if (res > 0)
                                {
                                    change -= (denomination[x] * res);
                                    if (denomination[x] < 1)
                                    {
                                        changeLine += string.Format("{0} coins {1} {2}   ", res, currencySymbol, denomination[x].ToString("N2"));
                                    }
                                    else
                                    {
                                        changeLine += string.Format("{0} bill {1} {2}   ", res, currencySymbol, denomination[x].ToString("N2"));
                                    }
                                }
                            }
                        }

                        Console.WriteLine(string.Format("Your change: {0}", changeLine));
                    }
                    else if (paymentAccum == itemPrice)
                    {
                        var changeLine = paymentAccum - itemPrice;
                        Console.WriteLine(string.Format("Your change: {0} {1}", currencySymbol, changeLine.ToString("N2")));
                    }

                }
                else
                {
                    Console.WriteLine("The selected value doesn't exist");
                    Console.WriteLine("");
                    bandera = true;
                }


            }

        }

        // This method adds the value from the AppConfig File
        private static void UpdateAppConfig()
        {
            try
            {
                string currency = ConfigurationManager.AppSettings.Get("currency");
                if (string.IsNullOrEmpty(currency))
                {
                    // Get current culture info
                    keyValue = CultureInfo.CurrentCulture.Name;
                    // Open App Config File
                    var appConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    // Get settings from the config file
                    var settings = appConfigFile.AppSettings.Settings;
                    // Check if the key exist
                    if (settings[keyName] == null)
                    {
                        //Add the key and value
                        settings.Add(keyName, keyValue);
                    }
                    // Save changes
                    appConfigFile.Save(ConfigurationSaveMode.Modified);
                    // Refresh section in the file
                    ConfigurationManager.RefreshSection(appConfigFile.AppSettings.SectionInformation.Name);
                }
            }
            // Catch errors from Configuration
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("*** Error: Writing the AppConfig File ***");
                Console.WriteLine("");
            }
            //Catch errors from current culture info
            catch (CultureNotFoundException)
            {
                Console.WriteLine("*** Error: Getting current culture info ***");
                Console.WriteLine("");
            }
        }

        // In this method we can get the value from the console and try to convert to number value
        // Putting the flag in true to continue the cycle if get an error while to do the conversion
        // Putting the flag in false if doesn't get an error while to do the conversion
        private static double ReadConsole()
        {
            double valueConvert = 0.00;
            try
            {
                valueConvert = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("");
                bandera = false;
            }
            // Catch format errors
            catch (FormatException)
            {
                Console.WriteLine("*** Error: Please enter only numbers ***");
                Console.WriteLine("");
                bandera = true;
            }
            return valueConvert;
        }

        private static void Denomination(string country)
        {
            switch (country)
            {
                case "US":
                    denomination = denominationLists.US;
                    break;
                case "MX":
                    denomination = denominationLists.MX;
                    break;
                default:
                    break;
            }
        }

    }
}
