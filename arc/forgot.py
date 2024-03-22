from tkinter import *
from tkinter import ttk  # ttk is used for styling
from PIL import Image, ImageTk
from main import face_recognition_system
from tkinter import messagebox
import mysql.connector
import smtplib
import numpy as np
import random
import datetime 


class forgot_password:
    def __init__(self, root):
        self.root = root
        self.root.geometry("1200x700+0+0")
        self.root.title("Face Recognition Attendance System")
        
        # ======= Variables ============
        self.var_get_email = StringVar()  # memberType
        self.var_otp = StringVar()
        self.final_otp = ""
        self.var_password = StringVar()
        self.var_password_again = StringVar()
        self.var_after_5_min = ''
        

        # img1 = main background
        img1 = Image.open("Images/thapar1.jpeg")
        img1 = img1.resize((1200, 700), Image.ANTIALIAS)
        self.photoimg1 = ImageTk.PhotoImage(img1)
        bg_img = Label(self.root, image=self.photoimg1)
        bg_img.place(x=0, y=0, width=1200, height=700)


        # login frame
        login_frame = Frame(bg_img, bd=2, bg="white", highlightthickness=5)
        login_frame.place(x=100, y=100, width=1000, height=700)

        login_frame.config(highlightbackground="black", highlightcolor="black")

        #Enter Email
        email_label = Label(
            login_frame,
            text="Enter email",
            font=("times new roman", 17),
            bg="white",
        )
        email_label.grid(row=1, column=1, padx=0, pady=10)
        email_label.place(x=50, y=30, anchor=NW)

        email_entry = ttk.Entry(
            login_frame,
            textvariable=self.var_get_email,
            width=22,
            font=("times new roman", 13),
        )
        email_entry.grid(row=1, column=2, padx=150, pady=10, sticky=W)
        email_entry.place(x=200, y=40, anchor=NW)

        # Otp button
        otp_btn = Button(
            login_frame,
            command=self.get_otp,
            width=16,
            height=2,
            text="Send otp",
            font=("times new roman", 13, "bold"),
            bg="white",
            fg="black",
        )
        otp_btn.grid(row=1, column=3, padx=300, pady = 25)


        #Enter otp
        otp_label = Label(
            login_frame,
            text="Enter otp",
            font=("times new roman", 17),
            bg="white",
        )
        otp_label.grid(row=2, column=1, padx=0, pady=10)
        otp_label.place(x=50, y=80, anchor=NW)

        otp_entry = ttk.Entry(
            login_frame,
            textvariable=self.var_otp,
            width=22,
            font=("times new roman", 13),
        )
        otp_entry.grid(row=2, column=2, padx=150, pady=10, sticky=W)
        otp_entry.place(x=200, y=90, anchor=NW)

        #Enter Passowrd
        password_label = Label(
            login_frame,
            text="New Passowrd",
            font=("times new roman", 17),
            bg="white",
        )
        password_label.grid(row=3, column=1, padx=0, pady=10)
        password_label.place(x=50, y=130, anchor=NW)

        password_entry = ttk.Entry(
            login_frame,
            textvariable=self.var_password,
            width=22,
            font=("times new roman", 13),
        )
        password_entry.grid(row=3, column=2, padx=150, pady=10, sticky=W)
        password_entry.place(x=200, y=140, anchor=NW)

        #Enter password agin
        confirm_label = Label(
            login_frame,
            text="Confirm passowrd",
            font=("times new roman", 17),
            bg="white",
        )
        confirm_label.grid(row=4, column=1, padx=0, pady=10)
        confirm_label.place(x=50, y=180, anchor=NW)

        confirm_entry = ttk.Entry(
            login_frame,
            textvariable=self.var_password_again,
            width=22,
            font=("times new roman", 13),
        )
        confirm_entry.grid(row=4, column=2, padx=150, pady=10, sticky=W)
        confirm_entry.place(x=200, y=190, anchor=NW)

        #Update button
        update_btn = Button(
            login_frame,
            command=self.update_otp,
            width=16,
            height=2,
            text="Update",
            font=("times new roman", 13, "bold"),
            bg="white",
            fg="black",
        )
        update_btn.grid(row=5, column=1, padx=100, pady=240)


        #================Function========================


    def get_otp(self):
        if (
            self.var_get_email.get() == ""
        ):
            messagebox.showerror("Error", "All Fields are required", parent=self.root)

        else:
            temp_email = self.var_get_email.get()
            
            try: # Now we will connect with SQL
                conn = mysql.connector.connect(
                    host="localhost",
                    user="root",
                    password="Shiv@2000",
                    database="face_recogniser",
                    auth_plugin="mysql_native_password",
                )
                my_cursor = conn.cursor() # To store the values given by the user
                temp_email1 = "'"+temp_email+"'"
                print(temp_email1)
                sql = 'select email from credentials where email={}'.format(str(temp_email1))
                my_cursor.execute(sql)

                my_email = my_cursor.fetchall()
                
                conn.commit()
                #self.fetch_data()
                conn.close() # Closing teh connection
                
            except Exception as es:
                messagebox.showerror("Error", f"Due to : {str(es)}", parent=self.root)

            if(len(my_email)>0):
                if str(my_email[0][0])==str(temp_email):
                    # Give OTP
                    admin_email = 'sphinxphoenix.adm@gmail.com'
                    admin_password = "Suv.square"

                    # Now wend the OTP

                    self.final_otp = np.random.randint(100000,900000)
                    self.var_after_5_min = datetime.datetime.now() + datetime.timedelta(minutes = 1)
                    
                    try: 
                        # creates SMTP session
                        s = smtplib.SMTP('smtp.gmail.com', 587)

                        # start TLS for security
                        s.starttls()

                        # Authentication
                        s.login(admin_email, admin_password)

                        # message to be sent
                        message = "Otp is "+str(self.final_otp)+". OTP is valid for 5 minutes only."

                        # sending the mail
                        s.sendmail(admin_email, temp_email, message)

                        # terminating the session
                        s.quit()

                        messagebox.showinfo("Success", "OTP has been sent to your registered email id", parent=self.root)
                    
                    except Exception as es:
                        messagebox.showerror("Error", f"Due to : {str(es)}", parent=self.root)

                    
                else:
                    messagebox.showerror("Error", "Invalid Email (Enter the registered thapar mail", parent=self.root)
            else:
                messagebox.showerror("Error", "Invalid Email (Enter the registered thapar mail", parent=self.root)

            
    def update_otp(self):
        if (
            self.var_otp.get() == ""
            or self.var_password.get() == ""
            or self.var_password_again.get() == ""
            or self.var_get_email.get()==""
        ):
            messagebox.showerror("Error", "All Fields are required", parent=self.root)
            
        else:
            now_time = datetime.datetime.now()
            if now_time < self.var_after_5_min:
                temp_otp = self.var_otp.get()
                temp_password = self.var_password.get()
                temp_password_again = self.var_password_again.get()
                temp_email = self.var_get_email.get()

                print(temp_otp)
                print(self.final_otp)

                if str(temp_password) == str(temp_password_again):

                    if str(temp_otp) == str(self.final_otp):
                        try: 
                            conn = mysql.connector.connect(
                                host="localhost",
                                user="root",
                                password="Shiv@2000",
                                database="face_recogniser",
                                auth_plugin="mysql_native_password",
                            )
                            my_cursor = conn.cursor() # To store the values given by the user
                            password1 = "'"+temp_password+"'"
                            temp_email1 = "'"+temp_email+"'"

                            sql = "UPDATE credentials SET password = {} WHERE email = {} ".format(str(password1),str(temp_email1))
                            my_cursor.execute(sql)
                            
                            #password = my_cursor.fetchall()
                            


                            conn.commit()
                            #self.fetch_data()
                            conn.close() # Closing teh connection

                            messagebox.showinfo("Success", "Student details have been added successfully", parent=self.root)
                            
                        except Exception as es:
                            messagebox.showerror("Error", f"Due to : {str(es)}", parent=self.root)

                    else:
                        messagebox.showerror("Error", "Invalid OTP", parent=self.root)

                else:
                    messagebox.showerror("Error", "Both passwords do not match", parent=self.root)

            else: 
                messagebox.showerror("Error", "Login Session expired.", parent=self.root)


        

        

if __name__ == "__main__":
    root = Tk()
    obj = forgot_password(root)
    root.mainloop()