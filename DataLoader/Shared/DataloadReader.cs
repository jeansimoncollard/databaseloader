using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLoader.Shared
{
    internal class DataloadReader
    {
        public string GetInsertQuery(string fileContent)
        {
            var insertQueryLength = getLengthOfInsertQuery(fileContent);


            return fileContent.Substring(insertQueryLength.Length + Environment.NewLine.Length, Convert.ToInt32(insertQueryLength));
        }

        public string GetDeleteQuery(string fileContent)
        {
            var insertQueryLength = getLengthOfInsertQuery(fileContent);

            var startIndex = insertQueryLength.Length + Environment.NewLine.Length + Convert.ToInt32(insertQueryLength);

            return fileContent.Substring(startIndex, fileContent.Length - startIndex);
        }

        private string getLengthOfInsertQuery(string fileContent)
        {
            var insertQueryLength = "";
            var i = 0;
            while (char.IsNumber(fileContent[i]))
            {
                insertQueryLength += fileContent[i];
                i++;
            }

            return insertQueryLength;
        }
    }
}
