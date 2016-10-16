using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.OleDb;

namespace jikwon
{
    public partial class Form2 : Form
    {
        //accessdb 연결
        static string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Administrator\Desktop\jikwon(완성작품)\jikwon\jikwon\jikwon.accdb;Persist Security Info=True";
        
        OleDbConnection conn = new OleDbConnection(connStr);
        
        public Form2()
        {
            InitializeComponent();
        }
       
             
               
        private void Form2_Load(object sender, EventArgs e)
        {
            //현재 날짜
            textBox5.Text = DateTime.Now.ToString("yyyy-MM-dd");
            
                //RFID카드랑 연결
                sp2.PortName = "COM3";
                sp2.BaudRate = (int)9600;
                sp2.DataBits = (int)8;
                sp2.Parity = Parity.None;
                sp2.StopBits = StopBits.One;
                sp2.Open();
          
      
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //rfid카드번호 가져옴
            if (sp2.IsOpen)
            {
                this.Invoke(new EventHandler(SerialReceived));

            }
        }
        private void SerialReceived(object sender, EventArgs e)
        {
            //rfid카드번호 읽어옴
            textBox4.Text = sp2.ReadExisting();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //테이블에 회원가입정보 삽입
            string InsertSQL = "Insert into jikwon (jcard, jname, jbirth, jbelong,jdate) values (@jcard , @jname , @jbirth , @jbelong ,@jdate)";

            
            OleDbCommand cmd = new OleDbCommand(InsertSQL, conn);

            
            cmd.Parameters.AddWithValue("@jcard", textBox4.Text);
            cmd.Parameters.AddWithValue("@jname", textBox3.Text);
            cmd.Parameters.AddWithValue("@jbirth", textBox2.Text);
            cmd.Parameters.AddWithValue("@jbelong", textBox1.Text);
            cmd.Parameters.AddWithValue("@jdate", textBox5.Text);
            
           
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

     

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            sp2.Dispose();
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    

    }
}
