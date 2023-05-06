using System;
using Microsoft.Data.SqlClient;

namespace COMPUTINGNEA.Models
{
	public class sql
	{
        // Storing connection string as a public variable to be accessed elsewhere in the program
        private string constring;
        public string Constring
        {
            get { return constring; }
            set { constring = value; }
        }

        // defining connection string
        public sql()
		{
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "investmentnea.database.windows.net";
            builder.UserID = "maryamseraj";
            builder.Password = "************************"; 
            builder.InitialCatalog = "investmentcalc";

            constring = builder.ConnectionString;
        }
	}
}
