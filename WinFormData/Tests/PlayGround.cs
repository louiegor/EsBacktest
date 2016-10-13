using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace WinFormData.Tests
{
    [TestFixture]
    public class PlayGround
    {
        [Test]
        public void StringSplitTest()
        {
            const string symbol = "ABX";
            const string canSymbol = "ABX.TO";

            var containsPeriod = symbol.Contains('.');
            Assert.AreEqual(containsPeriod,false);

            containsPeriod = canSymbol.Contains('.');
            Assert.AreEqual(containsPeriod, true);
        }

        [Test]
        public void TryToBreakGetLvl1()
        {
            var urls = new[]
                {
                    @"http://localhost:8682/GetLv1?symbol=7201.JP",
                    @"http://localhost:8682/GetLv1?symbol=7203.JP",
                    @"http://localhost:8682/GetLv1?symbol=9501.JP"
                };
            var count = 0;
            var failCount = 0;
            var failedAt = new List<int>();
            XDocument x;
            while (true)
            {
                while (true) //This is for retry
                {
                    try
                    {
                        var httpRequest = (HttpWebRequest) WebRequest.Create(urls[count%3]);

                        HttpWebResponse response = null;

                        response = (HttpWebResponse) httpRequest.GetResponse();
                        var receiveStream = response.GetResponseStream();

                        using (XmlReader reader = XmlReader.Create(receiveStream))
                        {
                            x = XDocument.Load(reader);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "The server committed a protocol violation. Section=ResponseStatusLine")
                        {
                            throw; //Not sure what this exception is, so throw
                        }
                        failedAt.Add(count);
                        var list = failedAt.ToList();
                        failCount++;
                    }
                }

                count++;
            }
        }

        [Test]
        public void TestRounding()
        {
            var x = 150;
            var y = 600;
            y.RoundUpToNearest100();

            Assert.AreEqual(100,y );
        }



        [Test]
        public void Testconnection()
        {
            var x = Connection.SiteConnection(new HttpWebDefaultSetting());
            Assert.IsTrue(x);
        }

        [Test]
        public void SwitchTest()
        {
            int below90 = 0;
            int above90 = 0;

            while (true)
            {
                int x = 0;

                var r = new Random();
                var rnd100 = r.Next(100);
                string name = (rnd100 < 90) ? "louiegor" : "petergor";

                switch (name)
                {
                    case "louiegor":
                        x = 1;
                        break;
                    case "petergor":
                        x = 2;
                        break;
                }

                if (x == 1) below90++;
                else above90++;
            }
        }


        [Test]
        public void RetryTest()
        {
            var akblist = Retry.Do(() => GetAkbMember(), 200);
            Assert.NotNull(akblist);
        }

        [Test]
        public void RetryTest2()
        {
            var rndNum = Retry.Do(() => MightThrowError(),200);
            Assert.NotNull(rndNum);

        }


        [Test]
        public void CsvReadTest()
        {
            
            CsvParser.UpdateDictionary(@"C:\Users\LNG\Documents\Interactive Data\FormulaOutput\total.csv");

            var temp1 = CsvParser.GetDictCount();
            var temp2 = CsvParser.GetDictionaryValues();

            Assert.NotNull(temp1);
        }

        [Test]
        public void ThreadTest()
        {
            for (var i = 0; i < 10; i++)
            {
                new Thread(() => WriteSomething(10)).Start();
            }

            Thread.Sleep(25000);
            Assert.AreEqual(NumCount.Count,20);
        }

        [Test]
        public void NoThreadTest()
        {
            for (var i = 0; i < 3; i++)
            {
                WriteSomething(10);
            }

            Thread.Sleep(10000);
            Assert.AreEqual(NumCount.Count, 6);
        }

        [Test]
        public void ConstructorTest()
        {
            var a = Tempgogogo("2");
            var b = Tempgogogo("3", 1);

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 2);
        }

        public int Tempgogogo(string a, int? b = 5)
        {
            if (b == 5) return 1;
            return 2;
        }

        private Akb GetAkbMember()
        {
            var member = GetAkbList().Single(x => x.Name == "Sashi");
            return member;
        }

        private Akb MightThrowError()
        {
            var r = new Random();
            var rnd100 = r.Next(100);
            if (rnd100 < 40)
            {
                throw new Exception("below 50");
            }
            return GetAkbMember();
        }

        private IEnumerable<Akb> GetAkbList()
        {
            return new List<Akb>();
        }

        private void WriteSomething(int second)
        {
            for (var i = 0; i < 2; i++)
            {
                Thread.Sleep(second*500);
                NumCount.Add(i.ToString(CultureInfo.InvariantCulture));
                Thread.Sleep(second*500);
            }
        }

        [Test]
        public void StopWatch()
        {
            int timeSec = 3000;
            long elapsedMs = 0;
            var watch = Stopwatch.StartNew();
            // the code that you want to measure comes here

            while(elapsedMs <timeSec)
            {
                elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
                Thread.Sleep(500);
            }
            watch.Stop();
            Console.WriteLine(elapsedMs);
        }

        private static readonly List<string> NumCount = new List<string>();

        [Test]
        public void BaseDirectory()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;   
            Assert.NotNull(directory);

            var log = new Log();
        }
        

        [Test]
        public void SettingTest()
        {
            var sh = new SettingHelper();
            var x = sh.GetSetting();
            x.AmList = "QQQ, SPY,  wer,    xom,,";
            sh.SaveSetting(x);

            var t = sh.GetAmList();
            var s = "good";
        }

        [Test]
        public void SortingTest()
        {
            var alphaList = new List<string>() {"def", "JkL", "ABC", "gHi"};
            var upperList = alphaList.Select(item => item.ToUpper()).ToList();

            upperList.Sort();
            var temp = string.Join(",", upperList);

            var xee = 0;
        }


        [Test]
        public void ProgramLogTest()
        {
            new Thread(() =>UpdateLog10Times()).Start();
            Thread.Sleep(200);
            new Thread(() => UpdateLog10Times()).Start();
            Thread.Sleep(100);
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            new Thread(() => UpdateLog10Times()).Start();
            Thread.Sleep(20000);

        }

        [Test]
        public void SplitTextTest()
        {
            const string text = "1,23.4,200";
            var line = text.Split(',');
            
            var side = line[0];
            var stop = line[1];
            var share = 100;
            if (line.Length == 3)
            {
                share = int.Parse(line[2]);
            }

            Assert.AreEqual(share,200);
        }

        public void UpdateLog10Times()
        {
            var log = new Log();
            for (int i = 0; i < 10; i++)
            {
                log.Updatelog("gogogo" + i);
                Thread.Sleep(1000);
            }
        
        }

        [Test]
        public void Rounding()
        {
            double n = 150;

            double e = 100;

            double k = 250;

            k= k.RoundToNearest100();
            e = e.RoundToNearest100();
            n = n.RoundToNearest100();
        }

        [Test]
        public void TestingDividing()
        {
            int a = 1000;

            int x = a/3;
            int y = GetPerentageOfNumber(a, 33);
            int z = GetPerentageOfNumber(a, 25);
        }

        public int GetPerentageOfNumber(int a, int b)
        {
            return a*b/100;
        }

        [Test]
        public void MultiplicationTestTo100()
        {
            

            const int init = 800;
            const int percentage = 30;
            var final = init*percentage/100;

            final = (int) Math.Round((decimal) (final/100),0)*100;

            Assert.NotNull(final);
        }

        [Test]
        public void MultiplicationTestTo1()
        {
            const int init = 1;
            const int percentage = 50;
            var final = init * percentage / 100;

            final = (int) Math.Round((decimal)(final / 1), 0) * 1;

            Assert.NotNull(final);
        }

        [Test]
        public void ExcelParseTest()
        {
            var t = new ExcelReader();
            //t.GetExcel();
            var table = t.ReadExcelToTable(@"c:\temp\Book1.xlsx").AsEnumerable();
            var i = 0;
            var contactList = new List<Contact>();

            foreach (var item in table)
            {
                if (i == 0)
                {
                    i++;
                }
                else
                {
                    var x = new Contact();

                    x.Open = double.Parse(item[0].ToString());
                    x.Condition = double.Parse(item[1].ToString());
                    x.YesOrNo = bool.Parse(item[2].ToString());
                    
                    contactList.Add(x);
                }
            }

            Assert.NotNull(table);
        }

        [Test]
        public void GuidTest()
        {
            var x = new List<string>();
            for (var i = 0; i < 1000; i++)
            {
                x.Add(System.Guid.NewGuid().ToString());
                
            }

            bool isUnique = x.Distinct().Count() == x.Count();
            Assert.IsTrue(isUnique);
        }

        [Test]
        public void Temp()
        {
            var abc = new Abc();
            var testing123 = abc.Getyoyoyo();
            foreach (var item in testing123)
            {
                Console.WriteLine(item);    
            }
            
        }

        [Test]
        public void DigitTest()
        {
            string a = "$%23.55    ";
            var s = Regex.Replace(a, "[^0-9.]", "");
            Assert.NotNull(s);
        }
        [Test]
        public void CeilingAndFloorTest()
        {
            var x = new List<double>() {7.1, 8.4, 323.11, 4444.88, 9.8};
            foreach (var i in x)
            {
                Console.Write(Math.Ceiling(i) +", ");
            }
            Console.WriteLine();
            foreach (var i in x)
            {
                Console.Write(Math.Floor(i) + ", ");
            }
        }

        [Test]
        public void MathRounding()
        {
            var x = 127.9999999999999999999999999999;
            var y = 17.43909103785646343;
            x  = Math.Round(x, 2);
            y = Math.Round(y, 2);
            Assert.AreEqual(x,128);
        }

    }

    public class Contact
    {
        public double Open { get; set; }
        public double Condition { get; set; }
        public bool YesOrNo { get; set; }
        
    }

    public class Akb
    {
        public string Name { get; set; }
    }

 
