using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChameleonGameWPF.Persistence
{
    class ChameleonFileDataAccess : IDataAccess
    {
        public async Task<ChameleonTable> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync();
                    Int32 tableSize = Int32.Parse(line);
                    line = await reader.ReadLineAsync();
                    Int32 currentCameleon = Int32.Parse(line);
                    ChameleonTable table = new ChameleonTable(tableSize);
                    table.CurrentChameleon = currentCameleon;
                    for (Int32 i = 0; i < tableSize; i++)
                    {
                        Debug.WriteLine(i);
                        line = await reader.ReadLineAsync();
                        Debug.WriteLine("line: "+line);
                        var numbers = line.Split(' ');

                        for (Int32 j = 0; j < tableSize; j++)
                        {
                            table.SetValue(i, j, Int32.Parse(numbers[j]));
                        }
                    }
                    return table;
                }
            }
            catch
            {
                throw new ChameleonDataException();
            }
        }

        public async Task SaveAsync(string path, ChameleonTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) 
                {
                    writer.WriteLine(table.Size);
                    writer.WriteLine(table.CurrentChameleon);
                    for (Int32 i = 0; i < table.Size; i++)
                    {
                        for (Int32 j = 0; j < table.Size; j++)
                        {
                            await writer.WriteAsync(table[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new ChameleonDataException();
            }
        }
    }
}
