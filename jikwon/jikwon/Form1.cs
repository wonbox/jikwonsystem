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
using System.Data.SqlClient;

namespace jikwon
{
    public partial class Form1 : Form
    {
        //accessdb 연결
        static string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Administrator\Desktop\jikwon(완성작품)\jikwon\jikwon\jikwon.accdb;Persist Security Info=True";

        OleDbConnection conn = new OleDbConnection(connStr);
        private int selectRowIndex;

        static string selectData;
        static bool box = true;
        
        public Form1()
        {
            InitializeComponent();
            selectData = label3.Text; //현재시간값 할당
        }

        
        
        private void button5_Click(object sender, EventArgs e)
        {
            //RFID카드랑 연결
            sp.PortName = "COM3";
            sp.BaudRate = (int)9600;
            sp.DataBits = (int)8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.Open();
            if (sp.IsOpen)
            {
                label2.Text = "연결성공";
            }
        }

        
        
        
        private void button4_Click(object sender, EventArgs e)
        {
            
            sp.Dispose();
            Form2 r = new Form2();
            r.Show();
            
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //rfid카드 번호 가져옴
            if (sp.IsOpen)
            {
                this.Invoke(new EventHandler(SerialReceived));

            }
        }

        private void SerialReceived(object sender, EventArgs e)
        {
            
            //rfid카드를 찍을때 배경화면 바뀜                 
            if (box)
            {
                pictureBox1.Visible = this.pictureBox1.Visible = true;
                pictureBox1.Image = Image.FromFile("C:\\Users\\Administrator\\Desktop\\jikwon(완성작품)\\44.png");
                box = false;
            }
            else
            {
                pictureBox1.Visible = this.pictureBox1.Visible = true;
                pictureBox1.Image = Image.FromFile("C:\\Users\\Administrator\\Desktop\\jikwon(완성작품)\\4444444.png");
                box = true;
            }
            
            selectData = label3.Text;

           list_card();
          
          //카드를 찍으면 오늘자 고객리스트 DB 저장
           string InsertSQL = "Insert into jikwonU (udate, ucard,uname,ubelong ,uhour) values (@udate, @ucard , @uname , @ubelong ,@uhour )";

          
           OleDbCommand cmd = new OleDbCommand(InsertSQL, conn);

           cmd.Parameters.AddWithValue("@udate", label3.Text);
           cmd.Parameters.AddWithValue("@ucard", textBox2.Text);
           cmd.Parameters.AddWithValue("@uname", textBox3.Text);
           cmd.Parameters.AddWithValue("@ubelong", textBox4.Text);
           cmd.Parameters.AddWithValue("@uhour", label4.Text);
           conn.Open();
           cmd.ExecuteNonQuery();
           conn.Close(); 
           list_date();
           count();
                   


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Tick += new EventHandler(t_Tick);  //타이머의 Interval 간격마다 발생

            t.Start();      
                        
             
             label3.Text = DateTime.Now.ToString("yyyyMMdd");//현재시간을 label 보여줌
             
             //오늘 날짜 고객 리스트 보여줌
             string selectSQL = "select * from jikwonU where udate= '" + selectData + "'";
             OleDbDataAdapter DBAdapter = new OleDbDataAdapter(selectSQL, conn);
             DataSet ds = new DataSet();
             DBAdapter.Fill(ds, "jikwonU");
             DBGrid.DataSource = ds.Tables["jikwonU"];
                     

        }
        
