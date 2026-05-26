using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class FormGiris : Form
    {
        private string loginTip;
        private TextBox txtKullanici;
        private TextBox txtSifre;
        private Button btnGiris;
        private Button btnGeriDon;
        private Label lblBaslik;
        private Label lblHata;

       
            public FormGiris(string tip)
        {
            this.Opacity = 0;
            this.DoubleBuffered = true;

            loginTip = tip;
            InitializeComponent();

            this.Shown += (s, e) => this.Opacity = 1;
        }
        

        private void InitializeComponent()
        {
            Panel girisPanel = new Panel();
            girisPanel.Size = new Size(520, 520);
            girisPanel.BackColor = Color.FromArgb(190, 0, 0, 0);

            girisPanel.Location = new Point(700, 300);

            this.Text = loginTip == "Admin" ? "Admin Girişi" : "Personel Girişi";
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(26, 26, 26);
            using (var fs = new FileStream("restoranfoto.jpg", FileMode.Open, FileAccess.Read))
            {
                this.BackgroundImage = Image.FromStream(fs);
            }
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;

            lblBaslik = new Label
            {
                Text = loginTip == "Admin" ? "🛡️ ADMİN GİRİŞİ" : "👥 PERSONEL GİRİŞİ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(80, 50)
            };

            var lblUser = new Label { Text = "Kullanıcı Adı", ForeColor = Color.Gray, AutoSize = true, Location = new Point(100, 120), Font = new Font("Segoe UI", 11) };
            txtKullanici = new TextBox

            {
                Location = new Point(100, 160),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13),
                BorderStyle = BorderStyle.FixedSingle
               
            };

            var lblPass = new Label { Text = "Şifre", ForeColor = Color.Gray, AutoSize = true, Location = new Point(100, 210), Font = new Font("Segoe UI", 11) };
            txtSifre = new TextBox
            {
                Location = new Point(100, 250),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●'
            };

            lblHata = new Label
            {
                Text = "Kullanıcı adı veya şifre hatalı!",
                ForeColor = Color.FromArgb(239, 83, 80),
                AutoSize = true,
                Location = new Point(100, 290),
                Visible = false,
                Font = new Font("Segoe UI", 10)
            };

            btnGiris = new Button
            {
                Text = "GİRİŞ YAP",
                Location = new Point(100, 320),
                Size = new Size(300, 60),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Click += BtnGiris_Click;

        
            btnGeriDon = new Button
            {
                Text = "← Geri Dön",
                Location = new Point(100, 400),
                Size = new Size(100, 28),
                BackColor = Color.Transparent,
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            btnGeriDon.FlatAppearance.BorderSize = 0;

            girisPanel.Controls.Add(lblBaslik);
            girisPanel.Controls.Add(lblUser);
            girisPanel.Controls.Add(txtKullanici);
            girisPanel.Controls.Add(lblPass);
            girisPanel.Controls.Add(txtSifre);
            girisPanel.Controls.Add(lblHata);
            girisPanel.Controls.Add(btnGiris);
            girisPanel.Controls.Add(btnGeriDon);

            this.Controls.Add(girisPanel);

            btnGeriDon.FlatAppearance.BorderSize = 0;
            btnGeriDon.Click += (s, e) => this.Close();

            txtSifre.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnGiris_Click(null, null); };

          
            
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
         
            string kullanici = txtKullanici.Text.Trim();
            string sifre = txtSifre.Text;

            if (string.IsNullOrEmpty(kullanici) || string.IsNullOrEmpty(sifre))
            {
                lblHata.Text = "Kullanıcı adı ve şifre boş olamaz!";
                lblHata.Visible = true;
                return;
            }

            string sql = "SELECT Rol FROM Kullanicilar WHERE KullaniciAdi=@u AND Sifre=@p";
            var dt = Database.ExecuteQuery(sql, new[] {
                new SQLiteParameter("@u", kullanici),
                new SQLiteParameter("@p", sifre)
            });

            if (dt.Rows.Count == 0)
            {
                lblHata.Visible = true;
                return;
            }

            string rol = dt.Rows[0]["Rol"].ToString();

            if (loginTip == "Admin" && rol != "Admin")
            {
                lblHata.Text = "Admin yetkisi gerekli!";
                lblHata.Visible = true;
                return;
            }
            
           
            Program.AktifKullanici = kullanici;
            Program.AktifRol = rol;

            this.Hide();

            FormPanel panel = new FormPanel();
            panel.ShowDialog();

            this.Close();
        }
    }
}