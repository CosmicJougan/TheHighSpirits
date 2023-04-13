using HighSpirits.Models;
using MySql.Data.MySqlClient;
using NuGet.Protocol.Core.Types;
using System.Net.Mail;

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
                stuks = dtr.GetInt32("voorraad");

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

        //herlaad winkelmandje voor verminderen van voorraad
        public Winkelmandje herlaadWinkelmandje(Winkelmandje winkelmandje)
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
            verminderVoorrraad(herlaadWinkelmandje(winkelmandje));
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
            verminderVoorrraad(winkelmandje);
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
            string corrDatum = Convert.ToDateTime(re.Geboortedatum).ToString("yyyy-MM-dd HH:mm:ss");
            string qry = "insert into tblKlanten (Naam, Voornaam, Email, Wachtwoord, Gebruikersnaam, Adres, Plaats, PostCode, Geboortedatum, Telefoonnummer) values " +
                "('" +re.Naam+"','"+re.Voornaam+"','"+re.Email + "','" + re.Wachtwoord + "','" + re.Gebruikersnaam + "','" + re.Adress + "','" + re.Plaats + "','" + re.PostCode + "','" + corrDatum + "','" + re.Telefoonnummer + "')";
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

        //laad klant
        public LoginCredentials laadKlant(int ID)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select KlantId, Naam, Voornaam, Adres,Plaats, email, Postcode from tblklanten " +
                "where KlantId=" + ID;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            LoginCredentials klant = new LoginCredentials();
            while (dtr.Read())
            {
                klant.KlantId = dtr.GetInt32("KlantId");
                klant.Naam = dtr.GetString("Naam");
                klant.Voornaam = dtr.GetString("Voornaam");
                klant.Adress = dtr.GetString("Adres");
                klant.Plaats = dtr.GetString("Plaats");
                klant.PostCode = dtr.GetString("Postcode");
                klant.Email = dtr.GetString("Email");
            }
            conn.Close();
            return klant;
        }

        //laad klantId
        public int zoekKlantId(string Gnaam)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select KlantId from tblKlanten where gebruikersnaam='" + Gnaam + "'";
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

        //Laad winkelmandje repo
        public List<Product> laadWinkelmandjeProducten(int KlantID)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select tblproducten.ProductId, Productnaam, round(Aankoopprijs*1.13,2) as Verkoopprijs, Foto, aantalstuks from tblproducten " +
                "inner join tblwinkelmandje on tblproducten.productId = tblwinkelmandje.ProductId where KlantId="+KlantID;
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
                product.Voorraad = dtr.GetInt32("aantalstuks");
                product.Foto = dtr.GetString("Foto");
                list.Add(product);
            }
            conn.Close();
            return list;
        }
        //laad winkelmandje voor verminderen van voorraad
        public Winkelmandje laadWinkelmandje(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblwinkelmandje where KlantId=" + winkelmandje.KlantId + " and ProductId="+ winkelmandje.ProductId;
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

        //haal de prijs totalen op voor winkelmandje
        public Totalen haalTotalen(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select Round(sum(aankoopprijs*1.13*aantalstuks),2) as VerkoopPrijsExcl, Round(sum(aankoopprijs*0.21*aantalstuks),2) as BTW, Round((sum(aankoopprijs*1.13*aantalstuks))+(sum(aankoopprijs*0.21*aantalstuks)),2) as VerkoopPrijsIncl from tblProducten " +
                "inner join tblwinkelmandje on tblproducten.productId = tblwinkelmandje.productId where KlantId=" + winkelmandje.KlantId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Totalen totalen = new Totalen();
            while (dtr.Read())
            {
                totalen.PrijsExclusief = dtr.GetDouble("VerkoopPrijsExcl");
                totalen.BTW = dtr.GetDouble("BTW");
                totalen.PrijsInclusief = dtr.GetDouble("VerkoopPrijsIncl");

            }
            conn.Close();
            return totalen;
        }

        //Vermeerder voorraad na delete

        public void vermeerderVoorrraad(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "update tblproducten set voorraad = voorraad+" + winkelmandje.Aantalstuks + " where ProductId=" + winkelmandje.ProductId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete uit winkelmandje
        
        public void deleteUitWinkelmandje(Winkelmandje winkelmandje)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            vermeerderVoorrraad(winkelmandje);
            conn.Open();
            string qry = "delete from tblwinkelmandje where ProductId=" + winkelmandje.ProductId + " and KlantId="+ winkelmandje.KlantId;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //winkemandje items in bestellijn opslaan

        public void insertIntoBestelLijn(Bestellijn bestellijn)
        {

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string corrPrijs = bestellijn.HistorischePrijs.ToString().Replace(",", ".");
            string qry = "insert into tblbestellijnen (BestellingID, ProductId, Historischeprijs, Aantalstuks) " +
                "values(" + bestellijn.BestellingId + "," + bestellijn.ProductId + ",'" + corrPrijs + "'," + bestellijn.Aantalstuks + ")";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //winkelmandje items in de bestelling zetten

        public void BestelItems(Bestelling bestelling)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            string corrDatum = bestelling.Datum.ToString("yyyy-MM-dd HH:mm:ss");
            conn.Open();
            string qry = "insert into tblbestelling (KlantID, Datum) " +
                "values("+bestelling.KlantId+",'"+corrDatum+"')";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //laad bestelling
        public Bestelling laadLaatsteBestelling(int KlantId)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblbestelling " +
                "where KlantId="+KlantId+" " +
                "order by datum desc " +
                "limit 1";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Bestelling bestelling = new Bestelling();
            while (dtr.Read())
            {
                bestelling.BestellingId = dtr.GetInt32("BestellingID");
                bestelling.KlantId = dtr.GetInt32("KlantId");
                bestelling.Datum = dtr.GetDateTime("Datum");
            }
            conn.Close();
            return bestelling;
        }

        //delete na bestelling
        public void clearWinkelmandje(int KlantID)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "delete from tblwinkelmandje where KlantId=" + KlantID;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //maak email
        public void stuurMail(VMbestellingen vmBestelling)
        {
            string Ontvanger = laadKlant(vmBestelling.bestelling.KlantId).Email;
            string Onderwerp = "Bestelling HighSpirits";
            string Bericht =    "We hebben u bestelling met bestelnummer "+ vmBestelling.bestelling.BestellingId +" goed ontvangen." +
                                "Na overschrijving van € "+vmBestelling.totalen.PrijsInclusief+" op rekeningnummer BE41 0019 4020 4710 worden de goederen verpakt en verstuurd." +
                                "Drive safe. En niet drinken en rijden, alleen bij stop lichten. " +
                                "Bedankt voor u vertrouwen.";
            //1. SMTP client Instellen
            SmtpClient SMTP = new SmtpClient("smtp-mail.outlook.com");
            SMTP.Credentials = new System.Net.NetworkCredential("HighSpirits.Shop@outlook.com", "VeelDrinkenVeelRijden");
            SMTP.Port = 587;
            SMTP.EnableSsl = true;


            //2. Mailbericht Instellen
            MailMessage Mail = new MailMessage();
            Mail.From = new MailAddress("HighSpirits.Shop@outlook.com");
            Mail.To.Add(Ontvanger);
            Mail.Subject = Onderwerp;
            Mail.Body = Bericht;

            //3. Versturen
            SMTP.Send(Mail);
}


    }
}
