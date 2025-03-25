
    try
    {
        Ship ship1 = new Ship("Ocean Voyager", 20, 100, 50000);
        Ship ship2 = new Ship("Sea Breeze", 18, 80, 40000);

        FluidContainer fluidContainer = new FluidContainer("L", 1000, 250, 500, false);
        fluidContainer.load_container(800);

        GasContainer gasContainer = new GasContainer("G", 500, 200, 300, 400, 5.0);
        gasContainer.load_container(250);

        RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer("Bananas", 1200, 300, 600, 500, 14.0);
        refrigeratedContainer.load_container(1000);

        ship1.AddContainer(fluidContainer);
        ship1.AddContainer(gasContainer);
        ship1.AddContainer(refrigeratedContainer);

        Console.WriteLine("Informacje o statku 1:");
        ship1.PrintShipInfo();

        Console.WriteLine("\nPrzenoszenie kontenera między statkami:");
        ship1.TransferContainer(ship2, fluidContainer.SerialNumber);

        Console.WriteLine("\nInformacje o statku 1 po przeniesieniu:");
        ship1.PrintShipInfo();

        Console.WriteLine("\nInformacje o statku 2 po przeniesieniu:");
        ship2.PrintShipInfo();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Wystąpił błąd: {ex.Message}");
    }



