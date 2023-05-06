
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;

namespace COMPUTINGNEA.Models
{
    public class Search : Investment
    {
        private string searchinput;
        public string SearchInput
        {
            get { return searchinput; }
            set { searchinput = value; }
        }

        // uses a select query to retrieve the number of investments from the database for the investment the user searches for
        public int CheckInvestmentExists(string searchInput)
        {
            // select query to retrieve the searched-for investment from the database
            string searchInvestmentName = "SELECT COUNT(*) InvestmentName FROM [dbo].[Investment] WHERE InvestmentName = @SearchTerm";

            using (SqlConnection con = new SqlConnection(Constring))
            {
                using (var cmd = new SqlCommand(searchInvestmentName, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@SearchTerm", searchInput));
                    con.Open();

                    // returns 0 if no investment are found
                    int investmentExists = (int)cmd.ExecuteScalar();
                    return investmentExists;
                }
            }
        }

        // uses a select query to retrieve the investment details from the database for the investment the user searches for
        public List<string> SearchInvestment(string searchInput)
        {
            // list to store investment details
            var listofinvestments = new List<string>();

            string searchInvestmentName = "SELECT InvestmentName, DateOfInvestment, Industry, AmountInvested, InvestmentReturn FROM [dbo].[Investment] WHERE InvestmentName = @SearchTerm";

            using (SqlConnection con = new SqlConnection(Constring))
            {
                using (var cmd = new SqlCommand(searchInvestmentName, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@SearchTerm", searchInput));
                    con.Open();
                    using (var readerr = cmd.ExecuteReader())
                    {
                        // reading each individual column of the table for the investment
                        while (readerr.Read())
                        {
                            string name = readerr.GetString(0);
                            DateTime doi = readerr.GetDateTime(1);
                            string ind = readerr.GetString(2);
                            float ai = readerr.GetFloat(3);
                            float ir = readerr.GetFloat(4);
                            // appending investment details to list
                            listofinvestments.Add(name);
                            listofinvestments.Add(doi.ToString());
                            listofinvestments.Add(ind);
                            listofinvestments.Add(ai.ToString());
                            listofinvestments.Add(ir.ToString());
                        }
                    }
                    int result = cmd.ExecuteNonQuery();
                    return listofinvestments;
                }
            }
        }
    }
}