// sql profiler - write up a tutorial like azure!
    
// Skill should include sql profiler
// Skill should include Mobile apps
// IOC and ORM
// Todos: develop knowledge about mobile apps
// Github, softdelete and Include/Eager loading
// Indexing the database
// Replace changing the queries to eager load stuff
// In Custom Feature - mention about branching github

//TECHNICAL SKILLS

//Programming Languages: C#, Java
//Web Development: ASP.NET MVC, CSS, HTML, JQuery, JavaScript, AngularJS, Microsoft Azure
//Database: SQL Server 2012, RavenDb, Entity Framework
//Software Applications: Visual Studio 2012, SQL Server Management Studio, Resharper 
//Operating Systems: Windows 7

//RELATED EXPERIENCE

//Promys PSA CRM Software
//Software Developer, March 2014 - Now

//ASP.NET MVC and Entity Framework was used on the server side for business logic and ORM to database layer 
//Spring.NET was used for dependency injection 
//Worked with some legacy code for UI written in react.js, backbone.js, Coffee Script, Kendo UI Controls
//Create login and reset password form by writing code in JQuery and JQuery UI
//Use Sql Profiler to identify the long-running queries 
//    Improve the overall performance of the software by changing the queries
//Implemented custom features as per our clients requested to help them better perform their business. 
//    Use Github to manage different branches for different clients and merge branches together when necessary
//Write stored procedure to migrate data from one database to another
//Create SSRS report templates by querying out insightful information

    //Implemented CRM Sales Module accroding to the business requirements, this allows our client to perform their business operation by using our software to create quote, sales order, purchase order and invoice. 
    //Created module to keep track of our client's product/ inventory 
    //Implemented timesheet functionality for the users to enter their time and the accountants to do payrolls

