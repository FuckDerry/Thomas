using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using MySql.Data.MySqlClient;
using System.IO;

namespace thomas
{
    public partial class Form1 : Form
    {
        public int pos = -1;
        public int i = 0;
        public int temp;
        public int x;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SendEmail("Thomas", Mysql());
            //temp=Convert.ToInt32(textBox1.Text);//获取时间间隔
            //i = 0;//设置临时变量
            if (textBox1.Text.ToString() != "")
            {
                pos = 1;//标志位1：开始计时
                output("邮件定时发送软件启动！！！");
                output("您设定的时间为:" + textBox1.Text);
            }
            else
            {
                output("请输入定时时间！！！");
            }


        }

        public void SendEmail(String title, String content)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress("m18395587763@163.com", "m18395587763@163.com");
                mailMsg.To.Add(new MailAddress("467165887@qq.com", "467165887@qq.com"));
                mailMsg.Subject = title;
                mailMsg.Body = content;
                SmtpClient client = new SmtpClient("smtp.163.com", 25);  //发送服务器
                client.Credentials = (ICredentialsByHost)new NetworkCredential("m18395587763@163.com", "Cdefgab960308");
                client.Send(mailMsg);
                output("发送成功！！！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常" + ex.StackTrace);
            }
        }
        public string Mysql()
        {
            string content = "";
            string lateDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string connectionString = string.Format("Server = {0}; Database = {1}; User ID = {2}; Password = {3};", host, database, id, pwd);
            openSqlConnection(connectionString);

            MySqlCommand mySqlCommand2 = new MySqlCommand("UPDATE idiot SET sent=1 where time like \"" + lateDate + "%\";", dbConnection);
            mySqlCommand2.ExecuteNonQuery();
            MySqlCommand mySqlCommand = new MySqlCommand("Select * from idiot where time like \"" + lateDate + "%\";", dbConnection);
            //MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                DataSet data = GetDataSet("Select * from idiot where time like \"" + lateDate + "%\";");
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    content += "------------------------------------------------\n"
                               + "编号：" + row["id"].ToString() + "\n"
                               + "标签：" + row["label"].ToString() + "\n"
                               + "时间：" + row["time"].ToString() + "\n"
                               + "地点：" + row["location"].ToString() + "\n"
                               + "标题：" + row["title"].ToString() + "\n"
                               + "内容：" + "\n" + Read(row["content"].ToString()) + "\n"
                               + "------------------------------------------------\n";
                }
            }
            catch (Exception)
            {
                output("查询失败了！");
            }
            finally
            {
                // reader.Close();

            }

            MyObj = GetDataSet(strCommand);
            return content;
        }


        //读取指定路径下的内容
        public static string Read(string path)
        {
            string content = "";
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            content = sr.ReadToEnd();
            return content;
        }

        // Global variables  
        public static MySqlConnection dbConnection;//Just like MyConn.conn in StoryTools before  
        static string host = "120.27.47.20";
        static string id = "Derry";
        static string pwd = "960308";
        static string database = "thomas";
        static string result = "";

        private static string strCommand = "Select id from idiot ;";
        public static DataSet MyObj;


        // On quit  
        public static void OnApplicationQuit()
        {
            closeSqlConnection();
        }

        // Connect to database  
        private static void openSqlConnection(string connectionString)
        {
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            result = dbConnection.ServerVersion;
            //Debug.Log("Connected to database."+result);  
        }

        // Disconnect from database  
        private static void closeSqlConnection()
        {
            dbConnection.Close();
            dbConnection = null;
            //Debug.Log("Disconnected from database."+result);  
        }

        // MySQL Query  
        public static void doQuery(string sqlQuery)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            IDataReader reader = dbCommand.ExecuteReader();
            reader.Close();
            reader = null;
            dbCommand.Dispose();
            dbCommand = null;
        }
        #region Get DataSet
        public static DataSet GetDataSet(string sqlString)
        {
            //string sql = UnicodeAndANSI.UnicodeAndANSI.UnicodeToUtf8(sqlString);  


            DataSet ds = new DataSet();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(sqlString, dbConnection);
                da.Fill(ds);

            }
            catch (Exception ee)
            {

                throw new Exception("SQL:" + sqlString + "\n" + ee.Message.ToString());
            }
            return ds;

        }
        #endregion

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //计时器
        private void timer1_Tick(object sender, EventArgs e)
        {

            label1.Text = DateTime.Now.ToString();

            string nowTime = DateTime.Now.ToString("HH:mm:ss");


            if (nowTime.Equals(textBox1.Text) && pos == 1)
            {
                output(DateTime.Now.ToString());
                output("时间已到,开始发送！！！");
                SendEmail(DateTime.Now.ToString(), Mysql());
            }

        }

        //日志输出窗口
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        //日志输出
        public void output(string log)
        {
            textBox2.AppendText(log + "\r\n");
        }

        //停止计时
        private void button2_Click(object sender, EventArgs e)
        {
            pos = 0;//标志位为0，停止执行
            output("停止计时");
        }

        //实时显示时间
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
