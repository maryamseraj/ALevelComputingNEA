using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Microsoft.Data.SqlClient;

namespace COMPUTINGNEA.Models
{
    public class NeuralNetWork : User
    {
        // defining neural network variables
        private int investmentid;
        public int InvestmentID
        {
            get { return investmentid; }
            set { investmentid = value; }
        }

        private string investmentname;
        public string InvestmentName
        {
            get { return investmentname; }
            set { investmentname = value; }
        }

        private double amountinvested;
        public double AmountInvested
        {
            get { return amountinvested; }
            set { amountinvested = value; }
        }

        private double revenue;
        public double Revenue
        {
            get { return revenue; }
            set { revenue = value; }
        }

        private double profit;
        public double Profit
        {
            get { return profit; }
            set { profit = value; }
        }

        private string industry;
        public string Industry
        {
            get { return industry; }
            set { industry = value; }
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

        // retrieves the industry market growth percentage from the IndustryGrowth table
        // to be used in calculation
        public double GetIndustryMarketGrowth()
        {
            string industryGrowth = "SELECT IndustryMarketGrowth FROM [dbo].[IndustryGrowth] WHERE Industry = @Industry";

            using (SqlConnection con = new SqlConnection(Constring))
            {
                using (SqlCommand cmd = new SqlCommand(industryGrowth, con))
                {
                    cmd.Parameters.AddWithValue("@Industry", industry);
                    con.Open();
                    double result = (float)cmd.ExecuteScalar();
                    return result;
                }
            }
        }

        // Save details for the neural network calculation into the ROI table
        public override int SaveDetails()
            {
                using (SqlConnection con = new SqlConnection(Constring))
                {
                    string insertInvestmentDetails = "INSERT INTO [dbo].[ROI](AmountInvested, Revenue, Profit, IndustryMarketGrowth, ROI, InvestmentID) VALUES (@AmountInvested,@Revenue,@Profit,@IndustryMarketGrowth,@ROI,@InvestmentID)";

                using (SqlCommand cmd = new SqlCommand(insertInvestmentDetails, con))
                    {
                    cmd.Parameters.AddWithValue("@AmountInvested", amountinvested);
                    cmd.Parameters.AddWithValue("@Revenue", revenue);
                    cmd.Parameters.AddWithValue("@Profit", profit);
                    cmd.Parameters.AddWithValue("@IndustryMarketGrowth", GetIndustryMarketGrowth()); 
                    cmd.Parameters.AddWithValue("@ROI", investmentreturn);
                    cmd.Parameters.AddWithValue("@InvestmentID", InvestmentID);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                    }
                }
            }

        // Updates investment details when a new investment is added -
        // the investment return is added to the table with the associated investment id
        public int UpdateInvestmentDetails()
        {
            InvestmentID = GetInvestmentID();
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string updateInvestment = "UPDATE [dbo].[Investment] SET InvestmentReturn = @InvestmentReturn WHERE InvestmentID = @InvestmentID";

                using (SqlCommand cmd = new SqlCommand(updateInvestment, con))
                {
                    cmd.Parameters.AddWithValue("@InvestmentReturn", investmentreturn);
                    cmd.Parameters.AddWithValue("@InvestmentID", InvestmentID);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }

        // retrieves the ROIID from the ROI table in the database
        public int GetROIID()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string checkROIID = "SELECT ROIID FROM [dbo].[ROI] WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(checkROIID, con))
                {
                    cmd.Parameters.AddWithValue("@Username", Username);
                    con.Open();
                    int ROIID = (int)cmd.ExecuteScalar();
                    return ROIID;
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

        // calculates the final investment return value by creating a new instance of the Matrix class
        // passes in the values entered by the user in the investment form as parameters for the synapse matrix
        public void Calculation()
            {
                // getting all the variables used in calculation ready
                // new instance of Matrix class made for new calculation
                industrymarketgrowth = GetIndustryMarketGrowth();
                var curNeuralNetwork = new Matrix(1, 5);
                double am = amountinvested;
                double r = revenue;
                double p = profit;
                double img = industrymarketgrowth;

                var inputs = new double[,] { { am, r, p, img, 0 } };

                // defining matrices for the training inputs and outputs
                var trainingInputs = new double[,] { { 1, 0, 0, 0, 0 }, { 1, 1, 1, 0, 0 }, { 1, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 } };
                var trainingOutputs = Matrix.MatrixTranspose(new double[,] { { 0, 1, 1, 0 } });

                // training the neural network
                curNeuralNetwork.Train(trainingInputs, trainingOutputs, 10000);

                // testing neural networks against a new problem 
                var output = curNeuralNetwork.Think(new double[,] { { 1, 0, 0, 0, 0 } });

                var ROI = Matrix.MatrixDotProduct(output, inputs);
                double roi = ROI[0, 0];
                double roi1 = ROI[0, 1];
                double roi2 = ROI[0, 2];
                double roi3 = ROI[0, 3];
                double roi4 = ROI[0, 4];
                // the final investment return
                investmentreturn = Math.Truncate(roi + roi1 + roi2 + roi3 + roi4) / 10;
            }
        }
    }
