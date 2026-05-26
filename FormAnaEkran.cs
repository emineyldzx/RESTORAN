using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class FormAnaEkran : Form
    {
        private Button btnPersonel;
        private Button btnAdmin;
        private Label lblBaslik;
        private Label lblAltBaslik;

        public FormAnaEkran()
        {
            this.Opacity = 0;
            this.DoubleBuffered = true;

            InitializeComponent();

            this.Shown += (s, e) => this.Opacity = 1; 
            
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

        }
        
        
       
        
        
            
        
        private void InitializeComponent()
        {
            Panel ortaPanel = new Panel();
            ortaPanel.Size = new Size(600, 400);
            ortaPanel.BackColor = Color.FromArgb(170, 0, 0, 0);
            ortaPanel.Left = (this.ClientSize.Width - ortaPanel.Width) / 2;
            ortaPanel.Top = (this.ClientSize.Height - ortaPanel.Height) / 2;

            this.Text = "Restoran Sipariş Sistemi";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            using (var fs = new FileStream("restoranfoto.jpg", FileMode.Open, FileAccess.Read))
            {
                this.BackgroundImage = Image.FromStream(fs);
            }
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            lblBaslik = new Label
            {
                Text = "🍽️ RESTORAN",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(120, 40)
            };

            lblAltBaslik = new Label
            {
                Text = "SİPARİŞ SİSTEMİ",
                Font = new Font("Segoe UI", 25),
                ForeColor = Color.FromArgb(229, 57, 53),
                AutoSize = true,
                Location = new Point(145, 100)
            };

            btnPersonel = new Button
            {
                Text = "👥 PERSONEL GİRİŞİ\nSipariş oluşturmak için",
                Size = new Size(260, 90),
                Location = new Point(170, 170),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 15),
                Cursor = Cursors.Hand
            };
            btnPersonel.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 80);
            btnPersonel.Click += (s, e) => AcGiris("Personel");

            btnAdmin = new Button
            {
                Text = "🛡️ ADMİN GİRİŞİ\nYönetim paneli için",
                Size = new Size(260, 90),
                Location = new Point(170, 280),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 15),
                Cursor = Cursors.Hand
            };
            btnAdmin.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 80);
            btnAdmin.Click += (s, e) => AcGiris("Admin");

            ortaPanel.Controls.Add(lblBaslik);
            ortaPanel.Controls.Add(lblAltBaslik);
            ortaPanel.Controls.Add(btnPersonel);
            ortaPanel.Controls.Add(btnAdmin);

            this.Controls.Add(ortaPanel);
            this.Shown += (s, e) =>
            {
                ortaPanel.Left = (this.ClientSize.Width - ortaPanel.Width) / 2;
                ortaPanel.Top = (this.ClientSize.Height - ortaPanel.Height) / 2;
            };


        }

        private void AcGiris(string tip)
        {
            var f = new FormGiris(tip);
            f.ShowDialog();
        }
    }
}