//Tools: C#, JavaScript, JQuery, NUnit, ASP.NET MVC, Entity Framework, Linq, IIS, SQL, Github, Microsoft Azure, Resharper, SSRS, Spring.NET, backbone.js, react.js, TeamCity, SQL Profiler, SQL Server Management Studio

//Personal Homepage
//oplog.eastus.cloudapp.azure.com

//Create Web Applicaton from start to finish and hosting it on Azure
//The Homepage was designed as a reward system for my good habit and keep track of my spendings
//Backend is written in ASP.NET MVC, Entity Framework, Linq and SQL Server
//Testing is done using NUnit and TestDriven.NET
//Frontend is written in AngularJS for model binding, some CSS and some JQuery
//
//Tools: C#, JavaScript, AngularJS 1.5, CSS, ASP.NET MVC, Entity Framework, Linq, SQL Server Management Studio, Microsoft Azure, BitBucket, NUnit, TestDriven.NET IIS, Resharper

//Lasalle Captial
//Software Developer

//Write code in C# to act as a bridge to communicate the trading signal from charting software/Excel to the ECN's api
//Autofac was used for dependency injection and for mocking the api response coming back from the api
//Created a WinForm application from start to finish to trade a basket of stocks in various stock markets
//Software was developed to allow users to place/cancel orders in ECN by using different algorithms
//Fixed and maintained the program so that it is as buug free of as possible

