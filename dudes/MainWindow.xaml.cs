using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageHelpers;

namespace dudes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageHelper ImageHelper = new ImageHelper();

        int canvasWidth = 500;
        int canvasHeight = 500;

        int townCount = 20;
        int dudeCount = 10;

        double dudeSpeed = 3;

        int maxShopsPerTown = 10;

        int townToShopDistance = 10;
        
        List<Town> towns = new List<Town>();
        List<Dude> dudes = new List<Dude>();
        List<Shop> shops = new List<Shop>();

        System.Drawing.Color townColour = System.Drawing.Color.Blue;
        System.Drawing.Color shopColour = System.Drawing.Color.Red;

        Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);
            GenerateWorld();
        }

        double Distance(int fromX, int fromY, int toX, int toY)
        {
            double distanceX = Math.Abs(toX - fromX);
            double distanceY = Math.Abs(toY - fromX);

            return Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
        }

        private void CreateTowns()
        {
            for (int i = 0; i < townCount; i++)
            {
                Town town = new Town();
                town.Id = i;
                town.X = random.Next(canvasWidth);
                town.Y = random.Next(canvasHeight);
                Thread.Sleep(10);

                int numberOfShops = random.Next(1, maxShopsPerTown);

                for (int j = 0; j < numberOfShops; j++)
                {
                    CreateShop(town);
                }
                towns.Add(town);
            }
        }

        private void CreateShop(Town town)
        {
            //create a shop a certain distance from the town
            Shop shop = new Shop();

            int shopX = -1;
            int shopY = -1;

            //pick a point townToShopDistance away from the town
            while (shopX < 0 || shopX > canvasWidth)
            {
                int shopXMin = town.X - townToShopDistance;
                shopX = shopXMin + random.Next(townToShopDistance * 2);
            }
            while (shopY < 0 || shopY > canvasHeight)
            {
                int shopYMin = town.Y - townToShopDistance;
                shopY = shopYMin + random.Next(townToShopDistance * 2);
            }
            shop.X = shopX;
            shop.Y = shopY;

            shops.Add(shop);
        }

        private void CreateDudes()
        {
            for (int i = 0; i < dudeCount; i++)
            {
                Dude dude = new Dude();
                dude.Age = 0;
                dude.X = random.Next(canvasWidth);
                dude.Y = random.Next(canvasHeight);
                dude.Health = 255;
                dudes.Add(dude);
            }
            
        }

        private void Draw()
        {
            DirectBitmap bitmap = new DirectBitmap(canvasWidth, canvasHeight);

            foreach (Town town in towns)
            {
                bitmap.SetPixel(town.X, town.Y, townColour);
            }

            foreach (Shop shop in shops)
            {
                bitmap.SetPixel(shop.X, shop.Y, shopColour);
            }

            foreach (Dude dude in dudes)
            {
                bitmap.SetPixel(dude.X, dude.Y, System.Drawing.Color.FromArgb(255, dude.Health, dude.Health, dude.Health));
            }
            canvas.Source = ImageHelper.BitmapToImageSource(bitmap.Bitmap);
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            CreateTowns();
            Thread.Sleep(10);
            CreateDudes();

            Draw();
        }

        private void ResetWorld_Click(object sender, RoutedEventArgs e)
        {
            ResetWorld();
        }

        private void ResetWorld()
        {
            towns = new List<Town>();
            shops = new List<Shop>();
            dudes = new List<Dude>();
            Draw();
        }
    }
}
