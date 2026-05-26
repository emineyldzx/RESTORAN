namespace RestoranSiparisSistemi;

internal static class Program
{
    public static string AktifKullanici = "";
    public static string AktifRol = "";

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Database.Initialize();
        Application.Run(new FormAnaEkran());
    }
}