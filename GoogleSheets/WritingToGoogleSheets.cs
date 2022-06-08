using System.Linq;
using GoogleSheetsHelper;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Diagnostics;

/* Do csproj pridat toto (aby fungoval System.Windows.Forms):
  <PropertyGroup>
    <TargetFramework>net5.0-windows</TargetFramework>
	  <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
*/

namespace GoogleSheets
{
    public class WritingToGoogleSheets
    {
        //musi byt static kvuli F#
        public static void WriteToGoogleSheets(string[] headers, string[][] rows, string jsonFileName, string id, string sheetName, int columnStart, int rowStart, bool complicatedCode)
        {
            try
            {
                var gsh = new GoogleSheetsHelper.GoogleSheetsHelper(jsonFileName, id);

                List<GoogleSheetCell> MyCells = new List<GoogleSheetCell>();
                List<GoogleSheetRow> MyRows = new List<GoogleSheetRow>();

                int numberOfColumns = headers.Length;
                int numberOfRows = 0;

                switch (complicatedCode)
                {
                    case true:
                        numberOfRows = rows.ElementAt(0).Length; // takhle slozite, bo F# zrobilo vicerozmerove pole tak, jak ho zrobilo
                        break;
                    case false:
                        numberOfRows = rows.Length;
                        break;
                }

                for (int i = -1; i < numberOfRows; i++)
                {
                    if (i == -1)
                    {
                        Enumerable.Range(0, numberOfColumns).ToList().ForEach(j => MyCells.Add(new GoogleSheetCell() { CellValue = headers[j].ToString(), IsBold = true }));
                        AddRows();
                    }
                    else
                    {
                        switch (complicatedCode)
                        {
                            case true:
                                //Tady je 'j' a 'i' opacne (tj. v dll pro F# komplikovana varianta), nez mam v normalnim C# kodu
                                Enumerable.Range(0, numberOfColumns).ToList().ForEach(j => MyCells.Add(new GoogleSheetCell() { CellValue = rows[j][i].ToString() }));
                                break;
                            case false:
                                Enumerable.Range(0, numberOfColumns).ToList().ForEach(j => MyCells.Add(new GoogleSheetCell() { CellValue = rows[i][j].ToString() }));
                                break;
                        }
                        AddRows();
                    }
                }

                gsh.AddCells(new GoogleSheetParameters() { SheetName = sheetName, RangeColumnStart = columnStart, RangeRowStart = rowStart }, MyRows);

                void AddRows()
                {
                    GoogleSheetRow gsr = new GoogleSheetRow();
                    gsr.Cells.AddRange(MyCells);
                    MyRows.Add(gsr);
                    MyCells.Clear();
                }
            }
            catch (Exception ex)
            {
                string title = "Závažná chyba při zápisu do Google tabulky";

                string message = $"Vyskytla se následující chyba: {ex.Message}. Klikni na \"OK\" pro restart této aplikace a oveř hodnoty pro Google tabulku v nastavení (json, id, názvy listů a hodnoty pro řádky a sloupce).";

                MessageBox.Show(message, title);

                string currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(currentExecutablePath);
                Environment.Exit(1);                
            }
        }
    }
}
