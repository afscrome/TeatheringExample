using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace WinRTTest
{
    public class Program
    {
        //Based on http://intunedin.net/2014/03/windows-8-1-and-metered-networks/

        static void Main(string[] args)
        {
            //http://msdn.microsoft.com/en-gb/library/windows/apps/hh465399.aspx
            Windows.Networking.Connectivity.NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;

            WriteNetworkInfo();

            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        static void NetworkInformation_NetworkStatusChanged(object sender)
        {
            WriteNetworkInfo();
        }


        private static void WriteNetworkInfo()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            var cost = profile.GetConnectionCost();

            Console.WriteLine("===================");
            Console.WriteLine("RESULTING MODE:");
            Console.WriteLine(GetOperationMode(cost));

            Console.Write("Cost Type: ");
            Console.WriteLine(cost.NetworkCostType);
            Console.Write("Roaming: ");
            Console.WriteLine(cost.Roaming);
            Console.Write("Over Data Limit: ");
            Console.WriteLine(cost.OverDataLimit);
        }

        // Accordign to documentation, app should run in either Normal, Conservative or Opt-In modes
        //http://blogs.msdn.com/b/cdndevs/archive/2013/10/02/using-windows-8-winrt-apis-in-net-desktop-applications.aspx

        private static OperationMode GetOperationMode(ConnectionCost cost)
        {
            switch (cost.NetworkCostType)
            {
                case NetworkCostType.Unrestricted:
                case NetworkCostType.Unknown:
                     return OperationMode.Normal;

                case NetworkCostType.Fixed:
                case NetworkCostType.Variable:
                    return cost.Roaming || cost.OverDataLimit
                        ? OperationMode.OptIn
                        : OperationMode.Conservative;
                default:
                    throw new InvalidOperationException();
            }

        }

        public enum OperationMode
        {
            Normal,
            Conservative,
            OptIn
        }
    }
}
