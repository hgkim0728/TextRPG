using System.Runtime.CompilerServices;

namespace TextRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("스파르타 마을에 오신 여러분을 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

            Player player = new Player("Danji");
        }
    }

    /// <summary>
    /// 플레이어
    /// </summary>
    class Player
    {
        #region 플레이어 능력치
        public int Level { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Health { get; set; }
        public int Gold;
        #endregion

        #region 장비 추가 능력치
        public int AttackEquip {  get; set; }
        public int DefenseEquip { get; set; }
        public int HealthEquip { get; set; }
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
            HealthEquip = 0;
        }

        /// <summary>
        /// 상태창 출력
        /// </summary>
        public void ViewStatus(GameManager _gm)
        {
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
            Console.WriteLine($"Lv. {Level}");
            Console.WriteLine($"{Name} ( {Job} )");
            Console.WriteLine($"공격력 : {Attack + AttackEquip}");
            Console.WriteLine($"방어력 : {Defense + DefenseEquip}");
            Console.WriteLine($"체력 : {Health + HealthEquip}");
            Console.WriteLine($"{Gold} G\n");
            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n>>");
            
            bool select = false;

            while (select)
            {
                string inputSt = Console.ReadLine();
                int input;

                if(int.TryParse(inputSt, out input))
                {
                    select = true;
                }
                else
                {
                    input = -1;
                }

                if(input == 0)
                {
                    _gm.ChangeState(0);
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

        void Buy(Player _player);
        void Use();
    }

    class AttackEquipment : IItem
    {
        public string Name { get; set; }
        public string Explanation { get; }

        public int Price { get; }
        public int Effect { get; }

        public AttackEquipment(string _name, string _explan, int _price, int _effect)
        {
            Name = _name;
            Explanation = _explan;
            Price = _price;
            Effect = _effect;
        }

        public void Buy(Player _player)
        {
            
        }

        public void Use()
        {

        }
    }

    class GameManager
    {
        GameState gameState = GameState.Main;
        bool isPlay = true;

        public void TownMenu()
        {
            Console.WriteLine($"1. 상태 보기\n2. 인벤토리\n3. 상점\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n>>");

            bool select = false;

            while(select)
            {
                string inputSt = Console.ReadLine();
                int input;
                
                if(int.TryParse(inputSt, out input))
                {
                    select = true;
                }
                else
                {
                    input = 0;
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
            Console.Clear();
            gameState = (GameState)_stateNum;
        }

        public void WarningInput()
        {
            Console.WriteLine("잘못된 입력입니다.");
        }
    }

    enum GameState
    {
        Main = 0, Status = 1, Inventory = 2, EquipManage = 3, Shop = 4, BuyItem = 5
    }
}
