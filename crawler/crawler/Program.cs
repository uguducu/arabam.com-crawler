using System.Net;
using crawler.Models;
using HtmlAgilityPack;
using crawler.Context;
using crawler.Models;

HtmlWeb web = new HtmlWeb();
HtmlDocument carListDoc = new HtmlDocument();
List<Cars> carsList = new List<Cars>();

int repeatedPostings = 0;
int errorCount = 1;


for (int pageCounter = 0; pageCounter <= 50; pageCounter++)
{
    for (int tabCounter = 0; tabCounter <= 3; tabCounter++)
    {
        if (tabCounter == 0)
            carListDoc = web.Load($"https://www.arabam.com/ikinci-el/otomobil?take=50&page={pageCounter}");
        else if (tabCounter == 1)
            carListDoc = web.Load($"https://www.arabam.com/ikinci-el/otomobil-sahibinden?take=50&page={pageCounter}");
        else if (tabCounter == 2)
            carListDoc = web.Load($"https://www.arabam.com/ikinci-el/otomobil-galeriden?take=50&page={pageCounter}");
        else if (tabCounter == 3)
            carListDoc =
                web.Load($"https://www.arabam.com/ikinci-el/otomobil-yetkili-bayiden?take=50&page={pageCounter}");

        #region Collecting Data On Detail Page

        var carListNodes =
            carListDoc.DocumentNode.SelectNodes("//tr[@class='listing-list-item pr should-hover bg-white']");

        foreach (HtmlNode carListNode in carListNodes)
        {
            Cars carsListParameter = new Cars();
            HtmlDocument docDetail = new HtmlDocument();

            var carDetailLink = carListNode.SelectSingleNode(".//a[@class='smallest-text-minus ovh']")
                .Attributes["href"]
                .Value;

            docDetail = web.Load($"https://www.arabam.com/{carDetailLink}");

            var advertId = docDetail.DocumentNode
                .SelectSingleNode("//li[1][@class='bcd-list-item']//span[@class='bli-particle semi-bold']")?.InnerHtml
                .Replace(" ", "")
                .Replace("\r\n", "")
                .Replace("\r\n\r\n", "")
                .Replace("hp", "");
            
            

            if (carsList.Any(x => x.AdvertId == advertId))
            {
                repeatedPostings++;
            }

            if (!carsList.Any(x => x.AdvertId == advertId))
            {
                using var _context = new AppDbContext();
                carsListParameter.Brand = docDetail.DocumentNode
                    .SelectSingleNode("//li[3][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                carsListParameter.Series = docDetail.DocumentNode
                    .SelectSingleNode("//li[4][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                carsListParameter.Model = docDetail.DocumentNode
                    .SelectSingleNode("//li[5][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                var year = docDetail.DocumentNode
                    .SelectSingleNode("//li[6][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                carsListParameter.Year = Convert.ToInt32(year);
                var kilometer = docDetail.DocumentNode
                    .SelectSingleNode("//li[7][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "")
                    .Replace("km", "")
                    .Replace(".","");
                carsListParameter.Kilometer = Convert.ToInt32(kilometer);
                carsListParameter.GearType = docDetail.DocumentNode
                    .SelectSingleNode("//li[8][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                carsListParameter.FuelType = docDetail.DocumentNode
                    .SelectSingleNode("//li[9][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                carsListParameter.CaseType = docDetail.DocumentNode
                    .SelectSingleNode("//li[10][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "");
                var engineCapacity = docDetail.DocumentNode
                    .SelectSingleNode("//li[11][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "")
                    .Replace("cc", "");
                carsListParameter.EngineCapacity = engineCapacity;
                var motorPower = docDetail.DocumentNode
                    .SelectSingleNode("//li[12][@class='bcd-list-item']//span[@class='bli-particle ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "")
                    .Replace("hp", "");
                carsListParameter.MotorPower = motorPower;
                var price = docDetail.DocumentNode
                    .SelectSingleNode(
                        "//div//span[@class='color-red4 font-default-plusmore bold fl  ']")?.InnerHtml
                    .Replace(" ", "")
                    .Replace("\r\n", "")
                    .Replace("\r\n\r\n", "")
                    .Replace("TL", "").Replace(".", "").Replace(",","");
                carsListParameter.Price = Convert.ToDecimal(price);

                carsListParameter.AdvertId = advertId;
                carsListParameter.Link = $"https://www.arabam.com/{carDetailLink}";
                carsListParameter.Source = "www.arabam.com";
                carsListParameter.CreateDate = DateTime.UtcNow;
                carsList.Add(carsListParameter);
                
                try
                {
                    _context.Cars.Add(carsListParameter);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    errorCount++;
                    Console.WriteLine("Hata: " + e.Message.ToString());
                    continue;
                }
                
                Console.WriteLine("Model: " + carsListParameter.Brand);
                Console.WriteLine("Seri: " + carsListParameter.Series);
                Console.WriteLine("Model: " + carsListParameter.Model);
                Console.WriteLine("Yıl: " + carsListParameter.Year);
                Console.WriteLine("Kilometre: " + carsListParameter.Kilometer);
                Console.WriteLine("Vites Tipi: " + carsListParameter.GearType);
                Console.WriteLine("Yakıt Tipi: " + carsListParameter.FuelType);
                Console.WriteLine("Kasa Tipi: " + carsListParameter.CaseType);
                Console.WriteLine("Motor Hacmi: " + carsListParameter.EngineCapacity);
                Console.WriteLine("Motor Gücü: " + carsListParameter.MotorPower);
                Console.WriteLine("Fiyat: " + carsListParameter.Price);
                Console.WriteLine("Link: " + carsListParameter.Link);
                Console.WriteLine("Kaynak: " + carsListParameter.Source);
                Console.WriteLine("Oluşturulma Tarihi: " + carsListParameter.CreateDate);
                Console.WriteLine("İlan No: " + carsListParameter.AdvertId);
                Console.WriteLine("--------------------");
            }
        }

        #endregion
    }
}


#region Printing Data to the Screen

Console.WriteLine("BULUNAN İLAN SAYISI: " + carsList.Count);
Console.WriteLine("BULUNAN MARKA SAYISI: " + carsList.Select(x => x.Brand).Distinct().Count());
Console.WriteLine("BULUNAN SERİ SAYISI: " + carsList.Select(x => x.Series).Distinct().Count());
Console.WriteLine("BULUNAN MODEL SAYISI: " + carsList.Select(x => x.Model).Distinct().Count());
Console.WriteLine("TEKRAR EDEN İLAN SAYISI: " + repeatedPostings);
Console.WriteLine("HATA SAYISI: " + errorCount);

#endregion

Console.ReadLine();