using HighSpirits.Models;
using MySql.Data.MySqlClient;

namespace HighSpirits.Persistence
{
    public class PersistenceCode
    {
        //Connection string maken
        string connStr = "server=localhost;user id=root;password=Test123;database=dbbooze";

        //Method voor laden van alle producten
        public List<Product> loadProducten()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select ProductId, Productnaam, round(Aankoopprijs*1.13,2) as Verkoopprijs, Voorraad, Foto from tblproducten";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            //maak een nieuwe lijst van producten aan
            List<Product> list = new List<Product>();
            while (dtr.Read())
            {
                //maak een product object aan en ken public properties hun waarde toe
                Product product = new Product();
                product.ProductID = dtr.GetInt32("ProductID");
                product.ProductNaam = dtr.GetString("Productnaam");
                product.Verkoopprijs = dtr.GetDouble("Verkoopprijs");
                product.Voorraad = dtr.GetInt32("Voorraad");
                product.Foto = dtr.GetString("Foto");
                //voeg het product toe aan de lijst
                list.Add(product);
            }
            conn.Close();
            return list;
        }

        //Laad een gekozen product
        public Product laadProduct(int ID)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select ProductId, Productnaam, round(Aankoopprijs*1.13,2) as Verkoopprijs, Voorraad, Foto, Omschrijving from tblproducten " +
                "where productId="+ID;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Product product = new Product();
            while (dtr.Read())
            { 
                product.ProductID = dtr.GetInt32("ProductID");
                product.ProductNaam = dtr.GetString("Productnaam");
                product.Verkoopprijs = dtr.GetDouble("Verkoopprijs");
                product.Voorraad = dtr.GetInt32("Voorraad");
                product.Foto = dtr.GetString("Foto");
                product.Omschrijving = dtr.GetString("Omschrijving");
            }
            conn.Close();
            return product;
        }


        //Haal hte aantal stuks op van database
        public int zoekAantalstuks(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select aantalstuks from tblwinkelmandje where KlantId=" + winkelmandje.KlantId + " and ProductId="+winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            int stuks = 0;
            while (dtr.Read())
            {
               stuks = dtr.GetInt32("aantalstuks");

            }
            conn.Close();
            return stuks;
        }

        //haal op hoeveel in voorraad er nog over is
        public int zoekVoorraad(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select voorraad from tblproducten where ProductId=" + winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            int stuks = 0;
            while (dtr.Read())
            {
                stuks = dtr.GetInt32("aantalstuks");

            }
            conn.Close();
            return stuks;
        }

        //Verminder voorraad
        public void verminderVoorrraad(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "update tblproducten set voorraad = voorraad-"+winkelmandje.Aantalstuks+" where ProductId="+winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //kijk voor unieke producten in winkelmandje
        public bool checkProduct(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select KlantId, ProductId from tblwinkelmandje where KlantId=" + winkelmandje.KlantId + " and ProductId=" + winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            bool bestaat = false;
            while (dtr.Read())
            {
                bestaat = true;
            }
            conn.Close();
            return bestaat;
        }

        //laad winkelmandje
        public Winkelmandje laadWinkelmandje(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblwinkelmandje where KlantId=" + winkelmandje.KlantId + " and ProductId=" + winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Winkelmandje wm = new Winkelmandje();
            while (dtr.Read())
            {
                wm.KlantId = dtr.GetInt32("KlantId");
                wm.ProductId = dtr.GetInt32("ProductId");
                wm.Aantalstuks = dtr.GetInt32("Aantalstuks");
            }
            conn.Close();
            return wm;
        }

        //Product in winkelmandje opslaan
        public void insertInWinkelmandje(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "insert into tblwinkelmandje (KlantID, ProductID, aantalstuks) " +
                "values(" + winkelmandje.KlantId + "," + winkelmandje.ProductId + "," + winkelmandje.Aantalstuks + ")";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            verminderVoorrraad(laadWinkelmandje(winkelmandje));
        }

        //aantal stuks van product updaten in winkelmandje
        public void updateWinkelmandje(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            int corrStuks = winkelmandje.Aantalstuks + zoekAantalstuks(winkelmandje);
            conn.Open();
            string qry = "update tblwinkelmandje set aantalstuks = " +
                 + corrStuks + " where KlantId=" + winkelmandje.KlantId + " and ProductId="+winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            verminderVoorrraad(laadWinkelmandje(winkelmandje));
        }

        //kijk of user ingelogd is
        public int checkCredentials(LoginCredentials loginCredentials)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select KlantId from tblklanten where gebruikersnaam='" + loginCredentials.Gebruikersnaam + "' and binary wachtwoord='" + loginCredentials.Wachtwoord + "'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            int id = -1;
            while (dtr.Read())
            {
                id = dtr.GetInt32("KlantId");
            }
            conn.Close();
            return id;
        }

        //registreer een user
        public void maakUserAan(Registratie re)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string corrDatum = re.Geboortedatum.ToString("yyyy-MM-dd HH:mm:ss");
            string qry = "insert into tblKlanten (Naam, Voornaam, Email, Wachtwoord, Gebruikersnaam, Adres, PostCode, Geboortedatum, Telefoonnummer) values " +
                "('" +re.Naam+"','"+re.Voornaam+"','"+re.Email + "','" + re.Wachtwoord + "','" + re.Gebruikersnaam + "','" + re.Adress + "','" + re.PostCode + "','" + corrDatum + "','" + re.Telefoonnummer + "')";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //kijk voor unieke gebruikersnaam
        public bool checkNaam(string naam)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select gebruikersnaam from tblklanten where gebruikersnaam='" + naam + "'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            bool bestaat = false;
            while (dtr.Read())
            {
                bestaat = true;
            }
            conn.Close();
            return bestaat;
        }
    }
}