//Tools: C#, Entity Framework, NUnit, TestDriven, Autofac, BitBucket

//Rocky Analytics 
//Software Developer, Oct 2013 - Feb 2014

//Implemented Selenium test cases from Gherkin code
//Write code in C# and JavaScript as needed to create tests 
//Developed automated tests in the language of business using Behavior-Driven Development (BDD) technique (SpecFlow), while maintaining a connection to the implemented system  
//Reported and the details of discovered bugs to Github
//Fixed and maintained the assigned bugs
//Familiar with TeamCity 

//Tools: C#, JavaScript, NUnit, TestDriven, Resharper, Github, Selenium, SpecFlow, TeamCity, LINQ


    public class Abc
    {

        public List<string> Getyoyoyo()
        {
            var testing123 = new List<string>();
            string go = "";
            testing123.Add(
                "Use some real life example to answer something that you are not familiar with, for example, if ppl ask you " +
                "about angular2, you can answer with I have used type script and angular15, this proves that you know what it is" +
                "and you are willing to learn more. Or if someone ask you about mango db, you can mention about the experience you have " +
                "with ravendb"
                );
            testing123.Add(
                "Change the wording of your day to day conversation, remove the errr's, I don't know's, doubts from your sentences" +
                "try to use more positive wording, instead of I don't know, say something like 'I did some investigation, this can be done by doing this" +
                "this and that' and is solvable by the end of the day");
            return testing123;
        }
    }

}

//louiegor9901@gmail.com
//As12!05802


//1946 Kempton Park Drive, Mississauga, ONT. L5M 2Z6
//Phone: 647-283-2179
//E-mail: kamchuenlouisng@gmail.com

//LOUIS NG

//SOFTWARE DEVELOPER

//Self-starter and self motivated professional with a Bachelor of Science in Computer Engineering with 4 years of programming experience
//Experienced in Web Application/Database development, website templates design and program testing
//Strong organizational, interpersonal, communication and problem solving skills
//Prioritized and balanced multiple concurrent projects, consistently meeting delivery dates

//EDUCATION

//Bachelor of Science, Computer Engineering, Queen's University

//TECHNICAL SKILLS

