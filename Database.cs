using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace RestoranSiparisSistemi
{
    public static class Database
    {
        private static string dbPath = "restoran.db";
        private static string connectionString => $"Data Source={dbPath};Version=3;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string createUrunler = @"
                    CREATE TABLE IF NOT EXISTS Urunler (
                        UrunID INTEGER PRIMARY KEY AUTOINCREMENT,
                        UrunAdi TEXT NOT NULL,
                        Kategori TEXT,
                        Fiyat DECIMAL NOT NULL
                    );";

                string createSiparisler = @"
                    CREATE TABLE IF NOT EXISTS Siparisler (
                        SiparisID INTEGER PRIMARY KEY AUTOINCREMENT,
                        MasaNo INTEGER NOT NULL,
                        SiparisTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                        ToplamTutar DECIMAL NOT NULL,
                        Durum TEXT DEFAULT 'Hazırlanıyor'
                    );";

                string createSiparisDetay = @"
                    CREATE TABLE IF NOT EXISTS SiparisDetay (
                        DetayID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SiparisID INTEGER,
                        UrunAdi TEXT,
                        Adet INTEGER,
                        BirimFiyat DECIMAL,
                        ToplamFiyat DECIMAL,
                        FOREIGN KEY(SiparisID) REFERENCES Siparisler(SiparisID)
                    );";

                string createKullanicilar = @"
                    CREATE TABLE IF NOT EXISTS Kullanicilar (
                        KullaniciID INTEGER PRIMARY KEY AUTOINCREMENT,
                        KullaniciAdi TEXT NOT NULL UNIQUE,
                        Sifre TEXT NOT NULL,
                        Rol TEXT NOT NULL
                    );";

                ExecuteNonQuery(conn, createUrunler);
                ExecuteNonQuery(conn, createSiparisler);
                ExecuteNonQuery(conn, createSiparisDetay);
                ExecuteNonQuery(conn, createKullanicilar);

                // Varsayılan kullanıcılar
                string insertUsers = @"
                    INSERT OR IGNORE INTO Kullanicilar (KullaniciAdi, Sifre, Rol) VALUES
                    ('admin', 'admin123', 'Admin'),
                    ('garson1', '1234', 'Personel'),
                    ('garson2', '1234', 'Personel');";
                ExecuteNonQuery(conn, insertUsers);

                // Varsayılan ürünler
                var urunKontrol = Convert.ToInt32(
                 new SQLiteCommand("SELECT COUNT(*) FROM Urunler", conn).ExecuteScalar());

                if (urunKontrol == 0)
                {
                    string insertUrunler = @"
                    INSERT OR IGNORE INTO Urunler (UrunAdi, Kategori, Fiyat) VALUES
                    ('Hamburger', 'Yiyecek', 150),
                    ('Cheeseburger', 'Yiyecek', 170),
                    ('Pizza', 'Yiyecek', 200),
                    ('Coca Cola', 'İçecek', 30),
                    ('Fanta', 'İçecek', 30),
                    ('Su', 'İçecek', 15);";
                ExecuteNonQuery(conn, insertUrunler);
                     }
                 }
             }

        private static void ExecuteNonQuery(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string sql, SQLiteParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                new SQLiteDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string sql, SQLiteParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                  return cmd.ExecuteNonQuery();
              
            }
        }

        public static object ExecuteScalar(string sql, SQLiteParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
    }
}
