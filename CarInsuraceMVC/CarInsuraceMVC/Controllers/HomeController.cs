using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarInsuranceMVC.ViewModels;
using CarInsuranceMVC.Models;

namespace CarInsuraceMVC.Controllers
{
    public class HomeController : Controller
    {
        public int quoteDefault = 50;
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog = CarInsurance; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
        public List<QuoteApp> applications = new List<QuoteApp>();

        public static int PorscheChecker(QuoteApp example1)
        {
            string Make = example1.CarMake.ToLower();
            string Model = example1.CarModel.ToLower();
            int porschePenalty = 0;
            if (Make == "porsche" && Model == "911 carrera")
            {
                porschePenalty = 50;
            }
            else if (Make == "porsche")
            {
                porschePenalty = 25;
            }
            return porschePenalty;
        }


        public  static int DuiChecker(QuoteApp example2)
        {
            if (example2.Dui == "yes")
            {
                double duiPenalty = example2.Quote * .25;
                example2.Quote = Convert.ToInt32(duiPenalty) + example2.Quote;
            }
            return example2.Quote;
        }
        public static int CoverageChecker(QuoteApp example3)
        {
            if (example3.Coverage == "full")
            {
                double fullPrice = example3.Quote * .50;
                example3.Quote = Convert.ToInt32(fullPrice) + example3.Quote;
            }
            return example3.Quote;
        }
        public static int CarChecker(QuoteApp example4)
        {
            int carPenalty = 0;
            if (example4.CarYear < 2000 )
            {
                carPenalty= 25;
            }
            else if (example4.CarYear > 2015)
            {
                carPenalty= 25;
            }
            return carPenalty;
        }

        public static int AgeChecker(QuoteApp example5)
        {
            int agePenalty = 0;
            DateTime timeNow = DateTime.Now;
            int age = 0;
            age = timeNow.Year - example5.DOB.Year;
            if (timeNow.Month < example5.DOB.Month)
            { age = age - 1; }
            else if (timeNow.Month == example5.DOB.Month && timeNow.Day < example5.DOB.Day)
            { age = age - 1; }

            if ( age < 25 && age >=18 )
            { agePenalty = 25; }
            else if (age < 18 )
            { agePenalty = 100; }
            else if (age > 100)
            { agePenalty = 25; }
            return agePenalty;
        }


        public static int QuoteChecker(QuoteApp example)
        {
           double duiPenalty = 0;
           double fullPrice = 0;
           int ageTotal = AgeChecker(example);
           int carTotal = CarChecker(example);
           int porscheTotal = PorscheChecker(example);
           int subTotal = porscheTotal + carTotal + ageTotal + example.Quote;
           int ticketPenalty = example.Tickets * 10;
           int ticketTotal = subTotal + ticketPenalty;
           if (example.Dui == "yes")
           { duiPenalty = ticketTotal * .25; }
           int duiCost = Convert.ToInt32(duiPenalty);
           if(example.Coverage == "full")
           { fullPrice = ticketTotal * .50; }
           int coverageCost = Convert.ToInt32(fullPrice);
           return ticketTotal + duiCost + coverageCost;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(QuoteApp quote)
        {
            
            string queryString = @"INSERT INTO Quotes (FirstName, LastName, EmailAddress, DOB, CarYear, CarMake, CarModel, Dui, Tickets, Coverage, Quote)
                    VALUES (@FirstName,@LastName,@EmailAddress,@DOB, @CarYear, @CarMake, @CarModel, @Dui, @Tickets, @Coverage, @Quote)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
                command.Parameters.Add("@DOB", SqlDbType.DateTime);
                command.Parameters.Add("@CarYear", SqlDbType.Int);
                command.Parameters.Add("@CarMake", SqlDbType.VarChar);
                command.Parameters.Add("@CarModel", SqlDbType.VarChar);
                command.Parameters.Add("@Dui", SqlDbType.VarChar);
                command.Parameters.Add("@Tickets", SqlDbType.Int);
                command.Parameters.Add("@Coverage", SqlDbType.VarChar);
                command.Parameters.Add("@Quote", SqlDbType.Int);

                command.Parameters["@FirstName"].Value = quote.FirstName;
                command.Parameters["@LastName"].Value = quote.LastName;
                command.Parameters["@EmailAddress"].Value = quote.EmailAddress;
                command.Parameters["@DOB"].Value = quote.DOB;
                command.Parameters["@CarYear"].Value = quote.CarYear;
                command.Parameters["@CarMake"].Value = quote.CarMake;
                command.Parameters["@CarModel"].Value = quote.CarModel;
                command.Parameters["@Dui"].Value = quote.Dui;
                command.Parameters["@Tickets"].Value = quote.Tickets;
                command.Parameters["@Coverage"].Value = quote.Coverage;
                command.Parameters["@Quote"].Value = QuoteChecker(quote);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close(); 
            }
            return View("Quote"); 
        }
        public ActionResult Admin()
        {
            string queryString = @"SELECT Id, FirstName, LastName, EmailAddress, DOB, CarYear, CarMake, CarModel, Dui, Tickets, Coverage,Quote from Quotes";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var app = new QuoteApp();
                    app.Id = Convert.ToInt32(reader["Id"]);
                    app.FirstName = reader["FirstName"].ToString();
                    app.LastName = reader["LastName"].ToString();
                    app.EmailAddress = reader["EmailAddress"].ToString();
                    app.Quote = Convert.ToInt32(reader["Quote"]);
                    applications.Add(app);
                }

            }
            var appVms = new List<appVm>();
            foreach (var app in applications)
            {
                var appVm = new appVm();
                appVm.FirstName = app.FirstName;
                appVm.LastName = app.LastName;
                appVm.EmailAddress = app.EmailAddress;
                appVm.Quote = app.Quote;
                appVms.Add(appVm);

            }

            return View(appVms);
        }


        public ActionResult Quote()
        {
            ViewBag.Message = "Quote Page.";

            return View();
        }
    }
}

