using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BookDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Session["user_id"] == null)
                    Response.Redirect("Login.aspx");
                Label9.Text = "Welcome :  " + Session["user_id"].ToString();
                lbl.Text = "Book Id  : " + Session["id"];
            }
            catch (NullReferenceException e1)
            {
                Response.Write("<script>alert('Please Login!')</script>");
            }
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select Name from Authors a join BookAuth ba ON a.Author_id = ba.Author_id " +
                "where Book_id = @Book_id;";
            cmd.Parameters.AddWithValue("@Book_id", Session["id"]);

            cn.Open();
            SqlDataReader drEmps = cmd.ExecuteReader();
            List<string> authors = new List<string>();
            while (drEmps.Read())
            {
                authors.Add(drEmps["Name"].ToString());
            }

            drEmps.Close();
            
            string authorList = "";
            foreach(string author in authors)
            {
                if (!authors.Last<string>().Equals(author))
                    authorList += author + ", ";
                else
                    authorList += author;
            }
            lbl2.Text = authorList;

            //For Owner
            cmd.CommandText = "select Name, Email, Mobileno from UserInfo u join Books b on b.User_id = u.User_id " +
                "where Book_id = @Bookid;";
            cmd.Parameters.AddWithValue("@Bookid", Session["id"]);

            drEmps = cmd.ExecuteReader();
            
            while (drEmps.Read())
            {
                lblName.Text = drEmps["Name"].ToString();
                lblEmail.Text = drEmps["Email"].ToString();
                lblMobileNo.Text = drEmps["Mobileno"].ToString();
            }

            drEmps.Close();
            cn.Close();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSqlLocalDb;AttachDbFilename=C:\Users\hp\Suraj.mdf;Integrated Security=True";

            SqlCommand cmdgrid = new SqlCommand();
            cmdgrid.Connection = con;
            cmdgrid.CommandType = CommandType.Text;
            cmdgrid.CommandText = "select Pic ,Title,Original_price,Selling_price, No_of_pages,Publication, Category, Exchange,Status_book from Books where Book_id = @Bookid1;";
            cmdgrid.Parameters.AddWithValue("@Bookid1", Session["id"]);

            
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = cmdgrid;
           
            DataSet ds = new DataSet();
            sda.Fill(ds, "Books");
            GridView1.DataSource = ds.Tables["Books"];
            GridView1.DataBind();

        }

    }

    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dr = (DataRowView)e.Row.DataItem;
            string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["Pic"]);
            (e.Row.FindControl("Image1") as Image).ImageUrl = imageUrl;

        }
    }

    protected void btnContact_Click(object sender, EventArgs e)
    {
        try
        {

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = @"Data Source=(LocalDB)\MSSqlLocalDb;AttachDbFilename=C:\Users\hp\Suraj.mdf;Integrated Security=True";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select Name , Email , MobileNo from UserInfo  where Email = @Email";
            cmd.Parameters.AddWithValue("@Email", lblEmail.Text);


            cn.Open();
           // SqlDataReader da = cmd.ExecuteReader();

           // if (da.Read())
           // {
                string Name = lblName.Text;
                string Email = lblEmail.Text;
                string PhoneNo = lblMobileNo.Text;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("surajpawar0303@gmail.com", "duniyadari");
                MailMessage msgobj = new MailMessage();
                msgobj.To.Add("select Email from UserInfo where Email=" + lblEmail.Text); ///query for owner's email id
                msgobj.From = new MailAddress("surajpawar0303@gmail.com");
                msgobj.Subject = " Buyer's Details";
                msgobj.Body = String.Format("Hello !! Buyer's details are :  \n Name " + Name + " \n Email : " + Email + "\n PhoneNo" + PhoneNo);


                client.Send(msgobj);

                Response.Write("msg was send successfully");
           // }

        }

        catch (Exception ex)
        {
            Response.Write("Email  Failed" + ex.Message);
        }
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        SqlConnection cn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = cn2;
        cmd2.CommandType = CommandType.Text;
        cmd2.CommandText = "insert into Feedback(Book_id, User_id, Comment) values (@bk_id, @u_id, @cmt)";

        cmd2.Parameters.AddWithValue("@bk_id",Session["id"]);
        cmd2.Parameters.AddWithValue("@u_id", Session["user_id"].ToString());
        cmd2.Parameters.AddWithValue("@cmt", txtFeed.Text);
        cn2.Open();
        cmd2.ExecuteNonQuery();
        cn2.Close();
        Response.Write("<script>alert('Feedback Posted Successfully')</script>");
    }
}