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
        
        int canvasWidth = 400;
        int canvasHeight = 200;

        int townCount = 20;
        int dudeCount = 20;
        int maxDudes = 250;

        int minBreedAge = 18;
        int maxBreedAge = 50;

        int maxAge = 100;

        double dudeSpeed = 3;

        int maxShopsPerTown = 10;
        int townToShopDistance = 10;

        double desireToStayAtHome = 7;
        double desireToGoToTown = 10;
        double desireToGoToShop = 5;
        double desireToGoFar = 5;

        List<Town> towns = new List<Town>();
        List<Dude> dudes = new List<Dude>();
        List<Shop> shops = new List<Shop>();

        List<string> maleNames = new List<string>();
        List<string> femaleNames = new List<string>();
        List<string> allNames = new List<string>();

        List<string> surnames = new List<string>();
        List<string> townNames = new List<string>();
        List<string> companyNames = new List<string>();

        System.Drawing.Color townColour = System.Drawing.Color.Purple;
        System.Drawing.Color shopColour = System.Drawing.Color.Green;

        Random random = new Random();

        public delegate void UpdateBitmapCallback(Bitmap bitmap);
        public delegate void UpdateActionLogCallback(string actionUpdate);
        public delegate void UpdateMousePosition(System.Drawing.Point location);

        List<string> logMessages = new List<string>();

        bool go = false;

        int gameObjectId = 0;

        List<GameObject>[,] whatsAtLocation;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);
            whatsAtLocation = new List<GameObject>[canvasWidth, canvasHeight];

            LoadNames();

            //GenerateWorld();
        }

        private void LoadNames()
        {
            string line;
            StreamReader file = new StreamReader(@"Resources/FemaleNames.txt");
            while ((line = file.ReadLine()) != null)
            {
                femaleNames.Add(line[0] + line.Substring(1).ToLower());
            }

            file = new StreamReader(@"Resources/MaleNames.txt");
            while ((line = file.ReadLine()) != null)
            {
                maleNames.Add(line[0] + line.Substring(1).ToLower());
            }

            file = new StreamReader(@"Resources/Surnames.txt");
            while ((line = file.ReadLine()) != null)
            {
                surnames.Add(line[0] + line.Substring(1).ToLower());
            }

            file = new StreamReader(@"Resources/TownNames.txt");
            while ((line = file.ReadLine()) != null)
            {
                townNames.Add(line[0] + line.Substring(1).ToLower());
            }
            file = new StreamReader(@"Resources/CompanyNames.txt");
            while ((line = file.ReadLine()) != null)
            {
                companyNames.Add(line[0] + line.Substring(1).ToLower());
            }
            allNames.AddRange(maleNames);
            allNames.AddRange(femaleNames);
            allNames.AddRange(townNames);
        }

        private void UpdateStatusText(System.Windows.Point location)
        {
            int actualX = (int)((location.X / canvas.ActualWidth) * canvasWidth);
            int actualY = (int)((location.Y / canvas.ActualHeight) * canvasHeight);
            MousePos.Text = $"X={actualX} Y={actualY}";
            if (actualX >= canvasWidth)
                actualX = canvasWidth - 1;
            if (actualY >= canvasHeight)
                actualY = canvasHeight - 1;

            List<GameObject> objectsAtLocation = whatsAtLocation[actualX, actualY];

            foreach(GameObject gameobject in objectsAtLocation)
            {
                MousePos.Text += $" {gameobject.Name}";
                if(gameobject is Dude)
                {
                    Dude thisDude = (Dude)gameobject;
                    MousePos.Text += $" ({(int)thisDude.Age})";
                }
            }
            
        }

        private void UpdateCanvas(Bitmap bitmap)
        {
            canvas.Source = ImageHelper.BitmapToImageSource(bitmap);
        }

        private void UpdateActionLog(string updateText)
        {
            logMessages.Add(updateText);

            if(logMessages.Count>30)
            {
                logMessages.RemoveAt(0);
            }
            string actionLogText = "";

            for (int i = logMessages.Count - 1; i > 0 ; i--)
            {
                actionLogText += logMessages[i] + "\r\n";
            }


            actionLog.Text = actionLogText;

            dudeCountLabel.Content = $"Dudes:{dudes.Count}";
        }

        public void Draw()
        {
            DirectBitmap bitmap = new DirectBitmap(canvasWidth, canvasHeight);
            
            try
            {
                while (go)
                {
                    for (int y = 0; y < canvasHeight; y++)
                    {
                        for (int x = 0; x < canvasWidth; x++)
                        {
                            bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                            whatsAtLocation[x, y] = new List<GameObject>();
                        }
                    }

                    foreach (Town town in towns)
                    {
                        if (town.X >= canvasWidth) town.X = canvasWidth - 1;
                        if (town.Y >= canvasHeight) town.Y = canvasHeight - 1;

                        // left
                        if (town.X > 1)
                        {
                            bitmap.SetPixel((int)town.X - 1, (int)town.Y, townColour);
                            whatsAtLocation[(int)town.X - 1, (int)town.Y].Add(town);
                        }
                        // right
                        if (town.X < canvasWidth - 1)
                        {
                            bitmap.SetPixel((int)town.X + 1, (int)town.Y, townColour);
                            whatsAtLocation[(int)town.X + 1, (int)town.Y].Add(town);
                        }
                        // top
                        if (town.Y > 1)
                        {
                            bitmap.SetPixel((int)town.X, (int)town.Y - 1, townColour);
                            whatsAtLocation[(int)town.X, (int)town.Y - 1].Add(town);
                        }
                        //bottom
                        if (town.Y < canvasWidth - 1)
                        {
                            bitmap.SetPixel((int)town.X, (int)town.Y + 1, townColour);
                            whatsAtLocation[(int)town.X, (int)town.Y + 1].Add(town);
                        }

                        // top left
                        if (town.X > 1 && town.Y > 1)
                        {
                            bitmap.SetPixel((int)town.X - 1, (int)town.Y - 1, townColour);
                            whatsAtLocation[(int)town.X - 1, (int)town.Y - 1].Add(town);
                        }
                        //top right
                        if (town.X < canvasWidth - 1 && town.Y > 1)
                        {
                            bitmap.SetPixel((int)town.X + 1, (int)town.Y - 1, townColour);
                            whatsAtLocation[(int)town.X + 1, (int)town.Y - 1].Add(town);
                        }
                        //bottom left
                        if (town.X > 1 && town.Y < canvasHeight - 1)
                        {
                            bitmap.SetPixel((int)town.X - 1, (int)town.Y + 1, townColour);
                            whatsAtLocation[(int)town.X - 1, (int)town.Y + 1].Add(town);
                        }
                        //bottom right
                        if (town.X < canvasWidth - 1 && town.Y < canvasWidth - 1)
                        {
                            bitmap.SetPixel((int)town.X + 1, (int)town.Y + 1, townColour);
                            whatsAtLocation[(int)town.X + 1, (int)town.Y + 1].Add(town);
                        }

                        bitmap.SetPixel((int)town.X, (int)town.Y, townColour);

                        whatsAtLocation[(int)town.X, (int)town.Y].Add(town);
                    }

                    foreach (Shop shop in shops)
                    {
                        if (shop.X >= canvasWidth) shop.X = canvasWidth - 1;
                        if (shop.Y >= canvasHeight) shop.Y = canvasHeight - 1;


                        // left
                        if (shop.X > 1)
                        {
                            bitmap.SetPixel((int)shop.X - 1, (int)shop.Y, shopColour);
                            whatsAtLocation[(int)shop.X - 1, (int)shop.Y].Add(shop);
                        }
                        // right
                        if (shop.X < canvasWidth - 1)
                        {
                            bitmap.SetPixel((int)shop.X + 1, (int)shop.Y, shopColour);
                            whatsAtLocation[(int)shop.X + 1, (int)shop.Y].Add(shop);
                        }
                        // top
                        if (shop.Y > 1)
                        {
                            bitmap.SetPixel((int)shop.X, (int)shop.Y - 1, shopColour);
                            whatsAtLocation[(int)shop.X, (int)shop.Y - 1].Add(shop);
                        }
                        //bottom
                        if (shop.Y < canvasWidth - 1)
                        {
                            bitmap.SetPixel((int)shop.X, (int)shop.Y + 1, shopColour);
                            whatsAtLocation[(int)shop.X, (int)shop.Y + 1].Add(shop);
                        }



                        bitmap.SetPixel((int)shop.X, (int)shop.Y, shopColour);

                        whatsAtLocation[(int)shop.X, (int)shop.Y].Add(shop);
                    }

                    foreach (Dude dude in dudes)
                    {
                        if (dude.IsAtDestination())
                        {
                            dude.PickNewDestination(shops, towns, desireToGoToShop, desireToGoToTown, desireToStayAtHome, desireToGoFar);
                            if (dude.Action != Dude.action.StayingAtHome)
                            {
                                string updateText = $"{dude.Name} ({(int)dude.Age}) is going to {dude.DestinationName}";
                                canvas.Dispatcher.Invoke(new UpdateActionLogCallback(this.UpdateActionLog), new object[] { updateText });
                            }
                        }
                        else
                        {
                            dude.GoToDestination(dudeSpeed);
                        }
                        if (dude.X >= canvasWidth) dude.X = canvasWidth - 1;
                        if (dude.Y >= canvasHeight) dude.Y = canvasHeight - 1;

                        dude.timeSinceLastChild += 0.01;
                        dude.Age += 0.01;
                        whatsAtLocation[(int)dude.X, (int)dude.Y].Add(dude);


                        int redFactor = 0;
                        int blueFactor = 0;
                        int greenFactor = 0;

                        if (dude.Gender == Dude.gender.Male)
                        {
                            blueFactor = 255;
                            redFactor = 0;
                        }
                        else
                        {
                            blueFactor = 0;
                            redFactor = 255;
                        }

                        double ageFactor = (dude.Age / maxAge) * 255;

                        redFactor = redFactor - (int)ageFactor;
                        greenFactor = greenFactor - (int)ageFactor;
                        blueFactor = blueFactor - (int)ageFactor;

                        if (redFactor < 0) redFactor = 0;
                        if (greenFactor < 0) greenFactor = 0;
                        if (blueFactor < 0) blueFactor = 0;

                        bitmap.SetPixel((int)dude.X, (int)dude.Y, System.Drawing.Color.FromArgb(255, redFactor, greenFactor, blueFactor));
                    }

                    //examine the gameobjects
                    for (int Y = 0; Y < canvasHeight; Y++)
                    {
                        for (int X = 0; X < canvasWidth; X++)
                        {
                            List<GameObject> gameObjects = whatsAtLocation[X, Y];

                            List<Dude> dudesAtLocation = new List<Dude>();

                            foreach(GameObject gameObject in gameObjects)
                            {
                                if(gameObject is Dude)
                                {
                                    dudesAtLocation.Add((Dude)gameObject);
                                }
                            }

                            bool hasMale = false;
                            bool hasFemale = false;

                            Dude firstMale = new Dude();
                            Dude firstFemale = new Dude();

                            foreach (Dude dude in dudesAtLocation)
                            {
                                if (dude.Age > maxAge)
                                {
                                    string updateText = $"\r\n{dude.Name} has died!!!\r\n";
                                    canvas.Dispatcher.Invoke(new UpdateActionLogCallback(this.UpdateActionLog), new object[] { updateText });
                                    dudes.Remove(dude);
                                }
                                if (dude.Gender == Dude.gender.Male && dude.Age > minBreedAge && dude.Age < maxBreedAge)
                                {
                                    firstMale = dude;
                                    hasMale = true;
                                }
                                if (dude.Gender == Dude.gender.Female && dude.Age > minBreedAge && dude.Age < maxBreedAge && dude.timeSinceLastChild > 1)
                                {
                                    firstFemale = dude;
                                    hasFemale = true;
                                }
                            }
                            if(hasMale && hasFemale && dudes.Count < maxDudes && firstFemale.Action == Dude.action.StayingAtHome && firstMale.Action == Dude.action.StayingAtHome)
                            {
                                firstFemale.timeSinceLastChild = 0;
                                firstMale.timeSinceLastChild = 0;
                                Dude newDude = new Dude();
                                newDude.X = X;
                                newDude.Y = Y;
                                newDude.DestinationX = X;
                                newDude.DestinationY = Y;
                                newDude.DestinationName = firstFemale.DestinationName;
                                newDude.Health = 255;
                                newDude.Gender = (Dude.gender)random.Next(2);
                                newDude.firstName = GetRandomFirstName(newDude.Gender);
                                newDude.surname = $"{firstMale.surname}-{firstFemale.surname}";
                                if (newDude.surname.Length > 20)
                                    newDude.surname = GetRandomSurname();
                                newDude.Name = $"{newDude.firstName} {newDude.surname}";
                                newDude.Age = 0;
                                newDude.Action = Dude.action.StayingAtHome;
                                dudes.Add(newDude);
                                string updateText = $"\r\n{newDude.Name} has been born to {firstFemale.Name} ({(int)firstFemale.Age}) and {firstMale.Name} ({(int)firstMale.Age})!!!\r\n";
                                canvas.Dispatcher.Invoke(new UpdateActionLogCallback(this.UpdateActionLog), new object[] { updateText });
                            }
                        }
                    }

                    canvas.Dispatcher.Invoke(new UpdateBitmapCallback(this.UpdateCanvas), new object[] { bitmap.Bitmap });
                    
                }
            }
            catch (Exception e)
            { }
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
                town.Id = gameObjectId++;
                town.X = random.Next(canvasWidth);
                town.Y = random.Next(canvasHeight);
                town.Name = GetRandomTownName();
                Thread.Sleep(10);

                int numberOfShops = random.Next(1, maxShopsPerTown);

                for (int j = 0; j < numberOfShops; j++)
                {
                    CreateShop(town);
                }
                towns.Add(town);
            }
        }

        private string GetRandomTownName()
        {
            return townNames[random.Next(townNames.Count)];
        }

        private void CreateShop(Town town)
        {
            //create a shop a certain distance from the town
            Shop shop = new Shop();
            shop.Id = gameObjectId++;

            int shopX = -1;
            int shopY = -1;

            //pick a point townToShopDistance away from the town
            while (shopX < 0 || shopX > canvasWidth)
            {
                int shopXMin = (int)town.X - townToShopDistance;
                shopX = shopXMin + random.Next(townToShopDistance * 2);
            }
            while (shopY < 0 || shopY > canvasHeight)
            {
                int shopYMin = (int)town.Y - townToShopDistance;
                shopY = shopYMin + random.Next(townToShopDistance * 2);
            }
            shop.X = shopX;
            shop.Y = shopY;
            shop.Name = GetRandomShopName(town);
            shops.Add(shop);
        }

        private string GetRandomShopName(Town town)
        {
            string companyNameTemplate = companyNames[random.Next(companyNames.Count)];

            string companyName = allNames[random.Next(allNames.Count)];
            string lastname = GetRandomSurname();

            companyNameTemplate = companyNameTemplate.Replace("{firstname}", companyName);
            companyNameTemplate = companyNameTemplate.Replace("{lastname}", lastname);
            companyNameTemplate = companyNameTemplate.Replace("{firstinitial}", companyName[0].ToString());
            companyNameTemplate = companyNameTemplate.Replace("{lastinitial}", lastname[0].ToString());
            companyNameTemplate = companyNameTemplate.Replace("{townname}", town.Name);

            return companyNameTemplate;
        }

        private void CreateDudes()
        {
            for (int i = 0; i < dudeCount; i++)
            {
                Dude dude = new Dude();
                dude.Age = 18;
                dude.X = random.Next(canvasWidth);
                dude.Y = random.Next(canvasHeight);
                dude.DestinationX = dude.X;
                dude.DestinationY = dude.Y;
                dude.Health = 255;
                dude.Id = gameObjectId++;
                dude.Gender = (Dude.gender)random.Next(2);
                dude.firstName = GetRandomFirstName(dude.Gender);
                dude.surname = GetRandomSurname();
                dude.Name = $"{dude.firstName} {dude.surname}";
                dudes.Add(dude);
            }
            
        }

        private string GetRandomSurname()
        {
            return surnames[random.Next(surnames.Count)];
        }

        private string GetRandomFirstName(Dude.gender gender)
        {
            if(gender == Dude.gender.Female)
            {
                return femaleNames[random.Next(femaleNames.Count)];
            }
            else
            {
                return maleNames[random.Next(maleNames.Count)];
            }

        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            //go = false;
            CreateTowns();
            Thread.Sleep(10);
            CreateDudes();
            //go = true;
            //Draw();
        }

        private void ResetWorld_Click(object sender, RoutedEventArgs e)
        {
            ResetWorld();
        }

        private void ResetWorld()
        {
            //go = false;
            towns = new List<Town>();
            shops = new List<Shop>();
            dudes = new List<Dude>();
            //go = true;
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if(go)
            {
                go = false;
                GoButton.Content = "Go";
            }
            else
            {
                go = true;
                GoButton.Content = "Stop";
                Thread draw = new Thread(new ThreadStart(Draw));
                draw.Start();
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            UpdateStatusText(e.GetPosition(canvas));
        }
    }
}
