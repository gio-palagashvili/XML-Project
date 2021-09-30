using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.String;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int z;
        User currentUser = new();
        static List<User> users = new();
        XmlDocument doc = new XmlDocument();

        private void SetUsers()
        {
            doc.Load("../../../User.xml");
            if (doc.DocumentElement == null) return;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                z++;
                var name = node.SelectSingleNode("name")?.InnerText;
                var password = node.SelectSingleNode("password")?.InnerText;
                var status = node.SelectSingleNode("status")?.InnerText;
                var course = node.SelectSingleNode("course")?.InnerText;
                
                var user = new User {Id = z, Name = name, Password = password, Course = course, Status = status};
                users.Add(user);
            }
        }
        private bool VerifyUser(string name)
        {
            var index = users.FindIndex(user => user.Name == name);
            return index != -1;
        }
        private bool LogIn(string username,string password)
        {
            var x = users.FindIndex(user => user.Password == password && user.Name == username);
            if (x < 0) return false;
            currentUser = users[x];
            
            return true;
        }
        
        
        private void Form1_Load(object sender, EventArgs e)
        { 
        SetUsers();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var username = textBox1.Text;
            var password = textBox2.Text;
            label3.ForeColor = Color.Red;

            if (username.Length == 0 || password.Length == 0)
            {
                label3.Text = @"Fields can't be empty";
            }
            else
            {
                if (VerifyUser(username))
                {
                    label3.Text = @"";
                    if (LogIn(username, password))
                    {
                        label3.Text = $@"Logged in as {currentUser.Name}";
                    }
                }
                else
                {
                    label3.Text = @"User not found";
                }
            }
        }
        //register button
        private void button1_Click(object sender, EventArgs e)
        {
            var username = textBox4.Text;
            var password = textBox3.Text;
            var rpassword = textBox5.Text;
            var course = comboBox1.Text;
            var status = comboBox2.Text;
            
            label7.ForeColor = Color.Red;

            if (username.Length > 0 && password.Length > 0 && rpassword.Length > 0)
            {
                label7.Text = Empty;
                if (!VerifyUser(username))
                {
                    label7.Text = Empty;
                    if (password == rpassword)
                    {
                        var lastIndex = users[^1].Id + 1;
                        var newUser = new User { Id = lastIndex, Name = username, Password = password, Course = course, Status = status};
                        users.Add(newUser);
                        if (newUser.Status == "student")
                        {
                            AddToStudents();
                        }
                        else
                        {
                            AddToTeachers();
                        }
                        WriteChanges();
                        label7.ForeColor = Color.Green;
                        label7.Text = @"Successfully registered";
                    }
                    else
                    {
                        label7.Text = Empty;
                        label7.Text = @"passwords don't match";
                    }       
                }
                else
                {
                    label7.Text = @"username Taken";
                }
            }
            else
            {
                label7.Text = @"Fields can't be empty";
            }
        }
        private static void WriteChanges()
        {
            var xDocument = new XDocument(
                new XElement("Users", users.Select(x =>
                    new XElement("User",
                        new XElement("id", x.Id),
                        new XElement("name", x.Name),
                        new XElement("password", x.Password),
                        new XElement("course", x.Course),
                        new XElement("status", x.Status)
                    )
                ))
            ); 
            xDocument.Save("C:\\Users\\Gio\\source\\repos\\WinFormsApp1\\WinFormsApp1\\User.xml");
        }

        private static void AddToStudents()
        {
            var xDocument = new XDocument(
                new XElement("Students", users.Where(x => x.Status == "student").Select(x => 
                    new XElement("Student",
                        new XElement("id", x.Id),
                        new XElement("name", x.Name),
                        new XElement("grades", users.Select(student => 
                            new XElement("grade", student.Id)
                            )),
                        new XElement("course", x.Course)
                    ))
                ));
            
            xDocument.Save("C:\\Users\\Gio\\source\\repos\\WinFormsApp1\\WinFormsApp1\\Student.xml");
        }

        private static void AddToTeachers()
        {
            var xDocument = new XDocument(
                new XElement("Teachers", users.Where(x => x.Status == "teacher").Select(x => 
                    new XElement("Teacher",
                        new XElement("id", x.Id),
                        new XElement("name", x.Name),
                        new XElement("course", x.Course),
                        new XElement("students", users.Where(x => x.Status == "student").Select(student =>
                        new XElement("Student", student.Id)
                        ))
                    ))
                ));
            xDocument.Save("C:\\Users\\Gio\\source\\repos\\WinFormsApp1\\WinFormsApp1\\Teachers.xml");

        }
    }
}
