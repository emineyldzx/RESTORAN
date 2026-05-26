using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class FormPanel : Form
    {
        private Panel sidebar;
        private Panel contentArea;
        private Label lblSaat;
        private System.Windows.Forms.Timer saat;

        // Sayfa panelleri
        private PanelAnaMenu    pnlAnaMenu;
        private PanelSiparis    pnlSiparis;
        private PanelSiparisler pnlSiparisler;
        private PanelMenu       pnlMenu;
        private PanelRaporlar   pnlRaporlar;

        public FormPanel()
        {
            try
            { 
            InitializeComponent();
            BuildSidebar();
            BuildContent();
            ShowPage("AnaMenu");
        }
        catch (Exception ex)
            {
            MessageBox.Show(ex.ToString());
            }
            }
        private void InitializeComponent()
        {
            this.Text = "Restoran Sipariş Sistemi";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 15);
            this.MinimumSize = new Size(900, 600);
            this.AutoScroll = true;

            // Top bar
            var topbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(26, 26, 26)
            };

            var lblBrand = new Label
            {
                Text = "🍽️  RESTORAN SİPARİŞ SİSTEMİ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            lblSaat = new Label
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Location = new Point(750, 8),
                TextAlign = ContentAlignment.MiddleRight
            };

            var lblUser = new Label
            {
                Text = $"👤 {Program.AktifKullanici} · {Program.AktifRol}",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(750, 32)
            };

            var btnCikis = new Button
            {
                Text = "Çıkış",
                Location = new Point(990, 13),
                Size = new Size(70, 28),
                BackColor = Color.FromArgb(229, 57, 53),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnCikis.FlatAppearance.BorderSize = 0;
            btnCikis.Click += (s, e) => { this.Close(); };

            topbar.Controls.AddRange(new Control[] { lblBrand, lblSaat, lblUser, btnCikis });
            this.Controls.Add(topbar);

            saat = new System.Windows.Forms.Timer { Interval = 1000 };
            saat.Tick += (s, e) => lblSaat.Text = DateTime.Now.ToString("HH:mm dd MMMM yyyy");
            saat.Start();
            lblSaat.Text = DateTime.Now.ToString("HH:mm dd MMMM yyyy");

            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 190,
                BackColor = Color.FromArgb(26, 26, 26),
                Top = 55
            };

            contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 15, 15),
                Padding = new Padding(20),
                AutoScroll = true
            };

            this.Controls.Add(contentArea);
            contentArea.SendToBack();
        }

        private void BuildSidebar()
        {
            sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 220;
            sidebar.BackColor = Color.FromArgb(25, 25, 25);

            this.Controls.Add(sidebar);
            sidebar.BringToFront();

            var navItems = new[]
            {
                ("🏠  Ana Menü",        "AnaMenu",    true),
                ("🛒  Sipariş Oluştur", "Siparis",    true),
                ("📋  Siparişler",      "Siparisler", true),
                ("🍔  Menü",            "Menu",       true),
                ("📊  Raporlar",        "Raporlar",   Program.AktifRol == "Admin"),
            };

            int y = 10;
            foreach (var (text, page, visible) in navItems)
            {
                if (!visible) continue;
                var btn = new Button
                {
                    Text = text,
                    Location = new Point(0, y),
                    Size = new Size(190, 42),
                    BackColor = Color.Transparent,
                    ForeColor = Color.Gray,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    Tag = page
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.MouseEnter += (s, e) => { if (btn.ForeColor == Color.Gray) btn.ForeColor = Color.White; };
                btn.MouseLeave += (s, e) => { if ((string)btn.Tag != ActivePage) btn.ForeColor = Color.Gray; };
                btn.Click += (s, e) => ShowPage((string)btn.Tag);
                sidebar.Controls.Add(btn);
                y += 44;
            }
        }

        private string ActivePage = "";

        private void BuildContent()
        {
            contentArea = new Panel();
            contentArea.Dock = DockStyle.Fill;
            contentArea.BackColor = Color.FromArgb(25, 25, 25);

            this.Controls.Add(contentArea);
            contentArea.BringToFront();

            pnlAnaMenu = new PanelAnaMenu(ShowPage);
            pnlSiparis    = new PanelSiparis();
            pnlSiparisler = new PanelSiparisler();
            pnlMenu       = new PanelMenu();
            pnlRaporlar   = new PanelRaporlar();

            foreach (var p in new Control[] { pnlAnaMenu, pnlSiparis, pnlSiparisler, pnlMenu, pnlRaporlar })
            {
                p.Dock = DockStyle.Fill;
                p.Visible = false;
                contentArea.Controls.Add(p);
            }
        }

        public void ShowPage(string page)
        {
            ActivePage = page;
            pnlAnaMenu.Visible    = page == "AnaMenu";
            pnlSiparis.Visible    = page == "Siparis";
            pnlSiparisler.Visible = page == "Siparisler";
            pnlMenu.Visible       = page == "Menu";
            pnlRaporlar.Visible   = page == "Raporlar";

            if (page == "Siparisler") pnlSiparisler.Yukle();
            if (page == "Menu")       pnlMenu.Yukle();
            if (page == "Raporlar")   pnlRaporlar.Yukle();
            if (page == "Siparis")    pnlSiparis.Yukle();

            // Sidebar renklendirme
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button b)
                    b.ForeColor = (string)b.Tag == page ? Color.White : Color.Gray;
            }
        }
    }
}
