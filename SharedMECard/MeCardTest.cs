using System;
using System.Collections.Generic;
using System.Text;

namespace MeCardParser
{
    public class MeCardTest
    {

        public static int TestMeCard()
        {
            int nerror = 0;

            nerror += Test_RawMeCard_One("WIFI:s:myssid;;", new MeCardRaw("WIFI", "s", "myssid"));

            nerror += Test_RawMeCard_List_One("netshg:len:1;;", new List<MeCardRaw>() { new MeCardRaw("NETSHG", "len", "1") });
            nerror += Test_RawMeCard_List_One("netshg:len:A;;len:B;;", new List<MeCardRaw>() { new MeCardRaw("NETSHG", "len", "A"), new MeCardRaw("NETSHG", "len", "B") });
            nerror += Test_RawMeCard_List_One("netshg:len:A;;len:B;;len:C", new List<MeCardRaw>() { new MeCardRaw("NETSHG", "len", "A"), new MeCardRaw("NETSHG", "len", "B"), new MeCardRaw("NETSHG", "len", "C") });

            nerror += StringUtility.TestNEndChars();
            return nerror;
        }

        private static int Test_RawMeCard_List_One(string url, List<MeCardRaw> expected)
        {
            int nerror = 0;
            var actual = MeCardParser.ParseList(url);
            if (actual.Count != expected.Count)
            {
                nerror++;
                Log($"ERROR: MECARD: Url={url} Expected count={expected.Count} Actual={actual.Count}");

            }
            else
            {
                for (int i = 0; i < expected.Count; i++)
                {
                    var actualone = actual[i];
                    var expectedone = expected[i];  
                    if (actualone != expectedone)
                    {
                        nerror++;
                        Log($"ERROR: MECARD: Url={url} Index={i} Expected={expectedone} Actual={actualone}");
                    }
                }
            }
            return nerror;
        }
        private static int Test_RawMeCard_One(string url, MeCardRaw expected)
        {
            int nerror = 0;
            var actual = MeCardParser.Parse(url);
            if (actual != expected)
            {
                nerror++;
                Log($"ERROR: MECARD: Url={url} Expected={expected} Actual={actual}");
            }
            return nerror;
        }

        public static void Log(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }


    }
}