public abstract class Container
    {
        public string SerialNumber { get; private set; }
        public double MaxLoadCapacity { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Load { get; set; }
        public double Depth { get; set; }

        private static int uniqueCounter = 0;

        public string getSerialNumber()
        {
            return SerialNumber;
        }
        protected Container(string type, double maxLoadCapacity, double height, double weight, double depth)
        {
            uniqueCounter++;
            SerialNumber = $"KON-{type}-{uniqueCounter}";
            MaxLoadCapacity = maxLoadCapacity;
            Height = height;
            Weight = weight;
            Depth = depth;
        }

        public void empty_container()
        {
            Load = 0;
            Console.WriteLine("Oprózniono ladunek");
        }

        public void load_container(double load)
        {
            if (load > MaxLoadCapacity)
            {
                throw new OverfillException("Ladunek nie zaladowany, nie ma miejsca");
            }
            else
            {
                Load += load;
            }
        }
    }

    public class OverfillException : Exception
    {
        public OverfillException(string message) : base(message)
        {
        }
    }
    public interface IHazardNotifier
    {
        void notify();
    }


    public class FluidContainer : Container, IHazardNotifier
    {
        public bool NiebezpiecznyLadunek { get; set; }

        public FluidContainer(string type, double maxLoadCapacity, double height, double depth,
            bool niebezpiecznyLadunek)
            : base(type, maxLoadCapacity, height, 0, depth)
        {
            NiebezpiecznyLadunek = niebezpiecznyLadunek;
        }

        public void notify()
        {
            Console.WriteLine($"Zaszla niebezpieczna sytuacja w {nameof(FluidContainer)}");
            throw new NotImplementedException();
        }

        public bool CheckSafety()
        {
            if (NiebezpiecznyLadunek)
            {
                return false;
            }

            return true;
        }

        public void load_container(double load)
        {
            if (CheckSafety())
            {
                if (load > 0.9 * MaxLoadCapacity)
                {
                    notify();
                }
                else
                {
                    Load += load;
                }
            }
            else
            {
                if (load > 0.5 * MaxLoadCapacity)
                {
                    notify();
                }
                else
                {
                    Load += load;
                }
            }
        }
    }

    public class GasContainer : Container, IHazardNotifier
    {
        public double pressure { get; set; }
        public GasContainer(string type, double maxLoadCapacity, double height, double weight, double depth, double pressure) : base(type, maxLoadCapacity, height, weight, depth)
        {
            this.pressure = pressure;
        }
        public void notify()
        {
            Console.WriteLine($"Zaszla niebezpieczna sytuacja w {nameof(GasContainer)},{getSerialNumber()}");
            throw new NotImplementedException();
        }

        public void empty_container()
        {
            Load = 0.05*Load;
        }

        public void load_container(double load)
        {
            if (load > MaxLoadCapacity)
            {
                notify();
            }
        }
    }

    public class RefrigeratedContainer : Container
    {
        
        public string Type { get; set; }
        public double Temperature { get; set; }
        private static readonly Dictionary<string, double> RecommendedTemperatures = new Dictionary<string, double>
        {
            { "Bananas", 13.3 },
            { "Chocolate", 18.0 },
            { "Fish", 2.0 },
            { "Meat", -15.0 },
            { "Ice cream", -18.0 },
            { "Frozen pizza", -30.0 },
            { "Cheese", 7.2 },
            { "Sausages", 5.0 },
            { "Butter", 20.5 },
            { "Eggs", 19.0 }
        };
        public RefrigeratedContainer(string type, double maxLoadCapacity, double height, double weight, double depth, double temperature) : base(type, maxLoadCapacity, height, weight, depth)
        {
            if (!RecommendedTemperatures.ContainsKey(type))
            {
                throw new ArgumentException($"Nieznany typ produktu: {type}. Nie można przechowywać w kontenerze chłodniczym.");
            }
          
            double recommendedTemp = RecommendedTemperatures[type];
            if (temperature < recommendedTemp )
            {
                throw new ArgumentException($"Nieprawidłowa temperatura! Dla produktu {type} rekomendowana temperatura to {recommendedTemp}°C.");
            }
            Temperature = temperature;
            
        }
        

        
    }
  public class Ship
    {
        public string Name { get; private set; }
        public double Speed { get; private set; }
        public int MaxContainerCount { get; private set; }
        public double MaxTotalWeight { get; private set; }

        private List<Container> containers = new List<Container>();

        public Ship(string name, double speed, int maxContainerCount, double maxTotalWeight)
        {
            Name = name;
            Speed = speed;
            MaxContainerCount = maxContainerCount;
            MaxTotalWeight = maxTotalWeight;
        }

        public void AddContainer(Container container)
        {
            if (GetTotalWeight() + container.Weight + container.Load > MaxTotalWeight)
            {
                throw new InvalidOperationException("Dodanie tego kontenera przekroczyłoby maksymalną ładowność statku.");
            }

            if (containers.Count >= MaxContainerCount)
            {
                throw new InvalidOperationException("Statek osiągnął maksymalną liczbę kontenerów.");
            }

            containers.Add(container);
        }

        public void AddContainers(List<Container> containersToAdd)
        {
            foreach (var container in containersToAdd)
            {
                AddContainer(container);
            }
        }

        public void RemoveContainer(string serialNumber)
        {
            var containerToRemove = containers.Find(c => c.SerialNumber == serialNumber);
            if (containerToRemove != null)
            {
                containers.Remove(containerToRemove);
            }
            else
            {
                throw new ArgumentException("Nie znaleziono kontenera na statku.");
            }
        }

        public void ReplaceContainer(string serialNumberToReplace, Container newContainer)
        {
            var indexToReplace = containers.FindIndex(c => c.SerialNumber == serialNumberToReplace);
            if (indexToReplace != -1)
            {
                containers[indexToReplace] = newContainer;
            }
            else
            {
                throw new ArgumentException("Nie znaleziono kontenera do zastąpienia.");
            }
        }

        public void TransferContainer(Ship destinationShip, string serialNumber)
        {
            var containerToTransfer = containers.Find(c => c.SerialNumber == serialNumber);
            if (containerToTransfer != null)
            {
                try
                {
                    destinationShip.AddContainer(containerToTransfer);
                    RemoveContainer(serialNumber);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException($"Nie można przenieść kontenera: {ex.Message}");
                }
            }
            else
            {
                throw new ArgumentException("Nie znaleziono kontenera do przeniesienia.");
            }
        }

        public double GetTotalWeight()
        {
            return containers.Sum(container => container.Weight + container.Load);
        }

        public void PrintShipInfo()
        {
            Console.WriteLine($"Nazwa statku: {Name}");
            Console.WriteLine($"Prędkość: {Speed} węzłów");
            Console.WriteLine($"Maksymalna liczba kontenerów: {MaxContainerCount}");
            Console.WriteLine($"Maksymalna waga całkowita: {MaxTotalWeight} ton");
            Console.WriteLine($"Aktualna liczba kontenerów: {containers.Count}");
            Console.WriteLine($"Aktualna całkowita waga: {GetTotalWeight()} ton");
            
            Console.WriteLine("\nKontenery na statku:");
            foreach (var container in containers)
            {
                PrintContainerInfo(container);
            }
        }

        public void PrintContainerInfo(Container container)
        {
            Console.WriteLine($"Numer seryjny: {container.SerialNumber}");
            Console.WriteLine($"Maksymalna ładowność: {container.MaxLoadCapacity} kg");
            Console.WriteLine($"Aktualny ładunek: {container.Load} kg");
            Console.WriteLine($"Wysokość: {container.Height} cm");
            Console.WriteLine($"Waga kontenera: {container.Weight} kg");
            Console.WriteLine($"Głębokość: {container.Depth} cm");
            
            if (container is FluidContainer fluidContainer)
            {
                Console.WriteLine($"Typ: Kontener na płyny");
                Console.WriteLine($"Ładunek niebezpieczny: {fluidContainer.NiebezpiecznyLadunek}");
            }
            else if (container is GasContainer gasContainer)
            {
                Console.WriteLine($"Typ: Kontener na gaz");
                Console.WriteLine($"Ciśnienie: {gasContainer.pressure} atm");
            }
            else if (container is RefrigeratedContainer refrigeratedContainer)
            {
                Console.WriteLine($"Typ: Kontener chłodniczy");
                Console.WriteLine($"Rodzaj produktu: {refrigeratedContainer.Type}");
                Console.WriteLine($"Temperatura: {refrigeratedContainer.Temperature}°C");
            }
            
            Console.WriteLine();
        }
    }


  

