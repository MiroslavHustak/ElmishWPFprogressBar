using System.Linq;
using System.Dynamic;
using GoogleSheetsHelper;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System;

/* Do csproj pridat toto (aby fungoval System.Windows.Forms):
  <PropertyGroup>
    <TargetFramework>net5.0-windows</TargetFramework>
	  <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
*/

namespace GoogleSheets
{
    public class ReadingFromGoogleSheets
    {
        //musi byt static kvuli F#
        public static string[] ReadFromGoogleSheets(string jsonFileName, string id, string sheetName, int columnStart, int rowStart, int columnEnd, int rowEnd, bool firstRowIsHeaders)
        {
            try
            {
                List<string> myList = new List<string>();

                var gsh = new GoogleSheetsHelper.GoogleSheetsHelper(jsonFileName, id);

                List<ExpandoObject> generatedItems =
                    gsh.GetDataFromSheet(new GoogleSheetParameters() { SheetName = sheetName, RangeColumnStart = columnStart, RangeRowStart = rowStart, RangeColumnEnd = columnEnd, RangeRowEnd = rowEnd, FirstRowIsHeaders = firstRowIsHeaders });

                generatedItems.ForEach(item => item.ToList().ForEach(item1 => myList.Add(item1.Value.ToString())));

                return myList.ToArray();
            }
            catch (Exception ex)
            {
                string title = "Závažná chyba při čtení z Google tabulky";

                string message = $"Vyskytla se následující chyba: {ex.Message}. Klikni na \"OK\" pro restart této aplikace a oveř hodnoty pro Google tabulku v nastavení (json, id, názvy listů a hodnoty pro řádky a sloupce).";

                MessageBox.Show(message, title);

                string currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(currentExecutablePath);
                Environment.Exit(1);

                return null;
            }            
        }
    }
}