        //rfid카드번호와 일치한 값 보여줌
        private void list_card()
        {
            
            string selectSQL = "select * from jikwon where jcard ='"+ sp.ReadExisting() +"'";
            OleDbDataAdapter DBAdapter = new OleDbDataAdapter(selectSQL, conn);
            DataSet ds = new DataSet();
            DBAdapter.Fill(ds, "jikwon");
            dataGridView1.DataSource = ds.Tables["jikwon"].DefaultView;
            
            textBox2.Text = dataGridView1.Rows[0].Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.Rows[0].Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.Rows[0].Cells[4].Value.ToString();
        }
        //오늘 날짜 고객 리스트 보여줌
        private void list_date()
        {
            
            string selectSQL = "select * from jikwonU where udate = '" + selectData + "'";
            OleDbDataAdapter DBAdapter = new OleDbDataAdapter(selectSQL, conn);
            DataSet ds = new DataSet();
            DBAdapter.Fill(ds, "jikwonU");
            DBGrid.DataSource = ds.Tables["jikwonU"].DefaultView;
           
        }
        //DB 삭제
        private void button3_Click(object sender, EventArgs e)
        {
          
            DialogResult result=MessageBox.Show("정말로 삭제하시겠습니까?","알림",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
             if (result == DialogResult.Yes)
             {
                 string DeleteSQL = "Delete * from jikwonU where ID = @ID";
                 OleDbCommand cmd = new OleDbCommand(DeleteSQL, conn);

                 cmd.Parameters.AddWithValue("@ID", selectRowIndex);

                 conn.Open();
                 cmd.ExecuteNonQuery();
                 conn.Close();
                 list_slect();
                 count();

             }
        }
        //날짜 검색
        private void button1_Click(object sender, EventArgs e)
        {
           
            selectData = textBox1.Text;
            string selectSQL = "select * from jikwonU where udate= '" + selectData + "'";
            OleDbDataAdapter DBAdapter = new OleDbDataAdapter(selectSQL, conn);
            DataSet ds = new DataSet();
            DBAdapter.Fill(ds, "jikwonU");
            DBGrid.DataSource = ds.Tables["jikwonU"];
            count();
        
        }
       
        //식당 매출 계산 및 계산한인원
        private void count()
        {
                    
            int mycount;
            string countSQL = "select count(*) from jikwonU where udate= '" + selectData + "'";
            OleDbCommand cmd = new OleDbCommand(countSQL, conn);
            conn.Open();
            
            mycount = Convert.ToInt32(cmd.ExecuteScalar());
            textBox5.Text = "총" + Convert.ToString(mycount) + "명";
            textBox6.Text = "총" + Convert.ToString((mycount) * 4900) + "원";
            
            cmd.ExecuteNonQuery();
            conn.Close();

        }




        //정산 기록 DB저장
        private void button2_Click(object sender, EventArgs e)
        {
            
            string InsertSQL = "Insert into jikwonS (sdate, shsum , sdsum, shour) values (@sdate, @shsum , @sdsum ,@shour )";

         
            OleDbCommand cmd = new OleDbCommand(InsertSQL, conn);

            cmd.Parameters.AddWithValue("@sdate", label3.Text);
            cmd.Parameters.AddWithValue("@shsum", textBox5.Text);
            cmd.Parameters.AddWithValue("@sdsum", textBox6.Text);
            cmd.Parameters.AddWithValue("@shour", label4.Text);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

           
            this.Close();



        }

        private void DBGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //데이터 끝 알림
            
            if (e.RowIndex < 0)
            {
                return;
            }
            else if (e.RowIndex >= DBGrid.RowCount - 1)
            {
                MessageBox.Show("해당 데이터가 존재하지 않습니다.");
                return;
            }
            selectRowIndex = int.Parse(DBGrid.Rows[e.RowIndex].Cells[0].Value.ToString());



        }
        //검색을 통한 삭제 후 검색 리스트 계속 유지
        private void list_slect()
        {   
            
            string selectSQL = "select * from jikwonU where udate = '" + selectData + "'";
            OleDbDataAdapter DBAdapter = new OleDbDataAdapter(selectSQL, conn);
            DataSet ds = new DataSet();
            DBAdapter.Fill(ds, "jikwonU");
            DBGrid.DataSource = ds.Tables["jikwonU"];

        }

        private void t_Tick(object sender, EventArgs e)
        {
            //현재 시간 
            this.label4.Text = DateTime.Now.ToString("hh:mm:ss");

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

       
    }
}
