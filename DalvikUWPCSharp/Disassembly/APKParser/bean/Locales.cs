using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class Locales
    {
        /**
     * when do localize, any locale will match this
     */
        public static CultureInfo any = new CultureInfo("");

        public static int match(CultureInfo locale, CultureInfo targetLocale)
        {
            if (locale == null)
            {
                return -1;
            }

            var targetlocReg = new RegionInfo(targetLocale.Name);

            if (locale.TwoLetterISOLanguageName.Equals(targetLocale.TwoLetterISOLanguageName))
            {
                var locReg = new RegionInfo(locale.Name);

                if (locReg.TwoLetterISORegionName.Equals(targetlocReg.TwoLetterISORegionName))
                {
                    return 3;
                }
                else if (targetlocReg.TwoLetterISORegionName.Equals(string.Empty))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (targetlocReg.TwoLetterISORegionName.Equals(string.Empty) || targetLocale.TwoLetterISOLanguageName.Equals(string.Empty))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