//Programming Languages: C#, Java
//Web Development: ASP .NET MVC, CSS, HTML, jQuery, JavaScript, AngularJS, Azure
//Database: SQL Server 2012, RavenDB, Entity Framework
//Software Applications: Visual Studio 2012, SQL Server Management Studio, Resharper 
//Operating Systems: Windows 7

//RELATED EXPERIENCE

//Promys PSA CRM Software
//Software Developer, March 2014 - Now

//Implemented custom features as per our client’s requested to help them better perform their business. Helped maintaining multiple branches by using Github and merging master to custom branches when necessary
//Use SQL Profiler Tool to identify the long-running queries. Improve speed by eager-loading some queries in Entity Framework and add indexes to some of the columns in the database in SQL Server Management Studio
//Create login and reset password form by writing code in jQuery and jQuery UI
//Write stored procedure to migrate data from one database to another
//Create SSRS report templates by querying out insightful information
//Worked with some legacy code for UI written in ReactJS, Backbone.js, CoffeeScript, Kendo UI Controls

//Tools: C#, ASP.NET MVC, Entity Framework, Linq, JavaScript, jQuery, NUnit, IIS, SQL, Github, Microsoft Azure, Resharper, SSRS, Spring.NET, backbone.js, ReactJS, TeamCity, SQL Profiler, SQL Server Management Studio

//Rocky Analytics
//Software Developer, Oct 2013 - Feb 2014

//Implemented Selenium test cases from Gherkin code
//Write code in C# and JavaScript as needed to create tests 
//Developed automated tests in the language of business using Behavior-Driven Development (BDD) technique (SpecFlow), while maintaining a connection to the implemented system  
//Reported and the details of discovered bugs to Github
//Fixed and maintained the assigned bugs
//Familiar with TeamCity 

//Tools: C#, JavaScript, NUnit, TestDriven, ReSharper, Github, Selenium, SpecFlow, TeamCity, LINQ

//Lasalle Capital, ON
//Software Developer, Jan 2016 – Now

//Write code in C# to act as a bridge to communicate the trading signal from charting software/Excel to the ECN's api
//Autofac was used for dependency injection and for mocking the api response coming back from the api 
//Created a WinForm application from start to finish to trade a basket of stocks in various stock markets
//Software was developed to allow users to place/cancel orders in ECN by using different algorithms
//Fixed and maintained the program so that it is as bug free of as possible

//Tools: C#, Entity Framework, NUnit, TestDriven, Autofac, BitBucket

//Personal Homepage
//oplog.eastus.cloudapp.azure.com

//Create Web Application from start to finish and hosting it on Azure
//The Homepage was designed as a reward system for my good habit and keep track of my spendings
//Backend is written in ASP.NET MVC, Entity Framework, Linq and SQL Server
//Testing is done using NUnit and TestDriven.NET
//Frontend is written in AngularJS for model binding, some CSS and jQuery

//Tools: C#, JavaScript, AngularJS 1.5, CSS, ASP.NET MVC, Entity Framework, Linq, SQL Server Management Studio, Microsoft Azure, BitBucket, NUnit, TestDriven.NET IIS, Resharper


//OTHER EXPERIENCES

//Remco Property Management, ON
//IT Technician, Apr 2013 - Oct 2013

//Troubleshoot hardware, software and network operating system

//BSM Wireless, ON
//QA Technician, Oct 2012 – Mar 2013

//Tested the accuracy of the GPS by using an antenna to check if the coordination of map matches the corresponding physical location

//Signifi Solutions, Mississauga, ON
//Program Tester, Dec 2011 – Nov 2011

//Developed automation Rational Robot test scripts using VBScript that mimics customer's activity for several products to increase application precision
//Created test plans and test specifications for functional and integration testing
//Focused on meeting client's needs by communicating with the client on regular basis through email and report any bugs found in VSS  

//Tools: Visual Basic, Rational Robot, SQL Server Profiler, XML, VSS, MS Word Document, WebConfig file, ASP.NET 2.0 web forms
