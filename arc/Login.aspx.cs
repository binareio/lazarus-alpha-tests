using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    readonly SqlConnection _mDb = new SqlConnection
    {
        ConnectionString =
        "Data Source=mmzwzgz169.database.windows.net;Initial Catalog=impressiondesign_db;User ID=ImpressionDB2;Password=FypHuat2016"
    };

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void loginBTN_Click(object sender, EventArgs e)
    {
        LoginHandler lh = new LoginHandler();

        bool loginFlag = lh.CheckLogin(usernameTB.Text, passwordTB.Text);
        bool AdminFlag = lh.CheckAdmin(usernameTB.Text, passwordTB.Text);
        bool MasterFlag = lh.CheckMaster(usernameTB.Text, passwordTB.Text);

        if (loginFlag)
        {
            _mDb.Open();
            const string login = "SELECT * FROM myCustomers WHERE @username = cUsername AND @password = cPassword";
            var cmd = new SqlCommand(login, _mDb);
            cmd.Parameters.AddWithValue("@username", usernameTB.Text);
            cmd.Parameters.AddWithValue("@password", passwordTB.Text);
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                string _username = (string) rdr["cUsername"];
                string _name = (string) rdr["cName"];
                string _email = (string) rdr["cEmail"];
                Session["sUsername"] = _username;
                Session["sName"] = _name;
                Session["sEmail"] = _email;
            }
            _mDb.Close();

            Session["sLogin"] = true;
            Response.Redirect("CustAppointment.aspx");
        }
        
        else if (AdminFlag)
        {
            _mDb.Open();
            const string login = "SELECT * FROM myEmployees WHERE @username = eUsername AND @password = ePassword";
            var cmd = new SqlCommand(login, _mDb);
            cmd.Parameters.AddWithValue("@username", usernameTB.Text);
            cmd.Parameters.AddWithValue("@password", passwordTB.Text);
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                string _username = (string)rdr["eUsername"];
                string _name = (string)rdr["eName"];
                Session["sUsername"] = _username;
                Session["sName"] = _name;
                Session["sLogin"] = true;
            }
            _mDb.Close();

            Session["sLogin"] = true;
            Response.Redirect("Default.aspx");
        }

        else if (MasterFlag)
        {
            _mDb.Open();
            const string login = "SELECT * FROM myAdmin WHERE @username = aUsername AND @password = aPassword";
            var cmd = new SqlCommand(login, _mDb);
            cmd.Parameters.AddWithValue("@username", usernameTB.Text);
            cmd.Parameters.AddWithValue("@password", passwordTB.Text);
            var rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                string _username = (string)rdr["aUsername"];
                string _name = (string)rdr["aName"];
                Session["sUsername"] = _username;
                Session["sName"] = _name;
                Session["sLogin"] = true;
            }
            _mDb.Close();

            Session["sLogin"] = true;
            Response.Redirect("SuperAdmin.aspx");
        }

        else 
        {
            ErrorLBL.InnerHtml = "Incorrect Username/Password";
        }
    }
}