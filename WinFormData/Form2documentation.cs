using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormData
{
    public partial class Form2documentation : Form
    {
        public Form2documentation()
        {
            InitializeComponent();
            CreateText();
        }

        public void CreateText()
        {
            // ReSharper disable LocalizableElement
            richTextBox1.Text = "Create a text file in the esignal path with the stock symbol as the file name \n" +
                                "for example (c:\\interactive data\\C.txt) for the symbol C - citi group\n" +
                                "The content of the text file determines the action to take in prosper pro\n" +
                                "The following are some examples of the content of the text file: \n\r\n\r" +
                                "0,0 -> Flat the symbol \n\r" +
                                "0sp,0 -> Exit the full position by bid/offer, if it does not fill in 5 second, then market out \n\r" +
                                "0sp,0,30 -> Exit the full position by bid/offer, if it does not fill in 30 second, then market out \n\r" +
                                "0exitHalfBidMarketAfterTime,0,15 -> exit half bid/offer and after 15 second market out half \n\r" +
                                "0exitHalf,0 -> exit half \n\r\n\r" +
                                "1bidAndMarketAfterTime,0,600,15 -> bid in 600 shares for 15 second, if doesn't get fill, market in long, replace 15 with 0 means marketing in immediately\n\r" +
                                "-1offerAndMarketAfterTime,0,600,15 -> offer in 600 shares for 15 second, if doesn't get fill, market in short, replace 15 with 0 means marketing in immediately\n\r\n\r" +
                                "1buyAtPrice,0,22.11,200 -> place a bid at 22.11 for 200 shares \n\r" +
                                "-1sellAtPrice,0,22.11,200 -> place an offer at 22.11 for 200 shares \n\r\n\r" +
                                "1bidAndCancelAfterTime,0,400,10 -> place a bid @ current highest bid for 400 shares and cancel after 10 seconds\n\r" +
                                "-1offerAndCancelAfterTime,0,400,15 -> place an offer @ current lowest offer for 400 shares and cancel after 10 seconds" +
                                "\n\r\n\r" +
                                "";



        }

        public void Testing()
        {
            //richTextBox1.Rtf = @"{\rtf1\ansi this is in \b bold\b0}";
        }

        public void CreateVersionText()
        {
            richTextBox1.Text =
                "1.9.21 - Fixed 0sp,0,0, when time to exit equals to 0, just flat" +
                "1.9.20\n" +
                "-- 1) add 0sp,0 or 0sp,0,30\n" +
                "-- 2) Fixed bidandmarketaftertime, now it checks open positions for remaining shares\n\n" +
                "1.9.19 - fix stock name with 2 dots" + "\n" + "\n" +
                "1.9.17 " + "\n" +
                "-- 1) Fixed problem japanese bid in and market after time does not work for bidding part because the price is not rounded to nearest decimal" + "\n" +
                "-- 2) create a just bid and cancel after time function" + "\n" +
                "-- 3) update the bid in and market after time, if the user choose to market in after 0 second, then just market in" + "\n" +
                "-- 4) on the ui, add the list of command so we don't have to look" + "\n" +
                "-- 5) remove Execute get all position from program log, xmlhelper line 155" + "\n" + "\n" +
                "1.9.15 - Fix exit half" + "\n" + "\n" +
                "1.9.14 - Add mass get lvl 1 for getting lots of lv1 quote at the same time" + "\n" + "\n" +
                "1.9.13 - Add method to buy and sell at certain price " + "\n" + "\n";
        }

        // ReSharper restore LocalizableElement    
        // Documentation
        private void button1_Click(object sender, EventArgs e)
        {
            CreateText();
        }

        // Version Changes
        private void button2_Click(object sender, EventArgs e)
        {
            CreateVersionText();
        }
    }
}

//todo: for soft exit allow user to choose how much time to wait 0sp,0,30 -> bid/offer out for 30 seconds if it doesn't fill flat position 
