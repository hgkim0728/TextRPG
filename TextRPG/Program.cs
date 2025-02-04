using System.Runtime.CompilerServices;

namespace TextRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("스파르타 마을에 오신 여러분을 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");
            Console.Write("이름을 입력해주세요, \n이름 : ");

            Player player = new Player(Console.ReadLine());
            Shop shop = new Shop();
            Inventory inventory = new Inventory(shop);
            GameManager gameManager = new GameManager(player, shop, inventory);

            while (gameManager.IsPlay)
            {
                Console.Clear();
                gameManager.GameFlow();
            }
        }
    }
    
    /// <summary>
    /// 플레이어
    /// </summary>
    class Player
    {
        #region 플레이어 능력치
        public int Level { get; }
        public string Name { get; }
        public string Job { get; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        #endregion

        #region 장비 추가 능력치
        public int AttackEquip {  get; set; }
        public int DefenseEquip { get; set; }
        #endregion

        /// <summary>
        /// 기본값 적용
        /// </summary>
        /// <param name="_name">플레이어 이름</param>
        public Player(string _name)
        {
            Level = 1;
            Name = _name;
            Job = "전사";
            Attack = 10;
            Defense = 5;
            Health = 100;
            Gold = 1500;

            AttackEquip = 0;
            DefenseEquip = 0;
        }

        /// <summary>
        /// 상태창 출력
        /// </summary>
        public void ViewStatus(GameManager _gm)
        {
            int attack = Attack + AttackEquip;
            int defense = Defense + DefenseEquip;

            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
            Console.WriteLine($"Lv. {Level}");
            Console.WriteLine($"{Name} ( {Job} )");
            Console.WriteLine($"공격력 : {attack} (+{AttackEquip})");
            Console.WriteLine($"방어력 : {defense} (+{DefenseEquip})");
            Console.WriteLine($"체력 : {Health}");
            Console.WriteLine($"{Gold} G\n");
            Console.WriteLine("0. 나가기\n");
            bool select = false;

            while (!select)
            {
                Console.Write("\n원하시는 행동을 입력해주세요.\n>>");
            
                int input = _gm.SelectInput();

                if(input == 0)
                {
                    _gm.ChangeState(0);
                    select = true;
                }
                else
                {
                    _gm.WarningInput();
                }
            }
        }
    }

    interface IItem
    {
        string Name { get; set; }
        string Explanation { get; }

        int Price { get; }
        int Effect { get; }
        int ItemType { get; }

        bool IsBuy { get; }
        bool Equipped { get; set; }

        bool Buy(Player _player, GameManager _gm);
        void Use(Player _player);
    }

    /// <summary>
    /// 아이템 정보
    /// </summary>
    class Item : IItem
    {
        public string Name { get; set; }
        public string Explanation { get; }

        public int Price { get; }
        public int Effect { get; }
        public int ItemType { get; }

        public bool IsBuy { get; set; }
        public bool Equipped { get; set; }

        public Item(string _name, string _explan, int _price, int _effect, int itemType)
        {
            Name = _name;
            Explanation = _explan;
            Price = _price;
            Effect = _effect;
            ItemType = itemType;
            IsBuy = false;
            Equipped = false;
        }

        public bool Buy(Player _player, GameManager _gm)
        {
            bool sale = false;
            int playerGole = _player.Gold;

            if(playerGole - Price >= 0)
            {
                _player.Gold -= Price;
                this.IsBuy = true;
                _gm.Inventory.listInventory.Add(this);

                sale = true;
            }

            return sale;
        }

        public void Use(Player _player)
        {
            if(!Equipped)
            {
                Equipped = true;
                
                if(ItemType == 0)
                {
                    _player.AttackEquip += Effect;
                }
                else
                {
                    _player.DefenseEquip += Effect;
                }
            }
            else
            {
                Equipped = false;

                if(ItemType == 0)
                {
                    _player.AttackEquip -= Effect;
                }
                else
                {
                    _player.DefenseEquip -= Effect;
                }
            }
        }
    }

    class Inventory
    {
        public List<IItem> listInventory = new List<IItem> ();
        public Shop Shop { get; }

        public Inventory(Shop _shop)
        {
            Shop = _shop;
        }

        public void ShowInventory(GameManager _gm)
        {
            Console.WriteLine("인벤토리\n보유중인 아이템을 관리할 수 있습니다.\n");
            Console.WriteLine("[아이템 목록]");

            if(listInventory.Count > 0)
            {
                foreach(IItem i in listInventory)
                {
                    string equip = "";

                    if(i.IsBuy && i.Equipped)
                    {
                        equip = "[E]";
                    }

                    string itemType;

                    if (i.ItemType == 0)
                    {
                        itemType = "공격력";
                    }
                    else
                    {
                        itemType = "방어력";
                    }

                    Console.WriteLine($"- {equip}{i.Name}\t| {itemType} +{i.Effect}  | {i.Explanation}");
                }
            }
            

            Console.WriteLine("\n1. 장착 관리\n0. 나가기\n");

            bool select = false;

            while(!select)
            {
                Console.WriteLine("원하시는 행동을 입력해주세요.\n>>");

                int input = _gm.SelectInput();

                if(input == 0 || input == 1)
                {
                    if(input == 0)
                    {
                        _gm.ChangeState(0);
                    }
                    else
                    {
                        _gm.ChangeState(3);
                    }

                    select = true;
                }
                else
                {
                    _gm.WarningInput();
                }
            }
        }

        public void EquipManage(GameManager _gm, Player _player)
        {
            Console.WriteLine("인벤토리 - 장착 관리\n보유중인 아이템을 관리할 수 있습니다.\n");
            Console.WriteLine("[아이템 목록]");

            int itemNum = 1;

            if (listInventory.Count > 0)
            {
                foreach (IItem i in listInventory)
                {
                    string equip = "";

                    if (i.IsBuy && i.Equipped)
                    {
                        equip = "[E]";
                    }

                    string itemType;

                    if (i.ItemType == 0)
                    {
                        itemType = "공격력";
                    }
                    else
                    {
                        itemType = "방어력";
                    }

                    Console.WriteLine($"- {itemNum} {equip}{i.Name}\t| {itemType} +{i.Effect}  | {i.Explanation}");
                    itemNum++;
                }

                Console.WriteLine("\n0. 나가기");
                bool select = false;

                while (!select)
                {
                    Console.Write("\n원하시는 행동을 입력해주세요.\n>>");
                    int input = _gm.SelectInput();

                    if(input == 0)
                    {
                        _gm.ChangeState(2);
                        select = true;
                    }
                    else if(input > 0 && input < itemNum)
                    {
                        listInventory[input - 1].Use(_player);
                        select = true;
                    }
                    else
                    {
                        _gm.WarningInput();
                    }
                }
            }
        }
    }

    class Shop
    {
        public List<IItem> listSale = new List<IItem>();

        public Shop()
        {
            Item item = new Item("수련자의 갑옷", "수련에 도움을 주는 갑옷입니다.", 1000, 5, 1);
            listSale.Add(item);
            item = new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 1500, 9, 1);
            listSale.Add(item);
            item = new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500, 15, 1);
            listSale.Add(item);
            item = new Item("낡은 검", "쉽게 볼 수 있는 낡은 겁입니다.", 600, 2, 0);
            listSale.Add(item);
            item = new Item("청동 도끼", "어디선가 사용했던 것 같은 도끼입니다.", 1500, 5, 0);
            listSale.Add(item);
            item = new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3500, 7, 0);
            listSale.Add(item);
        }

        public void ShowItem(Player _player, GameManager _gm)
        {
            Console.WriteLine("상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine($"[보유 골드]\n{_player.Gold} G\n");
            Console.WriteLine("[아이템 목록]");

            foreach(IItem i in listSale)
            {
                string itemType;
                string price;

                if(i.ItemType == 0)
                {
                    itemType = "공격력";
                }
                else
                {
                    itemType = "방어력";
                }

                if(!i.IsBuy)
                {
                    price = i.Price.ToString();
                }
                else
                {
                    price = "구매완료";
                }

                Console.WriteLine($"- {i.Name}\tt| {itemType} +{i.Effect}  | {i.Explanation}\t|   {price}");
            }

            Console.WriteLine("\n1. 아이템 구매\n0. 나가기\n");

            while (true)
            {
                Console.Write("\n원하시는 행동을 입력해주세요.\n>>");

                int input = _gm.SelectInput();

                if (input == 0 || input == 1)
                {
                    if(input == 0)
                    {
                        _gm.ChangeState(input);
                    }
                    else
                    {
                        _gm.ChangeState(5);
                    }
                    break;
                }
                else
                {
                    _gm.WarningInput();
                }
            }
        }

        public void BuyItem(Player _player, GameManager _gm)
        {
            Console.WriteLine("상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine($"[보유 골드]\n{_player.Gold} G\n");
            Console.WriteLine("[아이템 목록]");

            int itemNum = 1;

            foreach (IItem i in listSale)
            {
                string itemType;
                string price;

                if (i.ItemType == 0)
                {
                    itemType = "공격력";
                }
                else
                {
                    itemType = "방어력";
                }

                if (!i.IsBuy)
                {
                    price = i.Price.ToString();
                }
                else
                {
                    price = "구매완료";
                }

                Console.WriteLine($"- {itemNum} {i.Name}\t| {itemType} +{i.Effect}  | {i.Explanation}\t|   {price}");
                itemNum++;
            }

            Console.WriteLine("\n0. 나가기\n");

            bool select = false;

            while(!select)
            {
                Console.Write("원하시는 행동을 입력해주세요.\n>>");
                int input = _gm.SelectInput();

                if(input == 0)
                {
                    _gm.ChangeState(4);
                    break;
                }
                else if(input > 0 && input < itemNum)
                {
                    if (listSale[input - 1].IsBuy)
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.");
                        continue;
                    }

                    bool sale = listSale[input - 1].Buy(_player, _gm);
                    
                    if(sale)
                    {
                        Console.WriteLine("구매를 완료했습니다.");
                        Thread.Sleep(1000);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Gold 가 부족합니다.");
                    }
                }
                else
                {
                    _gm.WarningInput();
                }
            }
        }
    }

    class GameManager
    {
        GameState gameState = GameState.Main;
        Player player;
        Shop shop;
        public Inventory Inventory { get; }
        public bool IsPlay { get; }

        public GameManager(Player _player, Shop _shop, Inventory inventory)
        {
            player = _player;
            shop = _shop;
            IsPlay = true;
            Inventory = inventory;
        }

        public void GameFlow()
        {
            switch(gameState)
            {
                case GameState.Main:
                    TownMenu();
                    break;
                case GameState.Status:
                    player.ViewStatus(this);
                    break;
                case GameState.Inventory:
                    Inventory.ShowInventory(this);
                    break;
                case GameState.EquipManage:
                    Inventory.EquipManage(this, player);
                    break;
                case GameState.Shop:
                    shop.ShowItem(player, this);
                    break;
                case GameState.BuyItem:
                    shop.BuyItem(player, this);
                    break;
            }
        }

        private void TownMenu()
        {
            Console.WriteLine($"1. 상태 보기\n2. 인벤토리\n3. 상점\n");
            bool select = false;

            while(!select)
            {
                Console.Write("\n원하시는 행동을 입력해주세요.\n>>");

                int input = SelectInput();

                if(input > 0 && input < 4)
                {
                    select = true;
                }
                
                switch(input)
                {
                    case 1:
                        ChangeState(1);
                        break;
                    case 2:
                        ChangeState(2);
                        break;
                    case 3:
                        ChangeState(4);
                        break;
                    default:
                        WarningInput();
                        break;
                }
            }
        }

        public void ChangeState(int _stateNum)
        {
            gameState = (GameState)_stateNum;
        }

        public void WarningInput()
        {
            Console.WriteLine("잘못된 입력입니다.");
        }

        public int SelectInput()
        {
            int input;
            string inputSt = Console.ReadLine();
            bool check = int.TryParse(inputSt, out input);

            if(!check)
            {
                input = -1;
            }

            return input;
        }
    }

    enum GameState
    {
        Main = 0, Status = 1, Inventory = 2, EquipManage = 3, Shop = 4, BuyItem = 5
    }
}
