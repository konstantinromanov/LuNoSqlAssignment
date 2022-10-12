using LuNoSqlAssignment.Models;
using Newtonsoft.Json;

namespace LuNoSqlAssignment.Services
{   
    public class PersonsFileAccess : IPersonsFileAccess
    {
         
        private readonly IFileHandling _fileHandling;
               
        public PersonsFileAccess(IFileHandling fileHandling)
        {
            _fileHandling = fileHandling;
        }

       
        public void SavePersons(IList<Person> persons, string fileNameWrite)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string fileNameWrite = "persons.txt";
            _fileHandling.Directory = docPath;

            //Person personsToFile = new GamesSavingModel();

            //int width = persons[0].Width;
            //int height = persons[0].Height;

            //personsToFile.Generation = persons[0].GenerationCounter;
            //personsToFile.BoardWidth = width;
            //personsToFile.BoardHeight = height;
            //personsToFile.NumberOfGames = persons.Length;
            //personsToFile.GameBoards = new byte[persons.Length][,];

            //for (int i = 0; i < persons.Length; i++)
            //{
                //byte[,] board = new byte[width, persons[0].Height];

                //for (int y = 0; y < height; y++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        board[x, y] = (byte)persons[i].Board[x, y];
                //        personsToFile.GameBoards[i] = board;
                //    }
                //}
            //}

            string jsonPersonsToFile = JsonConvert.SerializeObject(persons, Formatting.None);

            _fileHandling.WriteToDisk(fileNameWrite, jsonPersonsToFile);
        }
               
        public IList<Person>? OpenPersons(string fileName)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string fileName = "game_of_life.txt";

            _fileHandling.Directory = docPath;

            try
            {
                string jsonPersonsFromFile = _fileHandling.ReadFromDisk(fileName);
                IList<Person>? personsFromFile = JsonConvert.DeserializeObject<List<Person>>(jsonPersonsFromFile);

                if (personsFromFile == null)
                {
                    return null;
                }

                //int width = personsFromFile.BoardWidth;
                //int height = personsFromFile.BoardHeight;

                //Person[] persons = new [personsFromFile.NumberOfGames];

                //for (int i = 0; i < personsFromFile.NumberOfGames; i++)
                //{
                //    CellType[,] board = new CellType[width, height];

                //    for (int y = 0; y < height; y++)
                //    {
                //        for (int x = 0; x < width; x++)
                //        {
                //            byte cell = personsFromFile.GameBoards[i][x, y];
                //            board[x, y] = cell == 0 ? CellType.Dead : (cell == 1 ? CellType.Live : CellType.InActive);
                //        }
                //    }

                //    int liveCells = board.Cast<CellType>().Where(x => x == CellType.Live).Count();
                //    IGame game = new Game(width, height, personsFromFile.Generation, liveCells, board);
                //    persons[i] = game;
                //}

                return personsFromFile;
            }
            catch (Exception)
            {

            }

            return null;
        }              
    }
}
