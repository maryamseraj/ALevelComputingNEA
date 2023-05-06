using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Azure;

namespace COMPUTINGNEA.Models
{
    public class Investment : User
    {
        // declaring Investment variables

        private string investmentname;
        public string InvestmentName
        {
            get { return investmentname; }
            set { investmentname = value; }
        }

        private string investmentdate;
        public string InvestmentDate
        {
            get { return investmentdate; }
            set { investmentdate = value; }
        }

        private string industry;
        public string Industry
        {
            get { return industry; }
            set { industry = value; }
        }

        private double amountinvested;
        public double AmountInvested
        {
            get { return amountinvested; }
            set { amountinvested = value; }
        }

        private double industrymarketgrowth;
        public double IndustryMarketGrowth
        {
            get { return industrymarketgrowth; }
            set { industrymarketgrowth = value; }
        }

        private double investmentreturn;
        public double InvestmentReturn
        {
            get { return investmentreturn; }
            set { investmentreturn = value; }
        }

        // Overrides the virtual method from the User class used to save details
        // to the Investment table in the database
        public override int SaveDetails()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string insertInvestmentDetails = "INSERT INTO [dbo].[Investment](UserID, InvestmentName, DateOfInvestment, Industry, AmountInvested) VALUES (@UserID, @InvestmentName,@DateOfInvestment,@Industry,@AmountInvested)";

                using (SqlCommand cmd = new SqlCommand(insertInvestmentDetails, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", GetUserID());
                    cmd.Parameters.AddWithValue("@InvestmentName", investmentname);
                    cmd.Parameters.AddWithValue("@DateOfInvestment", investmentdate);
                    cmd.Parameters.AddWithValue("@Industry", industry);
                    cmd.Parameters.AddWithValue("@AmountInvested", amountinvested);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        // Inserts a custom industry name and percentage market growth into the IndustryGrowth table in the database
        public int AddCustomGrowth()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string addCustomGrowth = "INSERT INTO [dbo].[IndustryGrowth](Industry, IndustryMarketGrowth) VALUES (@Industry,@IndustryMarketGrowth)";

                using (SqlCommand cmd = new SqlCommand(addCustomGrowth, con))
                {
                    cmd.Parameters.AddWithValue("@Industry", industry);
                    cmd.Parameters.AddWithValue("@IndustryMarketGrowth", industrymarketgrowth);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        // Updates the percentage market growth value for an existing industry name in the IndustryGrowth table in the database
        public int UpdateGrowth()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string updateGrowth = "UPDATE [dbo].[IndustryGrowth] SET IndustryMarketGrowth = @IndustryMarketGrowth WHERE Industry = @Industry";

                using (SqlCommand cmd = new SqlCommand(updateGrowth, con))
                {
                    cmd.Parameters.AddWithValue("@Industry", industry);
                    cmd.Parameters.AddWithValue("@IndustryMarketGrowth", industrymarketgrowth);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        public int UpdateInvestmentDetails()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string updateInvestment = "UPDATE [dbo].[Investment] SET InvestmentName = @InvestmentName WHERE InvestmentID = @InvestmentID";

                using (SqlCommand cmd = new SqlCommand(updateInvestment, con))
                {
                    cmd.Parameters.AddWithValue("@InvestmentID", 4);
                    cmd.Parameters.AddWithValue("@InvestmentName", investmentname);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        // retrieves investment id from the Investment table
        public int GetInvestmentID()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string investmentID = "SELECT InvestmentID FROM [dbo].[Investment] WHERE InvestmentName = @InvestmentName";

                using (SqlCommand cmd = new SqlCommand(investmentID, con))
                {
                    cmd.Parameters.AddWithValue("@InvestmentName", investmentname);
                    con.Open();
                    int InvestmentId = (int)cmd.ExecuteScalar();
                    return InvestmentId;
                }
            }
        }
    }   
}
